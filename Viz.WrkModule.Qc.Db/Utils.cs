using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using Devart.Data.Oracle;
using Microsoft.Win32;
using Smv.Data.Oracle;
using Smv.Utils;

namespace Viz.WrkModule.Qc.Db
{
  public static class Utils
  {
    public static void ParamRpt()
    {
      var sfd = new SaveFileDialog
      {
        OverwritePrompt = false,
        AddExtension = true,
        DefaultExt = ".csv",
        Filter = "csv file (.csv)|*.csv"
      };

      if (sfd.ShowDialog().GetValueOrDefault() != true)
        return;

      if (File.Exists(sfd.FileName))
      {
        DxInfo.ShowDxBoxInfo("Файл", "Файл: " + sfd.FileName + " уже существует!", MessageBoxImage.Error);
        return;
      }

      OracleDataReader odr;

      odr = Odac.GetOracleReader("select * from viz_prn.v_qmf_rptparam", CommandType.Text, false, null, null);
      

      if (odr != null)
      {
        Etc.WriteToEndTxtFile(sfd.FileName, "", Encoding.GetEncoding("windows-1251"));
        Etc.WriteToEndTxtFile(sfd.FileName, "", Encoding.GetEncoding("windows-1251"));
        Etc.WriteToEndTxtFile(sfd.FileName, "ID Грп;Группа;ID Парам;Параметр;В Расчете;Толщина допуст.;Мин допуст. знач.;Макс допуст. знач.;Лог. допуст. знач;Толщина опт.;Мин опт. знач.;Макс опт. знач.;Лог. опт. знач", Encoding.GetEncoding("windows-1251"));

        while (odr.Read())
        {
          var strTmp = "";

          for (int i = 0; i < odr.FieldCount; i++)
            strTmp += odr.GetValue(i).ToString() + ";";

          Etc.WriteToEndTxtFile(sfd.FileName, strTmp, Encoding.GetEncoding("windows-1251"));
        }

      }

      odr?.Close();
      odr?.Dispose();

      DxInfo.ShowDxBoxInfo("Выгрузка", "Выгрузка завершена.", MessageBoxImage.Information);
    }





  }
}
