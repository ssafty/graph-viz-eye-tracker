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

data =dataFrame[,c( 
  #"V1"
  "V2"                                    ## "ID"   
  ,"V3"                                   ## ,"Vision_correction" 
  ,"V4"                                   # ,"Do_you_suffer_from_a_displacement_of_equilibrium_or_similar " 
  ,"V5"                                   #,"Do you suffer from a motor disorder such as an impaired hand_eye coordination" 
  ,"V6"                                   # ,"Have you participated in a study with an Eye Tracker before"  
  ,"V7"                                   # ,"Have you participated in a study using a Haptic Device before" 
  ,"V8"                                   # ,"Do you have experience with 3D computer games" 
  ,"V9"                                    # ,"How many hours do you play per week"
  ,"V10"                                    # ,"Are you left-handed or right-handed "                                
  ,"V11"                                   # ,"Result of Stereo Test                            
  ,"V12"                                  # ,"General discomfort " 
  ,"V13"                                  #,"Fatigue " 
  ,"V14"                                  # ,"Headache " 
  ,"V15"                                 # ,"Eyestrain "  
  ,"V16"                                 # ,"Difficulty focusing " 
  ,"V17"                              # ,"Increased salivation "                                                                      
  ,"V18"                              # ,"Sweating "                           
  ,"V19"                              # ,"Nausea "                               
  ,"V20"                               # ,"Difficulty concentrating " 
  ,"V21"                                 # ,"Fullness of head " 
  ,"V22"                               # ,"Blurred vision "                                                                            
  ,"V23"                               #  ,"Dizzy (eyes open) . Duselig bei offenen Augen"                                              
  ,"V24"                               # ,"Dizzy (eyes closed) . Duselig bei geschlossenen Augen" 
  ,"V25"                               #  ,"Vertigo " 
  ,"V26"                               #  ,"Stomach awareness "
  ,"V27"                               #  ,"Burping "
  ,"V28"                               # ,"General discomfort " 
  ,"V29"                               # ,"Fatigue "  
  ,"V30"                               #  ,"Headache " 
  ,"V31"                               # ,"Eyestrain " 
  ,"V32"                               # ,"Difficulty focusing "                                                                       
  ,"V33"                               #  ,"Increased salivation "  
  ,"V34"                                 # ,"Sweating " 
  ,"V35"
  ,"V36"
  ,"V37"
  ,"V38"
  ,"V39"
  ,"V40"
  ,"V41"
  ,"V42"
  ,"V43"
  ,"V44"
  ,"V45"
  ,"V46"
  ,"V47"
  ,"V48"
  ,"V49"
  ,"V50"
  ,"V51"
  ,"V52"
  ,"V53"
  ,"V54"
  ,"V55"
  ,"V56"
  ,"V57"
  #  ,"V58"
  
  
)]

##data =dataFrame[,c( 

## "ID"                                                                                         #V2
## ,"Vision_correction"                                                                         #V3
# ,"Do_you_suffer_from_a_displacement_of_equilibrium_or_similar "                               #V4
#,"Do you suffer from a motor disorder such as an impaired hand_eye coordination"               #V5            
# ,"Do you have a known eye disorder"                                                           #V6
# ,"Have you participated in a study with an Eye Tracker before"                                #V7
# ,"Have you participated in a study using a Haptic Device before"                              #V8
# ,"Do you have experience with 3D computer games"                                              #V9
# ,"How many hours do you play per week"                                                        #V10
# ,"Are you left-handed or right-handed "                                                       #V11
# ,"Result of Stereo Test                                                                       #V12
# ,"General discomfort "                                                                        #V13
#,"Fatigue "                                                                                    #V14
# ,"Headache "                                                                                  #V15
# ,"Eyestrain "                                                                                 #V16
# ,"Difficulty focusing "                                                                       #V17
# ,"Increased salivation "                                                                      #V18
# ,"Sweating "                                                                                  #V19
# ,"Nausea "                                                                                    #V20
# ,"Difficulty concentrating "                                                                  #V21
# ,"Fullness of head "                                                                          #V22
# ,"Blurred vision "                                                                            #V23
#  ,"Dizzy (eyes open) . Duselig bei offenen Augen"                                              #V24
# ,"Dizzy (eyes closed) . Duselig bei geschlossenen Augen"                                      #V25
#  ,"Vertigo "                                                                                   #V26
#  ,"Stomach awareness "                                                                         #V27
#  ,"Burping "                                                                                   #V28
# ,"General discomfort "                                                                        #V29
# ,"Fatigue "                                                                                   #V30
#  ,"Headache "                                                                                  #V31
# ,"Eyestrain "                                                                                 #V32
# ,"Difficulty focusing "                                                                       #V33
#  ,"Increased salivation "                                                                      #V34
# ,"Sweating "                                                                                  #V35
#  ,"Nausea "                                                                                    #V36
# ,"Difficulty concentrating "                                                                  #V37
#  ,"Fullness of head "                                                                          #V38
#  ,"Blurred vision "                                                                            #V39
#  ,"Dizzy (eyes open) . Duselig bei offenen Augen"                                              #V40
#  ,"Dizzy (eyes closed) . Duselig bei geschlossenen Augen"                                      #V41
# ,"Vertigo "                                                                                   #V42
# ,"Stomach awareness "                                                                         #V43
#  ,"Burping "                                                                                   #V44
#  ,"Age"                                                                                        #V45
#  ,"Profession / field of study"                                                                #V46
# ,"Gender"                                                                                     #V47
# ,"Do you think the experiment task was too difficult"                                         #V48
# ,"Do you think the experiment was too long"                                                   #V49
# ,"How would you subjectively describe your level of attention during the experiment"          #V50
#  ,"Please rate your level of comfort during selections"                                        #V51
# ,"Do you think the visual feedback was helpful to select the target"                          #V52
#  ,"Did you notice a difference between both eye tracker trials"                                #V53
#  ,"Do you think the gaze detection was helpful to select the target"                           #V54
#  ,"Please rate your preference for the Keyboard-and-Mouse condition "                          #V56
#,"Please_rate_your_preferenc_for_the_Eye_Tracker_Device_condition"                             #V57
# ,"Additional comments "                                                                       #V58                                                                                                              
# ,"Not_Available"                                                                              #V59

#)]

data <- na.omit(data) 

participants = unique(data["ID"])
participants

ages = unique(data["Age"])
ages

genders = unique(data["Gender"])
genders

##################################Age##################################################
Age_Data = mean(data$Age) 
print(Age_Data)


Age_max = max(data$Age)
print(Age_max)

Age_min = min(data$Age)
print(Age_min)    
########################################################################################

###################################glasses#######################################################
b <- data.frame(number=data$Vision_correction_)
count_glasses <- length(which(b == "Glasses"))
print(count_glasses)
count_none <- length(which(b == "None"))
print(count_none )
count_contactlens <- length(which(b == "Contact lenses"))
print(count_contactlens)
#########################################################################################

################################Experience_with_3D_computer_games###############################
comp_game_mean = mean(data$Do_you_have_experience_with_3D_computer_games_Q_) 
print(comp_game_mean)
comp_game_sd= sd(data$Do_you_have_experience_with_3D_computer_games_Q_) 
print(comp_game_sd)
################################################################################################

############################hours_of_play_per_week_##########################################

hrs_of_play_mean = mean(data$How_many_hours_do_you_play_per_week_Q_) 
print(hrs_of_play_mean)
hrs_of_play_sd= sd(data$How_many_hours_do_you_play_per_week_Q_) 
print(hrs_of_play_sd)
#############################################################################################

###################################Friedman Test#######################################


data2 <- cbind(data[data$Group==1,]$Value, data[data$Group==2,]$Value, data[data$Group==3,]$Value)
friedman.test(data2)



