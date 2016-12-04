##################Loading the data file ####################
path = "C:/Savitha/HCI_Project/Statistics/Savitha/"
fileNames = list.files(path=paste(path,"exp1/P1/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, header=TRUE, sep=";", dec=",", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)

head(dataFrame,9)
#######################################################################################################

####################Reading all the columns of the loaded .csv file###########################

data = dataFrame[,c( 
  "Subject"
  ,"Condition"
  ,"Trial"
  ,"DV1"
  ,"DV2"
  ,"DV3"
  ,"EXP1"
  ,"IV1"
  ,"IV2"
)]
head(data)
################################################################################################
#####################ANOVA for Selection Time############################################
aov1 <- aov(DV1 ~ Condition, data)
summary(aov1)
plot1<-boxplot(DV1 ~ Condition, data, main="Selection Time", 
               xlab="Condition", ylab="DV1")

#######################ANOVA for Error Rate#############################################
aov2 <- aov(DV2 ~ Condition, data)
summary(aov2)
plot2<-boxplot(DV2 ~ Condition, data, main="Error Rate", 
               xlab="Condition", ylab="DV2")

#########################ANOVA for Orientation time######################################
aov3 <- aov(DV3 ~ Condition, data)
summary(aov3)
plot3<-boxplot(DV3 ~ Condition, data, main="Orientation Time", 
               xlab="Condition", ylab="DV3")

