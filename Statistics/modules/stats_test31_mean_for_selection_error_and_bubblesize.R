######## Print some statistical numbers for SelectionError ########
###################################################################

# for BubbleSize

df <- marg_PB_data_frame

stat_bubble_small <- df[df$BubbleSize == "Small", "SelectionError"]
summary(stat_bubble_small)
sd(stat_bubble_small)

stat_bubble_medium <- df[df$BubbleSize == "Medium", "SelectionError"]
summary(stat_bubble_medium)
sd(stat_bubble_medium)

stat_bubble_large <- df[df$BubbleSize == "Large", "SelectionError"]
summary(stat_bubble_large)
sd(stat_bubble_large)

rm(df, stat_bubble_small, stat_bubble_medium, stat_bubble_large)
