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

    public static double GetUst4LocNum(string typeSts, string locNum)
    {
      const string stmtSql = "select RATIO_LOCNUM from VIZ_PRN.V_QMF_STS where TYPE_CLC = :PTYPECLC and LOCNUM = :PLOCNUM";
      var lstPrm = new List<OracleParameter>();

      var prm = new OracleParameter
      {
        ParameterName = "PTYPECLC",
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        OracleDbType = OracleDbType.VarChar,
        Size = typeSts.Length,
        Value = typeSts
      };
      lstPrm.Add(prm);

      prm = new OracleParameter
      {
        ParameterName = "PLOCNUM",
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        OracleDbType = OracleDbType.VarChar,
        Size = locNum.Length,
        Value = locNum
      };
      lstPrm.Add(prm);

      return Convert.ToDouble(Odac.ExecuteScalar(stmtSql, CommandType.Text, false, lstPrm));
    }

    public static void CalcUst4LocNum(string typeSts, string locNum)
    {
      const string stmtSql = "VIZ_PRN.QMF_CALC.CALCMAIN";
      var lstPrm = new List<OracleParameter>();

      var prm = new OracleParameter
      {
        ParameterName = "pi_LocNum",
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        OracleDbType = OracleDbType.VarChar,
        Size = locNum.Length,
        Value = locNum
      };
      lstPrm.Add(prm);


      prm = new OracleParameter
      {
        ParameterName = "pi_TypeClc",
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        OracleDbType = OracleDbType.VarChar,
        Size = typeSts.Length,
        Value = typeSts
      };
      lstPrm.Add(prm);


      Odac.ExecuteNonQuery(stmtSql, CommandType.StoredProcedure, false, lstPrm);
    }


  }
}
