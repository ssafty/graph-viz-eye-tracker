############### Extract data_frame without errors #################
###################################################################
data_frame_without_err <- data_frame[(data_frame$SelectionError == 0),]
data_frame_without_err[4] <- NULL # remove selection error column