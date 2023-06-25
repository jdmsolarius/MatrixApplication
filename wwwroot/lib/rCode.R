# Load the necessary libraries
library(tidyverse)
library(DBI)

# Read in the data from the file
data <- read_tsv("C:\\Users\\Downloads\\CPMMatrix.tsv")

# Add the Gene_Id column as the first column
data <- data %>% add_column(Gene_Id = "your_gene_id_data", .before = 1)

# Split the Gene_Id data to get the EnsembleId and GeneName
data <- data %>%
  separate(Gene_Id, into = c("EnsembleId", "temp", "Gene"), sep = "_") %>%
  select(-temp)

# Add the ExperimentName column as the second column
data <- data %>% add_column(ExperimentName = str_split("your_file_name", "_")[[1]][1], .after = 1)

# Add the ExpMatrixIds column as the third column
data <- data %>% add_column(ExpMatrixIds = 0, .after = 2)

# Average columns with similar headers
data %>%
  pivot_longer(cols = everything(), names_to = "SampleName", values_to = "AveragedValue") %>%
  mutate(SampleName = str_remove(SampleName, "_\\d$")) %>%
  group_by(SampleName) %>%
  summarize(AveragedValue = mean(AveragedValue))

print(data)

# Connect to the Microsoft SQL database
con <- dbConnect(odbc::odbc(), .connection_string = "your_connection_string")

# Insert the data into the Microsoft SQL table
dbWriteTable(con, "[MatrixAnalyzer].[ExperimentTable]", data, append = TRUE)