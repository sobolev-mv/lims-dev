using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.LookUp;
using Smv.Print.RawData;
using Smv.Utils;
using Viz.WrkModule.Qc.Db;

namespace Viz.WrkModule.Qc
{
  public class ViewModelQc
  {
    #region Fields
    private readonly UserControl usrControl;
    private readonly DsQc dsQc = new DsQc();
    private readonly GridControl gcRef;
    #endregion

    #region Public Property
    #endregion

    #region Protected Method
    #endregion

    #region Private Method

    void CreateGroupParamRef()
    {
      gcRef.ItemsSource = dsQc.ParamGroup;
      var col = new GridColumn()
      {
        FieldName = "Id",
        Header = "ID",
      };

      TextEditSettings textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "d",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;

      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "Name",
        Header = "Наименование"
      };
      gcRef.Columns.Add(col);
    }

    void CreateParamRef()
    {
      //Обновляем группы параметров
      dsQc.ParamGroup.LoadData();

      gcRef.ItemsSource = dsQc.Param;
      var col = new GridColumn()
      {
        FieldName = "Id",
        Header = "ID"
      };
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "GroupId",
        Header = "Группа параметров"
      };

      LookUpEditSettings lookUpSettings = new LookUpEditSettings
      {
        StyleSettings = new SearchLookUpEditStyleSettings(), 
        DisplayMember = "Name", 
        ValueMember = "Id",
        ItemsSource = dsQc.ParamGroup
      };
      col.EditSettings = lookUpSettings;
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "Name",
        Header = "Наименование"
      };
      gcRef.Columns.Add(col);
      
      col = new GridColumn()
      {
        FieldName = "MinVal",
        Header = "Мин. значение"
      };

      TextEditSettings textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "n4",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "MaxVal",
        Header = "Мах. значение"
      };
      col.EditSettings = textSetinngs;
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "LogVal",
        Header = "Лог. значение"
      };
      var checkSettings = new CheckEditSettings();
      col.EditSettings = checkSettings;
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "InCalc",
        Header = "Участвует в расчете"
      };
      checkSettings = new CheckEditSettings();
      col.EditSettings = checkSettings;
      gcRef.Columns.Add(col);

    }

    void CreateQmIndicatorRef()
    {
      gcRef.ItemsSource = dsQc.QmIdicator;
      var col = new GridColumn()
      {
        FieldName = "Id",
        Header = "ID",
      };

      TextEditSettings textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "d",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;

      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "Name",
        Header = "Наименование"
      };
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "Tou",
        Header = "ТОУ"
      };

      textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "n3",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;
      gcRef.Columns.Add(col);

    }

    #endregion

    #region Constructor

    public ViewModelQc(UserControl control, Object mainWindow)
    {
      usrControl = control;
      gcRef = LogicalTreeHelper.FindLogicalNode(this.usrControl, "GcRef") as GridControl;
      /*
      if (this.dbgMaterial != null)
        this.dbgMaterial.CurrentItemChanged += CurrentItemChanged;
      */


      dsQc.ParamGroup.LoadData();
      dsQc.Param.LoadData();
      dsQc.QmIdicator.LoadData();

    }
    #endregion

    #region Command
    public void SelectTypeRef(Object param)
    {
      gcRef.Columns.Clear();
      
      switch (Convert.ToInt32(param))
      {
        case (int)ModuleConst.TypeReferences.GroupParam:
          CreateGroupParamRef();
          break;
        case (int)ModuleConst.TypeReferences.Param:
          CreateParamRef();
          break;
        case (int)ModuleConst.TypeReferences.QmIndicator:
          CreateQmIndicatorRef();
          break;
        case (int)ModuleConst.TypeReferences.Influence:

          break;
      }
    }

    public bool CanShowDefectMap()
    {
      return true;
    }
    #endregion


  }
}
