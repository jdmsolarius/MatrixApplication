using System;
using System.Collections.Generic;

namespace MatrixApp.Models;

public partial class CountsMatrixResult
{
    public int Id { get; set; }

    public int MatrixId { get; set; }

    public string EnsembleId { get; set; } = null!;

    public string Gene { get; set; } = null!;

    public string SearchableKey { get; set; } = null!;

    public string? KeyValueFloats { get; set; }

    public DateTime InsertDate { get; set; }

    public double? LogFc { get; set; }

    public double? LogCpm { get; set; }

    public double? Pvalue { get; set; }

    public double? Fdr { get; set; }

    public virtual CountsConsolidateTxt Matrix { get; set; } = null!;
}
