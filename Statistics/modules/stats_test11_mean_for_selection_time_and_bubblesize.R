######## Print some statistical numbers for SelectionTime #########
###################################################################

# for BubbleSize

df <- marg_PB_data_frame_without_err

stat_bubble_small <- df[df$BubbleSize == "Small", "SelectionTime"]
summary(stat_bubble_small)
sd(stat_bubble_small)

stat_bubble_medium <- df[df$BubbleSize == "Medium", "SelectionTime"]
summary(stat_bubble_medium)
sd(stat_bubble_medium)

stat_bubble_large <- df[df$BubbleSize == "Large", "SelectionTime"]
summary(stat_bubble_large)
sd(stat_bubble_large)

rm(df, stat_bubble_small, stat_bubble_medium, stat_bubble_large)