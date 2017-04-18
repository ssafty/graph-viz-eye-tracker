
# install and load libraries
#source("modules/install.R")
source("modules/load_libraries.R")

# load data
source('modules/load_data.R')
source('modules/create_data_frame.R')

# calculate selection time and remove outlier
source('modules/calculate_selection_times_and_remove_outliers.R')

# remove wrong participants
source('modules/remove_wrong_participants.R')

# remove rows with selection error while computing stats related to SelectionTime
source('modules/extract_data_frame_without_err.R')

# SelectionTime correction
source('modules/check_selection_time_hist.R')
source('modules/transform_selection_time_for_normality.R')
source('modules/check_transformed_selection_time.R')

# calculate marginal dist based on condition and bubblesize
source('modules/calculate_marginal_dist_per_participant_per_condition.R')
source('modules/calculate_marginal_dist_per_participant_per_bubblesize.R')

################################################################
# statitical tests
################################################################

# corrected selection time and condition
source('modules/stats_test01_mean_for_selection_time_and_condition.R')
source('modules/stats_test02_anova_for_selection_time_and_condition.R')
source('modules/stats_test03_posthoc_for_selection_time_and_condition.R')

# corrected selection time and bubblesize
source('modules/stats_test11_mean_for_selection_time_and_bubblesize.R')
source('modules/stats_test12_anova_for_selection_time_and_bubblesize.R')
source('modules/stats_test13_posthoc_for_selection_time_and_bubblesize.R')

# selection error and condition
source('modules/stats_test21_mean_for_selection_error_and_condition.R')
source('modules/stats_test22_anova_for_selection_error_and_condition.R')
source('modules/stats_test23_posthoc_for_selection_error_and_condition.R')

# selection error and bubblesize
source('modules/stats_test31_mean_for_selection_error_and_bubblesize.R')
source('modules/stats_test32_anova_for_selection_error_and_bubblesize.R')
source('modules/stats_test33_posthoc_for_selection_error_and_bubblesize.R')

################################################################
# plots
################################################################
source('modules/all_plots.R')




















