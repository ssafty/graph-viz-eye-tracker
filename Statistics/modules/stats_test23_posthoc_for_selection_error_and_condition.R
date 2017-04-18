######## Statistical tests for SelectionError ##################### for 'Condition'
###################################################################

# 2. PostHoc for SelectionError
df <- marg_PC_data_frame
df_posthoc <- df
df_posthoc[5] <- NULL # remove column CorrectedSelectionTime
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$SelectionError ~ df_posthoc$Condition, df_posthoc)
summary(df_posthoc_aov)

xxxx = "
                     Df Sum Sq Mean Sq F value Pr(>F)  
df_posthoc$Condition  2 0.1594 0.07971     4.1 0.0257 *
Residuals            33 0.6415 0.01944                 
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1

"

plot(TukeyHSD(df_posthoc_aov)) #plot for Tukey link -http://www.analyticsforfun.com/2014/06/performing-anova-test-in-r-results-and.html


xxxx = "
  Tukey multiple comparisons of means
    95% family-wise confidence level

Fit: aov(formula = df_posthoc$SelectionError ~ df_posthoc$Condition, data = df_posthoc)

$`df_posthoc$Condition`
                                               diff         lwr         upr     p adj
Custom Calibration-Built-in Calibration  0.04404762 -0.09562371  0.18371894 0.7214642
Mouse & Keyboard-Built-in Calibration   -0.11388889 -0.25356021  0.02578244 0.1278549
Mouse & Keyboard-Custom Calibration     -0.15793651 -0.29760783 -0.01826518 0.0238772

"

# rm
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionError' 
# (F(2,22)=5.765, p<0.01, partial_eta_2 = 0.3438309 with CI=[0.01916242, 0.4524208]).
df <- marg_PC_data_frame
df_effect_size <- aov(df$SelectionError ~ factor(df$Condition) + Error(factor(df$Subject) / factor(df$Condition)), df)
summary(df_effect_size)

xxxx = "

Error: factor(df$Subject)
          Df Sum Sq Mean Sq F value Pr(>F)
Residuals 11 0.3373 0.03067               

Error: factor(df$Subject):factor(df$Condition)
                     Df Sum Sq Mean Sq F value Pr(>F)   
factor(df$Condition)  2 0.1594 0.07971   5.765 0.0097 **
Residuals            22 0.3042 0.01383                  
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1
> 

"

partial_eta_2 = 0.1594 / (0.1594 + 0.3042) # 0.3438309
partial_eta_2
ci.pvaf(F.value = 5.765, df.1 = 2, df.2 = 22, N = nrow(df))

xxxx = "
$Lower.Limit.Proportion.of.Variance.Accounted.for
[1] 0.01916242

$Probability.Less.Lower.Limit
[1] 0.025

$Upper.Limit.Proportion.of.Variance.Accounted.for
[1] 0.4524208

$Probability.Greater.Upper.Limit
[1] 0.025

$Actual.Coverage
[1] 0.95

> 

"

# rm
rm(df, df_effect_size, partial_eta_2)
rm(xxxx)
