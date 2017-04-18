############### Subject xx Seems to be wrong ######################
###################################################################
#data_frame <- data_frame[!(data_frame$Subject == "14" & data_frame$Condition == "Custom Calibration"),]
#data_frame <- data_frame[!(data_frame$Subject == "15"),]
data_frame <- data_frame[!(data_frame$Subject == "8"),]
data_frame <- data_frame[!(data_frame$Subject == "9"),]
data_frame <- data_frame[!(data_frame$Subject == "11"),]
data_frame <- data_frame[!(data_frame$Subject == "14"),]
unique(data_frame$Subject)