
########### Check for corrected selection time normality  #########
###################################################################

attach(mtcars)
par(mfrow = c(2, 2))
hist(data_frame$SelectionTime, breaks = "FD")
hist(data_frame_without_err$SelectionTime, breaks = "FD")
hist(data_frame$CorrectedSelectionTime, breaks = "FD")
hist(data_frame_without_err$CorrectedSelectionTime, breaks = "FD")


############### other checks for selection time         ###########
###################################################################
par(mfrow = c(1, 1))
qqnorm(data_frame$SelectionTime)
qqline(data_frame$SelectionTime)
shapiro.test(data_frame$SelectionTime)
print(ks.test(data_frame$SelectionTime, "pnorm", mean = mean(data_frame$SelectionTime), sd = sd(data_frame$SelectionTime)))

qqnorm(data_frame_without_err$SelectionTime)
qqline(data_frame_without_err$SelectionTime)
shapiro.test(data_frame_without_err$SelectionTime)
print(ks.test(data_frame_without_err$SelectionTime, "pnorm", mean = mean(data_frame_without_err$SelectionTime), sd = sd(data_frame_without_err$SelectionTime)))

qqnorm(data_frame$CorrectedSelectionTime)
qqline(data_frame$CorrectedSelectionTime)
shapiro.test(data_frame$CorrectedSelectionTime)
print(ks.test(data_frame$CorrectedSelectionTime, "pnorm", mean = mean(data_frame$CorrectedSelectionTime), sd = sd(data_frame$CorrectedSelectionTime)))

qqnorm(data_frame_without_err$CorrectedSelectionTime)
qqline(data_frame_without_err$CorrectedSelectionTime)
shapiro.test(data_frame_without_err$CorrectedSelectionTime)
print(ks.test(data_frame_without_err$CorrectedSelectionTime, "pnorm", mean = mean(data_frame_without_err$CorrectedSelectionTime), sd = sd(data_frame_without_err$CorrectedSelectionTime)))


