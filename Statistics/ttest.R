
#################Loading the data file################
path = "C:/Savitha/HCI_Project/Statistics/Savitha/"
fileNames = list.files(path=paste(path,"exp1/P1/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, header=TRUE, sep=";", dec=",", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)
head(dataFrame,9)

##############################################################################################


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

##################Welch Upaired two Sample t-test for selection time for Condition 1 and 2############################
t.test(data[data["Condition"]==1,4], data[data["Condition"]==2,4], var.equal = F )
#############################################################################################################
#####################Plots for Selection time###################################################
plot1<-boxplot(DV1 ~ Condition, data, main="Selection Time", 
               xlab="Condition", ylab="DV1")

#######################Plots for Error Rate#############################################
plot2<-boxplot(DV2 ~ Condition, data, main="Error Rate", 
               xlab="Condition", ylab="DV2")

#########################Plots for Orientation time######################################
plot3<-boxplot(DV3 ~ Condition, data, main="Orientation Time", 
               xlab="Condition", ylab="DV3")


