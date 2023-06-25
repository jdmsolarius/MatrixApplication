using Azure.Core;
using DotNetHelper.FastMember.Extension.Extension;
using Faster.Map;
using jdk.nashorn.@internal.objects.annotations;
using MatrixApp.CoreLogic;
using MatrixApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Newtonsoft.Json;
using OpenAI_API;
using SourceRCode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using static com.sun.tools.@internal.xjc.reader.xmlschema.bindinfo.BIConversion;
using static javax.jws.soap.SOAPBinding;

namespace MatrixApp.Controllers
{
    public class HomeController : Controller
    {
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        #pragma warning disable CS8604 // Possible null reference argument.
        private readonly ILogger<HomeController> _logger;
        private AzazeldbContext _azazeldbContext;
        private IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment _environment, AzazeldbContext AzazeldbContext, IMemoryCache Cache)
        {
            _logger = logger;
            _azazeldbContext = AzazeldbContext;
            _cache = Cache;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult UploadMatrixInitial()
        {
            AnalyzeData.InsertInitialCountsMatrix(Request.Form, _azazeldbContext);
            return new JsonResult(true);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public int getMatrixId()
        {
            int MatrixId = AnalyzeData.getMatrixId(Request.Form, _azazeldbContext);
            return MatrixId;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult UploadCPMMatrixResult()
        {
            string Experiment = Request.Form["ExperimentName"].ToString().Trim();
            Dictionary<string, List<string>> items = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(Request.Form["MultiMap"].ToString());
            Dictionary<String, List<string>> final = new Dictionary<string, List<string>>();
            Dictionary<string, List<double>> newFinal = new Dictionary<string, List<double>>();
             foreach (KeyValuePair<string, List<string>> pair in final)
            { // get the key and value from the original dictionary string key = pair.Key; List<string> values = pair.Value;

                // check if the key is "SearchableKey"
                if (pair.Key == "SearchableKey")
                {
                    // skip this key-value pair
                    continue;
                }

                // create a new list of doubles
                List<double> newList = new List<double>();

                // loop through each string element in the original list
                foreach (string item in pair.Value)
                {
                    // try to parse it as a double
                    double num;
                    if (double.TryParse(item, out num))
                    {
                        // add it to the new list
                        newList.Add(num);
                    }
                }

                // add the new key-value pair to the new dictionary
                newFinal.Add(pair.Key, newList);
            }
            foreach (KeyValuePair<string, List<string>> item in items)
            {
                string key = item.Key;
                string finalKey = key;
                List<string> values = item.Value;
                List<string> finalValues = new List<string>();
                for(int i = 0; i < values.Count; i++)
                {
                    string value = values[i];
                    if (finalKey == "SearchableKey")
                    {
                        value = value.Split("_")[0] + "_" + value.Split("_")[2];
                    }
                    finalValues.Add(value);
                }
                final.Add(finalKey, finalValues);
            }
         
            List<List<string>> result = new List<List<string>>();   
            AnalyzeData.InsertCPMMatrix(final, Request.Form["ExperimentName"].ToString().Trim(), _azazeldbContext, newFinal);
            return new JsonResult(true);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public int uploadCPMMatrix()
        {
            int MatrixId = AnalyzeData.getMatrixId(Request.Form, _azazeldbContext);
            return MatrixId;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public JsonResult UploadMatrixResult()
        {
            AnalyzeData.InsertInitialResultsMatrix(Request.Form, _azazeldbContext);
            return new JsonResult(true);
        }
        [HttpGet]
        public string[] GetVolcanoTSV()
        {
                List<string> uploadedFiles = new List<string>();
                int MatrixId = int.Parse(HttpContext.Request.Query["MatrixId"]);
                string XAxisItem = HttpContext.Request.Query["XAxisItem"];

                double significanceThreshold = double.Parse(HttpContext.Request.Query["significanceThreshold"]);
                double foldChangeThreshold = double.Parse(HttpContext.Request.Query["foldChangeThreshold"]);
                
                Dictionary<string, VolcanoRow> GeneKeyData = new Dictionary<string, VolcanoRow>();
                
                List<CountsMatrixResult> ProgenitorList;
                if (!_cache.TryGetValue(MatrixId + XAxisItem, out  ProgenitorList))
                {
                    ProgenitorList = _azazeldbContext.CountsMatrixResults.AsNoTracking().Where(t => t.MatrixId == MatrixId).ToList();
                }
                HashSet<string> geneNames = new HashSet<string>();
                List<CountsMatrixResult> duplicateList = new List<CountsMatrixResult>();
                for (int i = 0; i < ProgenitorList.Count; i++)
                {
                    try
                    {
                        if (geneNames.Contains(ProgenitorList[i].Gene))
                        {
                            duplicateList.AddRange(ProgenitorList.Where(t => t.Gene.ToUpper() == ProgenitorList[i].Gene.ToUpper()));
                            ProgenitorList.RemoveAll(t => t.Gene.ToUpper() == ProgenitorList[i].Gene.ToUpper());
                        }
                        else
                        {
                            geneNames.Add(ProgenitorList[i].Gene);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                ProgenitorList.AddRange(AnalyzeData.averageDataPoint(duplicateList));
                List<dynamic>? dynamicList = new List<dynamic>();
                if (true)
                {
                   
                    foreach(dynamic item in ProgenitorList)
                {
                    try
                    {
                        GeneKeyData.Add(item.Gene, new VolcanoRow(item.Gene, (float)item.Pvalue, (float)item.LogFc, item.EnsembleId, (float)item.LogCpm, (float)item.Fdr, new MultiMap<string, SortedList<string, double>>()));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                
                }
                   
                }
                ViewBag.GeneKeyData = GeneKeyData;
                dynamic dataFilter = null;
                switch(XAxisItem)
                {
                    case "LogFC":
                        dataFilter = ProgenitorList.Where(t => t.MatrixId == MatrixId && t.Pvalue != null && t.LogFc != null).Select(t => new { t.Gene, PValue = t.Pvalue == 0 ? 0.001 : t.Pvalue, LogFC = t.LogFc == 0 ? 0.001 : t.LogFc }).ToList();
                        break;
                    case "LogCPM":
                        dataFilter = ProgenitorList.Where(t => t.MatrixId == MatrixId && t.Pvalue != null && t.LogFc != null).Select(t => new { t.Gene, PValue = t.Pvalue == 0 ? 0.001 : t.Pvalue, LogCPM = t.LogCpm == 0 ? 0.001 : t.LogCpm }).ToList();
                        break;
                    case "FDR":
                        dataFilter = ProgenitorList.Where(t => t.MatrixId == MatrixId && t.Pvalue != null && t.LogFc != null).Select(t => new { t.Gene, PValue = t.Pvalue == 0 ? 0.001 : t.Pvalue, FDR = t.Fdr == 0 ? 0.001 : t.Fdr }).ToList();
                        break;
                }
                foreach (var row in dataFilter)
                {
                switch (XAxisItem)
                    {
                        case "LogFC":
                            if (double.IsNaN(row.LogFC) || double.IsNaN(-Math.Log10(row.PValue)))
                                continue;
                                dynamicList.Add(row);
                          
                            break;
                        case "LogCPM":
                            if (double.IsNaN(row.LogCPM) || double.IsNaN(-Math.Log10(row.PValue)))
                                continue;
                                dynamicList.Add(row);
                            break;
                        case "FDR":
                            if(double.IsNaN(row.FDR) || double.IsNaN(-Math.Log10(row.PValue)))
                                continue;
                                dynamicList.Add(row);
                            break;
                    }
                
                }
                
            //_cache.Set("GeneRowDictionary" + MatrixId + significanceThreshold + foldChangeThreshold, GeneKeyData, TimeSpan.FromSeconds(720));
            //_cache.Set(HttpContext.Request.Query["XAxisItem"].ToString(), XAxisItem, TimeSpan.FromSeconds(1000));
            //_cache.Set(MatrixId + XAxisItem, ProgenitorList, TimeSpan.FromSeconds(1000));
            string[] items = new string[] { Newtonsoft.Json.JsonConvert.SerializeObject(dynamicList).Replace("PValue", "P").Replace("LogFC", "L").Replace("FDR", "L").Replace("LogCPM", "L").Replace("LogCpm", "L").Replace("Gene", "G"), Newtonsoft.Json.JsonConvert.SerializeObject(GeneKeyData) };
            return items;
        }
        [HttpGet]
        public string GetVolcanoRow()
        {
            Dictionary<string, VolcanoRow> geneKeyData;
            List<string> uploadedFiles = new List<string>();
            int MatrixId = int.Parse(HttpContext.Request.Query["MatrixId"]);
            string Gene = HttpContext.Request.Query["Gene"].ToString().Trim();
            string ExperimentName = HttpContext.Request.Query["ExperimentName"].ToString().Trim();
            double significanceThreshold = double.Parse(HttpContext.Request.Query["significanceThreshold"]);
            double foldChangeThreshold = double.Parse(HttpContext.Request.Query["foldChangeThreshold"]);
            VolcanoRow? data;
            var items = new Faster.Map.MultiMap<string, SortedList<string, double>>();
            SortedList<string, double> sortedItems = new SortedList<string, double>();
            foreach (var item in _azazeldbContext.ExperimentTable.Where(t => t.ExperimentName == ExperimentName && Gene.ToUpper() == t.Gene.ToUpper()).Select(t => new { t.Gene, t.SampleName, t.AveragedValue }).ToList())
            {
                sortedItems.Add(item.SampleName, item.AveragedValue);
                items.Emplace(item.Gene, sortedItems);
            }

            if (true)
            {
                data = _azazeldbContext.CountsMatrixResults.Where(t => t.MatrixId == MatrixId  && t.Gene.ToUpper() == Gene).Select(t => new VolcanoRow(t.Gene, (double)t.Pvalue, (double)t.LogFc, t.EnsembleId, (double)t.LogCpm, (double)t.Fdr, items)).First();
                _azazeldbContext.Dispose();
                return Newtonsoft.Json.JsonConvert.SerializeObject(data).Replace("GeneName", "Gene").Replace("pValue", "-Log(PValue)");
            }
            else
            {
               return Newtonsoft.Json.JsonConvert.SerializeObject(geneKeyData[Gene]).Replace("GeneName", "Gene").Replace("pValue", "-Log(PValue)");
            }
        }
        public string GetVolcanoRows()
        {
            Dictionary<string, VolcanoRow> geneKeyData;

            List<string> uploadedFiles = new List<string>();
            int MatrixId = int.Parse(HttpContext.Request.Query["MatrixId"]);
            string Genes = HttpContext.Request.Query["GeneList"];
            string ExperimentName = HttpContext.Request.Query["ExperimentName"].ToString().Trim();
            List<string> GeneList = Genes.Split(",").ToList();
            List<VolcanoRow>? data;
            var items = new Faster.Map.MultiMap<string, SortedList<string, double>>();
            foreach (var item in _azazeldbContext.ExperimentTable.Where(t => t.ExperimentName == ExperimentName && GeneList.Contains(t.Gene)).Select(t => new {t.Gene, t.SampleName, t.AveragedValue}).ToList())
            {
                items.Emplace(item.Gene, new SortedList<string, double> { { item.SampleName, item.AveragedValue } });
            }

            data = _azazeldbContext.CountsMatrixResults.Where(t => t.MatrixId == MatrixId && GeneList.Contains(t.Gene)).Select(t => new VolcanoRow(t.Gene, (double)t.Pvalue, (double)t.LogFc, t.EnsembleId, (double)t.LogCpm, (double)t.Fdr, items)).ToList();
            _azazeldbContext.Dispose();
            return Newtonsoft.Json.JsonConvert.SerializeObject(data).Replace("GeneName", "Gene").Replace("pValue", "-Log(PValue)");
        }
        public string get_parent_dir_path(string path)
        {
            // notice that i used two separators windows style "\\" and linux "/" (for bad formed paths)
            // We make sure to remove extra unneeded characters.
            int index = path.Trim('/', '\\').LastIndexOfAny(new char[] { '\\', '/' });

            // now if index is >= 0 that means we have at least one parent directory, otherwise the given path is the root most.
            if (index >= 0)
                return path.Remove(index);
            else
                return "";
        }
        public async Task<IActionResult> Index()
        {
         

            string experimentName = HttpContext.Request.Query?["XAxisItem"];
            List<string> experimentNames = new List<string>();
            List<string> Xaxis = typeof(VolcanoRow).GetProperties().Select(p => p.Name).Where(p=>p == "FDR" || p.Contains("Log")).ToList();
            ViewBag.XAxisVariables = Xaxis;
            List<Tuple<int, string, string>>? FileIdList;
            //this is going
            if(!_cache.TryGetValue("FileIdList", out FileIdList))
            {
                FileIdList = _azazeldbContext.CountsConsolidateTxts.Select(t => new Tuple<int, string, string>(t.Id, t.FileName, t.ExperimentName)).ToList();
                _cache.Set("FileIdList", FileIdList, TimeSpan.FromSeconds(15));
            }
            List<string>? items = FileIdList.Select(t => t.Item3).Distinct().ToList();
            ViewBag.totalOptions = items;
            ViewBag.MatrixIdFileName = FileIdList;
            _azazeldbContext.Dispose();
       
            return View();
      

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
