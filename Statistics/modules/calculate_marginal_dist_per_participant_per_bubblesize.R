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
