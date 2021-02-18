// Decompiled with JetBrains decompiler
// Type: FutureLogisticsMASImport.MASImportView
// Assembly: FutureLogisticsMASImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C3B527D-500A-4CC6-B04D-583364735731
// Assembly location: C:\Temp\Red Pine Systems\FutureLogisticsMASImport.exe

using MASImportDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FutureLogisticsMASImport
{
  public class MASImportView : Form
  {
    protected List<ExtendedFileInfo> _importFiles = new List<ExtendedFileInfo>();
    protected List<ImportBatchFile> _importBatchFiles = new List<ImportBatchFile>();
    protected FutureLogMASImportEntities _context;
    protected List<MASImportDLL.SystemParameter> _parms;
    private IContainer components;
    private TabControl tcApplication;
    private TabPage tpGetFiles;
    private Button cmdSearchFiles;
    private Label SelectDateLabel;
    private DateTimePicker dpSelectDate;
    private TabPage tpMAS;
    private Button cmdDeselectAll;
    private Button cmdSelectAll;
    private Button cmdImport;
    private Label lbAvailableFilesList;
    private ListView lvAvailableFiles;
    private ColumnHeader FileName;
    private ColumnHeader FileDate;
    private Label lblImportStatus;
    private ListBox lbImportStatus;
    private Button cmdDeleteSelected;
    private Button cmdDeselectAllMAS;
    private Button cmdSelectAllMAS;
    private Button cmdPostSelected;
    private Label lbFilesToProcess;
    private ListView lvFilesToProcess;
    private ColumnHeader File;
    private ColumnHeader LoadDate;
    private Button cmdDeselectAllPosted;
    private Button cmdSelectAllPosted;
    private Button cmdRollbackSelected;
    private ListView lvPostedFiles;
    private ColumnHeader PostedFiles;
    private ColumnHeader PostedDate;
    private Label lblPostedFiles;
    private Button cmdArchive;
    private ProgressBar pbFtpDownload;
    private BackgroundWorker bwFtpDownload;

    public MASImportView()
    {
      this.InitializeComponent();
    }

    private void MASImportView_Load(object sender, EventArgs e)
    {
      try
      {
        this._context = new FutureLogMASImportEntities();
        this._parms = this._context.SystemParameters.ToList<MASImportDLL.SystemParameter>();
      }
      catch (Exception ex)
      {
      }
    }

    private void bwFtpDownload_DoWork(object sender, DoWorkEventArgs e)
    {
      List<FtpFileInfo> source = e.Argument as List<FtpFileInfo>;
      if (source == null)
        return;
      int num = 0;
      FtpHelper ftpHelper = new FtpHelper();
      string parameterValue1 = this.GetParameterValue("FtpUserId");
      string parameterValue2 = this.GetParameterValue("FtpPassword");
      string parameterValue3 = this.GetParameterValue("InputFilePath");
      string str = this.GetParameterValue("DeleteFilesFromFTP");
      if (str.StartsWith("A", StringComparison.OrdinalIgnoreCase) && MessageBox.Show("Delete files from ftp server once downloaded?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
        str = "Y";
      bool flag = !string.IsNullOrEmpty(str) && str.StartsWith("Y", StringComparison.OrdinalIgnoreCase);
      foreach (FtpFileInfo file in source)
      {
        ++num;
        ftpHelper.DownloadFileFromFtp(file, parameterValue1, parameterValue2, parameterValue3);
        if (flag)
          ftpHelper.DeleteFileFromFtp(file, parameterValue1, parameterValue2);
        this.bwFtpDownload.ReportProgress(num / source.Count<FtpFileInfo>() * 100);
      }
    }

    private void bwFtpDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.pbFtpDownload.Value = e.ProgressPercentage;
    }

    private void bwFtpDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.pbFtpDownload.Visible = false;
    }

    private void cmdArchive_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Archive selected files without importing?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
        foreach (ListViewItem checkedItem in this.lvAvailableFiles.CheckedItems)
        {
          ListViewItem lvi = checkedItem;
          ExtendedFileInfo extendedFileInfo = this._importFiles.Single<ExtendedFileInfo>((Func<ExtendedFileInfo, bool>) (f => f.FileInformation.Name.Equals(lvi.SubItems[0].Text)));
          this._importFiles.Remove(extendedFileInfo);
          extendedFileInfo.FileInformation.MoveTo(string.Format("{0}\\{1}_{2}{3}", (object) this.GetParameterValue("ArchiveFilePath"), (object) Path.GetFileNameWithoutExtension(extendedFileInfo.FileInformation.Name), (object) DateTime.Now.ToString("yyyyMMddhhmmssff"), (object) extendedFileInfo.FileInformation.Extension));
        }
        this.lvAvailableFiles.Items.Clear();
        this._importFiles.ForEach((Action<ExtendedFileInfo>) (f => this.lvAvailableFiles.Items.Add(new ListViewItem(new string[2]
        {
          f.FileInformation.Name,
          f.FileInformation.LastWriteTime.ToString()
        }))));
        this.SetListViewRows(this.lvAvailableFiles);
      }
      this.ToggleImportCommandButtons();
    }

    private void cmdDeleteSelected_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Delete selected files from the MAS import queue?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
        foreach (ListViewItem checkedItem in this.lvFilesToProcess.CheckedItems)
        {
          int tempBatchFileKey = int.Parse(checkedItem.SubItems[2].Text);
          this._context.ImportBatchFiles.Where<ImportBatchFile>((Expression<Func<ImportBatchFile, bool>>) (bf => bf.ImportBatchFileKey == tempBatchFileKey)).ToList<ImportBatchFile>().ForEach((Action<ImportBatchFile>) (a =>
          {
            int tempBatchKey = a.ImportBatch.ImportBatchKey;
            this._context.ImportBatchFiles.Remove(a);
            this._context.ImportBatches.Where<ImportBatch>((Expression<Func<ImportBatch, bool>>) (b => b.ImportBatchKey == tempBatchKey)).ToList<ImportBatch>().ForEach((Action<ImportBatch>) (aa =>
            {
              if (aa.ImportBatchFiles.Count != 0)
                return;
              this._context.ImportBatches.Remove(aa);
            }));
          }));
        }
        this._context.SaveChanges();
        this.LoadMASFileLists();
      }
      this.ToggleMASProcessCommandButtons();
    }

    private void cmdDeselectAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvAvailableFiles.Items)
        listViewItem.Checked = false;
      this.ToggleImportCommandButtons();
    }

    private void cmdDeselectAllMAS_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvFilesToProcess.Items)
        listViewItem.Checked = false;
      this.ToggleMASProcessCommandButtons();
    }

    private void cmdDeselectAllPosted_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvPostedFiles.Items)
        listViewItem.Checked = false;
      this.ToggleMASPostedCommandButtons();
    }

    private void cmdImport_Click(object sender, EventArgs e)
    {
      this.ProcessBatch();
      this.ToggleImportCommandButtons();
    }

    private void cmdPostSelected_Click(object sender, EventArgs e)
    {
      this.ProcessPostToMAS();
      this.ToggleMASProcessCommandButtons();
    }

    private void cmdRollbackSelected_Click(object sender, EventArgs e)
    {
      int num = (int) MessageBox.Show("Rollback functionality is not supported in this release");
      foreach (ListViewItem listViewItem in this.lvPostedFiles.Items)
        listViewItem.Checked = false;
      this.ToggleMASPostedCommandButtons();
    }

    private void cmdSearchFiles_Click(object sender, EventArgs e)
    {
      try
      {
        this.lvAvailableFiles.Items.Clear();
        this._importFiles.Clear();
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.StartupPath, "Input"));
        var fileInfos = (IEnumerable<FileInfo>) directoryInfo.GetFiles("AR*.xml")
	        .Where<FileInfo>((Func<FileInfo, bool>) (f => f.LastWriteTime.Date >= this.dpSelectDate.Value.Date));
        foreach (FileInfo fi in fileInfos) 
          this._importFiles.Add(new ExtendedFileInfo(fi, "AR"));
        var enumerable = (IEnumerable<FileInfo>) directoryInfo.GetFiles("AP*.xml")
	        .Where<FileInfo>((Func<FileInfo, bool>) (f => f.LastWriteTime.Date >= this.dpSelectDate.Value.Date));
        foreach (FileInfo fi2 in enumerable)
          this._importFiles.Add(new ExtendedFileInfo(fi2, "AP"));
        this._importFiles.ForEach((Action<ExtendedFileInfo>) (f => this.lvAvailableFiles.Items.Add(new ListViewItem(new string[2]
        {
          f.FileInformation.Name,
          f.FileInformation.LastWriteTime.ToString()
        }))));
        this.SetListViewRows(this.lvAvailableFiles);
        this.cmdSelectAll.Enabled = this.lvAvailableFiles.Items.Count > 0;
      }
      catch (Exception ex)
      {
	      MessageBox.Show("Unexpected Problem", ex.Message, MessageBoxButtons.OK);
      }
    }

    private void cmdSelectAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvAvailableFiles.Items)
        listViewItem.Checked = true;
      this.ToggleImportCommandButtons();
    }

    private void cmdSelectAllMAS_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvFilesToProcess.Items)
        listViewItem.Checked = true;
      this.ToggleMASProcessCommandButtons();
    }

    private void cmdSelectAllPosted_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvPostedFiles.Items)
        listViewItem.Checked = true;
      this.ToggleMASPostedCommandButtons();
    }

    private void lvAvailableFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      this.lvAvailableFiles.Items[e.Item.Index].Checked = e.Item.Checked;
      this.ToggleImportCommandButtons();
    }

    private void lvFilesToProcess_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      this.lvFilesToProcess.Items[e.Item.Index].Checked = e.Item.Checked;
      this.ToggleMASProcessCommandButtons();
    }

    private void lvPostedFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      this.lvPostedFiles.Items[e.Item.Index].Checked = e.Item.Checked;
      this.ToggleMASPostedCommandButtons();
    }

    private void tcApplication_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        this.LoadMASFileLists();
      }
      catch (Exception ex)
      {
      }
    }

    private string FormatFileField(
      string fieldToFormat,
      FillLocation location,
      int fieldLength,
      string fill)
    {
      string str = fieldToFormat;
      if (fieldToFormat.Length < fieldLength)
      {
        while (str.Length < fieldLength)
          str = location == FillLocation.Prepend ? string.Format("{0}{1}", (object) fill, (object) str) : string.Format("{0}{1}", (object) str, (object) fill);
        if (str.Length > fieldLength)
          str = str.Substring(0, fieldLength);
      }
      else
        str = str.Substring(0, fieldLength);
      return str;
    }

    private string GetParameterValue(string parmName)
    {
      return this._parms.Where<MASImportDLL.SystemParameter>((Func<MASImportDLL.SystemParameter, bool>) (a => a.SystemParmID == parmName)).SingleOrDefault<MASImportDLL.SystemParameter>()?.SystemParmValue;
    }

    private void LoadMASFileLists()
    {
      if (this.tcApplication.SelectedTab != this.tpMAS)
        return;
      this._importBatchFiles = this._context.ImportBatchFiles.ToList<ImportBatchFile>();
      this.lvFilesToProcess.Items.Clear();
      this._importBatchFiles.Where<ImportBatchFile>((Func<ImportBatchFile, bool>) (bf =>
      {
        if (bf.ImportBatchFilePostDateTime.HasValue)
          return bf.ImportBatchFileRollbackDateTime.HasValue;
        return true;
      })).ToList<ImportBatchFile>().ForEach((Action<ImportBatchFile>) (bf => this.lvFilesToProcess.Items.Add(new ListViewItem(new string[3]
      {
        bf.ImportBatchFileName,
        bf.ImportBatch.ImportBatchLoadDateTime.ToString(),
        bf.ImportBatchFileKey.ToString()
      }))));
      this.SetListViewRows(this.lvFilesToProcess);
      this.cmdSelectAllMAS.Enabled = this.lvFilesToProcess.Items.Count > 0;
      this.lvPostedFiles.Items.Clear();
    }

    private string MapAccountingCode(string expType)
    {
      string empty = string.Empty;
      string str;
      switch (expType.ToUpper())
      {
        case "DEL":
        case "":
          str = "1";
          break;
        case "FSC":
          str = "2";
          break;
        case "DET":
          str = "4";
          break;
        case "TAX":
          str = "5";
          break;
        default:
          str = "3";
          break;
      }
      return str;
    }

    private int ProcessAPFile(ImportBatchFile batchFile, FileInfo fi)
    {
      int num = 0;
      try
      {
        List<string> stringList = new List<string>();
        foreach (XElement xelement in XElement.Load(fi.FullName).Elements((XName) "MasterBillOfLading").Elements<XElement>((XName) "PriceSheets").Elements<XElement>((XName) "PriceSheet").Where<XElement>((Func<XElement, bool>) (i => i.GetAttributeValue<string>("type", new string[0]).Equals("billed", StringComparison.OrdinalIgnoreCase))))
        {
          bool flag = xelement.Element((XName) "Consolidation").GetAttributeValue<int>("count", new int[1]) > 0;
          string elementValue1 = xelement.Element((XName) "Settlement").Element((XName) "InvoiceNumber").GetElementValue<string>(new string[0]);
          if (!flag && !stringList.Contains(elementValue1))
          {
            stringList.Add(elementValue1);
            ImportAPHeader importApHeader = new ImportAPHeader();
            importApHeader.ImportAPVendorNumber = this.FormatFileField(xelement.Element((XName) "Id").GetElementValue<string>(new string[0]), FillLocation.Prepend, 7, "0");
            importApHeader.ImportAPInvoiceNumber = elementValue1.StartsWith("M") ? elementValue1.Substring(1) : elementValue1;
            importApHeader.ImportAPInvoiceDate = xelement.Element((XName) "Settlement").Element((XName) "Date").GetElementValue<DateTime>(new DateTime[0]);
            ++num;
            foreach (XElement element in xelement.Elements((XName) "Charges").Elements<XElement>((XName) "Charge"))
            {
              Decimal? elementValue2 = element.Element((XName) "Amount").GetElementValue<Decimal?>(new Decimal?[0]);
              if (elementValue2.HasValue)
              {
                importApHeader.ImportAPDetails.Add(new ImportAPDetail()
                {
                  ImportAPAmount = elementValue2.Value,
                  ImportAPGLAccount = element.Element((XName) "GLCode").GetElementValue<string>(new string[0])
                });
                ++num;
              }
            }
            batchFile.ImportAPHeaders.Add(importApHeader);
          }
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      return num;
    }

    private int ProcessARFile(ImportBatchFile batchFile, FileInfo fi)
    {
      int num = 0;
      XElement xelement1 = XElement.Load(fi.FullName);
      List<string> stringList1 = new List<string>();
      foreach (XElement element1 in xelement1.Elements((XName) "Shipment"))
      {
        string elementValue1 = element1.Element((XName) "ReferenceNumbers").Elements((XName) "ReferenceNumber").First<XElement>((Func<XElement, bool>) (i => i.GetAttributeValue<string>("type", new string[0]).Equals("Master Invoice Ref", StringComparison.OrdinalIgnoreCase))).GetElementValue<string>(new string[0]);
        string elementValue2 = element1.Element((XName) "ReferenceNumbers").Elements((XName) "ReferenceNumber").FirstOrDefault<XElement>((Func<XElement, bool>) (i => i.GetAttributeValue<string>("type", new string[0]).Equals("Customer Acct Number", StringComparison.OrdinalIgnoreCase))).GetElementValue<string>(new string[0]);
        foreach (XElement xelement2 in element1.Elements((XName) "PriceSheets").Elements<XElement>((XName) "PriceSheet").Where<XElement>((Func<XElement, bool>) (i => i.GetAttributeValue<string>("type", new string[0]).Equals("invoice", StringComparison.OrdinalIgnoreCase))))
        {
          bool flag = xelement2.Element((XName) "Consolidation").GetAttributeValue<int>("count", new int[1]) > 0;
          string elementValue3 = xelement2.Element((XName) "Settlement").Element((XName) "InvoiceNumber").GetElementValue<string>(new string[0]);
          if (flag && !stringList1.Contains(elementValue1) && elementValue1.Equals(elementValue3, StringComparison.OrdinalIgnoreCase))
          {
            stringList1.Add(elementValue1);
            ImportARHeader importArHeader = new ImportARHeader();
            importArHeader.ImportARInvoiceNumber = elementValue3;
            importArHeader.ImportARCustomerNumber = elementValue2.EndsWith("A", StringComparison.OrdinalIgnoreCase) ? elementValue2.Substring(0, elementValue2.Length - 1) : elementValue2;
            importArHeader.ImportARInvoiceDate = xelement2.Element((XName) "Settlement").Element((XName) "Date").GetElementValue<DateTime>(new DateTime[0]);
            importArHeader.ImportARCustomerPONumber = string.Empty;
            List<string> stringList2 = new List<string>();
            foreach (XElement elem in element1.Element((XName) "ReferenceNumbers").Elements((XName) "ReferenceNumber").Where<XElement>((Func<XElement, bool>) (i => i.GetAttributeValue<string>("type", new string[0]).Equals("JobNumber", StringComparison.OrdinalIgnoreCase))))
            {
              string elementValue4 = elem.GetElementValue<string>(new string[0]);
              if (!stringList2.Contains(elementValue4))
                stringList2.Add(elementValue4);
            }
            importArHeader.ImportARJobNumber = string.Join(",", (IEnumerable<string>) stringList2);
            ++num;
            foreach (XElement element2 in xelement2.Elements((XName) "Charges").Elements<XElement>((XName) "Charge"))
            {
              Decimal? elementValue4 = element2.Element((XName) "Amount").GetElementValue<Decimal?>(new Decimal?[0]);
              if (elementValue4.HasValue)
              {
                importArHeader.ImportARDetails.Add(new ImportARDetail()
                {
                  ImportARItemCode = this.MapAccountingCode(element2.Element((XName) "EdiCode").GetElementValue<string>(new string[0])),
                  ImportARItemQuantity = element2.Element((XName) "Quantity").GetElementValue<int?>(new int?[0]) ?? 1,
                  ImportARItemDescription = element2.Element((XName) "Description").GetElementValue<string>(new string[0]),
                  ImportARPrice = elementValue4.Value,
                  ImportARSalesGLAccount = element2.Element((XName) "GLCode").GetElementValue<string>(new string[0])
                });
                ++num;
              }
            }
            batchFile.ImportARHeaders.Add(importArHeader);
          }
        }
      }
      return num;
    }

    private void ProcessBatch()
    {
      ImportBatch entity = new ImportBatch();
			try
			{
				entity.ImportLoadUser = new Guid?(new Guid(this.GetParameterValue("ImportUser")));
			}
			catch (Exception ex)
			{
				entity.ImportLoadUser = Guid.NewGuid();
			}
			
      entity.ImportBatchLoadDateTime = DateTime.Now;
      foreach (ListViewItem checkedItem in this.lvAvailableFiles.CheckedItems)
      {
        ListViewItem lvi = checkedItem;
        ImportBatchFile batchFile = new ImportBatchFile();
        ExtendedFileInfo extendedFileInfo = this._importFiles.Single<ExtendedFileInfo>((Func<ExtendedFileInfo, bool>) (f => f.FileInformation.Name.Equals(lvi.SubItems[0].Text)));
        batchFile.ImportBatchFileName = extendedFileInfo.FileInformation.Name;
        batchFile.ImportBatchFileType = extendedFileInfo.FileType;
        batchFile.ImportBatchFileRecordCount = 0;
        batchFile.ImportBatchFilePostDateTime = new DateTime?();
        batchFile.ImportBatchFilePostUser = new Guid?();
        batchFile.ImportBatchFileRollbackDateTime = new DateTime?();
        batchFile.ImportBatchFileRollbackUser = new Guid?();
        batchFile.ImportBatchFileRecordCount = !batchFile.ImportBatchFileType.Equals("AR") ? this.ProcessAPFile(batchFile, extendedFileInfo.FileInformation) : this.ProcessARFile(batchFile, extendedFileInfo.FileInformation);
        entity.ImportBatchFiles.Add(batchFile);
        this._importFiles.Remove(extendedFileInfo);
        var archivePath = Path.Combine(Application.StartupPath, "Archive");
        if(!Directory.Exists(archivePath))
			  Directory.CreateDirectory(archivePath);
        extendedFileInfo.FileInformation.MoveTo(string.Format("{0}\\{1}_{2}{3}", (object)archivePath, (object) Path.GetFileNameWithoutExtension(extendedFileInfo.FileInformation.Name), (object) DateTime.Now.ToString("yyyyMMddhhmmssff"), (object) extendedFileInfo.FileInformation.Extension));
      }
      this._context.ImportBatches.Add(entity);
      this._context.SaveChanges();
      this.lvAvailableFiles.Items.Clear();
      this._importFiles.ForEach((Action<ExtendedFileInfo>) (f => this.lvAvailableFiles.Items.Add(new ListViewItem(new string[2]
      {
        f.FileInformation.Name,
        f.FileInformation.LastWriteTime.ToString()
      }))));
      this.SetListViewRows(this.lvAvailableFiles);
    }

    private void ProcessPostToMAS()
    {
      string parameterValue = this.GetParameterValue("MASExportPath");
      DateTime now = DateTime.Now;
      bool flag1 = false;
      bool flag2 = false;
      StreamWriter streamWriter1 = (StreamWriter) null;
      StreamWriter streamWriter2 = (StreamWriter) null;
      foreach (ListViewItem checkedItem in this.lvFilesToProcess.CheckedItems)
      {
        int tempBatchFileKey = int.Parse(checkedItem.SubItems[2].Text);
        ImportBatchFile importBatchFile = this._context.ImportBatchFiles.Single<ImportBatchFile>((Expression<Func<ImportBatchFile, bool>>) (bf => bf.ImportBatchFileKey == tempBatchFileKey));
        if (importBatchFile.ImportAPHeaders.Count<ImportAPHeader>() > 0)
        {
          if (!flag2)
          {
            streamWriter1 = new StreamWriter(string.Format("{0}\\MASExport_AP_{1}.csv", (object) parameterValue, (object) now.ToString("yyyyMMddhhmmssff")));
            flag2 = true;
          }
          if (flag2)
          {
            foreach (ImportAPHeader importApHeader in (IEnumerable<ImportAPHeader>) importBatchFile.ImportAPHeaders)
            {
              foreach (ImportAPDetail importApDetail in (IEnumerable<ImportAPDetail>) importApHeader.ImportAPDetails)
              {
                if (importApDetail.ImportAPAmount != new Decimal(0))
                  streamWriter1.WriteLine(string.Join(",", importApHeader.ImportAPVendorNumber, importApHeader.ImportAPInvoiceNumber, importApHeader.ImportAPInvoiceDate.ToShortDateString(), importApDetail.ImportAPGLAccount, importApDetail.ImportAPAmount.ToString()));
              }
            }
          }
        }
        if (importBatchFile.ImportARHeaders.Count<ImportARHeader>() > 0)
        {
          if (!flag1)
          {
            streamWriter2 = new StreamWriter(string.Format("{0}\\MASExport_AR_{1}.csv", (object) parameterValue, (object) now.ToString("yyyyMMddhhmmssff")));
            flag1 = true;
          }
          if (flag1)
          {
            foreach (ImportARHeader importArHeader in (IEnumerable<ImportARHeader>) importBatchFile.ImportARHeaders)
            {
              foreach (ImportARDetail importArDetail in (IEnumerable<ImportARDetail>) importArHeader.ImportARDetails)
              {
                if (importArDetail.ImportARPrice != new Decimal(0))
                {
                  string[] strArray1 = importArHeader.ImportARJobNumber.Split(new string[1]
                  {
                    ","
                  }, StringSplitOptions.None);
                  if (strArray1 == null)
                    strArray1 = new string[1]
                    {
                      string.Empty
                    };
                  string[] strArray2 = strArray1;
                  streamWriter2.WriteLine(string.Join(",", importArHeader.ImportARInvoiceNumber.ToString(), importArHeader.ImportARCustomerNumber, importArHeader.ImportARInvoiceDate.ToShortDateString(), importArHeader.ImportARCustomerPONumber, importArDetail.ImportARItemCode, importArDetail.ImportARItemQuantity.ToString(), importArDetail.ImportARItemDescription, importArDetail.ImportARPrice.ToString(), importArDetail.ImportARSalesGLAccount, strArray2[0]));
                }
              }
            }
          }
        }
        importBatchFile.ImportBatchFilePostUser = new Guid?(new Guid(this.GetParameterValue("ImportUser")));
        importBatchFile.ImportBatchFilePostDateTime = new DateTime?(now);
        this._context.SaveChanges();
      }
      if (flag2)
      {
        streamWriter1.Flush();
        streamWriter1.Close();
        streamWriter1.Dispose();
      }
      if (flag1)
      {
        streamWriter2.Flush();
        streamWriter2.Close();
        streamWriter2.Dispose();
      }
      this.LoadMASFileLists();
      this.ToggleMASProcessCommandButtons();
    }

    private void SetListViewRows(ListView lv)
    {
      foreach (ListViewItem listViewItem in lv.Items)
        listViewItem.BackColor = listViewItem.Index % 2 == 0 ? Color.AntiqueWhite : Color.NavajoWhite;
    }

    private void ToggleImportCommandButtons()
    {
      this.cmdSelectAll.Enabled = this.lvAvailableFiles.CheckedItems.Count != this.lvAvailableFiles.Items.Count;
      this.cmdDeselectAll.Enabled = this.lvAvailableFiles.CheckedItems.Count > 0;
      this.cmdImport.Enabled = this.lvAvailableFiles.CheckedItems.Count > 0;
      this.cmdArchive.Enabled = this.lvAvailableFiles.CheckedItems.Count > 0;
    }

    private void ToggleMASPostedCommandButtons()
    {
      this.cmdSelectAllPosted.Enabled = this.lvPostedFiles.CheckedItems.Count != this.lvPostedFiles.Items.Count;
      this.cmdDeselectAllPosted.Enabled = this.lvPostedFiles.CheckedItems.Count > 0;
      this.cmdRollbackSelected.Enabled = this.lvPostedFiles.CheckedItems.Count > 0;
    }

    private void ToggleMASProcessCommandButtons()
    {
      this.cmdSelectAllMAS.Enabled = this.lvFilesToProcess.CheckedItems.Count != this.lvFilesToProcess.Items.Count;
      this.cmdDeselectAllMAS.Enabled = this.lvFilesToProcess.CheckedItems.Count > 0;
      this.cmdPostSelected.Enabled = this.lvFilesToProcess.CheckedItems.Count > 0;
      this.cmdDeleteSelected.Enabled = this.lvFilesToProcess.CheckedItems.Count > 0;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tcApplication = new TabControl();
      this.tpGetFiles = new TabPage();
      this.pbFtpDownload = new ProgressBar();
      this.cmdArchive = new Button();
      this.lblImportStatus = new Label();
      this.lbImportStatus = new ListBox();
      this.cmdDeselectAll = new Button();
      this.cmdSelectAll = new Button();
      this.cmdImport = new Button();
      this.lbAvailableFilesList = new Label();
      this.lvAvailableFiles = new ListView();
      this.FileName = new ColumnHeader();
      this.FileDate = new ColumnHeader();
      this.cmdSearchFiles = new Button();
      this.SelectDateLabel = new Label();
      this.dpSelectDate = new DateTimePicker();
      this.tpMAS = new TabPage();
      this.cmdDeselectAllPosted = new Button();
      this.cmdSelectAllPosted = new Button();
      this.cmdRollbackSelected = new Button();
      this.lvPostedFiles = new ListView();
      this.PostedFiles = new ColumnHeader();
      this.PostedDate = new ColumnHeader();
      this.lblPostedFiles = new Label();
      this.cmdDeleteSelected = new Button();
      this.cmdDeselectAllMAS = new Button();
      this.cmdSelectAllMAS = new Button();
      this.cmdPostSelected = new Button();
      this.lbFilesToProcess = new Label();
      this.lvFilesToProcess = new ListView();
      this.File = new ColumnHeader();
      this.LoadDate = new ColumnHeader();
      this.bwFtpDownload = new BackgroundWorker();
      this.tcApplication.SuspendLayout();
      this.tpGetFiles.SuspendLayout();
      this.tpMAS.SuspendLayout();
      this.SuspendLayout();
      this.tcApplication.Controls.Add((Control) this.tpGetFiles);
      this.tcApplication.Controls.Add((Control) this.tpMAS);
      this.tcApplication.Location = new Point(12, 12);
      this.tcApplication.Name = "tcApplication";
      this.tcApplication.SelectedIndex = 0;
      this.tcApplication.Size = new Size(698, 671);
      this.tcApplication.TabIndex = 0;
      this.tcApplication.SelectedIndexChanged += new EventHandler(this.tcApplication_SelectedIndexChanged);
      this.tpGetFiles.Controls.Add((Control) this.pbFtpDownload);
      this.tpGetFiles.Controls.Add((Control) this.cmdArchive);
      this.tpGetFiles.Controls.Add((Control) this.lblImportStatus);
      this.tpGetFiles.Controls.Add((Control) this.lbImportStatus);
      this.tpGetFiles.Controls.Add((Control) this.cmdDeselectAll);
      this.tpGetFiles.Controls.Add((Control) this.cmdSelectAll);
      this.tpGetFiles.Controls.Add((Control) this.cmdImport);
      this.tpGetFiles.Controls.Add((Control) this.lbAvailableFilesList);
      this.tpGetFiles.Controls.Add((Control) this.lvAvailableFiles);
      this.tpGetFiles.Controls.Add((Control) this.cmdSearchFiles);
      this.tpGetFiles.Controls.Add((Control) this.SelectDateLabel);
      this.tpGetFiles.Controls.Add((Control) this.dpSelectDate);
      this.tpGetFiles.Location = new Point(4, 22);
      this.tpGetFiles.Name = "tpGetFiles";
      this.tpGetFiles.Padding = new Padding(3);
      this.tpGetFiles.Size = new Size(690, 645);
      this.tpGetFiles.TabIndex = 0;
      this.tpGetFiles.Text = "Get Files For Import";
      this.tpGetFiles.UseVisualStyleBackColor = true;
      this.pbFtpDownload.Location = new Point(261, 241);
      this.pbFtpDownload.Name = "pbFtpDownload";
      this.pbFtpDownload.Size = new Size(209, 23);
      this.pbFtpDownload.Style = ProgressBarStyle.Marquee;
      this.pbFtpDownload.TabIndex = 1;
      this.pbFtpDownload.Visible = false;
      this.cmdArchive.Enabled = false;
      this.cmdArchive.Location = new Point(380, 397);
      this.cmdArchive.Name = "cmdArchive";
      this.cmdArchive.Size = new Size(117, 23);
      this.cmdArchive.TabIndex = 10;
      this.cmdArchive.Text = "Archive Selected Files";
      this.cmdArchive.UseVisualStyleBackColor = true;
      this.cmdArchive.Click += new EventHandler(this.cmdArchive_Click);
      this.lblImportStatus.AutoSize = true;
      this.lblImportStatus.Location = new Point(8, 446);
      this.lblImportStatus.Name = "lblImportStatus";
      this.lblImportStatus.Size = new Size(72, 13);
      this.lblImportStatus.TabIndex = 8;
      this.lblImportStatus.Text = "Import Status:";
      this.lbImportStatus.FormattingEnabled = true;
      this.lbImportStatus.Location = new Point(11, 462);
      this.lbImportStatus.Name = "lbImportStatus";
      this.lbImportStatus.SelectionMode = SelectionMode.None;
      this.lbImportStatus.Size = new Size(650, 160);
      this.lbImportStatus.TabIndex = 9;
      this.cmdDeselectAll.Enabled = false;
      this.cmdDeselectAll.Location = new Point(119, 397);
      this.cmdDeselectAll.Name = "cmdDeselectAll";
      this.cmdDeselectAll.Size = new Size(93, 23);
      this.cmdDeselectAll.TabIndex = 6;
      this.cmdDeselectAll.Text = "&Deselect All";
      this.cmdDeselectAll.UseVisualStyleBackColor = true;
      this.cmdDeselectAll.Click += new EventHandler(this.cmdDeselectAll_Click);
      this.cmdSelectAll.Enabled = false;
      this.cmdSelectAll.Location = new Point(11, 397);
      this.cmdSelectAll.Name = "cmdSelectAll";
      this.cmdSelectAll.Size = new Size(93, 23);
      this.cmdSelectAll.TabIndex = 5;
      this.cmdSelectAll.Text = "Select &All";
      this.cmdSelectAll.UseVisualStyleBackColor = true;
      this.cmdSelectAll.Click += new EventHandler(this.cmdSelectAll_Click);
      this.cmdImport.Enabled = false;
      this.cmdImport.Location = new Point(231, 397);
      this.cmdImport.Name = "cmdImport";
      this.cmdImport.Size = new Size(133, 23);
      this.cmdImport.TabIndex = 7;
      this.cmdImport.Text = "&Import Selected Files";
      this.cmdImport.UseVisualStyleBackColor = true;
      this.cmdImport.Click += new EventHandler(this.cmdImport_Click);
      this.lbAvailableFilesList.AutoSize = true;
      this.lbAvailableFilesList.Location = new Point(8, 73);
      this.lbAvailableFilesList.Name = "lbAvailableFilesList";
      this.lbAvailableFilesList.Size = new Size(77, 13);
      this.lbAvailableFilesList.TabIndex = 3;
      this.lbAvailableFilesList.Text = "Available Files:";
      this.lvAvailableFiles.CheckBoxes = true;
      this.lvAvailableFiles.Columns.AddRange(new ColumnHeader[2]
      {
        this.FileName,
        this.FileDate
      });
      this.lvAvailableFiles.GridLines = true;
      this.lvAvailableFiles.HideSelection = false;
      this.lvAvailableFiles.Location = new Point(11, 89);
      this.lvAvailableFiles.Name = "lvAvailableFiles";
      this.lvAvailableFiles.Size = new Size(650, 300);
      this.lvAvailableFiles.Sorting = SortOrder.Ascending;
      this.lvAvailableFiles.TabIndex = 4;
      this.lvAvailableFiles.UseCompatibleStateImageBehavior = false;
      this.lvAvailableFiles.View = View.Details;
      this.lvAvailableFiles.ItemChecked += new ItemCheckedEventHandler(this.lvAvailableFiles_ItemChecked);
      this.FileName.Text = "File Name";
      this.FileName.Width = 450;
      this.FileDate.Text = "Modified Date";
      this.FileDate.TextAlign = HorizontalAlignment.Center;
      this.FileDate.Width = 175;
      this.cmdSearchFiles.Location = new Point(360, 14);
      this.cmdSearchFiles.Name = "cmdSearchFiles";
      this.cmdSearchFiles.Size = new Size(75, 23);
      this.cmdSearchFiles.TabIndex = 2;
      this.cmdSearchFiles.Text = "&Search";
      this.cmdSearchFiles.UseVisualStyleBackColor = true;
      this.cmdSearchFiles.Click += new EventHandler(this.cmdSearchFiles_Click);
      this.SelectDateLabel.AutoSize = true;
      this.SelectDateLabel.Location = new Point(8, 23);
      this.SelectDateLabel.Name = "SelectDateLabel";
      this.SelectDateLabel.Size = new Size(177, 13);
      this.SelectDateLabel.TabIndex = 0;
      this.SelectDateLabel.Text = "Select a start date for available files:";
      this.dpSelectDate.Format = DateTimePickerFormat.Short;
      this.dpSelectDate.Location = new Point(191, 17);
      this.dpSelectDate.MaxDate = new DateTime(3014, 12, 31, 0, 0, 0, 0);
      this.dpSelectDate.MinDate = new DateTime(2014, 1, 1, 0, 0, 0, 0);
      this.dpSelectDate.Name = "dpSelectDate";
      this.dpSelectDate.Size = new Size(147, 20);
      this.dpSelectDate.TabIndex = 1;
      this.tpMAS.Controls.Add((Control) this.cmdDeselectAllPosted);
      this.tpMAS.Controls.Add((Control) this.cmdSelectAllPosted);
      this.tpMAS.Controls.Add((Control) this.cmdRollbackSelected);
      this.tpMAS.Controls.Add((Control) this.lvPostedFiles);
      this.tpMAS.Controls.Add((Control) this.lblPostedFiles);
      this.tpMAS.Controls.Add((Control) this.cmdDeleteSelected);
      this.tpMAS.Controls.Add((Control) this.cmdDeselectAllMAS);
      this.tpMAS.Controls.Add((Control) this.cmdSelectAllMAS);
      this.tpMAS.Controls.Add((Control) this.cmdPostSelected);
      this.tpMAS.Controls.Add((Control) this.lbFilesToProcess);
      this.tpMAS.Controls.Add((Control) this.lvFilesToProcess);
      this.tpMAS.Location = new Point(4, 22);
      this.tpMAS.Name = "tpMAS";
      this.tpMAS.Padding = new Padding(3);
      this.tpMAS.Size = new Size(690, 645);
      this.tpMAS.TabIndex = 1;
      this.tpMAS.Text = "MAS Interaction";
      this.tpMAS.UseVisualStyleBackColor = true;
      this.cmdDeselectAllPosted.Enabled = false;
      this.cmdDeselectAllPosted.Location = new Point(126, 597);
      this.cmdDeselectAllPosted.Name = "cmdDeselectAllPosted";
      this.cmdDeselectAllPosted.Size = new Size(93, 23);
      this.cmdDeselectAllPosted.TabIndex = 19;
      this.cmdDeselectAllPosted.Text = "&Deselect All";
      this.cmdDeselectAllPosted.UseVisualStyleBackColor = true;
      this.cmdDeselectAllPosted.Visible = false;
      this.cmdDeselectAllPosted.Click += new EventHandler(this.cmdDeselectAllPosted_Click);
      this.cmdSelectAllPosted.Enabled = false;
      this.cmdSelectAllPosted.Location = new Point(18, 597);
      this.cmdSelectAllPosted.Name = "cmdSelectAllPosted";
      this.cmdSelectAllPosted.Size = new Size(93, 23);
      this.cmdSelectAllPosted.TabIndex = 18;
      this.cmdSelectAllPosted.Text = "Select &All";
      this.cmdSelectAllPosted.UseVisualStyleBackColor = true;
      this.cmdSelectAllPosted.Visible = false;
      this.cmdSelectAllPosted.Click += new EventHandler(this.cmdSelectAllPosted_Click);
      this.cmdRollbackSelected.Enabled = false;
      this.cmdRollbackSelected.Location = new Point(238, 597);
      this.cmdRollbackSelected.Name = "cmdRollbackSelected";
      this.cmdRollbackSelected.Size = new Size(133, 23);
      this.cmdRollbackSelected.TabIndex = 20;
      this.cmdRollbackSelected.Text = "&Rollback Selected";
      this.cmdRollbackSelected.UseVisualStyleBackColor = true;
      this.cmdRollbackSelected.Visible = false;
      this.cmdRollbackSelected.Click += new EventHandler(this.cmdRollbackSelected_Click);
      this.lvPostedFiles.CheckBoxes = true;
      this.lvPostedFiles.Columns.AddRange(new ColumnHeader[2]
      {
        this.PostedFiles,
        this.PostedDate
      });
      this.lvPostedFiles.GridLines = true;
      this.lvPostedFiles.Location = new Point(18, 416);
      this.lvPostedFiles.Name = "lvPostedFiles";
      this.lvPostedFiles.Size = new Size(656, 175);
      this.lvPostedFiles.TabIndex = 17;
      this.lvPostedFiles.UseCompatibleStateImageBehavior = false;
      this.lvPostedFiles.View = View.Details;
      this.lvPostedFiles.Visible = false;
      this.lvPostedFiles.ItemChecked += new ItemCheckedEventHandler(this.lvPostedFiles_ItemChecked);
      this.PostedFiles.Text = "File Name";
      this.PostedFiles.Width = 450;
      this.PostedDate.Text = "Posted Date";
      this.PostedDate.TextAlign = HorizontalAlignment.Center;
      this.PostedDate.Width = 175;
      this.lblPostedFiles.AutoSize = true;
      this.lblPostedFiles.Location = new Point(15, 400);
      this.lblPostedFiles.Name = "lblPostedFiles";
      this.lblPostedFiles.Size = new Size(67, 13);
      this.lblPostedFiles.TabIndex = 16;
      this.lblPostedFiles.Text = "Posted Files:";
      this.lblPostedFiles.Visible = false;
      this.cmdDeleteSelected.Enabled = false;
      this.cmdDeleteSelected.Location = new Point(387, 597);
      this.cmdDeleteSelected.Name = "cmdDeleteSelected";
      this.cmdDeleteSelected.Size = new Size(101, 23);
      this.cmdDeleteSelected.TabIndex = 15;
      this.cmdDeleteSelected.Text = "D&elete Selected";
      this.cmdDeleteSelected.UseVisualStyleBackColor = true;
      this.cmdDeleteSelected.Click += new EventHandler(this.cmdDeleteSelected_Click);
      this.cmdDeselectAllMAS.Enabled = false;
      this.cmdDeselectAllMAS.Location = new Point(126, 597);
      this.cmdDeselectAllMAS.Name = "cmdDeselectAllMAS";
      this.cmdDeselectAllMAS.Size = new Size(93, 23);
      this.cmdDeselectAllMAS.TabIndex = 13;
      this.cmdDeselectAllMAS.Text = "&Deselect All";
      this.cmdDeselectAllMAS.UseVisualStyleBackColor = true;
      this.cmdDeselectAllMAS.Click += new EventHandler(this.cmdDeselectAllMAS_Click);
      this.cmdSelectAllMAS.Enabled = false;
      this.cmdSelectAllMAS.Location = new Point(18, 597);
      this.cmdSelectAllMAS.Name = "cmdSelectAllMAS";
      this.cmdSelectAllMAS.Size = new Size(93, 23);
      this.cmdSelectAllMAS.TabIndex = 12;
      this.cmdSelectAllMAS.Text = "Select &All";
      this.cmdSelectAllMAS.UseVisualStyleBackColor = true;
      this.cmdSelectAllMAS.Click += new EventHandler(this.cmdSelectAllMAS_Click);
      this.cmdPostSelected.Enabled = false;
      this.cmdPostSelected.Location = new Point(238, 597);
      this.cmdPostSelected.Name = "cmdPostSelected";
      this.cmdPostSelected.Size = new Size(133, 23);
      this.cmdPostSelected.TabIndex = 14;
      this.cmdPostSelected.Text = "&Post Selected";
      this.cmdPostSelected.UseVisualStyleBackColor = true;
      this.cmdPostSelected.Click += new EventHandler(this.cmdPostSelected_Click);
      this.lbFilesToProcess.AutoSize = true;
      this.lbFilesToProcess.Location = new Point(15, 16);
      this.lbFilesToProcess.Name = "lbFilesToProcess";
      this.lbFilesToProcess.Size = new Size(77, 13);
      this.lbFilesToProcess.TabIndex = 10;
      this.lbFilesToProcess.Text = "Available Files:";
      this.lvFilesToProcess.CheckBoxes = true;
      this.lvFilesToProcess.Columns.AddRange(new ColumnHeader[2]
      {
        this.File,
        this.LoadDate
      });
      this.lvFilesToProcess.GridLines = true;
      this.lvFilesToProcess.Location = new Point(18, 32);
      this.lvFilesToProcess.Name = "lvFilesToProcess";
      this.lvFilesToProcess.Size = new Size(656, 559);
      this.lvFilesToProcess.TabIndex = 11;
      this.lvFilesToProcess.UseCompatibleStateImageBehavior = false;
      this.lvFilesToProcess.View = View.Details;
      this.lvFilesToProcess.ItemChecked += new ItemCheckedEventHandler(this.lvFilesToProcess_ItemChecked);
      this.File.Text = "File Name";
      this.File.Width = 450;
      this.LoadDate.Text = "Loaded Date";
      this.LoadDate.TextAlign = HorizontalAlignment.Center;
      this.LoadDate.Width = 175;
      this.bwFtpDownload.WorkerReportsProgress = true;
      this.bwFtpDownload.DoWork += new DoWorkEventHandler(this.bwFtpDownload_DoWork);
      this.bwFtpDownload.ProgressChanged += new ProgressChangedEventHandler(this.bwFtpDownload_ProgressChanged);
      this.bwFtpDownload.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwFtpDownload_RunWorkerCompleted);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(727, 727);
      this.Controls.Add((Control) this.tcApplication);
      this.Name = nameof (MASImportView);
      this.Text = "Future Logistics MAS AR/AP Import";
      this.Load += new EventHandler(this.MASImportView_Load);
      this.tcApplication.ResumeLayout(false);
      this.tpGetFiles.ResumeLayout(false);
      this.tpGetFiles.PerformLayout();
      this.tpMAS.ResumeLayout(false);
      this.tpMAS.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
