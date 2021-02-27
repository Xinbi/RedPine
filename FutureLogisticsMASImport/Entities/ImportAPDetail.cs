// Decompiled with JetBrains decompiler
// Type: MASImportDLL.ImportAPDetail
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System;
using System.ComponentModel.DataAnnotations;

namespace MASImportDLL
{
    public class ImportAPDetail
    {
        [Key] 
        public int ImportAPDetailKey { get; set; }

        public int ImportAPHeaderKey { get; set; }

        public string ImportAPGLAccount { get; set; }

        public Decimal ImportAPAmount { get; set; }

        public virtual ImportAPHeader ImportAPHeader { get; set; }
    }
}
