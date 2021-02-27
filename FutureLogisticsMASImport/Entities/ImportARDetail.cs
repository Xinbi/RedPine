// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportARDetail
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
  public class ImportARDetail
  {
      [Key]
    public int ImportARDetailKey { get; set; }

    public int ImportARHeaderKey { get; set; }

    public string ImportARItemCode { get; set; }

    public int ImportARItemQuantity { get; set; }

    public string ImportARItemDescription { get; set; }

    public Decimal ImportARPrice { get; set; }

    public string ImportARSalesGLAccount { get; set; }

    public virtual ImportARHeader ImportARHeader { get; set; }
  }
}
