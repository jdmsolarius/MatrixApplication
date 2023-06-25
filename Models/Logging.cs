using System;
using System.Collections.Generic;

namespace MatrixApp.Models;

public partial class Logging
{
    public int Id { get; set; }

    public int MatrixId { get; set; }

    public string EventType { get; set; } = null!;

    public string Log { get; set; } = null!;

    public int? ErrorCode { get; set; }

    public DateTime InsertDate { get; set; }
}
/*
         string path = Path.Combine(this.Environment.WebRootPath, "lib");
         if (!Directory.Exists(path))
         {
             Directory.CreateDirectory(path);
         }

          string headers = "Gene,logFC,NegativeLogPValue";
          int j = 0; StringBuilder sb1 = new StringBuilder();
          using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
          {

              StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
               writer.WriteLine(headers);
               foreach (VolcanoRow row in data)
               {
                      j++;
                      if (double.IsNaN((double)row.LogFC) || double.IsNaN((double)-Math.Log(row.PValue)))
                      {
                          continue;
                      }

                      sb1.Append(row.GeneName);
                      sb1.Append(",");
                      sb1.Append(row.LogFC);
                      sb1.Append(",");
                      sb1.Append(row.PValue); 

                      writer.WriteLine(sb1.ToString());

                      sb1.Clear();
               }
               writer.Close();
       }
        */