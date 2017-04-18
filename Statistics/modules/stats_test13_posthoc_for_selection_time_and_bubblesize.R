######## Statistical tests for CorrectedSelectionTime ############# for 'BubbleSize'
###################################################################

# 2. PostHoc for Corrected Selection Time
df <- marg_PB_data_frame_without_err
df_posthoc <- df
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$CorrectedSelectionTime ~ df_posthoc$BubbleSize, df_posthoc)
summary(df_posthoc_aov)
xxxx = "
                      Df Sum Sq Mean Sq F value Pr(>F)
df_posthoc$BubbleSize  2  0.267  0.1333   0.663  0.522
Residuals             33  6.630  0.2009               

"

plot(TukeyHSD(df_posthoc_aov)) #plot for Tukey link -http://www.analyticsforfun.com/2014/06/performing-anova-test-in-r-results-and.html


xxxx = "
  Tukey multiple comparisons of means
    95% family-wise confidence level

Fit: aov(formula = df_posthoc$CorrectedSelectionTime ~ df_posthoc$BubbleSize, data = df_posthoc)

$`df_posthoc$BubbleSize`
                    diff        lwr       upr     p adj
Medium-Large -0.07485429 -0.5238553 0.3741467 0.9121311
Small-Large  -0.20806648 -0.6570675 0.2409345 0.4985691
Small-Medium -0.13321219 -0.5822132 0.3157888 0.7487798
"

# rm
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionTime' 
# (F(2,22)=0.663, p<0.01, partial_eta_2 = 0.05692964 with CI=[0, 0.1836511]).
df <- marg_PB_data_frame_without_err
df_effect_size <- aov(df$CorrectedSelectionTime ~ factor(df$BubbleSize) + Error(factor(df$Subject) / factor(df$BubbleSize)), df)
summary(df_effect_size)

xxxx = "

Error: factor(df$Subject)
          Df Sum Sq Mean Sq F value Pr(>F)
Residuals 11  2.207  0.2006               

Error: factor(df$Subject):factor(df$BubbleSize)
                      Df Sum Sq Mean Sq F value Pr(>F)
factor(df$BubbleSize)  2  0.267  0.1333   0.663  0.525
Residuals             22  4.423  0.2010               

"
partial_eta_2 = 0.267 / (0.267 + 4.423) # 0.05692964
partial_eta_2
ci.pvaf(F.value = 0.663, df.1 = 2, df.2 = 22, N = nrow(df))

xxxx = "
$Lower.Limit.Proportion.of.Variance.Accounted.for
[1] 0

$Probability.Less.Lower.Limit
[1] 0

$Upper.Limit.Proportion.of.Variance.Accounted.for
[1] 0.1836511

$Probability.Greater.Upper.Limit
[1] 0.025

$Actual.Coverage
[1] 0.975

"
# rm
rm(df, df_effect_size, partial_eta_2)
rm(xxxx)