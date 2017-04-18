######## Statistical tests for SelectionError ##################### for 'BubbleSize'
###################################################################

# 1. ANOVA with the Sphericity test

df_anova <- marg_PB_data_frame
df_anova[5] <- NULL # remove CorrectedSelectedTime we only analyze SelectionError
df_anova[3] <- NULL # remove SelectedTime we only analyze SelectionError
df_anova_matrix <- with(df_anova,
    cbind(
        SelectionError[BubbleSize == "Large"],
        SelectionError[BubbleSize == "Medium"],
        SelectionError[BubbleSize == "Small"]
        )
    )
df_anova_model <- lm(df_anova_matrix ~ 1)
df_anova_design <- factor(c("Large", "Medium", "Small"))

options(contrasts = c("contr.sum", "contr.poly"))
df_anova_aov <- Anova(df_anova_model, idata = data.frame(df_anova_design), idesign = ~df_anova_design, type = "III")

summary(df_anova_aov, multivariate = F)

xxxx = "
Univariate Type III Repeated-Measures ANOVA Assuming Sphericity

                     SS num Df Error SS den Df       F   Pr(>F)   
(Intercept)     0.32246      1  0.35068     11 10.1147 0.008755 **
df_anova_design 0.01853      2  0.31824     22  0.6404 0.536635   
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1


Mauchly Tests for Sphericity

                Test statistic p-value
df_anova_design        0.84712 0.43623


Greenhouse-Geisser and Huynh-Feldt Corrections
 for Departure from Sphericity

                 GG eps Pr(>F[GG])
df_anova_design 0.86739     0.5167

                  HF eps Pr(>F[HF])
df_anova_design 1.015483  0.5366346

"
# rm
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(xxxx)