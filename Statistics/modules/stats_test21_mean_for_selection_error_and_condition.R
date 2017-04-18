######## Print some statistical numbers for SelectionError ########
###################################################################

# for Condition

df <- marg_PC_data_frame

stat_calib_builtin <- df[df$Condition == "Built-in Calibration", "SelectionError"]
summary(stat_calib_builtin)
sd(stat_calib_builtin)

stat_calib_custom <- df[df$Condition == "Custom Calibration", "SelectionError"]
summary(stat_calib_custom)
sd(stat_calib_custom)

stat_calib_mouseKB <- df[df$Condition == "Mouse & Keyboard", "SelectionError"]
summary(stat_calib_mouseKB)
sd(stat_calib_mouseKB)

rm(df, stat_calib_builtin, stat_calib_custom, stat_calib_mouseKB)
