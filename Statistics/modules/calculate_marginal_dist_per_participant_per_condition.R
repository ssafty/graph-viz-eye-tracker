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