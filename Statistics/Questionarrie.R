##################Loading the data file ####################

path = "C:/Savitha/HCI/observations/"
fileNames = list.files(path=paste(path, sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, na.strings=c("", "NA"), header=FALSE, sep=",", dec=".", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)

head(dataFrame,16)

#######################################################################################################

data_frame =dataFrame[,c( 
  #"V1"
  "V2"                                    ## "ID"   
  ,"V3"                                   ## ,"Vision_correction" 
  ,"V4"                                   # ,"Do_you_suffer_from_a_displacement_of_equilibrium_or_similar " 
  ,"V5"                                   #,"Do you suffer from a motor disorder such as an impaired hand_eye coordination" 
  ,"V6"                                  # ,"Do you have a known eye disorder"   
  ,"V7"                                   # ,"Have you participated in a study with an Eye Tracker before" 
  ,"V8"                                  # ,"Have you participated in a study using a Haptic Device before" 
  ,"V9"                                    # ,"Do you have experience with 3D computer games"
  ,"V10"                                   # ,"How many hours do you play per week"                               
  ,"V11"                                   # ,"Are you left-handed or right-handed "                           
  ,"V12"                                # ,"Result of Stereo Test
  
  #########################################Conditions Pre-Test#############################################################
  ,"V13"                                  # ,"General discomfort "
  ,"V14"                                  #,"Fatigue "  
  ,"V15"                                # ,"Headache "  
  ,"V16"                                 # ,"Eyestrain " 
  ,"V17"                              # ,"Difficulty focusing "                                                                       
  ,"V18"                              # ,"Increased salivation "                           
  ,"V19"                              # ,"Sweating "                             
  ,"V20"                              # ,"Nausea "  
  ,"V21"                               # ,"Difficulty concentrating " 
  ,"V22"                               # ,"Fullness of head "                                                                            
  ,"V23"                               # ,"Blurred vision "                                               
  ,"V24"                               #  ,"Dizzy (eyes open) . Duselig bei offenen Augen"  
  ,"V25"                               # ,"Dizzy (eyes closed) . Duselig bei geschlossenen Augen"   
  ,"V26"                               #  ,"Vertigo "
  ,"V27"                               #  ,"Stomach awareness " 
  ,"V28"                             #  ,"Burping "
  ########################################################################################################################
  ###########################################Condition Post-Test##########################################################
  ,"V29"                               # ,"General discomfort "  
  ,"V30"                               # ,"Fatigue "  
  ,"V31"                               #  ,"Headache " 
  ,"V32"                               # ,"Eyestrain "                                                                        
  ,"V33"                              # ,"Difficulty focusing " 
  ,"V34"                                #  ,"Increased salivation "  
  ,"V35"                                # ,"Sweating "
  ,"V36"                                #  ,"Nausea " 
  ,"V37"                                # ,"Difficulty concentrating "  
  ,"V38"                                #  ,"Fullness of head " 
  ,"V39"                               #  ,"Blurred vision "  
  ,"V40"                               #  ,"Dizzy (eyes open) . Duselig bei offenen Augen" 
  ,"V41"                              #  ,"Dizzy (eyes closed) . Duselig bei geschlossenen Augen" 
  ,"V42"                               # ,"Vertigo " 
  ,"V43"                               # ,"Stomach awareness "
  ,"V44"                               #  ,"Burping " 
  #######################################################################################################################
  ,"V45"                               #  ,"Age"
  ,"V46"                               #  ,"Profession / field of study" 
  ,"V47"                              # ,"Gender" 
  ,"V48"                                # ,"Do you think the experiment task was too difficult" 
  ,"V49"                               # ,"Do you think the experiment was too long"
  ,"V50"                              # ,"How would you subjectively describe your level of attention during the experiment" 
  ,"V51"                              #  ,"Please rate your level of comfort during selections"
  ,"V52"                              # ,"Do you think the visual feedback was helpful to select the target" 
  ,"V53"                              #  ,"Did you notice a difference between both eye tracker trials" 
  ,"V54"                              #  ,"Do you think the gaze detection was helpful to select the target" 
  ,"V55"                          #  ,"Please rate your preference for the Keyboard-and-Mouse condition "
  ,"V56"                              #,"Please_rate_your_preferenc_for_the_Eye_Tracker_Device_condition"  
  ,"V57"                              # ,"Additional comments " 
  #  ,"V58"                            # ,"Not_Available" 
  
  
)]


#data <- na.omit(data_frame) 

data_frame$V10[data_frame$V10 == "2h"] <- as.numeric(as.character("2"))

participants = unique(data_frame["V2"])
participants
typeof(participants)

Value_pre <- c(data_frame$V13, 
               data_frame$V14,  
               data_frame$V15, 
               data_frame$V16, 
               data_frame$V17, 
               data_frame$V18, 
               data_frame$V19, 
               data_frame$V20, 
               data_frame$V21, 
               data_frame$V22, 
               data_frame$V23, 
               data_frame$V24, 
               data_frame$V25, 
               data_frame$V26,
               data_frame$V27, 
               data_frame$V28)
typeof(Value_pre)
length(Value_pre)



Group <- c(rep(data_frame$V2, (length(Value_pre))/16))
typeof(Group)
length(Group)

data_frame_GV_pre <- data.frame(Group,Value_pre)


  
Groups_pre= unique(data_frame_GV["Group"])
Groups_pre

Values_pre = unique(data_frame_GV["Value_pre"])
Values_pre 

Groups=list()
Values=list()


lastTime=0;

for(g in unlist(Groups_pre))
{
  for(v in unlist(Values_pre ))
  {
    gvData = data_frame_GV[data_frame_GV$Group==g & data_frame_GV$Value_pre==v,]
   # print(gvData)
    
    
    firstRow = TRUE
    
    
   for(i in 1:nrow(gvData))
   {
     row <- gvData[i,]
      
     if(firstRow==FALSE)
     {
      Groups=c(Groups, g)
       Values=c( Values, v)
        
      }
     
      firstRow = FALSE
    }
  }
}

Groups=unlist(Groups)
Values=unlist(Values)



data_frame_FT = data.frame(Groups, Values)
nrow(data_frame_FT)

head(data_frame_FT, nrow(data_frame_FT))


ages = unique(data_frame["V45"])
ages

genders = unique(data_frame["V47"])
genders

##################################Age##################################################
Age_Data = mean(data_frame$V45) 
print(Age_Data)


Age_max = max(data_frame$V45)
print(Age_max)

Age_min = min(data_frame$V45)
print(Age_min)    
########################################################################################

###################################glasses#######################################################
Glasses_Data <- data.frame(number=data_frame$V3)
count_glasses <- length(which(Glasses_Data== "Glasses"))
print(count_glasses)
count_none <- length(which(Glasses_Data == "None"))
print(count_none )
count_contactlens <- length(which(Glasses_Data == "Contact lenses"))
print(count_contactlens)
#########################################################################################

################################Experience_with_3D_computer_games###############################
comp_game_mean = mean(data_frame$V9) 
print(comp_game_mean)
comp_game_sd= sd(data_frame$V9) 
print(comp_game_sd)
################################################################################################

############################hours_of_play_per_week_##########################################
#as.numeric(as.character(data_frame$V10))

hrs_of_play_mean = mean(data_frame$V10) 
print(hrs_of_play_mean)
hrs_of_play_sd= sd(data_frame$V10) 
print(hrs_of_play_sd)
#############################################################################################



###################################Friedman Test#######################################


data_FT_pre <- cbind(data_frame_GV_pre[data_frame_GV_pre$Group==1,]$Value_pre, 
                 data_frame_GV_pre[data_frame_GV_pre$Group==3,]$Value_pre, 
                 data_frame_GV_pre[data_frame_GV_pre$Group==4,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==6,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==7,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==8,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==9,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==10,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==11,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==12,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==13,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==14,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==15,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==16,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==17,]$Value_pre,
                 data_frame_GV_pre[data_frame_GV_pre$Group==18,]$Value_pre)

typeof(data_FT_pre)
friedman.test(data_FT_pre)



