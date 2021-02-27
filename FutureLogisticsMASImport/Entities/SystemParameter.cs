// Decompiled with JetBrains decompiler
// Type: MASImportDLL.SystemParameter
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class SystemParameter
  {
      [Key]
    public string SystemParmID { get; set; }

    public string SystemParmValue { get; set; }
  }
}
