// Decompiled with JetBrains decompiler
// Type: FutureLogisticsMASImport.FtpFileInfo
// Assembly: FutureLogisticsMASImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C3B527D-500A-4CC6-B04D-583364735731
// Assembly location: C:\Temp\Red Pine Systems\FutureLogisticsMASImport.exe

namespace FutureLogisticsMASImport
{
  public class FtpFileInfo
  {
    public string FileName { get; set; }

    public string FtpFilePath { get; set; }

    public FtpFileInfo()
    {
      this.FileName = string.Empty;
      this.FtpFilePath = string.Empty;
    }

    public FtpFileInfo(string fileName, string ftpFilePath)
    {
      this.FileName = fileName;
      this.FtpFilePath = ftpFilePath;
    }
  }
}
