// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportAPHeader
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.Collections.Generic;

namespace MASImportDLL
{
  public class ImportAPHeader
  {
    public ImportAPHeader()
    {
      this.ImportAPDetails = (ICollection<ImportAPDetail>) new HashSet<ImportAPDetail>();
    }

    public int ImportAPHeaderKey { get; set; }

    public int ImportBatchFileKey { get; set; }

    public string ImportAPVendorNumber { get; set; }

    public string ImportAPInvoiceNumber { get; set; }

    public DateTime ImportAPInvoiceDate { get; set; }

    public virtual ICollection<ImportAPDetail> ImportAPDetails { get; set; }

    public virtual ImportBatchFile ImportBatchFile { get; set; }
  }
}
