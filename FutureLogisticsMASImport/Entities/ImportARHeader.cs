// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportARHeader
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class ImportARHeader
  {

        public ImportARHeader()
        {
            ImportARDetails = new List<ImportARDetail>();
        }
        [Key]
    public int ImportARHeaderKey { get; set; }

    public int ImportBatchFileKey { get; set; }

    public string ImportARInvoiceNumber { get; set; }

    public string ImportARCustomerNumber { get; set; }

    public DateTime ImportARInvoiceDate { get; set; }

    public string ImportARCustomerPONumber { get; set; }

    public string ImportARJobNumber { get; set; }

    public virtual ICollection<ImportARDetail> ImportARDetails { get; set; }

    public virtual ImportBatchFile ImportBatchFile { get; set; }
  }
}
