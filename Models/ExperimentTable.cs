using System;

namespace MatrixApp.Models
{
    public class ExperimentTable
    {
        public int Id { get; set; }
        public string ExperimentName { get; set; } = null!;
        public string ExpMatrixIds { get; set; } = null!;
        public string EnsembleId { get; set; } = null!;
        public string Gene { get; set; } = null!;
        public string SearchableKey { get; set; } = null!;
        public string SampleName  { get; set; } = null!;
        public double AveragedValue { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
