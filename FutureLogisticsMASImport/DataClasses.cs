// Decompiled with JetBrains decompiler
// Type: FutureLogisticsMASImport.ExtendedFileInfo
// Assembly: FutureLogisticsMASImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C3B527D-500A-4CC6-B04D-583364735731
// Assembly location: C:\Temp\Red Pine Systems\FutureLogisticsMASImport.exe

using System.IO;

namespace FutureLogisticsMASImport
{
  public class ExtendedFileInfo
  {
    public FileInfo FileInformation { get; set; }

    public string FileType { get; set; }

    public ExtendedFileInfo()
    {
      this.FileInformation = (FileInfo) null;
      this.FileType = string.Empty;
    }

    public ExtendedFileInfo(FileInfo fi, string ft)
    {
      this.FileInformation = fi;
      this.FileType = ft;
    }
  }
}
