using System;
using System.Collections.Generic;

namespace MatrixApp.Models;

public partial class CountsConsolidateTxt
{
    public int Id { get; set; }

    public string ConsolidatedColumns { get; set; }

    public string ExperimentName  { get; set; }  

    public string FileName { get; set; }

    public DateTime InsertDate { get; set; }

    public virtual ICollection<CountsMatrixInitial> CountsMatrixInitials { get; } = new List<CountsMatrixInitial>();

    public virtual ICollection<CountsMatrixResult> CountsMatrixResults { get; } = new List<CountsMatrixResult>();

    public static Dictionary<string, string> keyValueColumns(string ConsolidatedColumns)
    {
        if(String.IsNullOrWhiteSpace(ConsolidatedColumns))
        {
            return new Dictionary<string, string>();
        }
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ConsolidatedColumns);
    }
}
