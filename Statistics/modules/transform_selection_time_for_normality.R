############### transform selection time for normality  ###########
###################################################################

data_frame$CorrectedSelectionTime <- log(data_frame$SelectionTime)
data_frame_without_err$CorrectedSelectionTime <- log(data_frame_without_err$SelectionTime)

data_frame$CorrectedSelectionTime <- data_frame$CorrectedSelectionTime + abs(summary(data_frame$CorrectedSelectionTime)[1])
data_frame_without_err$CorrectedSelectionTime <- data_frame_without_err$CorrectedSelectionTime + abs(summary(data_frame_without_err$CorrectedSelectionTime)[1])
