// Decompiled with JetBrains decompiler
// Type: MASImportDLL.FutureLogMASImportEntities
// Assembly: MASImportDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AB4A0B53-0D39-4107-ADF5-3ED15259AA45
// Assembly location: C:\Temp\Red Pine Systems\MASImportDLL.dll

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace MASImportDLL
{
    public class FutureLogMASImportEntities : DbContext
    {
        public FutureLogMASImportEntities()
            : base("name=FutureLogMASImportEntities")
        {
        }


        public DbSet<ImportBatchFileError> ImportBatchFileErrors { get; set; }

        public DbSet<ImportBatchFile> ImportBatchFiles { get; set; }

        public DbSet<SystemParameter> SystemParameters { get; set; }

        public DbSet<ImportBatch> ImportBatches { get; set; }

        public DbSet<ImportAPDetail> ImportAPDetails { get; set; }

        public DbSet<ImportAPHeader> ImportAPHeaders { get; set; }

        public DbSet<ImportARDetail> ImportARDetails { get; set; }

        public DbSet<ImportARHeader> ImportARHeaders { get; set; }
    }
}