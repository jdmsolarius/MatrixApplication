using EFCore.BulkExtensions;
using MatrixApp.Models;
using Microsoft.AspNetCore.Http;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MatrixApp.CoreLogic
{
    public class AnalyzeData
    {
        public static List<CountsMatrixResult> averageDataPoint(IEnumerable<CountsMatrixResult> rows)
        {
            List<CountsMatrixResult> AveragedList = new List<CountsMatrixResult>();
            Dictionary<string, List<CountsMatrixResult>> AllRows = rows.GroupBy(x=>x.Gene).ToDictionary(t=>t.Key, t=>t.ToList());
            foreach (KeyValuePair<string, List<CountsMatrixResult>> row in AllRows)
            {
                row.Value.Select(t=>t.Gene).First();
                CountsMatrixResult result = new CountsMatrixResult();
                result.MatrixId = row.Value.Select(t=>t.MatrixId).First();
                result.Gene = row.Value.Select(t => t.Gene).First();
                result.Pvalue = row.Value.Select(t => t.Pvalue).Average();
                result.KeyValueFloats = row.Value.Select(t => t.KeyValueFloats).First();
                result.LogCpm = row.Value.Select(t => t.LogCpm).Average();
                result.LogFc = row.Value.Select(t => t.LogFc).Average();
                result.Fdr = row.Value.Select(t => t.Fdr).Average();
                result.EnsembleId = row.Value.Select(t => t.EnsembleId).First();
                result.InsertDate = row.Value.Select(t => t.InsertDate).First();
                result.Id =row.Value.Select(t => t.Id).First();
                AveragedList.Add(result);
            }
            return AveragedList;
        }


        public static List<ExperimentTable> InsertCPMMatrix(Dictionary<string, List<string>> rows, string experiment, AzazeldbContext azazeldbContext, Dictionary<string, List<double>> Doubles)
        {
            Dictionary<string, double>? result = Doubles
            .GroupBy(kvp => kvp.Key.StartsWith(kvp.Key.Substring(0,4)) ? kvp.Key[0].ToString() : kvp.Key)
            .ToDictionary(
                g => g.Key,
                g => g.SelectMany(kvp => kvp.Value).Average()
            );
            List<int> ids = azazeldbContext.CountsConsolidateTxts.Where(t => t.ExperimentName == "NMNAT2-TH").Select(t => t.Id).ToList();
            Tuple<string, string, string> similarHeaders = Tuple.Create("wt_2mo_3", "wt_2mo_3", "wt_2mo_3");
            Tuple<string, string, string> similarHeaders2 = Tuple.Create("th_2mo_3", "th_2mo_3", "th_2mo_3");
            Tuple<string, string, string> similarHeaders3 = Tuple.Create("wt_6mo_3", "wt_6mo_3", "wt_6mo_3");
            Tuple<string, string, string> similarHeaders4 = Tuple.Create("th_6mo_3", "th_6mo_3", "th_6mo_3");
            Tuple<string, string, string, string> similarHeaders5 = Tuple.Create("th_6", "wt_6", "th_2", "wt_2");
            Dictionary<string, List<double>> valueHolder = new Dictionary<string, List<double>>();
            List<ExperimentTable> results = new List<ExperimentTable>();
            HashSet<string> uniqueKeys = new HashSet<string>();
            Dictionary<string, List<double>> itemsToAverage = new Dictionary<string, List<double>>();
            Dictionary<string, string> stringItems = new Dictionary<string, string>();
            Dictionary<string, List<ExperimentTable>> keyValuePairs = new Dictionary<string, List<ExperimentTable>>();
            List<double> values = new List<double>();
            int iterator = rows.Keys.Count;
            List<string> trueKeys = new List<string>();

            List<ExperimentTable> results1 = new List<ExperimentTable>();
            List<ExperimentTable> results2 = new List<ExperimentTable>();
            List<ExperimentTable> results3 = new List<ExperimentTable>();
            List<ExperimentTable> results4 = new List<ExperimentTable>();


            for (int i = 0; i < rows[rows.Keys.First()].Count; i++)
            {
                ExperimentTable experimentTable = new ExperimentTable();
                experimentTable.ExperimentName = "NMNAT2-TH";
                experimentTable.ExpMatrixIds = ids.ToDelimitedString(",");
                experimentTable.EnsembleId = rows["SearchableKey"][i].Split("_")[0];
                experimentTable.Gene = rows["SearchableKey"][i].Split("_")[1];
                ExperimentTable experimentTable2 = new ExperimentTable();
                experimentTable2.ExperimentName = "NMNAT2-TH";
                experimentTable2.ExpMatrixIds = ids.ToDelimitedString(",");
                experimentTable2.EnsembleId = rows["SearchableKey"][i].Split("_")[0];
                experimentTable2.Gene = rows["SearchableKey"][i].Split("_")[1];
                ExperimentTable experimentTable3 = new ExperimentTable();
                experimentTable3.ExperimentName = "NMNAT2-TH";
                experimentTable3.ExpMatrixIds = ids.ToDelimitedString(",");
                experimentTable3.EnsembleId = rows["SearchableKey"][i].Split("_")[0];
                experimentTable3.Gene = rows["SearchableKey"][i].Split("_")[1];
                ExperimentTable experimentTable4 = new ExperimentTable();
                experimentTable4.ExperimentName = "NMNAT2-TH";
                experimentTable4.ExpMatrixIds = ids.ToDelimitedString(",");
                experimentTable4.EnsembleId = rows["SearchableKey"][i].Split("_")[0];
                experimentTable4.Gene = rows["SearchableKey"][i].Split("_")[1];
                double itemOne = double.Parse(rows["wt_2mo_3"][i]);
                double itemTwo = double.Parse(rows["wt_2mo_2"][i]);
                double itemThree = double.Parse(rows["wt_2mo_1"][i]);
                experimentTable.SampleName = "wt_2mo";
                experimentTable.AveragedValue = (itemOne + itemTwo + itemThree)/3;
                double itemFour = double.Parse(rows["wt_6mo_3"][i]);
                double itemFive = double.Parse(rows["wt_6mo_2"][i]);
                double itemSix = double.Parse(rows["wt_6mo_1"][i]);
                experimentTable2.SampleName = "wt_6mo";
                experimentTable2.AveragedValue = (itemFour + itemFive + itemSix)/3;
                double itemSeven = double.Parse(rows["th_6mo_3"][i]);
                double itemEight = double.Parse(rows["th_6mo_2"][i]);
                double itemNine = double.Parse(rows["th_6mo_1"][i]);
                experimentTable3.SampleName = "th_6mo";
                experimentTable3.AveragedValue = (itemSeven + itemEight + itemNine)/3;
                double itemTen = double.Parse(rows["th_2mo_3"][i]);
                double itemEleven = double.Parse(rows["th_2mo_2"][i]);
                double itemTwelve = double.Parse(rows["th_2mo_1"][i]);
                experimentTable4.SampleName = "th_2mo";
                experimentTable4.AveragedValue = (itemTen + itemEleven + itemTwelve)/3;
                results1.Add(experimentTable);
                results2.Add(experimentTable2);
                results3.Add(experimentTable3);
                results4.Add(experimentTable4);
            }
       
            CoreLogic.BulkInsert.BulkCopyToServer(azazeldbContext, results1, new List<string> { "Id", "SearchableKey", "InsertDate" });
            CoreLogic.BulkInsert.BulkCopyToServer(azazeldbContext, results2, new List<string> { "Id", "SearchableKey", "InsertDate" });
            CoreLogic.BulkInsert.BulkCopyToServer(azazeldbContext, results3, new List<string> { "Id", "SearchableKey", "InsertDate" });
            CoreLogic.BulkInsert.BulkCopyToServer(azazeldbContext, results4, new List<string> { "Id", "SearchableKey", "InsertDate" });


            azazeldbContext.SaveChanges();
            return null;
        }
            
     
        
        public static int getMatrixId(IFormCollection Form, AzazeldbContext azazeldbContext)
        {
            Dictionary<string, List<string>> cols = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(Form["Keys"]);
            Dictionary<string, string> ConsolidatedColumns = new Dictionary<string, string>();
            CountsConsolidateTxt countsConsolidateTxt = new CountsConsolidateTxt();
            countsConsolidateTxt.InsertDate = DateTime.Now;
            countsConsolidateTxt.FileName = Form["FileName"];
            foreach (string key in cols.Keys)
            {
                ConsolidatedColumns.Add(key, cols[key].First());
            }
            countsConsolidateTxt.ConsolidatedColumns =  countsConsolidateTxt.ConsolidatedColumns = Newtonsoft.Json.JsonConvert.SerializeObject(ConsolidatedColumns);
            azazeldbContext.Add(countsConsolidateTxt);
            azazeldbContext.SaveChanges();
            return countsConsolidateTxt.Id;
        }

        public static bool InsertInitialCountsMatrix(IFormCollection Form, AzazeldbContext azazeldbContext2)
        {
           
            TableInfo tableInfo = new TableInfo();
            tableInfo.IdentityColumnName = "Id";
            tableInfo.TableName = "CountsMatrixInitial";
         
            azazeldbContext2.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
         
            Dictionary<string, List<string>> uploadedfiles = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(Form["MultiMap"]);
            Dictionary<string, List<int>> IntegerMatrix = new Dictionary<string, List<int>>();

            List<CountsMatrixInitial> counts = new List<CountsMatrixInitial>();
            foreach (string key in uploadedfiles.Keys)
            {
                if (key == "Gene" || key == "EnsembleId")
                {
                    continue;
                }
                List<int> items = new List<int>();
                foreach (string value in uploadedfiles[key])
                {
                    int.TryParse(value, out int val);
                    items.Add(val);
                }
                IntegerMatrix.Add(key, items);
            }
    
            using (azazeldbContext2)
            {
             
                bool first = true;
                for (int i = 0; i < uploadedfiles["Gene"].Count; i++)
                {
                    CountsMatrixInitial countsMatrixInitial = new CountsMatrixInitial();
                    countsMatrixInitial.MatrixId = int.Parse(Form["MatrixId"]);
                    countsMatrixInitial.InsertDate = DateTime.Now;
                    KeyValuePair<string, int> kvp = new KeyValuePair<string, int>("MatrixId", countsMatrixInitial.MatrixId);

                    countsMatrixInitial.KeyValueIntegers = Newtonsoft.Json.JsonConvert.SerializeObject(kvp);
                    if (first)
                    {
                        first = false;
                        countsMatrixInitial.KeyValueIntegers =  Newtonsoft.Json.JsonConvert.SerializeObject(IntegerMatrix);
                    }
                    countsMatrixInitial.Gene = uploadedfiles["Gene"][i];
                    countsMatrixInitial.EnsembleId = uploadedfiles["EnsembleId"][i];
                    counts.Add(countsMatrixInitial);
                }
                azazeldbContext2.BulkCopyToServer(counts, new List<string>() { "SearchableKey"});
            }
            return true;
        }

        internal static bool InsertInitialResultsMatrix(IFormCollection Form, AzazeldbContext azazeldbContext3)
        {

            azazeldbContext3.ChangeTracker.AutoDetectChangesEnabled = false;
            TableInfo tableInfo = new TableInfo();
            tableInfo.IdentityColumnName = "Id";
            tableInfo.TableName = "CountsMatrixResults";


            Dictionary<string, List<string?>> uploadedfiles = JsonConvert.DeserializeObject<Dictionary<string, List<string?>>>(Form["MultiMap2"]);
            Dictionary<string, List<double>> FloatMatrix = new Dictionary<string, List<double>>();
            Dictionary<string, List<string>> StringMatrix = new Dictionary<string, List<string>>();
            Dictionary<string, string> ConsolidatedColumns = new Dictionary<string, string>();
            List<CountsMatrixResult> counts = new List<CountsMatrixResult>();


            using (azazeldbContext3)
            {
                int MatrixIds;
                bool first = true;
                try
                {
                    MatrixIds = azazeldbContext3.CountsMatrixResults.Select(t => t.MatrixId).OrderByDescending(t => t).First() + 1;
                }
                catch
                {
                    MatrixIds = 1;
                }

                for (int i = 0; i < uploadedfiles["UniqueKey"].Count; i++)
                {
                    CountsMatrixResult countsMatrixResults = new CountsMatrixResult();
              
                    countsMatrixResults.InsertDate = DateTime.Now;
                    KeyValuePair<string, int> kvp = new KeyValuePair<string, int>("MatrixId", countsMatrixResults.MatrixId);
                    countsMatrixResults.KeyValueFloats = Newtonsoft.Json.JsonConvert.SerializeObject(kvp);
                    countsMatrixResults.MatrixId = MatrixIds;
                    if (first)
                    {
                        first = false;
                        countsMatrixResults.KeyValueFloats =  Newtonsoft.Json.JsonConvert.SerializeObject(FloatMatrix);
                    }
                    try
                    {

                        string UniqueKey = uploadedfiles["UniqueKey"][i];
                        if (String.IsNullOrWhiteSpace(UniqueKey) || UniqueKey.ToUpper() == "UNDEFINED")
                        {
                            continue;
                        }
                        countsMatrixResults.EnsembleId = uploadedfiles["UniqueKey"][i].Split("_").First();
                        countsMatrixResults.Gene = uploadedfiles["UniqueKey"][i].Split("_").Last();
                        countsMatrixResults.LogFc = uploadedfiles["logFC"][i] == null ? null : double.Parse(uploadedfiles["logFC"][i]);
                        countsMatrixResults.LogCpm = uploadedfiles["logCPM"][i] == null ? null : double.Parse(uploadedfiles["logCPM"][i]);
                        countsMatrixResults.Pvalue = uploadedfiles["PValue"][i] == null ? null : double.Parse(uploadedfiles["PValue"][i]);
                        countsMatrixResults.Fdr = uploadedfiles["FDR"][i] == null ? null : double.Parse(uploadedfiles["FDR"][i]);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                    counts.Add(countsMatrixResults);
                }
                foreach(var chunk in counts.Batch(counts.Count/6).AsEnumerable())
                {
                    azazeldbContext3.BulkCopyToServer(chunk, new List<string>() { "SearchableKey" });
                }


            }
            return true;
        }

        public static (Dictionary<string, List<string>> GeneSampleValues, Dictionary<string, List<int>> CountsMatrix) grabInitialMatrixes(int MatrixId = 16)
        {
            AzazeldbContext azazeldbContext = new AzazeldbContext();
            azazeldbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            azazeldbContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
            List<CountsMatrixInitial>? CountsMatrixInitial = azazeldbContext.CountsMatrixInitials.Where(t => t.MatrixId == MatrixId).ToList();
            Dictionary<string, List<int>> CountsMatrixInitialKeyValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(CountsMatrixInitial.First().KeyValueIntegers.ToString());
            Dictionary<string, List<string>> CountsMatrixGeneSampleValues = new Dictionary<string, List<string>>();
            CountsMatrixGeneSampleValues.Add("Gene", CountsMatrixInitial.Select(t=>t.Gene).ToList());
            CountsMatrixGeneSampleValues.Add("EnsembleId", CountsMatrixInitial.Select(t => t.EnsembleId).ToList());
            (Dictionary<string, List<string>> GeneSampleValues, Dictionary<string, List<int>> CountsMatrix) Matrixes = ValueTuple.Create(CountsMatrixGeneSampleValues, CountsMatrixInitialKeyValues);
            return Matrixes;
        }

        public static (Dictionary<string, List<string>> GeneSampleValues, Dictionary<string, List<float>> CountsMatrix) grabResultMatrixes(int MatrixId = 16)
        {
            AzazeldbContext azazeldbContext = new AzazeldbContext();
            azazeldbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            azazeldbContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
            List<CountsMatrixResult>? CountsMatrixResult = azazeldbContext.CountsMatrixResults.Where(t => t.MatrixId == MatrixId).ToList();
            Dictionary<string, List<float>> CountsMatrixInitialKeyValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<float>>>(CountsMatrixResult.First().KeyValueFloats.ToString());
            Dictionary<string, List<string>> CountsMatrixResults = new Dictionary<string, List<string>>();
            CountsMatrixResults.Add("Gene", CountsMatrixResult.Select(t => t.Gene).ToList());
            CountsMatrixResults.Add("EnsembleId", CountsMatrixResult.Select(t => t.EnsembleId).ToList());
            (Dictionary<string, List<string>> GeneSampleValues, Dictionary<string, List<float>> CountsMatrix) Matrixes = ValueTuple.Create(CountsMatrixResults, CountsMatrixInitialKeyValues);
            return Matrixes;
        }
        public static CountsMatrixInitial? getInitialMatrixRowByKey(string searchableKey)
        {
            AzazeldbContext azazeldbContext = new AzazeldbContext();
            azazeldbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            azazeldbContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
            return azazeldbContext.CountsMatrixInitials.Where(t=>t.SearchableKey == searchableKey).FirstOrDefault();
        }

        public static CountsMatrixResult? getResultMatrixRowByKey(string searchableKey)
        {
            AzazeldbContext azazeldbContext = new AzazeldbContext();
            azazeldbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            azazeldbContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
            return azazeldbContext.CountsMatrixResults.Where(t => t.SearchableKey == searchableKey).FirstOrDefault();
        }

    }
}
