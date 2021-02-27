// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportBatchFileError
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class ImportBatchFileError
  {
      [Key]
    public int ImportBatchFileErrorKey { get; set; }

    public int ImportBatchFileKey { get; set; }

    public int ImportBatchFileRecordNumber { get; set; }

    public string ImportBatchFileErrorID { get; set; }

    public string ImportBatchFileErrorMessage { get; set; }

    public virtual ImportBatchFile ImportBatchFile { get; set; }
  }
}
