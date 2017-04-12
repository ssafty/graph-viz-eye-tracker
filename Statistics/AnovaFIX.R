##################Install the packages ####################


##################Loading the data file ####################

path = "C:/Savitha/HCI/Eye Tracker Data/"
fileNames = list.files(path=paste(path,"data/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, na.strings=c("", "NA"), header=TRUE, sep=",", dec=".", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)

head(dataFrame,15)

#######################################################################################################

data = dataFrame[,c( 
    "participantId"
    ,"condition"
    ,"timeSinceStartup"
    ,"correctNodeHit"
    ,"keypressed"
    #,"calibrationData"
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

data <- na.omit(data) # remove all NA values

data = data[
    data$currentState=="Trial"
   # &data$correctNodeHit=="TRUE"
    &(data$keypressed=="Enter"|data$keypressed=="HAPRING_TIP")
    ,]

#remove wrong data :-(
data<-data[!(data$condition=="MOUSE" & data$keypressed=="HAPRING_TIP"),]
data<-data[!(data$condition=="noCalibrationDataSet"),]

#data <- data[c(-7)] # remove "currentState" column
data <- data[c(-5)] # remove "keypressed" column

head(data,18)

participants = unique(data["participantId"])
participants

conditions = unique(data["condition"])
conditions

states = unique(data["currentState"])
states

################################################################################################
#Calculate selection times

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
        pcData = data[data$participantId==p&data$condition==c,]
        
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

data = data.frame(Subject, Condition, SelectionTime, SelectionError, BubbleSize)

head(data, nrow(data))

################################################################################################
#Calculate outlier thresholds

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

Outliers=list()
ExperimentClean=list()
for(t in unlist(SelectionTime))
{
    if(t > threshold.upper || t < threshold.lower)
    {
        Outliers=c(Outliers, as.numeric(t))
    }
    else
    {
        ExperimentClean=c(ExperimentClean, as.numeric(t))
    }
}
Outliers = unlist(Outliers)
print(length(Outliers))
print(summary(Outliers))

ExperimentClean = unlist(ExperimentClean)
print(length(ExperimentClean))
print(summary(ExperimentClean))

#boxplot(ExperimentClean)
#identify(rep(1, length(ExperimentClean)), ExperimentClean, labels = seq_along(ExperimentClean))

hist(ExperimentClean, breaks="FD")
qqnorm(ExperimentClean)
qqline(ExperimentClean)
shapiro.test(ExperimentClean)
print(ks.test(ExperimentClean, "pnorm", mean=mean(ExperimentClean), sd=sd(ExperimentClean)))

ExperimentCorrected <- ExperimentClean

ExperimentCorrected = log(ExperimentCorrected)
ExperimentCorrected = ExperimentCorrected + abs(summary(ExperimentCorrected)[1])
print(summary(ExperimentCorrected))

hist(ExperimentCorrected, breaks="FD")
qqnorm(ExperimentCorrected)
qqline(ExperimentCorrected)
shapiro.test(ExperimentCorrected)
print(ks.test(ExperimentCorrected, "pnorm", mean=mean(ExperimentCorrected), sd=sd(ExperimentCorrected)))

#Remove wrong selection times from data frame
data<-data[!(data$SelectionTime < 0),]
#Remove outliers from data frame
data<-data[!(data$SelectionTime > threshold.upper | data$SelectionTime < threshold.lower),]

data$CorrectedSelectionTime<-ExperimentCorrected

head(data, nrow(data))

################################################################################################
#Calculate means per condition per participant

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

cleanDataNew <- cleanData
levels(cleanDataNew$Condition)[levels(cleanDataNew$Condition)=="WITHCUSTOMCALIB"] <- "Custom Calibration"
levels(cleanDataNew$Condition)[levels(cleanDataNew$Condition)=="MOUSE"] <- "Mouse & Keyboard"
levels(cleanDataNew$Condition)[levels(cleanDataNew$Condition)=="EYE"] <- "Built-in Calibration"
cleanData <- cleanDataNew

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
install.packages("car")
8

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
install.packages("MBESS")
library(MBESS)
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
install.packages("car")
library(car)
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
install.packages("MBESS")
library(MBESS)
nSamples<-length(unique(data[,"SelectionError"]))
ci.pvaf(F.value=48.24, df.1=2, df.2=30, N=nSamples)


##################################################################################################################
install.packages(c('devtools','curl'))
library(devtools)
install.packages('BayesFactor', dependencies = TRUE)
devtools::install_github('ndphillips/yarrr', build_vignettes = T)

install_github("ndphillips/yarrr")
library("yarrr")
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
