using Faster.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixApp.Models
{
    public class VolcanoRow
    {

        public VolcanoRow(string geneName, double pvalue, double Logfc, string Ensembleid, double logcpm, double? fdr, MultiMap<string, SortedList<string, double>> GeneSampleValuePairs)
        {
            try
            {
                GeneName = geneName;
                PValue = pvalue;
                LogFC = Logfc;
                EnsembleId = Ensembleid;
                LogCPM = logcpm;
                FDR = (double)(fdr == null ? 0.01 : fdr);
                geneValuePairs = GeneSampleValuePairs;
            }
            catch { }
        }



        public string EnsembleId { get; set; }
        public string GeneName { get; set; }
        public double PValue { get; set; }
        public double LogFC { get; set; }
        public double LogCPM { get; set; }
        public double FDR { get; set; }
        public MultiMap<string, SortedList<string, double>> geneValuePairs { get;set;}
        public double MatrixId { get; set; }
        public float Id { get; set; }

        public string createJSON()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"{""Gene"":");
            sb.Append('"');
            sb.Append(GeneName);
            sb.Append('"');
            sb.Append(',');
            sb.Append(@"""LogFC"":");
            sb.Append(LogFC);
            sb.Append(',');
            sb.Append(@"""NegativeLogPValue"":");
            sb.Append(PValue);
            sb.Append("},");
            return sb.ToString();
        }

    }
}
