###################################################################
###################################################################
###################################################################
###################################################################
###################################################################
################################################################### All pirate plots
# the plots for paper SelectionTime
attach(mtcars)
par(mfrow = c(2, 1))
par(mfrow = c(1, 2))
boxplot(SelectionTime ~ Condition, marg_PC_data_frame_without_err, main = "Selection time vs. Condtitions",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame_without_err, main = "Selection time vs. Bubble size",
               xlab = "Bubble Size", ylab = "Selection Time")

par(mfrow = c(1, 1)) #, main = "Selection_Time_for_Different_Conditions"
pdf("images/Selection_Time_for_Different_Conditions.pdf", width = 7.32, height = 5.05)
pirateplot(formula = SelectionTime ~ Condition, data = marg_PC_data_frame_without_err,
    xlab = "",
    theme = 2, # theme 2
    pal = "google", # xmen palette
    point.o = .4, # Add points
    point.col = "black",
    point.bg = "white",
    point.pch = 21,
    bean.f.o = .2, # Turn down bean filling
    inf.f.o = .8, # Turn up inf filling
    gl.col = "gray", # gridlines
    gl.lwd = c(.5, 0)) # turn off minor grid lines)
dev.off()

par(mfrow = c(1, 1)) #, main = "Selection Time for Different Bubble Sizes"
pdf("images/Selection_Time_for_Different_Bubble_Sizes.pdf", width = 7.32, height = 5.05)
pirateplot(formula = SelectionTime ~ BubbleSize, data = marg_PB_data_frame_without_err,
    xlab = "",
    theme = 2, # theme 2
    pal = "google", # xmen palette
    point.o = .4, # Add points
    point.col = "black",
    point.bg = "white",
    point.pch = 21,
    bean.f.o = .2, # Turn down bean filling
    inf.f.o = .8, # Turn up inf filling
    gl.col = "gray", # gridlines
    gl.lwd = c(.5, 0)) # turn off minor grid lines)
dev.off()

# the plots for paper SelectionError
attach(mtcars)
par(mfrow = c(2, 1))
par(mfrow = c(1, 2))
boxplot(SelectionError ~ Condition, marg_PC_data_frame, main = "Selection Error vs. Condtitions",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionError ~ BubbleSize, marg_PB_data_frame, main = "Selection Error vs. Bubble size",
               xlab = "Bubble Size", ylab = "Selection Time")

par(mfrow = c(1, 1)) #, main = "Selection Error for Different Conditions"
pdf("images/Selection_Error_for_Different_Conditions.pdf", width = 7.32, height = 5.05)
pirateplot(formula = SelectionError ~ Condition, data = marg_PC_data_frame,
    xlab = "",
    theme = 2, # theme 2
    pal = "google", # xmen palette
    point.o = .4, # Add points
    point.col = "black",
    point.bg = "white",
    point.pch = 21,
    bean.f.o = .2, # Turn down bean filling
    inf.f.o = .8, # Turn up inf filling
    gl.col = "gray", # gridlines
    gl.lwd = c(.5, 0)) # turn off minor grid lines)
dev.off()

par(mfrow = c(1, 1)) #, main = "Selection Error for Different Bubble Sizes"
pdf("images/Selection_Error_for_Different_Bubble_Sizes.pdf", width = 7.32, height = 5.05)
pirateplot(formula = SelectionError ~ BubbleSize, data = marg_PB_data_frame,
    xlab = "",
    theme = 2, # theme 2
    pal = "google", # xmen palette
    point.o = .4, # Add points
    point.col = "black",
    point.bg = "white",
    point.pch = 21,
    bean.f.o = .2, # Turn down bean filling
    inf.f.o = .8, # Turn up inf filling
    gl.col = "gray", # gridlines
    gl.lwd = c(.5, 0)) # turn off minor grid lines)
dev.off()

######## Overview of all data using boxplots   #################### Debugging only for SelectionTime
###################################################################
attach(mtcars)
par(mfrow = c(2, 2))
boxplot(SelectionTime ~ Condition, data_frame, main = "No Marginal & with errors",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ Condition, data_frame_without_err, main = "No Marginal & without errors",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ Condition, marg_PC_data_frame, main = "Marginal & with errors",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ Condition, marg_PC_data_frame_without_err, main = "Marginal & without errors",
               xlab = "Condition", ylab = "Selection Time")

attach(mtcars)
par(mfrow = c(2, 2))
boxplot(SelectionTime ~ BubbleSize, data_frame, main = "No Marginal & with errors",
               xlab = "Bubble Size", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, data_frame_without_err, main = "No Marginal & without errors",
               xlab = "Bubble Size", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame, main = "Marginal & with errors",
               xlab = "Bubble Size", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame_without_err, main = "Marginal & without errors",
               xlab = "Bubble Size", ylab = "Selection Time")

###################################################################
###################################################################
###################################################################
###################################################################
###################################################################
