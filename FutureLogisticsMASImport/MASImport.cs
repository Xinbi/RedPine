// Decompiled with JetBrains decompiler
// Type: FutureLogisticsMASImport.MASImport
// Assembly: FutureLogisticsMASImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C3B527D-500A-4CC6-B04D-583364735731
// Assembly location: C:\Temp\Red Pine Systems\FutureLogisticsMASImport.exe

using System;
using System.Windows.Forms;

namespace FutureLogisticsMASImport
{
  internal static class MASImport
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new MASImportView());
    }
  }
}
