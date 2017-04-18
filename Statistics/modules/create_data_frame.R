################## Creating DataFrame  ############################
###################################################################

data_frame = dataFrameRaw[, c(
  "participantId"
  , "condition"
  , "timeSinceStartup"
  , "correctNodeHit"
  , "keypressed"
#,"calibrationdata_frame"
  , "bubbleSize"
#,"numberNodes"
#,"targetNode"
#,"currentSelectedNode"
  , "currentState"
#,"correctedEyeX"
#,"correctedEyeY"
#,"rawEyeX"
#,"rawEyeY"
)]

data_frame <- na.omit(data_frame) # remove all NA values

data_frame = data_frame[
  data_frame$currentState == "Trial"
# &data_frame$correctNodeHit=="TRUE"
  & (data_frame$keypressed == "Enter" | data_frame$keypressed == "HAPRING_TIP")
  ,]

#remove wrong data_frame :-(
data_frame <- data_frame[!(data_frame$condition == "MOUSE" & data_frame$keypressed == "HAPRING_TIP"),]
data_frame <- data_frame[!(data_frame$condition == "noCalibrationdata_frameSet"),]

#data_frame <- data_frame[c(-7)] # remove "currentState" column
data_frame <- data_frame[c(-5)] # remove "keypressed" column

# rename condition field for readability
data_frame$condition <- as.character(data_frame$condition)
data_frame$condition[data_frame$condition == "WITHCUSTOMCALIB"] <- "Custom Calibration"
data_frame$condition[data_frame$condition == "MOUSE"] <- "Mouse & Keyboard"
data_frame$condition[data_frame$condition == "EYE"] <- "Built-in Calibration"
data_frame$bubbleSize[data_frame$bubbleSize == "10"] <- "Large"
data_frame$bubbleSize[data_frame$bubbleSize == "7.5"] <- "Medium"
data_frame$bubbleSize[data_frame$bubbleSize == "5"] <- "Small"

head(data_frame, 18)

# remove unused variables
rm(dataFrameRaw)