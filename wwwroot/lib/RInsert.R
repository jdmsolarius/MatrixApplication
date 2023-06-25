library(tidyverse)
library(DBI)
library(odbc)
#some of this is hardcoded but you would know the filename/path if you produced the files

filename <- "C:\\Users\\jdmso\\Downloads\\counts.matrix.wt_2mon_vs_wt_6mon.edgeR.DE_results"
FileNameFormatted <- "'counts.matrix.wt_2mon_vs_wt_6mon.edgeR.DE_results%'"
#Insert the first row to keep foreign key relationship consistent and get a matrixId
con <- dbConnect(odbc::odbc(),
                 Driver = "ODBC Driver 17 for SQL Server",
                 Server = "tcp:azazelserver.database.windows.net,1433",
                 Database = "Azazeldb",
                 UID = "jdmsolarius",
                 PWD = "N3onblue",
                 Port = 1433)
#hardcoded data
data_to_insert <- data.frame(
  ConsolidatedColumns = "{}",
  FileName = FileNameFormatted,
  ExperimentName = "NMNAT-TH2"
)

dbDisconnect(con)
#Retrieve the MatrixId
con <- dbConnect(odbc::odbc(),
                 Driver = "ODBC Driver 17 for SQL Server",
                 Server = "tcp:azazelserver.database.windows.net,1433",
                 Database = "Azazeldb",
                 UID = "jdmsolarius",
                 PWD = "N3onblue",
                 Port = 1433)

query <- paste("SELECT Id FROM [MatrixAnalyzer].[CountsConsolidateTxt] WHERE FileName like ",data_to_insert$FileName)
matrix_id <- dbGetQuery(con, query)

print(matrix_id)
dbDisconnect(con)



first_line <- readLines(filename, n = 1)

if (!grepl("Gene_Id", first_line)) {
  # If the first line does not contain "Gene_Id", add a header row to the file
  new_header <- paste("Gene_Id", first_line, sep = "\t")
  file_content <- paste(new_header, readChar(filename, file.info(filename)$size), sep = "\n")
  writeLines(file_content, filename)
}


the_data <- read.table(file = filename, header = TRUE, fill= TRUE)
the_data <- the_data[-1,]
cols_to_remove <- grep("^sample", colnames(the_data), ignore.case = TRUE)
the_data <- the_data[,-cols_to_remove]
# Split the Gene_Id column to get the EnsembleId and GeneName
the_new_data <- the_data %>%
  separate(Gene_Id, into = c("EnsembleId", "temp", "Gene"), sep = "_") %>%
  select(-temp)

the_new_data$KeyValueFloats <- "{MatrixId:0}"

# Add a new column called MatrixId using the variable matrix_id as the values
the_new_data$MatrixId <- matrix_id


con <- dbConnect(odbc::odbc(),
                 Driver = "ODBC Driver 17 for SQL Server",
                 Server = "tcp:azazelserver.database.windows.net,1433",
                 Database = "Azazeldb",
                 UID = "jdmsolarius",
                 PWD = "N3onblue",
                 Port = 1433)

dbWriteTable(con, "[MatrixAnalyzer].[CountsMatrixResults]", the_new_data, append = TRUE)

dbDisconnect(con)

data <- read_tsv("C:\\Users\\jdmso\\Downloads\\CPM_Matrix.tsv")

data <- data %>% add_column(ExperimentName = str_split("CPM_Matrix.TSV", "_")[[1]][1], .before = 1)

data <- data %>% add_column(ExpMatrixIds = 0, .after = 1)

# Split the ...1 column to get the EnsembleId and GeneName
data <- data %>%
  separate(`...1`, into = c("EnsembleId", "temp", "Gene"), sep = "_") %>%
  select(-temp)

# Average columns with similar headers
new_data <- data %>%
  pivot_longer(cols = matches("_\\d$"), names_to = "SampleName", values_to = "AveragedValue") %>%
  mutate(SampleName = str_remove(SampleName, "_\\d$")) %>%
  group_by(EnsembleId, Gene, SampleName, ExperimentName, ExpMatrixIds) %>%
  summarize(AveragedValue = mean(AveragedValue)) %>%
  select(ExperimentName, ExpMatrixIds, everything())

con <- dbConnect(odbc::odbc(),
                 Driver = "ODBC Driver 17 for SQL Server",
                 Server = "tcp:azazelserver.database.windows.net,1433",
                 Database = "Azazeldb",
                 UID = "jdmsolarius",
                 PWD = "N3onblue",
                 Port = 1433)

# Final Insert
dbWriteTable(con, "[MatrixAnalyzer].[ExperimentTable]", new_data, append = TRUE)

dbDisconnect(con)
