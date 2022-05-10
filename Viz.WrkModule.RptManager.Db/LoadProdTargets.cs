using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using Devart.Data.Oracle;
using Smv.Data.Oracle;
using Viz.DbApp.Psi;
using Microsoft.Win32;
using DevExpress.Spreadsheet;
using Smv.Utils;


namespace Viz.WrkModule.RptManager.Db
{
  public sealed class LoadProdTargetsRptParam : Smv.Xls.XlsInstanceParam
  {
    public DateTime DateBegin { get; set; }
    public string CfgFile { get; set; }
    public LoadProdTargetsRptParam(string sourceXlsFile, string destXlsFile) : base(sourceXlsFile, destXlsFile)
    {}
  }

  public sealed class LoadProdTargets : Smv.Xls.XlsRpt
  {

    protected override void DoWorkXls(object sender, DoWorkEventArgs e)
    {
      var prm = (e.Argument as LoadProdTargetsRptParam);
      
      try{
        this.RunRpt(prm);
      }
      catch (Exception ex){
        Debug.Assert(prm != null, "prm != null");
        prm.Disp.Invoke(DispatcherPriority.Normal, (ThreadStart)(() => Smv.Utils.DxInfo.ShowDxBoxInfo("Ошибка загрузки", ex.Message, MessageBoxImage.Stop)));
      }
      finally{
        GC.Collect();
      }
    }

    private Boolean ClearRptPeriod(DateTime rptPeriod)
    {
      const string stmtSql = "DELETE FROM VIZ_PRN.TV_PLNTARGPROD WHERE RPT_PERIOD = :RPT_PERIOD";
      var lstPrm = new List<OracleParameter>();

      var prm = new OracleParameter
      {
        DbType = DbType.DateTime,
        Direction = ParameterDirection.Input,
        ParameterName = "RPT_PERIOD",
        OracleDbType = OracleDbType.Date,
        Value = rptPeriod
      };
      lstPrm.Add(prm);

      return Odac.ExecuteNonQuery(stmtSql, CommandType.Text, true, lstPrm, true);
    }

    private Boolean InsertEventsToDashBoard(DateTime rptPeriod)
    {
      const string stmtSql = "INSERT INTO DSB.JB_PLNPROD(RPT_PERIOD) VALUES(:RPT_PERIOD)";
      var lstPrm = new List<OracleParameter>();

      var prm = new OracleParameter
      {
        DbType = DbType.DateTime,
        Direction = ParameterDirection.Input,
        ParameterName = "RPT_PERIOD",
        OracleDbType = OracleDbType.Date,
        Value = rptPeriod
      };
      lstPrm.Add(prm);

      return Odac.ExecuteNonQuery(stmtSql, CommandType.Text, true, lstPrm, true);
    }
    
    private Boolean LoadToPlanTable(DateTime rptPeriod, string teStep, string agr, DateTime plnDay, decimal plnValue, string adInstr)
    {
      const string stmtSql = "INSERT INTO VIZ_PRN.TV_PLNTARGPROD VALUES(:RPT_PERIOD, :TESTEP, :AGR, :PLN_DAY, :PLN_VALUE, :AD_INSTR)";
      var lstPrm = new List<OracleParameter>();

      var prm = new OracleParameter
      {
        DbType = DbType.DateTime,
        Direction = ParameterDirection.Input,
        ParameterName = "RPT_PERIOD",
        OracleDbType = OracleDbType.Date,
        Value = rptPeriod
      };
      lstPrm.Add(prm);

      prm = new OracleParameter
      {
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        ParameterName = "TESTEP",
        OracleDbType = OracleDbType.VarChar,
        Size = teStep.Length,
        Value = teStep
      };
      lstPrm.Add(prm);

      prm = new OracleParameter
      {
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        ParameterName = "AGR",
        OracleDbType = OracleDbType.VarChar,
        Size = agr.Length,
        Value = agr
      };
      lstPrm.Add(prm);

      prm = new OracleParameter
      {
        DbType = DbType.DateTime,
        Direction = ParameterDirection.Input,
        ParameterName = "PLN_DAY",
        OracleDbType = OracleDbType.Date,
        Value = plnDay
      };
      lstPrm.Add(prm);

      prm = new OracleParameter
      {
        DbType = DbType.Decimal,
        Direction = ParameterDirection.Input,
        ParameterName = "PLN_VALUE",
        OracleDbType = OracleDbType.Number,
        Precision = 17,
        Scale = 2,
        Value = plnValue
      };
      lstPrm.Add(prm);

      prm = new OracleParameter
      {
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        ParameterName = "AD_INSTR",
        OracleDbType = OracleDbType.VarChar,
        Size = agr.Length,
        Value = adInstr
      };
      lstPrm.Add(prm);

      return Odac.ExecuteNonQuery(stmtSql, CommandType.Text, false, lstPrm, true);
    }
    
    private Boolean RunRpt(LoadProdTargetsRptParam prm)
    {
      Boolean result;
      DateTime fistDayWrkPeriod = new DateTime(prm.DateBegin.Year, prm.DateBegin.Month, 1);
      DateTime lastDayWrkPeriod = new DateTime(prm.DateBegin.Year, prm.DateBegin.Month,  DateTime.DaysInMonth(prm.DateBegin.Year, prm.DateBegin.Month));

      string strFirstCell;
      string strTeStep;
      string strAgr;
      
      //Читаем строки из конфиг.файла
      try
      {
        strFirstCell = Smv.App.Config.ConfigParam.ReadAppSettingsParamValue(Etc.StartPath + prm.CfgFile, "StrFirstCell");
        strTeStep = Smv.App.Config.ConfigParam.ReadAppSettingsParamValue(Etc.StartPath + prm.CfgFile, "StrTeStep");
        strAgr = Smv.App.Config.ConfigParam.ReadAppSettingsParamValue(Etc.StartPath + prm.CfgFile, "StrAgr");
      }
      catch (Exception){
        DxInfo.ShowDxBoxInfo("Ошибка конфигурации", "Ошибка при чтении конфигурационных параметров!", MessageBoxImage.Error);
        return true;
      }

      var ofd = new OpenFileDialog
      {
        AddExtension = true,
        DefaultExt = ".xslx",
        Filter = "xlsx file (.xlsx)|*.xlsx"
      };

      if (!ofd.ShowDialog().GetValueOrDefault(false))
        return true;
      
      Workbook workbook = new Workbook();
      // Load a workbook from the file. 
      workbook.LoadDocument(ofd.FileName, DocumentFormat.Xlsx);
      //MessageBox.Show(workbook.Worksheets[0].Cells["B8"].Value.TextValue);


      try{
        result = true;
        ClearRptPeriod(fistDayWrkPeriod);
  
        //Загрузка по-агрегатно
        var strFirstCellList = strFirstCell.Split(',');
        var strTeStepList = strTeStep.Split(',');
        var strAgrList = strAgr.Split(',');
        
        for (var i = 0; i < strFirstCellList.Length; i++){

          var j = 0;
          var row = workbook.Worksheets[0].Cells[strFirstCellList[i]].RowIndex;
          var col = workbook.Worksheets[0].Cells[strFirstCellList[i]].ColumnIndex;

          /*
          if (strFirstCellList[i] == "E53"){
            MessageBox.Show(row.ToString() + "," + col.ToString());
            MessageBox.Show(workbook.Worksheets[0].Cells[row, col + j].Value.NumericValue.ToString());
          }
          */

          for (DateTime dt = fistDayWrkPeriod; dt <= lastDayWrkPeriod; dt = dt.AddDays(1), j++){
            var r = LoadToPlanTable(fistDayWrkPeriod, strTeStepList[i], strAgrList[i], dt, Convert.ToDecimal(workbook.Worksheets[0].Cells[row, col + j].Value.NumericValue), "D");
            
            if (!r)
              return true;
            
            //здесь загружаем данные из столбца "План Мес"
            if (dt == lastDayWrkPeriod){
              r = LoadToPlanTable(fistDayWrkPeriod, strTeStepList[i], strAgrList[i], fistDayWrkPeriod, Convert.ToDecimal(workbook.Worksheets[0].Cells[row, col + j + 1].Value.NumericValue), "M");
              if (!r)
                return true;
            }
            
          }
        }

        InsertEventsToDashBoard(fistDayWrkPeriod);
      }
      catch (Exception ex){
        prm.Disp.Invoke(DispatcherPriority.Normal, (ThreadStart)(() => Smv.Utils.DxInfo.ShowDxBoxInfo("Ошибка загрузки", ex.Message, MessageBoxImage.Stop)));
        result = false;
      }
      finally{
        result = false;
      }
    
      return result;
    }


  }






}

