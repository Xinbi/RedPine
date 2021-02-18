// Decompiled with JetBrains decompiler
// Type: FutureLogisticsMASImport.FtpHelper
// Assembly: FutureLogisticsMASImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C3B527D-500A-4CC6-B04D-583364735731
// Assembly location: C:\Temp\Red Pine Systems\FutureLogisticsMASImport.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FutureLogisticsMASImport
{
  public class FtpHelper
  {
    public void DeleteFileFromFtp(FtpFileInfo file, string ftpUserId, string ftpPassword)
    {
      FtpWebRequest ftpWebRequest = (FtpWebRequest) WebRequest.Create(new Uri(string.Format("ftp://{0}/{1}", (object) file.FtpFilePath, (object) file.FileName)));
      ftpWebRequest.UseBinary = true;
      ftpWebRequest.Credentials = (ICredentials) new NetworkCredential(ftpUserId, ftpPassword);
      ftpWebRequest.Method = "DELE";
      ftpWebRequest.KeepAlive = false;
      ftpWebRequest.Proxy = (IWebProxy) null;
      ftpWebRequest.UsePassive = false;
      FtpWebResponse response = (FtpWebResponse) ftpWebRequest.GetResponse();
    }

    public void DownloadFileFromFtp(
      FtpFileInfo file,
      string ftpUserId,
      string ftpPassword,
      string localPath)
    {
      try
      {
        FtpWebRequest ftpWebRequest = (FtpWebRequest) WebRequest.Create(new Uri(string.Format("ftp://{0}/{1}", (object) file.FtpFilePath, (object) file.FileName)));
        ftpWebRequest.UseBinary = true;
        ftpWebRequest.Credentials = (ICredentials) new NetworkCredential(ftpUserId, ftpPassword);
        ftpWebRequest.Method = "RETR";
        ftpWebRequest.KeepAlive = false;
        ftpWebRequest.Proxy = (IWebProxy) null;
        ftpWebRequest.UsePassive = false;
        FtpWebResponse response = (FtpWebResponse) ftpWebRequest.GetResponse();
        Stream responseStream = response.GetResponseStream();
        FileStream fileStream = new FileStream(string.Format("{0}\\{1}", (object) localPath, (object) file.FileName), FileMode.Create);
        int count1 = 2048;
        byte[] buffer = new byte[count1];
        for (int count2 = responseStream.Read(buffer, 0, count1); count2 > 0; count2 = responseStream.Read(buffer, 0, count1))
          fileStream.Write(buffer, 0, count2);
        fileStream.Close();
        response.Close();
      }
      catch (Exception ex)
      {
      }
    }

    public List<FtpFileInfo> GetFileList(
      string[] ftpServers,
      string ftpUserId,
      string ftpPassword,
      string[] fileMasks)
    {
      StringBuilder stringBuilder = new StringBuilder();
      WebResponse webResponse = (WebResponse) null;
      StreamReader streamReader = (StreamReader) null;
      List<FtpFileInfo> ftpFileInfoList = new List<FtpFileInfo>();
      try
      {
        foreach (string ftpServer in ftpServers)
        {
          FtpWebRequest ftpWebRequest = (FtpWebRequest) WebRequest.Create(new Uri(string.Format("ftp://{0}/", (object) ftpServer)));
          ftpWebRequest.UseBinary = true;
          ftpWebRequest.Credentials = (ICredentials) new NetworkCredential(ftpUserId, ftpPassword);
          ftpWebRequest.Method = "NLST";
          ftpWebRequest.KeepAlive = false;
          ftpWebRequest.Proxy = (IWebProxy) null;
          ftpWebRequest.UsePassive = false;
          webResponse = ftpWebRequest.GetResponse();
          streamReader = new StreamReader(webResponse.GetResponseStream());
          for (string input = streamReader.ReadLine(); input != null; input = streamReader.ReadLine())
          {
            foreach (string fileMask in fileMasks)
            {
              if (Regex.IsMatch(input, fileMask))
              {
                stringBuilder.Append(input);
                stringBuilder.Append("\n");
                break;
              }
            }
          }
          if (!string.IsNullOrEmpty(stringBuilder.ToString()))
          {
            int startIndex = stringBuilder.ToString().LastIndexOf('\n');
            if (startIndex >= 0)
              stringBuilder.Remove(startIndex, 1);
            string str = stringBuilder.ToString();
            char[] chArray = new char[1]{ '\n' };
            foreach (string fileName in str.Split(chArray))
              ftpFileInfoList.Add(new FtpFileInfo(fileName, ftpServer));
          }
        }
      }
      catch (Exception ex)
      {
        streamReader?.Close();
        webResponse?.Close();
        return (List<FtpFileInfo>) null;
      }
      return ftpFileInfoList;
    }
  }
}
