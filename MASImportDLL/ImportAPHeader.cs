// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportAPHeader
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class ImportAPHeader
  {
        public ImportAPHeader()
        {
            ImportAPDetails = new List<ImportAPDetail>() ;
        }

        [Key]
        public int ImportAPHeaderKey { get; set; }

    public int ImportBatchFileKey { get; set; }

    public string ImportAPVendorNumber { get; set; }

    public string ImportAPInvoiceNumber { get; set; }

    public DateTime ImportAPInvoiceDate { get; set; }

    public ICollection<ImportAPDetail> ImportAPDetails { get; set; }

    public ImportBatchFile ImportBatchFile { get; set; }
  }
}
