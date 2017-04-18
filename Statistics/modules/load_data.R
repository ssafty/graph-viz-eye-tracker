################## Loading the data file ##########################
###################################################################

path = "Eye Tracker Data/"
fileNames = list.files(path = paste(path, "data/", sep = ""), pattern = "*.csv", full.names = TRUE)
fileNames
dataFrameRaw = do.call("rbind", lapply(fileNames, function(x) { read.csv(file = x, na.strings = c("", "NA"), header = TRUE, sep = ",", dec = ".", stringsAsFactors = FALSE) }))
is.data.frame(dataFrameRaw)
ncol(dataFrameRaw)
nrow(dataFrameRaw)
rows = nrow(dataFrameRaw)

head(dataFrameRaw, 15)

# remove unused session variables
rm(fileNames)
rm(path)
rm(rows)