######## Statistical tests for SelectionError ##################### for 'Condition'
###################################################################

# 1. ANOVA with the Sphericity test

df_anova <- marg_PC_data_frame
df_anova[5] <- NULL # remove CorrectedSelectedTime we only analyze SelectionError
df_anova[3] <- NULL # remove SelectedTime we only analyze SelectionError
df_anova_matrix <- with(df_anova,
    cbind(
        SelectionError[Condition == "Built-in Calibration"],
        SelectionError[Condition == "Custom Calibration"],
        SelectionError[Condition == "Mouse & Keyboard"]
        )
    )
df_anova_model <- lm(df_anova_matrix ~ 1)
df_anova_design <- factor(c("Built-in Calibration", "Custom Calibration", "Mouse & Keyboard"))

options(contrasts = c("contr.sum", "contr.poly"))
df_anova_aov <- Anova(df_anova_model, idata = data.frame(df_anova_design), idesign = ~df_anova_design, type = "III")

summary(df_anova_aov, multivariate = F)

xxxx = "

Univariate Type III Repeated-Measures ANOVA Assuming Sphericity

                     SS num Df Error SS den Df      F  Pr(>F)   
(Intercept)     0.29556      1  0.33735     11 9.6373 0.01003 * 
df_anova_design 0.15942      2  0.30416     22 5.7654 0.00970 **
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1


Mauchly Tests for Sphericity

                Test statistic p-value
df_anova_design        0.95402  0.7903


Greenhouse-Geisser and Huynh-Feldt Corrections
 for Departure from Sphericity

                 GG eps Pr(>F[GG])  
df_anova_design 0.95604    0.01085 *
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1

                  HF eps  Pr(>F[HF])
df_anova_design 1.152357 0.009700145

"
# rm
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(xxxx)