// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportBatchFile
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class ImportBatchFile
  {
      private ICollection<ImportAPHeader> _importApHeaders;
      private ICollection<ImportARHeader> _importArHeaders;


      public int ImportBatchFileKey { get; set; }

      [Key]
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

    public virtual ICollection<ImportAPHeader> ImportAPHeaders
    {
        get => _importApHeaders ?? (_importApHeaders = new List<ImportAPHeader>());
        set => _importApHeaders = value;
    }

    public virtual ICollection<ImportARHeader> ImportARHeaders
    {
        get => _importArHeaders ?? (_importArHeaders = new List<ImportARHeader>());
        set => _importArHeaders = value;
    }
  }
}
