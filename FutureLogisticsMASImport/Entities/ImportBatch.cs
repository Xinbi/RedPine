// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportBatch
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class ImportBatch
  {
      private ICollection<ImportBatchFile> _importBatchFiles;

      [Key]
      public int ImportBatchKey { get; set; }

    public DateTime ImportBatchLoadDateTime { get; set; }

    public Guid? ImportLoadUser { get; set; }

    public virtual ICollection<ImportBatchFile> ImportBatchFiles
    {
        get => _importBatchFiles ?? (_importBatchFiles = new List<ImportBatchFile>());
        set => _importBatchFiles = value;
    }
  }
}
