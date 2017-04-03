##################Loading the data file ####################

path = "C:/Savitha/HCI/observations/"
fileNames = list.files(path=paste(path, sep=""), pattern="*.csv", full.names=TRUE)
fileNames
dataFrame = do.call("rbind", lapply(fileNames, function(x){read.csv(file=x, na.strings=c("", "NA"), header=TRUE, sep=",", dec=".", stringsAsFactors=FALSE)}))
is.data.frame(dataFrame)
ncol(dataFrame)
nrow(dataFrame)
rows = nrow(dataFrame)

head(dataFrame,16)

#######################################################################################################
data =dataFrame[,c( 
 
  "ID"
  ,"Vision_correction_"
  #,"Do_you_suffer_from_a_displacement_of_equilibrium_or_similar_Q___B__B_"
  #,"Do_you_suffer_from_a_motor_disorder_such_as_an_impaired_hand-eye_coordination_Q_"
  #,"Do_you_have_a_known_eye_disorder_Q_"
 # ,"Have_you_participated_in_a_study_with_an_Eye_Tracker_before_Q_"
  #,"Have_you_participated_in_a_study_using_a_Haptic_Device_before_Q_"
 ,"Do_you_have_experience_with_3D_computer_games_Q_"
  ,"How_many_hours_do_you_play_per_week_Q_"
 # ,"Are_you_left-handed_or_right-handed_Q__"
 # ,"Result_of_Stereo_Test"
 # ,"General_discomfort__S__Allg__Unwohlsein"
  #,"Fatigue__S__Erschöpfung"
  #,"Headache__S__Kopfschmerzen"
 # ,"Eyestrain__S__Überanstrengung_der_Augen"
 # ,"Difficulty_focusing__S__Probleme_bei_der_Fokussierung"
 # ,"Increased_salivation__S__Erhöhte_Speichelbildung"
 # ,"Sweating__S__Schweißbildung"
 # ,"Nausea__S__Übelkeit"
 # ,"Difficulty_concentrating__S__Konzentrationsschwierigkeiten"
  #,"Fullness_of_head__S__Kopf_voller_Gedanken"
 # ,"Blurred_vision__S__Unscharfe_Sicht"
  #,"Dizzy__B_eyes_open_B___S__Schwindelig_o__Duselig_bei_offenen_Augen"
 # ,"Dizzy__B_eyes_closed_B___S__Schwindelig_o__Duselig_bei_geschlossenen_Augen"
 # ,"Vertigo__S__Gleichgewichtsstörung"
 # ,"Stomach_awareness__S__Den_Bauch_wahrnehmen"
  #,"Burping__S__Aufstoßen"
 # ,"General_discomfort__S__Allg__Unwohlsein"
 # ,"Fatigue__S__Erschöpfung"
 # ,"Headache__S__Kopfschmerzen"
 # ,"Eyestrain__S__Überanstrengung_der_Augen"
 # ,"Difficulty_focusing__S__Probleme_bei_der_Fokussierung"
 # ,"Increased_salivation__S__Erhöhte_Speichelbildung"
 # ,"Sweating__S__Schweißbildung"
 # ,"Nausea__S__Übelkeit"
 # ,"Difficulty_concentrating__S__Konzentrationsschwierigkeiten"
 # ,"Fullness_of_head__S__Kopf_voller_Gedanken"
 # ,"Blurred_vision__S__Unscharfe_Sicht"
 # ,"Dizzy__B_eyes_open_B___S__Schwindelig_o__Duselig_bei_offenen_Augen"
 # ,"Dizzy__B_eyes_closed_B___S__Schwindelig_o__Duselig_bei_geschlossenen_Augen"
 # ,"Vertigo__S__Gleichgewichtsstörung"
#  ,"Stomach_awareness__S__Den_Bauch_wahrnehmen"
#  ,"Burping__S__Aufstoßen"
  ,"Age"
 # ,"Profession___field_of_study_"
  ,"Gender"
 # ,"Do_you_think_the_experiment_task_was_too_difficult_Q_"
 # ,"Do_you_think_the_experiment_was_too_long_Q_"
 # ,"How_would_you_subjectively_describe_your_level_of_attention_during_the_experiment_Q_"
 # ,"Please_rate_your_level_of_comfort_during_selections"
 # ,"Do_you_think_the_visual_feedback_was_helpful_to_select_the_target_Q_"
  #,"Did_you_notice_a_difference_between_both_eye_tracker_trials"
 # ,"Do_you_think_the_gaze_detection_was_helpful_to_select_the_target_Q_"
 # ,"Please_rate_your_preference_for_the_Keyboard-and-Mouse_condition_"
 # ,"Please_rate_your_preference_for_the_Eye-Tracker-Device_condition_"
 # ,"Additional_comments__B_i_e__I_liked____I_didn't_like____because____was_too_high____to_low__B__"
#  ,"NOT_AVAILABLE"
)]

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





