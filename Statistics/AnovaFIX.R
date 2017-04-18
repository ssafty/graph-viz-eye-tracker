################## Install the packages ###########################
###################################################################
install.packages("car")
install.packages("MBESS")
install.packages(c('devtools','curl'))
install.packages('BayesFactor', dependencies = TRUE)
devtools::install_github('ndphillips/yarrr', build_vignettes = T)
install_github("ndphillips/yarrr")
install.packages("plyr")
install.packages("yarrr")
library(yarrr)
library(car)
library(MBESS)
library(devtools)
library(plyr)

################## Loading the data file ##########################
###################################################################

path = "Eye Tracker Data/"
fileNames = list.files(path=paste(path,"data/", sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrameRaw = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, na.strings=c("", "NA"), header=TRUE, sep=",", dec=".", stringsAsFactors=FALSE)}))
is.data.frame(dataFrameRaw)
ncol(dataFrameRaw)
nrow(dataFrameRaw)
rows = nrow(dataFrameRaw)

head(dataFrameRaw, 15)

# remove unused session variables
rm(fileNames)
rm(path)
rm(rows)

################## Creating DataFrame  ############################
###################################################################

data_frame = dataFrameRaw[,c( 
  "participantId"
  ,"condition"
  ,"timeSinceStartup"
  ,"correctNodeHit"
  ,"keypressed"
  #,"calibrationdata_frame"
  ,"bubbleSize"
  #,"numberNodes"
  #,"targetNode"
  #,"currentSelectedNode"
  ,"currentState"
  #,"correctedEyeX"
  #,"correctedEyeY"
  #,"rawEyeX"
  #,"rawEyeY"
)]

data_frame <- na.omit(data_frame) # remove all NA values

data_frame = data_frame[
  data_frame$currentState=="Trial"
  # &data_frame$correctNodeHit=="TRUE"
  &(data_frame$keypressed=="Enter"|data_frame$keypressed=="HAPRING_TIP")
  ,]

#remove wrong data_frame :-(
data_frame<-data_frame[!(data_frame$condition=="MOUSE" & data_frame$keypressed=="HAPRING_TIP"),]
data_frame<-data_frame[!(data_frame$condition=="noCalibrationdata_frameSet"),]

#data_frame <- data_frame[c(-7)] # remove "currentState" column
data_frame <- data_frame[c(-5)] # remove "keypressed" column

# rename condition field for readability
data_frame$condition <- as.character(data_frame$condition)
data_frame$condition[data_frame$condition == "WITHCUSTOMCALIB"] <- "Custom Calibration"
data_frame$condition[data_frame$condition == "MOUSE"] <- "Mouse & Keyboard"
data_frame$condition[data_frame$condition == "EYE"] <- "Built-in Calibration"
data_frame$bubbleSize[data_frame$bubbleSize == "10"] <- "Large"
data_frame$bubbleSize[data_frame$bubbleSize == "7.5"] <- "Medium"
data_frame$bubbleSize[data_frame$bubbleSize == "5"] <- "Small"

head(data_frame, 18)

# remove unused variables
rm(dataFrameRaw)

################## Calculate selection times  #####################
###################################################################

# get unique values
u_participants = unique(data_frame["participantId"])
u_conditions = unique(data_frame["condition"])
u_states = unique(data_frame["currentState"])

t_Subject=list()
t_Condition = list()
t_SelectionTime = list()
t_SelectionError = list()
t_BubbleSize = list()

t_lastTime=0;

for(p in unlist(u_participants))
{
    for(c in unlist(u_conditions))
    {
        t_pcData = data_frame[data_frame$participantId==p&data_frame$condition==c,]
        # typeof(pcData)
    
        t_firstRow = TRUE
    
        for (i in 1:nrow(t_pcData))
        {
            t_row <- t_pcData[i,]
      
            if (t_firstRow == FALSE)
            {
                t_Subject = c(t_Subject, p)
                t_Condition = c(t_Condition, c)
                t_SelectionTime = c(t_SelectionTime, as.numeric(t_row["timeSinceStartup"]) - t_lastTime)
                if (t_row["correctNodeHit"] == TRUE)
                    t_SelectionError = c(t_SelectionError, 0)
                else
                    t_SelectionError = c(t_SelectionError, 1)
                t_BubbleSize = c(t_BubbleSize, t_row["bubbleSize"])
            }
      
            t_lastTime = as.numeric(t_row["timeSinceStartup"])
            t_firstRow = FALSE
        }
    }
}

Subject = unlist(t_Subject)
Condition = unlist(t_Condition)
SelectionTime = unlist(t_SelectionTime)
SelectionError = unlist(t_SelectionError)
BubbleSize = unlist(t_BubbleSize)

data_frame = data.frame(Subject, Condition, SelectionTime, SelectionError, BubbleSize)

head(data_frame, nrow(data_frame))

# remove unused variables
rm(u_conditions)
rm(u_participants)
rm(u_states)
rm(t_Subject)
rm(t_Condition)
rm(t_SelectionTime)
rm(t_SelectionError)
rm(t_BubbleSize)
rm(t_lastTime)
rm(t_firstRow)
rm(t_pcData)
rm(t_row)
rm(c)
rm(i)
rm(p)

############### Calculate outlier thresholds  #####################
###################################################################

t_threshold.upper = 0;
t_threshold.lower = 0;
t_threshold.factor = 1.5

# Remove negative times...
SelectionTime = subset(data_frame, (SelectionTime > 0), SelectionTime)
SelectionTime = unlist(SelectionTime)
print(length(SelectionTime))
print(summary(SelectionTime))

t_lowerq = quantile(SelectionTime)[2]
t_upperq = quantile(SelectionTime)[4]
t_iqr = t_upperq - t_lowerq # IQR(ExperimentDiscrepancy) can be used as an alternative

t_threshold.upper = (t_iqr * t_threshold.factor) + t_upperq
print(t_threshold.upper)
t_threshold.lower = t_lowerq - (t_iqr * t_threshold.factor)
print(t_threshold.lower)

# rm
rm(Subject)
rm(Condition)
rm(SelectionTime)
rm(SelectionError)
rm(BubbleSize)
rm(t_iqr, t_lowerq, t_upperq)

############### Remove outliers from data     #####################
###################################################################

#Remove wrong selection times from data frame
data_frame<-data_frame[!(data_frame$SelectionTime < 0),]
#Remove outliers from data frame
data_frame <- data_frame[!(data_frame$SelectionTime > t_threshold.upper | data_frame$SelectionTime < t_threshold.lower),]


head(data_frame, nrow(data_frame))

# rm
rm(t_threshold.factor, t_threshold.lower, t_threshold.upper)

############### Subject xx Seems to be wrong ######################
###################################################################
#data_frame <- data_frame[!(data_frame$Subject == "14" & data_frame$Condition == "Custom Calibration"),]
#data_frame <- data_frame[!(data_frame$Subject == "15"),]
data_frame <- data_frame[!(data_frame$Subject == "8"),]
data_frame <- data_frame[!(data_frame$Subject == "9"),]
data_frame <- data_frame[!(data_frame$Subject == "11"),]
data_frame <- data_frame[!(data_frame$Subject == "14"),]
unique(data_frame$Subject)

############### Extract data_frame without errors #################
###################################################################
data_frame_without_err <- data_frame[(data_frame$SelectionError == 0),]
data_frame_without_err[4] <- NULL # remove selection error column


############### Check for selection time normality  ###############
###################################################################

ExperimentClean <- data_frame$SelectionTime
ExperimentClean_without_err <- data_frame_without_err$SelectionTime

hist(ExperimentClean, breaks = "FD")
hist(ExperimentClean_without_err, breaks = "FD")



############### transform selection time for normality  ###########
###################################################################

ExperimentCorrected <- ExperimentClean
ExperimentCorrected_without_error <- ExperimentClean_without_err

ExperimentCorrected = log(ExperimentCorrected)
ExperimentCorrected_without_error = log(ExperimentCorrected_without_error)

ExperimentCorrected = ExperimentCorrected + abs(summary(ExperimentCorrected)[1])
ExperimentCorrected_without_error = ExperimentCorrected_without_error + abs(summary(ExperimentCorrected_without_error)[1])

print(summary(ExperimentCorrected))
print(summary(ExperimentCorrected_without_error))

hist(ExperimentCorrected, breaks = "FD")
hist(ExperimentCorrected_without_error, breaks = "FD")

############### other checks for selection time         ###########
###################################################################
qqnorm(ExperimentClean)
qqline(ExperimentClean)
shapiro.test(ExperimentClean)
print(ks.test(ExperimentClean, "pnorm", mean = mean(ExperimentClean), sd = sd(ExperimentClean)))

qqnorm(ExperimentCorrected)
qqline(ExperimentCorrected)
shapiro.test(ExperimentCorrected)
print(ks.test(ExperimentCorrected, "pnorm", mean=mean(ExperimentCorrected), sd=sd(ExperimentCorrected)))

############### Store back corrected data         #################
###################################################################

data_frame$CorrectedSelectionTime <- ExperimentCorrected
data_frame_without_err$CorrectedSelectionTime <- ExperimentCorrected_without_error

# rm 
rm(ExperimentClean)
rm(ExperimentCorrected)
rm(ExperimentClean_without_err)
rm(ExperimentCorrected_without_error)

########Calculate marginals per condition per participant##########
###################################################################

Subject = list()
Condition = list()
SelectionTime = list()
SelectionError = list()
CorrectedSelectionTime = list()

# get unique values
u_participants = unique(data_frame["Subject"])
u_conditions = unique(data_frame["Condition"])


for (p in unlist(u_participants)) {
    for (c in unlist(u_conditions)) {
        pcData = data_frame[data_frame$Subject == p & data_frame$Condition == c,]

        Subject = c(Subject, p)
        Condition = c(Condition, c)
        SelectionTime = c(SelectionTime, mean(unlist(pcData["SelectionTime"])))
        SelectionError = c(SelectionError, mean(unlist(pcData["SelectionError"]))) # not sum use mean as we are interested in fraction of errors
        CorrectedSelectionTime = c(CorrectedSelectionTime, mean(unlist(pcData["CorrectedSelectionTime"])))
    }
}

Subject = unlist(Subject)
Condition = unlist(Condition)
SelectionTime = unlist(SelectionTime)
SelectionError = unlist(SelectionError)
CorrectedSelectionTime = unlist(CorrectedSelectionTime)

marg_PC_data_frame = data.frame(Subject, Condition, SelectionTime, SelectionError, CorrectedSelectionTime)

head(marg_PC_data_frame, nrow(marg_PC_data_frame))

# rm
rm(Subject)
rm(Condition)
rm(SelectionTime)
rm(SelectionError)
rm(CorrectedSelectionTime)
rm(p)
rm(c)
rm(pcData)
rm(u_conditions)
rm(u_participants)

########Calculate marginals per condition per participant########## without error case
###################################################################

Subject = list()
Condition = list()
SelectionTime = list()
CorrectedSelectionTime = list()

# get unique values
u_participants = unique(data_frame["Subject"])
u_conditions = unique(data_frame["Condition"])


for (p in unlist(u_participants)) {
    for (c in unlist(u_conditions)) {
        pcData = data_frame_without_err[data_frame_without_err$Subject == p & data_frame_without_err$Condition == c,]

        Subject = c(Subject, p)
        Condition = c(Condition, c)
        SelectionTime = c(SelectionTime, mean(unlist(pcData["SelectionTime"])))
        CorrectedSelectionTime = c(CorrectedSelectionTime, mean(unlist(pcData["CorrectedSelectionTime"])))
    }
}

Subject = unlist(Subject)
Condition = unlist(Condition)
SelectionTime = unlist(SelectionTime)
CorrectedSelectionTime = unlist(CorrectedSelectionTime)

marg_PC_data_frame_without_err = data.frame(Subject, Condition, SelectionTime, CorrectedSelectionTime)

head(marg_PC_data_frame_without_err, nrow(marg_PC_data_frame_without_err))

# rm
rm(Subject)
rm(Condition)
rm(SelectionTime)
rm(CorrectedSelectionTime)
rm(p)
rm(c)
rm(pcData)
rm(u_conditions)
rm(u_participants)

######## Calculate marginals per BubbleSize per participant #######
###################################################################

Subject = list()
BubbleSize = list()
SelectionTime = list()
SelectionError = list()
CorrectedSelectionTime = list()

# get unique values
u_participants = unique(data_frame["Subject"])
u_bubblesize = unique(data_frame["BubbleSize"])


for (p in unlist(u_participants)) {
    for (b in unlist(u_bubblesize)) {
        pbData = data_frame[data_frame$Subject == p & data_frame$BubbleSize == b,]

        Subject = c(Subject, p)
        BubbleSize = c(BubbleSize, b)
        SelectionTime = c(SelectionTime, mean(unlist(pbData["SelectionTime"])))
        SelectionError = c(SelectionError, mean(unlist(pbData["SelectionError"]))) # not sum use mean as we are interested in fraction of errors
        CorrectedSelectionTime = c(CorrectedSelectionTime, mean(unlist(pbData["CorrectedSelectionTime"])))
    }
}

Subject = unlist(Subject)
BubbleSize = unlist(BubbleSize)
SelectionTime = unlist(SelectionTime)
SelectionError = unlist(SelectionError)
CorrectedSelectionTime = unlist(CorrectedSelectionTime)

marg_PB_data_frame = data.frame(Subject, BubbleSize, SelectionTime, SelectionError, CorrectedSelectionTime)

head(marg_PB_data_frame, nrow(marg_PB_data_frame))

# rm
rm(Subject)
rm(BubbleSize)
rm(SelectionTime)
rm(SelectionError)
rm(CorrectedSelectionTime)
rm(p)
rm(b)
rm(pbData)
rm(u_bubblesize)
rm(u_participants)

######## Calculate marginals per BubbleSize per participant ####### without error case
###################################################################

Subject = list()
BubbleSize = list()
SelectionTime = list()
CorrectedSelectionTime = list()

# get unique values
u_participants = unique(data_frame["Subject"])
u_bubblesize = unique(data_frame["BubbleSize"])


for (p in unlist(u_participants)) {
    for (b in unlist(u_bubblesize)) {
        pbData = data_frame_without_err[data_frame_without_err$Subject == p & data_frame_without_err$BubbleSize == b,]

        Subject = c(Subject, p)
        BubbleSize = c(BubbleSize, b)
        SelectionTime = c(SelectionTime, mean(unlist(pbData["SelectionTime"])))
        CorrectedSelectionTime = c(CorrectedSelectionTime, mean(unlist(pbData["CorrectedSelectionTime"])))
    }
}

Subject = unlist(Subject)
BubbleSize = unlist(BubbleSize)
SelectionTime = unlist(SelectionTime)
CorrectedSelectionTime = unlist(CorrectedSelectionTime)

marg_PB_data_frame_without_err = data.frame(Subject, BubbleSize, SelectionTime, CorrectedSelectionTime)

head(marg_PB_data_frame_without_err, nrow(marg_PB_data_frame_without_err))

# rm
rm(Subject)
rm(BubbleSize)
rm(SelectionTime)
rm(CorrectedSelectionTime)
rm(p)
rm(b)
rm(pbData)
rm(u_bubblesize)
rm(u_participants)


######## Print some statistical numbers for SelectionTime #########
###################################################################

# for Condition

df <- marg_PC_data_frame_without_err

stat_calib_builtin <- df[df$Condition == "Built-in Calibration", "SelectionTime"]
summary(stat_calib_builtin)
sd(stat_calib_builtin)

stat_calib_custom <- df[df$Condition == "Custom Calibration", "SelectionTime"]
summary(stat_calib_custom)
sd(stat_calib_custom)

stat_calib_mouseKB <- df[df$Condition == "Mouse & Keyboard", "SelectionTime"]
summary(stat_calib_mouseKB)
sd(stat_calib_mouseKB)

rm(df, stat_calib_builtin, stat_calib_custom, stat_calib_mouseKB)

# for BubbleSize

df <- marg_PB_data_frame_without_err

stat_bubble_small <- df[df$BubbleSize == "Small", "SelectionTime"]
summary(stat_bubble_small)
sd(stat_bubble_small)

stat_bubble_medium <- df[df$BubbleSize == "Medium", "SelectionTime"]
summary(stat_bubble_medium)
sd(stat_bubble_medium)

stat_bubble_large <- df[df$BubbleSize == "Large", "SelectionTime"]
summary(stat_bubble_large)
sd(stat_bubble_large)

rm(df, stat_bubble_small, stat_bubble_medium, stat_bubble_large)


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

xxxx="

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

# 2. PostHoc for Corrected Selection Time
df <- marg_PC_data_frame_without_err
df_posthoc <- df
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$CorrectedSelectionTime ~ df_posthoc$Condition, df_posthoc)
summary(df_posthoc_aov)

xxxx="

                     Df Sum Sq Mean Sq F value  Pr(>F)    
df_posthoc$Condition  2  14.40   7.200    39.4 1.8e-09 ***
Residuals            33   6.03   0.183                    
---
Signif. codes:  0 '***' 0.001 '**' 0.01 '*' 0.05 '.' 0.1 ' ' 1

"

TukeyHSD(df_posthoc_aov)

xxxx="
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
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionTime' 
# (F(2,22)=39.18, p<0.01, partial_eta_2 = 0.7807722 with CI=[0.4545357, 0.8006797]).
df <- marg_PC_data_frame_without_err
df_effect_size <- aov(df$CorrectedSelectionTime ~ factor(df$Condition) + Error(factor(df$Subject) / factor(df$Condition)), df)
summary(df_effect_size)

xxxx="

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

xxxx="
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

xxxx="
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

# 2. PostHoc for Corrected Selection Time
df <- marg_PB_data_frame_without_err
df_posthoc <- df
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$CorrectedSelectionTime ~ df_posthoc$BubbleSize, df_posthoc)
summary(df_posthoc_aov)
xxxx="
                      Df Sum Sq Mean Sq F value Pr(>F)
df_posthoc$BubbleSize  2  0.267  0.1333   0.663  0.522
Residuals             33  6.630  0.2009               

"
TukeyHSD(df_posthoc_aov)
xxxx="
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
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionTime' 
# (F(2,22)=0.663, p<0.01, partial_eta_2 = 0.05692964 with CI=[0, 0.1836511]).
df <- marg_PB_data_frame_without_err
df_effect_size <- aov(df$CorrectedSelectionTime ~ factor(df$BubbleSize) + Error(factor(df$Subject) / factor(df$BubbleSize)), df)
summary(df_effect_size)

xxxx="

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

xxxx="
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


###################################################################
###################################################################
###################################################################
###################################################################
###################################################################
################################################################### All pirate plots
# the plots for paper SelectionTime
attach(mtcars)
par(mfrow = c(2, 1))
par(mfrow = c(1, 2))
boxplot(SelectionTime ~ Condition, marg_PC_data_frame_without_err, main = "Selection time vs. Condtitions",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame_without_err, main = "Selection time vs. Bubble size",
               xlab = "Bubble Size", ylab = "Selection Time")

par(mfrow = c(1, 1))
pirateplot(formula = SelectionTime ~ Condition, data = marg_PC_data_frame_without_err, main = "Selection Time for Different Conditions"
           , theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

par(mfrow = c(1, 1))
pirateplot(formula = SelectionTime ~ BubbleSize, data = marg_PB_data_frame_without_err, main = "Selection Time for Different Bubble Sizes"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

# the plots for paper SelectionError
attach(mtcars)
par(mfrow = c(2, 1))
par(mfrow = c(1, 2))
boxplot(SelectionError ~ Condition, marg_PC_data_frame, main = "Selection Error vs. Condtitions",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionError ~ BubbleSize, marg_PB_data_frame, main = "Selection Error vs. Bubble size",
               xlab = "Bubble Size", ylab = "Selection Time")

par(mfrow = c(1, 1))
pirateplot(formula = SelectionError ~ Condition, data = marg_PC_data_frame, main = "Selection Error for Different Conditions"
           , theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

par(mfrow = c(1, 1))
pirateplot(formula = SelectionError ~ BubbleSize, data = marg_PB_data_frame, main = "Selection Error for Different Bubble Sizes"
           ,theme = 2, # theme 2
           pal = "google", # xmen palette
           point.o = .4, # Add points
           point.col = "black",
           point.bg = "white",
           point.pch = 21,
           bean.f.o = .2, # Turn down bean filling
           inf.f.o = .8, # Turn up inf filling
           gl.col = "gray", # gridlines
           gl.lwd = c(.5, 0)) # turn off minor grid lines)

######## Overview of all data using boxplots   #################### Debugging only for SelectionTime
###################################################################
attach(mtcars)
par(mfrow = c(2, 2))
boxplot(SelectionTime ~ Condition, data_frame, main = "No Marginal & with errors",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ Condition, data_frame_without_err, main = "No Marginal & without errors",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ Condition, marg_PC_data_frame, main = "Marginal & with errors",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ Condition, marg_PC_data_frame_without_err, main = "Marginal & without errors",
               xlab = "Condition", ylab = "Selection Time")

attach(mtcars)
par(mfrow = c(2, 2))
boxplot(SelectionTime ~ BubbleSize, data_frame, main = "No Marginal & with errors",
               xlab = "Bubble Size", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, data_frame_without_err, main = "No Marginal & without errors",
               xlab = "Bubble Size", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame, main = "Marginal & with errors",
               xlab = "Bubble Size", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame_without_err, main = "Marginal & without errors",
               xlab = "Bubble Size", ylab = "Selection Time")

###################################################################
###################################################################
###################################################################
###################################################################
###################################################################



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

TukeyHSD(df_posthoc_aov)

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
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
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

TukeyHSD(df_posthoc_aov)

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
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
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

