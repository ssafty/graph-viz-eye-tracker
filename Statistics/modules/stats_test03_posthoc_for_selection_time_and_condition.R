######## Statistical tests for CorrectedSelectionTime ############# for 'Condition'
###################################################################

# 2. PostHoc for Corrected Selection Time
df <- marg_PC_data_frame_without_err
df_posthoc <- df
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$CorrectedSelectionTime ~ df_posthoc$Condition, df_posthoc)
summary(df_posthoc_aov)

xxxx = "

                     Df Sum Sq Mean Sq F value  Pr(>F)    
df_posthoc$Condition  2  14.40   7.200    39.4 1.8e-09 ***
Residuals            33   6.03   0.183                    
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1

"

#Tukey result significant, will be included in paper
plot(TukeyHSD(df_posthoc_aov)) #plot for Tukey link -http://www.analyticsforfun.com/2014/06/performing-anova-test-in-r-results-and.html




xxxx = "
  Tukey multiple comparisons of means
    95% family-wise confidence level

Fit: aov(formula = df_posthoc$CorrectedSelectionTime ~ df_posthoc$Condition, data = df_posthoc)

$`df_posthoc$Condition`
                                             diff        lwr         upr     p adj
Custom Calibration-Built-in Calibration -0.506844 -0.9350616 -0.07862647 0.0174508
Mouse & Keyboard-Built-in Calibration   -1.521183 -1.9494010 -1.09296591 0.0000000
Mouse & Keyboard-Custom Calibration     -1.014339 -1.4425570 -0.58612187 0.0000049

"

# rm
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionTime' 
# (F(2,22)=39.18, p<0.01, partial_eta_2 = 0.7807722 with CI=[0.4545357, 0.8006797]).
df <- marg_PC_data_frame_without_err
df_effect_size <- aov(df$CorrectedSelectionTime ~ factor(df$Condition) + Error(factor(df$Subject) / factor(df$Condition)), df)
summary(df_effect_size)

xxxx = "

Error: factor(df$Subject)
          Df Sum Sq Mean Sq F value Pr(>F)
Residuals 11  1.987  0.1807               

Error: factor(df$Subject):factor(df$Condition)
                     Df Sum Sq Mean Sq F value   Pr(>F)    
factor(df$Condition)  2 14.399   7.200   39.18 5.62e-08 ***
Residuals            22  4.043   0.184                     
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1

"

partial_eta_2 = 14.399 / (14.399 + 4.043) # 0.7807722
partial_eta_2
ci.pvaf(F.value = 39.18, df.1 = 2, df.2 = 22, N = nrow(df))

xxxx = "
$Lower.Limit.Proportion.of.Variance.Accounted.for
[1] 0.4545357

$Probability.Less.Lower.Limit
[1] 0.025

$Upper.Limit.Proportion.of.Variance.Accounted.for
[1] 0.8006797

$Probability.Greater.Upper.Limit
[1] 0.025

$Actual.Coverage
[1] 0.95

"

# rm
rm(df, df_effect_size, partial_eta_2)
rm(xxxx)