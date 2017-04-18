################## Calculate selection times  #####################
###################################################################

# get unique values
u_participants = unique(data_frame["participantId"])
u_conditions = unique(data_frame["condition"])
u_states = unique(data_frame["currentState"])

t_Subject = list()
t_Condition = list()
t_SelectionTime = list()
t_SelectionError = list()
t_BubbleSize = list()

t_lastTime = 0;

for (p in unlist(u_participants)) {
    for (c in unlist(u_conditions)) {
        t_pcData = data_frame[data_frame$participantId == p & data_frame$condition == c,]
        # typeof(pcData)

        t_firstRow = TRUE

        for (i in 1:nrow(t_pcData)) {
            t_row <- t_pcData[i,]

            if (t_firstRow == FALSE) {
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
data_frame <- data_frame[!(data_frame$SelectionTime < 0),]
#Remove outliers from data frame
data_frame <- data_frame[!(data_frame$SelectionTime > t_threshold.upper | data_frame$SelectionTime < t_threshold.lower),]


head(data_frame, nrow(data_frame))

# rm
rm(t_threshold.factor, t_threshold.lower, t_threshold.upper)