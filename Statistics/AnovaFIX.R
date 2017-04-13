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

########ANOVA for Corrected Selection Time ########################
###################################################################

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
pc_aov_PH_CST <- aov(pc_data_frame$CorrectedSelectionTime ~ pc_data_frame$Condition, pc_data_frame)
summary(pc_aov_PH_CST)
TukeyHSD(pc_aov_PH_CST)


###################################################################
################################################################### To be tested
###################################################################
pc_data_frame_PH_CST <- data.frame(pc_data_frame$Condition, pc_data_frame$SelectionTime)
pc_aov_PH_CST <- aov(pc_data_frame$SelectionTime ~ pc_data_frame$Condition, pc_data_frame)
summary(pc_aov_PH_CST)
TukeyHSD(pc_aov_PH_CST)

pc_data_frame_PH_CST <- data.frame(data_frame$Condition, data_frame$SelectionTime)
pc_aov_PH_CST <- aov(data_frame$SelectionTime ~ data_frame$Condition, data_frame)
summary(pc_aov_PH_CST)
TukeyHSD(pc_aov_PH_CST)

pc_data_frame_PH_CST <- data.frame(data_frame$Condition, data_frame$CorrectedSelectionTime)
pc_aov_PH_CST <- aov(data_frame$CorrectedSelectionTime ~ data_frame$Condition, data_frame)
summary(pc_aov_PH_CST)
TukeyHSD(pc_aov_PH_CST)
###################################################################
################################################################### To be tested
###################################################################

#Effect Size
aovES <- aov(SelectionTime ~ factor(Condition) + Error(factor(Subject)/factor(Condition)), pc_data_frame_PH_CST)
summary(aovES)
EffecSize<-2054.9/(2054.9+742.4)
EffecSize

nSamples<-length(unique(data[,"CorrectedSelectionTime"]))
ci.pvaf(F.value=48.24, df.1=2, df.2=30, N=nSamples)

#######################ANOVA for Selection Error#############################################

#aov2 <- aov(SelectionError ~ Condition, cleanData)
#summary(aov2)
plot2<-boxplot(SelectionError ~ Condition, cleanData, main="Selection Error", 
               xlab="Condition", ylab="Selection Error")

dataSE <- data[data$Condition=="EYE","SelectionError"]
((sum(dataSE)/length(dataSE))*100)

dataSE <- data[data$Condition=="WITHCUSTOMCALIB","SelectionError"]
((sum(dataSE)/length(dataSE))*100)

dataSE <- data[data$Condition=="MOUSE","SelectionError"]
((sum(dataSE)/length(dataSE))*100)


data2 <- data.frame(Subject, Condition, SelectionError)
matrix2 <- with(data2, 
                cbind(
                  SelectionError[Condition=="EYE"], 
                  SelectionError[Condition=="WITHCUSTOMCALIB"], 
                  SelectionError[Condition=="MOUSE"])) 
model2 <- lm(matrix2 ~ 1)
design2 <- factor(c("EYE", "WITHCUSTOMCALIB", "MOUSE"))

options(contrasts=c("contr.sum", "contr.poly"))
aov2 <- Anova(model2, idata=data.frame(design2), idesign=~design2, type="III")
summary(aov2, multivariate=F)


#PostHoc
dataPHSE <- data.frame(Condition, SelectionError)
aovPHSE <- aov(SelectionError ~ Condition, cleanData)
summary(aovPHSE)
TukeyHSD(aovPHSE)

#Effect Size
aovESSE <- aov(SelectionError ~ factor(Condition) + Error(factor(Subject)/factor(Condition)), data2)
summary(aovESSE)
EffecSize<-45.5/(45.5+209.8)
EffecSize

nSamples<-length(unique(data[,"SelectionError"]))
ci.pvaf(F.value=48.24, df.1=2, df.2=30, N=nSamples)


##################################################################################################################

#####################Pirate Plots - Selection Time######################################

pirateplot(formula = SelectionTime ~ Condition, data = cleanData, main = "Selection Time"
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


pirateplot(formula = SelectionError ~ Condition, data = cleanData, main = "Selection Error"
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


pirateplot(formula = CorrectedSelectionTime ~ Condition, data = cleanData, main = "Selection Time"
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
