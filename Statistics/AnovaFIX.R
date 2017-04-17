################## Install the packages ###########################
###################################################################
install.packages("car")
install.packages("MBESS")
install.packages(c('devtools','curl'))
install.packages('BayesFactor', dependencies = TRUE)
devtools::install_github('ndphillips/yarrr', build_vignettes = T)
install_github("ndphillips/yarrr")
library("yarrr")
library(car)
library(MBESS)
library(devtools)

################## Loading the data file ##########################
###################################################################

path = "C:/Savitha/HCI/Eye Tracker Data/"
fileNames = list.files(path=paste(path,"data/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrameRaw = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, na.strings=c("", "NA"), header=TRUE, sep=",", dec=".", stringsAsFactors=FALSE)}))
is.data.frame(dataFrameRaw)
ncol(dataFrameRaw)
nrow(dataFrameRaw)
rows = nrow(dataFrameRaw)

head(dataFrameRaw,15)

################## Creating DataFrame  ############################
###################################################################

data_frame = dataFrameRaw[,c( 
  "participantId"
  ,"condition"
  ,"timeSinceStartup"
  ,"correctNodeHit"
  ,"keypressed"
  #,"calibrationdata_frame"
  ,"bubbleSize"
  #,"numberNodes"
  #,"targetNode"
  #,"currentSelectedNode"
  ,"currentState"
  #,"correctedEyeX"
  #,"correctedEyeY"
  #,"rawEyeX"
  #,"rawEyeY"
)]

data_frame <- na.omit(data_frame) # remove all NA values

data_frame = data_frame[
  data_frame$currentState=="Trial"
  # &data_frame$correctNodeHit=="TRUE"
  &(data_frame$keypressed=="Enter"|data_frame$keypressed=="HAPRING_TIP")
  ,]

#remove wrong data_frame :-(
data_frame<-data_frame[!(data_frame$condition=="MOUSE" & data_frame$keypressed=="HAPRING_TIP"),]
data_frame<-data_frame[!(data_frame$condition=="noCalibrationdata_frameSet"),]

#data_frame <- data_frame[c(-7)] # remove "currentState" column
data_frame <- data_frame[c(-5)] # remove "keypressed" column

# rename condition field for readability
data_frame$condition <- as.character(data_frame$condition)
data_frame$condition[data_frame$condition == "WITHCUSTOMCALIB"] <- "Custom Calibration"
data_frame$condition[data_frame$condition == "MOUSE"] <- "Mouse & Keyboard"
data_frame$condition[data_frame$condition == "EYE"] <- "Built-in Calibration"

head(data_frame,18)

participants = unique(data_frame["participantId"])
participants

conditions = unique(data_frame["condition"])
conditions

states = unique(data_frame["currentState"])
states

################## Calculate selection times  #####################
###################################################################

Subject=list()
Condition=list()
SelectionTime=list()
SelectionError=list()
BubbleSize=list()

lastTime=0;

for(p in unlist(participants))
{
  for(c in unlist(conditions))
  {
    pcData = data_frame[data_frame$participantId==p&data_frame$condition==c,]
   # typeof(pcData)
    
    
    firstRow = TRUE
    
    for(i in 1:nrow(pcData))
    {
      row <- pcData[i,]
      
      if(firstRow==FALSE)
      {
        Subject=c(Subject, p)
        Condition=c(Condition, c)
        SelectionTime=c(SelectionTime, as.numeric(row["timeSinceStartup"])-lastTime)
        if(row["correctNodeHit"] == TRUE)
          SelectionError=c(SelectionError, 0)
        else
          SelectionError=c(SelectionError, 1)
        BubbleSize=c(BubbleSize, row["bubbleSize"])
      }
      
      lastTime = as.numeric(row["timeSinceStartup"])
      firstRow = FALSE
    }
  }
}

Subject = unlist(Subject)
Condition = unlist(Condition)
SelectionTime = unlist(SelectionTime)
SelectionError = unlist(SelectionError)
BubbleSize = unlist(BubbleSize)

data_frame = data.frame(Subject, Condition, SelectionTime, SelectionError, BubbleSize)

head(data_frame, nrow(data_frame))

############### Calculate outlier thresholds  #####################
###################################################################

threshold.upper = 0;
threshold.lower = 0;

threshold.factor = 1.5

# Remove negative times...
SelectionTime = subset(data_frame, (SelectionTime > 0), SelectionTime)
SelectionTime = unlist(SelectionTime)
print(length(SelectionTime))
print(summary(SelectionTime))

lowerq = quantile(SelectionTime)[2]
upperq = quantile(SelectionTime)[4]
iqr = upperq - lowerq # IQR(ExperimentDiscrepancy) can be used as an alternative

threshold.upper = (iqr * threshold.factor) + upperq
print(threshold.upper)
threshold.lower = lowerq - (iqr * threshold.factor)
print(threshold.lower)

############### Remove outliers from data     #####################
###################################################################

#Remove wrong selection times from data frame
data_frame<-data_frame[!(data_frame$SelectionTime < 0),]
#Remove outliers from data frame
data_frame<-data_frame[!(data_frame$SelectionTime > threshold.upper | data_frame$SelectionTime < threshold.lower),]


head(data_frame, nrow(data_frame))


############### Check for selection time normality  ###############
###################################################################

#boxplot(ExperimentClean)
#identify(rep(1, length(ExperimentClean)), ExperimentClean, labels = seq_along(ExperimentClean))
ExperimentClean <- data_frame$SelectionTime

hist(ExperimentClean, breaks="FD")
qqnorm(ExperimentClean)
qqline(ExperimentClean)
shapiro.test(ExperimentClean)
print(ks.test(ExperimentClean, "pnorm", mean=mean(ExperimentClean), sd=sd(ExperimentClean)))


############### transform selection time for normality  ###########
###################################################################

ExperimentCorrected <- ExperimentClean

ExperimentCorrected = log(ExperimentCorrected)
ExperimentCorrected = ExperimentCorrected + abs(summary(ExperimentCorrected)[1])
print(summary(ExperimentCorrected))

hist(ExperimentCorrected, breaks="FD")
qqnorm(ExperimentCorrected)
qqline(ExperimentCorrected)
shapiro.test(ExperimentCorrected)
print(ks.test(ExperimentCorrected, "pnorm", mean=mean(ExperimentCorrected), sd=sd(ExperimentCorrected)))


data_frame$CorrectedSelectionTime<-ExperimentCorrected

########Calculate marginals per condition per participant##########
###################################################################

Subject=list()
Condition=list()
SelectionTime=list()
SelectionError=list()
CorrectedSelectionTime=list()


for(p in unlist(participants))
{
  for(c in unlist(conditions))
  {
    pcData = data_frame[data_frame$Subject==p&data_frame$Condition==c,]
    
    Subject=c(Subject, p)
    Condition=c(Condition, c)
    SelectionTime=c(SelectionTime, mean(unlist(pcData["SelectionTime"])))
    SelectionError=c(SelectionError, sum(unlist(pcData["SelectionError"])))
    CorrectedSelectionTime=c(CorrectedSelectionTime, mean(unlist(pcData["CorrectedSelectionTime"])))
  }
}

Subject = unlist(Subject)
Condition = unlist(Condition)
SelectionTime = unlist(SelectionTime)
SelectionError = unlist(SelectionError)
CorrectedSelectionTime=unlist(CorrectedSelectionTime)

pc_data_frame = data.frame(Subject, Condition, SelectionTime, SelectionError, CorrectedSelectionTime)

head(pc_data_frame, nrow(pc_data_frame))
###################################################################################################################################


########Statistics for selection time #############################
###################################################################
plot1<-boxplot(SelectionTime ~ Condition, pc_data_frame, main="Selection Time", 
               xlab="Condition", ylab="Selection Time")

data_frame_ST_builtin <- data_frame[data_frame$Condition=="Built-in Calibration","SelectionTime"]
summary(data_frame_ST_builtin)
sd(data_frame_ST_builtin)

data_frame_ST_custom <- data_frame[data_frame$Condition=="Custom Calibration","SelectionTime"]
summary(data_frame_ST_custom)
sd(data_frame_ST_custom)

data_frame_ST_kb <- data_frame[data_frame$Condition=="Mouse & Keyboard","SelectionTime"]
summary(data_frame_ST_kb)
sd(data_frame_ST_kb)

########ANOVA for Corrected Selection Time(ANOVA with the Sphericity test) ########################
###################################################################################################

pc_data_frame_ANOVA_CST <- data.frame(pc_data_frame$Subject, pc_data_frame$Condition, pc_data_frame$CorrectedSelectionTime)
pc_matrix_ANOVA_CST <- with(pc_data_frame_ANOVA_CST, 
                cbind(
                  CorrectedSelectionTime[Condition=="Built-in Calibration"], 
                  CorrectedSelectionTime[Condition=="Custom Calibration"], 
                  CorrectedSelectionTime[Condition=="Mouse & Keyboard"])) 
pc_model_ANOVA_CST <- lm(pc_matrix_ANOVA_CST ~ 1)
pc_design_ANOVA_CST <- factor(c("Built-in Calibration", "Custom Calibration", "Mouse & Keyboard"))

options(contrasts=c("contr.sum", "contr.poly"))
pc_aov_ANOVA_CST <- Anova(pc_model_ANOVA_CST, idata=data.frame(pc_design_ANOVA_CST), idesign=~pc_design_ANOVA_CST, type="III")
summary(pc_aov_ANOVA_CST, multivariate=F)


########PostHoc for Corrected Selection Time ######################
###################################################################
pc_data_frame_PH_CST <- data.frame(pc_data_frame$Condition, pc_data_frame$CorrectedSelectionTime)
pc_aov_PH_CST <- aov(pc_data_frame$CorrectedSelectionTime ~ pc_data_frame$Condition, pc_data_frame_PH_CST)
summary(pc_aov_PH_CST)
TukeyHSD(pc_aov_PH_CST)


###################Effect Size#####################################
###################################################################

aovES_ST <- aov(CorrectedSelectionTime ~ factor(Condition) + Error(factor(Subject)/factor(Condition)), pc_data_frame_PH_CST)
summary(aovES_ST)
EffecSize<-17.875/(17.875+5.558)
EffecSize

##############################################################################################################################


########Statistics for selection error #############################
###################################################################
plot2<-boxplot(SelectionError ~ Condition, pc_data_frame, main="Selection Error", 
               xlab="Condition", ylab="Selection Error")

data_frame_SE_builtin <- data_frame[data_frame$Condition=="Built-in Calibration","SelectionError"]
((sum(data_frame_SE_builtin)/length(data_frame_SE_builtin))*100)

data_frame_SE_custom <- data_frame[data_frame$Condition=="Custom Calibration","SelectionError"]
((sum(data_frame_SE_custom)/length(data_frame_SE_custom))*100)

data_frame_SE_kb <- data_frame[data_frame$Condition=="Mouse & Keyboard","SelectionError"]
((sum(data_frame_SE_kb)/length(data_frame_SE_kb))*100)

########ANOVA for Corrected Selection Error(ANOVA with the Sphericity test) ########################
###################################################################################################

pc_data_frame_ANOVA_SE <- data.frame(pc_data_frame$Subject, pc_data_frame$Condition, pc_data_frame$SelectionError)
pc_matrix_ANOVA_SE <- with(pc_data_frame_ANOVA_SE, 
                            cbind(
                              SelectionError[Condition=="Built-in Calibration"], 
                              SelectionError[Condition=="Custom Calibration"], 
                              SelectionError[Condition=="Mouse & Keyboard"])) 
pc_model_ANOVA_SE <- lm(pc_matrix_ANOVA_SE ~ 1)
pc_design_ANOVA_SE <- factor(c("Built-in Calibration", "Custom Calibration", "Mouse & Keyboard"))

options(contrasts=c("contr.sum", "contr.poly"))
pc_aov_ANOVA_SE <- Anova(pc_model_ANOVA_SE, idata=data.frame(pc_design_ANOVA_SE), idesign=~pc_design_ANOVA_SE, type="III")
summary(pc_aov_ANOVA_SE, multivariate=F)


########PostHoc for Corrected Selection Error ######################
###################################################################
pc_data_frame_PH_SE <- data.frame(pc_data_frame$Condition, pc_data_frame$SelectionError)
pc_aov_PH_SE <- aov(pc_data_frame$SelectionError ~ pc_data_frame$Condition, pc_data_frame_PH_SE)
summary(pc_aov_PH_SE)
TukeyHSD(pc_aov_PH_SE)


###################Effect Size#####################################
###################################################################

aovES_SE <- aov(SelectionError ~ factor(Condition) + Error(factor(Subject)/factor(Condition)), pc_data_frame_PH_SE)
summary(aovES_SE)
EffecSize<-45.5/(45.5+209.8)
EffecSize


##################################################################################################################

#####################Pirate Plots - Selection Time######################################

pirateplot(formula = SelectionTime ~ Condition, data = pc_data_frame, main = "Selection Time for Different Conditions"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

########################Pirate Plots - Selection Error##################################


pirateplot(formula = SelectionError ~ Condition, data = pc_data_frame, main = "Selection Error for Different Conditions"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

########################Pirate Plots - Corrected Selection Time##################################


pirateplot(formula = CorrectedSelectionTime ~ Condition, data = pc_data_frame, main = "Selection Time"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

###########################################################################################################################################


########Calculate marginals per bubblesize per participant##########
#####################################################################

bubble_size = unique(data_frame["BubbleSize"])
bubble_size



Subject=list()
Condition=list()
SelectionTime=list()
SelectionError=list()
CorrectedSelectionTime=list()
bubblesize = list()


for(p in unlist(participants))
{
  for(c in unlist(conditions))
  {
    for(b in unlist(bubble_size))
    {
      pbData = data_frame[data_frame$Subject==p & data_frame$BubbleSize==b & data_frame$Condition==c,]
      
      Subject=c(Subject, p)
      bubblesize=c(bubblesize, b)
      Condition=c(Condition,c)
      SelectionTime=c(SelectionTime, mean(unlist(pbData["SelectionTime"])))
      SelectionError=c(SelectionError, sum(unlist(pbData["SelectionError"])))
      CorrectedSelectionTime=c(CorrectedSelectionTime, mean(unlist(pbData["CorrectedSelectionTime"])))
    }
  }
  
}

Subject = unlist(Subject)
Condition = unlist(Condition)
bubblesize = unlist(bubblesize)
SelectionTime = unlist(SelectionTime)
SelectionError = unlist(SelectionError)
CorrectedSelectionTime=unlist(CorrectedSelectionTime)

pb_data_frame_bubble = data.frame(Subject, Condition,bubblesize, SelectionTime, SelectionError, CorrectedSelectionTime)

head(pb_data_frame_bubble, nrow(pb_data_frame_bubble))

pb_data_frame_bubble <- na.omit(pb_data_frame_bubble) # remove all NA values

########Statistics for bubble size ################################
###################################################################

plot3<-boxplot(SelectionTime ~ bubblesize, pb_data_frame_bubble, main="SelectionTime", 
               xlab="BubbleSize", ylab="Selection Time")




data_frame_ST_bubble10 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==10,"SelectionTime"]
summary(data_frame_ST_bubble10 )
sd(data_frame_ST_bubble10 )


data_frame_ST_bubble5 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==5,"SelectionTime"]
summary(data_frame_ST_bubble5 )
sd(data_frame_ST_bubble5 )


data_frame_ST_bubble75 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==7.5,"SelectionTime"]
summary(data_frame_ST_bubble75 )
sd(data_frame_ST_bubble75 )

########################################################################################################################################
########ANOVA for Corrected Selection Time(ANOVA with the Sphericity test) ########################
###################################################################################################

pb_data_frame_ANOVA_CST <- data.frame(pb_data_frame_bubble$Subject,pb_data_frame_bubble$bubblesize, pb_data_frame_bubble$CorrectedSelectionTime)
pb_matrix_ANOVA_CST <- with(pb_data_frame_ANOVA_CST, 
                            cbind(
                              CorrectedSelectionTime[bubblesize==10], 
                              CorrectedSelectionTime[bubblesize==7.5], 
                              CorrectedSelectionTime[bubblesize==5])) 

pb_matrix_ANOVA_CST<- na.omit(pb_matrix_ANOVA_CST) # remove all NA values
pb_model_ANOVA_CST <- lm(pb_matrix_ANOVA_CST ~ 1)
pb_design_ANOVA_CST <- factor(c(10, 7.5, 5))

options(contrasts=c("contr.sum", "contr.poly"))
pb_aov_ANOVA_CST <- Anova(pb_model_ANOVA_CST, idata=data.frame(pb_design_ANOVA_CST), idesign=~pb_design_ANOVA_CST, type="III")
summary(pb_aov_ANOVA_CST, multivariate=F)


########PostHoc for Corrected Selection Time ######################
###################################################################
pb_data_frame_PH_CST <- data.frame(pb_data_frame_bubble$bubblesize, pb_data_frame_bubble$CorrectedSelectionTime)
pb_aov_PH_CST <- aov(pb_data_frame_bubble$CorrectedSelectionTime ~ pb_data_frame_bubble$bubblesize, pb_data_frame_PH_CST)
summary(pb_aov_PH_CST)
TukeyHSD(pb_aov_PH_CST)


###################Effect Size#####################################
###################################################################

aovES_ST_bubble <- aov(CorrectedSelectionTime ~ factor(bubblesize) + Error(factor(Subject)/factor(bubblesize)), pb_data_frame_PH_CST)
summary(aovES_ST_bubble)
EffecSize<-0.03/(0.03+10.41)
EffecSize

###########################################################################################################################################

########Statistics for bubble size for Selection Error ################################
###################################################################

plot4<-boxplot(SelectionError ~ bubblesize, pb_data_frame_bubble, main="Selection Error", 
               xlab="BubbleSize", ylab="Selection Error")

data_frame_SE_bubble10 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==10,"SelectionError"]
((sum(data_frame_SE_bubble10)/length(data_frame_SE_bubble10))*100)

data_frame_SE_bubble5 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==5,"SelectionError"]
((sum(data_frame_SE_bubble5)/length(data_frame_SE_bubble5))*100)

data_frame_SE_bubble75 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==7.5,"SelectionError"]
((sum(data_frame_SE_bubble75)/length(data_frame_SE_bubble75))*100)


########################################################################################################################################

########ANOVA for Corrected Selection Time(ANOVA with the Sphericity test) ########################
###################################################################################################

pb_data_frame_ANOVA_SE <- data.frame(pb_data_frame_bubble$Subject,pb_data_frame_bubble$bubblesize, pb_data_frame_bubble$SelectionError)
pb_matrix_ANOVA_SE <- with(pb_data_frame_ANOVA_SE, 
                            cbind(
                              SelectionError[bubblesize==10], 
                               
                              SelectionError[bubblesize==7.5],
                              SelectionError[bubblesize==5])) 

pb_matrix_ANOVA_SE<- na.omit(pb_matrix_ANOVA_SE) # remove all NA values
pb_model_ANOVA_SE <- lm(pb_matrix_ANOVA_SE ~ 1)
pb_design_ANOVA_SE <- factor(c(10, 7.5, 5))

options(contrasts=c("contr.sum", "contr.poly"))
pb_aov_ANOVA_SE <- Anova(pb_model_ANOVA_SE, idata=data.frame(pb_design_ANOVA_SE), idesign=~pb_design_ANOVA_SE, type="III")
summary(pb_aov_ANOVA_SE, multivariate=F)


########PostHoc for Corrected Selection Time ######################
###################################################################
pb_data_frame_PH_SE <- data.frame(pb_data_frame_bubble$bubblesize, pb_data_frame_bubble$SelectionError)
pb_aov_PH_SE <- aov(pb_data_frame_bubble$SelectionError ~ pb_data_frame_bubble$bubblesize, pb_data_frame_PH_SE)
summary(pb_aov_PH_SE)
TukeyHSD(pb_aov_PH_SE)


###################Effect Size#####################################
###################################################################

aovES_SE_bubble <- aov(SelectionError ~ factor(bubblesize) + Error(factor(Subject)/factor(bubblesize)), pb_data_frame_PH_SE)
summary(aovES_SE_bubble)
EffecSize<- 3.04/( 3.04+39.40)
EffecSize

#########################################################################################################################################

#####################Pirate Plots - Selection Time######################################

pirateplot(formula = SelectionTime ~ bubblesize, data = pb_data_frame_bubble, main = "Selection Time for Different Bubble Sizes"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

########################Pirate Plots - Selection Error##################################


pirateplot(formula = SelectionError ~ bubblesize, data = pb_data_frame_bubble, main = "Selection Error for Different Bubble Sizes"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)
