############### Check for selection time normality  ###############
###################################################################


attach(mtcars)
par(mfrow = c(1, 2))
hist(data_frame$SelectionTime, breaks = "FD")
hist(data_frame_without_err$SelectionTime, breaks = "FD")