using System;
using System.Collections.Generic;

namespace MatrixApp.Models;

public partial class CountsMatrixInitial
{
    public int Id { get; set; }

    public int MatrixId { get; set; }

    public string EnsembleId { get; set; } = null!;

    public string Gene { get; set; } = null!;

    public string SearchableKey { get; set; } = null!;

    public string? KeyValueIntegers { get; set; }

    public DateTime InsertDate { get; set; }

    public virtual CountsConsolidateTxt Matrix { get; set; } = null!;
}
