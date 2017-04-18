################## Install the packages ###########################
###################################################################


install.packages("car")
install.packages("MBESS")
install.packages(c('devtools', 'curl'))
install.packages('BayesFactor', dependencies = TRUE)
devtools::install_github('ndphillips/yarrr', build_vignettes = T)
install_github("ndphillips/yarrr")
install.packages("plyr")
install.packages("yarrr")