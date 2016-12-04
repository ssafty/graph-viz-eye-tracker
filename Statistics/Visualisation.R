
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


########################Loading libraries required for visulisation#######################

library(devtools)

install_github("ndphillips/yarrr")
library("yarrr")
############################################################################
STData = dataFrame[,c( 
  #"Subject"
  "Condition"
  #,"Trial"
  ,"DV1"
  #,"DV2"
  #,"DV3"
  #,"EXP1"
  #,"IV1"
  #,"IV2"
)]
head(STData)
#############################Pirate plots###########################################

pirateplot(formula = DV1~Condition, data = STData, main = "Selection Time"
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

