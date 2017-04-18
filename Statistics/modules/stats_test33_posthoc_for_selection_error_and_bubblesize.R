######## Statistical tests for SelectionError ##################### for 'BubbleSize'
###################################################################

# 2. PostHoc for SelectionError
df <- marg_PB_data_frame
df_posthoc <- df
df_posthoc[5] <- NULL # remove column CorrectedSelectionTime
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$SelectionError ~ df_posthoc$BubbleSize, df_posthoc)
summary(df_posthoc_aov)

xxxx = "
                      Df Sum Sq  Mean Sq F value Pr(>F)
df_posthoc$BubbleSize  2 0.0185 0.009264   0.457  0.637
Residuals             33 0.6689 0.020270

"

plot(TukeyHSD(df_posthoc_aov)) #plot for Tukey link -http://www.analyticsforfun.com/2014/06/performing-anova-test-in-r-results-and.html


xxxx = "
  Tukey multiple comparisons of means
    95% family-wise confidence level

Fit: aov(formula = df_posthoc$SelectionError ~ df_posthoc$BubbleSize, data = df_posthoc)

$`df_posthoc$BubbleSize`
                    diff        lwr        upr     p adj
Medium-Large -0.01329365 -0.1559183 0.12933104 0.9716002
Small-Large  -0.05337302 -0.1959977 0.08925167 0.6327814
Small-Medium -0.04007937 -0.1827041 0.10254532 0.7711949
"

# rm
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'BubbleSize' on Value 'SelectionError' 
# (F(2,22)=0.64, p<0.01, partial_eta_2 = 0.05494505 with CI=[0, 0.1809875]).
df <- marg_PB_data_frame
df_effect_size <- aov(df$SelectionError ~ factor(df$BubbleSize) + Error(factor(df$Subject) / factor(df$BubbleSize)), df)
summary(df_effect_size)

xxxx = "


Error: factor(df$Subject)
          Df Sum Sq Mean Sq F value Pr(>F)
Residuals 11 0.3507 0.03188               

Error: factor(df$Subject):factor(df$BubbleSize)
                      Df Sum Sq  Mean Sq F value Pr(>F)
factor(df$BubbleSize)  2 0.0185 0.009264    0.64  0.537
Residuals             22 0.3182 0.014466               
> 

"

partial_eta_2 = 0.0185 / (0.0185 + 0.3182) # 0.05494505
partial_eta_2
ci.pvaf(F.value = 0.64, df.1 = 2, df.2 = 22, N = nrow(df))

xxxx = "
$Lower.Limit.Proportion.of.Variance.Accounted.for
[1] 0

$Probability.Less.Lower.Limit
[1] 0

$Upper.Limit.Proportion.of.Variance.Accounted.for
[1] 0.1809875

$Probability.Greater.Upper.Limit
[1] 0.025

$Actual.Coverage
[1] 0.975

> 

"

# rm
rm(df, df_effect_size, partial_eta_2)
rm(xxxx)