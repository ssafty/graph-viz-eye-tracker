######## Statistical tests for CorrectedSelectionTime ############# for 'Condition'
###################################################################

# 1. ANOVA with the Sphericity test

df_anova <- marg_PC_data_frame_without_err
df_anova[3] <- NULL # remove SelectedTime we only analyze CorrectedSelectionTime
df_anova_matrix <- with(df_anova,
    cbind(
        CorrectedSelectionTime[Condition == "Built-in Calibration"],
        CorrectedSelectionTime[Condition == "Custom Calibration"],
        CorrectedSelectionTime[Condition == "Mouse & Keyboard"]
        )
    )
df_anova_model <- lm(df_anova_matrix ~ 1)
df_anova_design <- factor(c("Built-in Calibration", "Custom Calibration", "Mouse & Keyboard"))

options(contrasts = c("contr.sum", "contr.poly"))
df_anova_aov <- Anova(df_anova_model, idata = data.frame(df_anova_design), idesign = ~df_anova_design, type = "III")

summary(df_anova_aov, multivariate = F)

xxxx = "

Univariate Type III Repeated - Measures ANOVA Assuming Sphericity

SS num Df Error SS den Df F Pr( > F)
(Intercept) 190.574 1 1.9871 11 1054.944 2.808e-12 ** *
df_anova_design 14.399 2 4.0429 22 39.178 5.620e-08 ** *
---
Signif. codes:0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1


Mauchly Tests for
    Sphericity

    Test statistic p - value
df_anova_design 0.84886 0.44075


Greenhouse - Geisser and Huynh - Feldt Corrections
for
    Departure from Sphericity

GG eps Pr( > F[GG])
df_anova_design 0.86871 3.495e-07 ** *
---
Signif. codes:0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1

HF eps Pr( > F[HF])
df_anova_design 1.017478 5.619662e-08

"
# rm
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(xxxx)
