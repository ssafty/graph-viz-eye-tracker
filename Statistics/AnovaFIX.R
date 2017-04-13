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
SelectionTime = subset(data, (SelectionTime > 0), SelectionTime)
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
data<-data[!(data$SelectionTime < 0),]
#Remove outliers from data frame
data<-data[!(data$SelectionTime > threshold.upper | data$SelectionTime < threshold.lower),]


head(data, nrow(data))


############### Check for selection time normality  ###############
###################################################################

#boxplot(ExperimentClean)
#identify(rep(1, length(ExperimentClean)), ExperimentClean, labels = seq_along(ExperimentClean))
ExperimentClean <- data$SelectionTime

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


data$CorrectedSelectionTime<-ExperimentCorrected

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
    pcData = data[data$Subject==p&data$Condition==c,]
    
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

cleanData = data.frame(Subject, Condition, SelectionTime, SelectionError, CorrectedSelectionTime)

head(cleanData, nrow(cleanData))


#####################ANOVA for Selection Time############################################
plot1<-boxplot(SelectionTime ~ Condition, cleanData, main="Selection Time", 
               xlab="Condition", ylab="Selection Time")

dataST <- data[data$Condition=="EYE","SelectionTime"]
summary(dataST)
sd(dataST)

dataST <- data[data$Condition=="WITHCUSTOMCALIB","SelectionTime"]
summary(dataST)
sd(dataST)

dataST <- data[data$Condition=="MOUSE","SelectionTime"]
summary(dataST)
sd(dataST)

#####################ANOVA for Corrected Selection Time############################################

data1 <- data.frame(Subject, Condition, SelectionTime)
matrix1 <- with(data1, 
                cbind(
                  CorrectedSelectionTime[Condition=="EYE"], 
                  CorrectedSelectionTime[Condition=="WITHCUSTOMCALIB"], 
                  CorrectedSelectionTime[Condition=="MOUSE"])) 
model1 <- lm(matrix1 ~ 1)
design1 <- factor(c("EYE", "WITHCUSTOMCALIB", "MOUSE"))

options(contrasts=c("contr.sum", "contr.poly"))
aov1 <- Anova(model1, idata=data.frame(design1), idesign=~design1, type="III")
summary(aov1, multivariate=F)


#PostHoc
dataPH <- data.frame(Condition, CorrectedSelectionTime)
aovPH <- aov(CorrectedSelectionTime ~ Condition, cleanData)
summary(aovPH)
TukeyHSD(aovPH)

#Effect Size
aovES <- aov(CorrectedSelectionTime ~ factor(Condition) + Error(factor(Subject)/factor(Condition)), data1)
summary(aovES)
EffecSize<-17.875/(17.875+5.558)
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
