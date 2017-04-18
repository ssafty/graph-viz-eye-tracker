######## Statistical tests for CorrectedSelectionTime ############# for 'BubbleSize'
###################################################################

# 1. ANOVA with the Sphericity test

df_anova <- marg_PB_data_frame_without_err
df_anova[3] <- NULL # remove SelectedTime we only analyze CorrectedSelectionTime
df_anova_matrix <- with(df_anova,
    cbind(
        CorrectedSelectionTime[BubbleSize == "Large"],
        CorrectedSelectionTime[BubbleSize == "Medium"],
        CorrectedSelectionTime[BubbleSize == "Small"]
        )
    )
df_anova_model <- lm(df_anova_matrix ~ 1)
df_anova_design <- factor(c("Large", "Medium", "Small"))

options(contrasts = c("contr.sum", "contr.poly"))
df_anova_aov <- Anova(df_anova_model, idata = data.frame(df_anova_design), idesign = ~df_anova_design, type = "III")

summary(df_anova_aov, multivariate = F)

xxxx = "
Univariate Type III Repeated-Measures ANOVA Assuming Sphericity

                     SS num Df Error SS den Df       F    Pr(>F)    
(Intercept)     177.502      1   2.2069     11 884.725 7.317e-12 ***
df_anova_design   0.267      2   4.4226     22   0.663    0.5253    
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1


Mauchly Tests for Sphericity

                Test statistic p-value
df_anova_design        0.73935 0.22093


Greenhouse-Geisser and Huynh-Feldt Corrections
 for Departure from Sphericity

                 GG eps Pr(>F[GG])
df_anova_design 0.79324     0.4941

                   HF eps Pr(>F[HF])
df_anova_design 0.9049637  0.5118322
"
# rm
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(xxxx)