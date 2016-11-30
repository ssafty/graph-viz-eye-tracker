path = "C:/Savitha/HCI_Project/Statistics/Savitha/"
fileNames = list.files(path=paste(path,"exp1/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, header=TRUE, sep=";", dec=",", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)

head(dataFrame,9)

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

 
aov1 <- aov(DV1 ~ Condition, data)
summary(aov1)
plot1<-boxplot(DV1 ~ Condition, data)
