// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportBatchFile
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.Collections.Generic;

namespace MASImportDLL
{
  public class ImportBatchFile
  {
    public ImportBatchFile()
    {
      this.ImportBatchFileErrors = (ICollection<ImportBatchFileError>) new HashSet<ImportBatchFileError>();
      this.ImportAPHeaders = (ICollection<ImportAPHeader>) new HashSet<ImportAPHeader>();
      this.ImportARHeaders = (ICollection<ImportARHeader>) new HashSet<ImportARHeader>();
    }

    public int ImportBatchFileKey { get; set; }

    public int ImportBatchKey { get; set; }

    public string ImportBatchFileName { get; set; }

    public string ImportBatchFileType { get; set; }

    public int ImportBatchFileRecordCount { get; set; }

    public DateTime? ImportBatchFilePostDateTime { get; set; }

    public Guid? ImportBatchFilePostUser { get; set; }

    public DateTime? ImportBatchFileRollbackDateTime { get; set; }

    public Guid? ImportBatchFileRollbackUser { get; set; }

    public virtual ICollection<ImportBatchFileError> ImportBatchFileErrors { get; set; }

    public virtual ImportBatch ImportBatch { get; set; }

    public virtual ICollection<ImportAPHeader> ImportAPHeaders { get; set; }

    public virtual ICollection<ImportARHeader> ImportARHeaders { get; set; }
  }
}
