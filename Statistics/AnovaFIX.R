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

############### Subject 14 Seems to be wrong ######################
###################################################################
data_frame <- data_frame[!(data_frame$Subject == "14"),]

############### Extract data_frame without errors #################
###################################################################
data_frame_without_err <- data_frame[(data_frame$SelectionError == 0),]
data_frame_without_err[4] <- NULL # remove selection error column


############### Check for selection time normality  ###############
###################################################################

ExperimentClean <- data_frame$SelectionTime
ExperimentClean_we <- data_frame_without_err$SelectionTime

hist(ExperimentClean, breaks = "FD")
hist(ExperimentClean_we, breaks = "FD")



############### transform selection time for normality  ###########
###################################################################

ExperimentCorrected <- ExperimentClean
ExperimentCorrected_we <- ExperimentClean_we

ExperimentCorrected = log(ExperimentCorrected)
ExperimentCorrected_we = log(ExperimentCorrected_we)

ExperimentCorrected = ExperimentCorrected + abs(summary(ExperimentCorrected)[1])
ExperimentCorrected_we = ExperimentCorrected_we + abs(summary(ExperimentCorrected_we)[1])

print(summary(ExperimentCorrected))
print(summary(ExperimentCorrected_we))

hist(ExperimentCorrected, breaks = "FD")
hist(ExperimentCorrected_we, breaks = "FD")

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
data_frame_without_err$CorrectedSelectionTime <- ExperimentCorrected_we

# rm 
rm(ExperimentClean)
rm(ExperimentCorrected)
rm(ExperimentClean_we)
rm(ExperimentCorrected_we)

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
        SelectionError = c(SelectionError, sum(unlist(pcData["SelectionError"])))
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
        SelectionError = c(SelectionError, sum(unlist(pbData["SelectionError"])))
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

######## Overview of all data using boxplots   #################### Debugging
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

# the plots for paper
attach(mtcars)
par(mfrow = c(2, 1))
par(mfrow = c(1, 2))
boxplot(SelectionTime ~ Condition, marg_PC_data_frame_without_err, main = "Selection time vs. Condtitions",
               xlab = "Condition", ylab = "Selection Time")
boxplot(SelectionTime ~ BubbleSize, marg_PB_data_frame_without_err, main = "Bubble size vs. Condtitions",
               xlab = "Bubble Size", ylab = "Selection Time")


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

# 2. PostHoc for Corrected Selection Time
df <- marg_PC_data_frame_without_err
df_posthoc <- df
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$CorrectedSelectionTime ~ df_posthoc$Condition, df_posthoc)
summary(df_posthoc_aov)
TukeyHSD(df_posthoc_aov)

# rm
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionTime' 
# (F(2,28)=50.24, p<0.01, partial_eta_2 = 0.7820432 with CI=[0.4960631, 0.795146]).
df <- marg_PC_data_frame_without_err
df_effect_size <- aov(df$CorrectedSelectionTime ~ factor(df$Condition) + Error(factor(df$Subject) / factor(df$Condition)), df)
summary(df_effect_size)
partial_eta_2 = 18.579 / (18.579 + 5.178) # 0.7820432
partial_eta_2
ci.pvaf(F.value = 50.24, df.1 = 2, df.2 = 28, N = nrow(df))

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

# 2. PostHoc for Corrected Selection Time
df <- marg_PB_data_frame_without_err
df_posthoc <- df
df_posthoc[3] <- NULL # remove column SelectionTime
df_posthoc[1] <- NULL # remove column Subject

df_posthoc_aov <- aov(df_posthoc$CorrectedSelectionTime ~ df_posthoc$BubbleSize, df_posthoc)
summary(df_posthoc_aov)
TukeyHSD(df_posthoc_aov)

# rm
rm(df_anova, df_anova_aov, df_anova_design, df_anova_matrix, df_anova_model)
rm(df, df_posthoc, df_posthoc_aov)

# 3. Effect Size
# With one-way repeated-measure ANOVA, we found a significant effect of Group 'Condition' on Value 'SelectionTime' 
# (F(2,28)=0.038, p<0.01, partial_eta_2 = 0.002711007 with CI=[0, 0.01775197]).
df <- marg_PB_data_frame_without_err
df_effect_size <- aov(df$CorrectedSelectionTime ~ factor(df$BubbleSize) + Error(factor(df$Subject) / factor(df$BubbleSize)), df)
summary(df_effect_size)
partial_eta_2 = 0.015 / (0.015 + 5.518) # 0.002711007
partial_eta_2
ci.pvaf(F.value = 0.038, df.1 = 2, df.2 = 28, N = nrow(df))

# rm
rm(df, df_effect_size, partial_eta_2)


########Statistics for selection error ############################
###################################################################
plot2<-boxplot(SelectionError ~ Condition, pc_data_frame, main="Selection Error", 
               xlab="Condition", ylab="Selection Error")

data_frame_SE_builtin <- data_frame[data_frame$Condition=="Built-in Calibration","SelectionError"]
((sum(data_frame_SE_builtin)/length(data_frame_SE_builtin))*100)

data_frame_SE_custom <- data_frame[data_frame$Condition=="Custom Calibration","SelectionError"]
((sum(data_frame_SE_custom)/length(data_frame_SE_custom))*100)

data_frame_SE_kb <- data_frame[data_frame$Condition=="Mouse & Keyboard","SelectionError"]
((sum(data_frame_SE_kb)/length(data_frame_SE_kb))*100)

########ANOVA for Corrected Selection Error(ANOVA with the Sphericity test) ########################
###################################################################################################

pc_data_frame_ANOVA_SE <- data.frame(pc_data_frame$Subject, pc_data_frame$Condition, pc_data_frame$SelectionError)
pc_matrix_ANOVA_SE <- with(pc_data_frame_ANOVA_SE, 
                            cbind(
                              SelectionError[Condition=="Built-in Calibration"], 
                              SelectionError[Condition=="Custom Calibration"], 
                              SelectionError[Condition=="Mouse & Keyboard"])) 
pc_model_ANOVA_SE <- lm(pc_matrix_ANOVA_SE ~ 1)
pc_design_ANOVA_SE <- factor(c("Built-in Calibration", "Custom Calibration", "Mouse & Keyboard"))

options(contrasts=c("contr.sum", "contr.poly"))
pc_aov_ANOVA_SE <- Anova(pc_model_ANOVA_SE, idata=data.frame(pc_design_ANOVA_SE), idesign=~pc_design_ANOVA_SE, type="III")
summary(pc_aov_ANOVA_SE, multivariate=F)


########PostHoc for Corrected Selection Error ######################
###################################################################
pc_data_frame_PH_SE <- data.frame(pc_data_frame$Condition, pc_data_frame$SelectionError)
pc_aov_PH_SE <- aov(pc_data_frame$SelectionError ~ pc_data_frame$Condition, pc_data_frame_PH_SE)
summary(pc_aov_PH_SE)
TukeyHSD(pc_aov_PH_SE)


###################Effect Size#####################################
###################################################################

aovES_SE <- aov(SelectionError ~ factor(Condition) + Error(factor(Subject)/factor(Condition)), pc_data_frame_PH_SE)
summary(aovES_SE)
EffecSize<-45.5/(45.5+209.8)
EffecSize


##################################################################################################################


###########################################################################################################################################
#########################################################################################################################


###########################################################################################################################################

########Statistics for bubble size for Selection Error ################################
###################################################################

plot4<-boxplot(SelectionError ~ bubblesize, pb_data_frame_bubble, main="Selection Error", 
               xlab="BubbleSize", ylab="Selection Error")

data_frame_SE_bubble10 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==10,"SelectionError"]
((sum(data_frame_SE_bubble10)/length(data_frame_SE_bubble10))*100)

data_frame_SE_bubble5 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==5,"SelectionError"]
((sum(data_frame_SE_bubble5)/length(data_frame_SE_bubble5))*100)

data_frame_SE_bubble75 <- pb_data_frame_bubble[pb_data_frame_bubble$bubblesize==7.5,"SelectionError"]
((sum(data_frame_SE_bubble75)/length(data_frame_SE_bubble75))*100)


########################################################################################################################################

########ANOVA for Corrected Selection Time(ANOVA with the Sphericity test) ########################
###################################################################################################

pb_data_frame_ANOVA_SE <- data.frame(pb_data_frame_bubble$Subject,pb_data_frame_bubble$bubblesize, pb_data_frame_bubble$SelectionError)
pb_matrix_ANOVA_SE <- with(pb_data_frame_ANOVA_SE, 
                            cbind(
                              SelectionError[bubblesize==10], 
                               
                              SelectionError[bubblesize==7.5],
                              SelectionError[bubblesize==5])) 

pb_matrix_ANOVA_SE<- na.omit(pb_matrix_ANOVA_SE) # remove all NA values
pb_model_ANOVA_SE <- lm(pb_matrix_ANOVA_SE ~ 1)
pb_design_ANOVA_SE <- factor(c(10, 7.5, 5))

options(contrasts=c("contr.sum", "contr.poly"))
pb_aov_ANOVA_SE <- Anova(pb_model_ANOVA_SE, idata=data.frame(pb_design_ANOVA_SE), idesign=~pb_design_ANOVA_SE, type="III")
summary(pb_aov_ANOVA_SE, multivariate=F)


########PostHoc for Corrected Selection Time ######################
###################################################################
pb_data_frame_PH_SE <- data.frame(pb_data_frame_bubble$bubblesize, pb_data_frame_bubble$SelectionError)
pb_aov_PH_SE <- aov(pb_data_frame_bubble$SelectionError ~ pb_data_frame_bubble$bubblesize, pb_data_frame_PH_SE)
summary(pb_aov_PH_SE)
TukeyHSD(pb_aov_PH_SE)


###################Effect Size#####################################
###################################################################

aovES_SE_bubble <- aov(SelectionError ~ factor(bubblesize) + Error(factor(Subject)/factor(bubblesize)), pb_data_frame_PH_SE)
summary(aovES_SE_bubble)
EffecSize<- 3.04/( 3.04+39.40)
EffecSize

#########################################################################################################################################


###################################################################
###################################################################
###################################################################
###################################################################
###################################################################
################################################################### All pirate plots


pirateplot(formula = SelectionTime ~ Condition, data = data_frame_without_err, main = "Selection Time for Different Conditions"
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

pirateplot(formula = SelectionTime ~ BubbleSize, data = data_frame_without_err, main = "Selection Time for Different Bubble Sizes"
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
