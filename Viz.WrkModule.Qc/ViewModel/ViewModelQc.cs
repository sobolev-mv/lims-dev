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
    private ModuleConst.TypeReferences crTypeRef;
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

      var lookUpSettings = new LookUpEditSettings
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

    void CreateInfluenceRef()
    {
      //Обновляем параметры и показатели
      dsQc.ParamGroup.LoadData();
      dsQc.QmIdicator.LoadData();

      gcRef.ItemsSource = dsQc.Influence;

      
      var col = new GridColumn()
      {
        FieldName = "ParamId",
        Header = "Параметр"
      };

      var lookUpSettings = new LookUpEditSettings
      {
        StyleSettings = new SearchLookUpEditStyleSettings(),
        DisplayMember = "Name",
        ValueMember = "Id",
        ItemsSource = dsQc.Param
      };
      col.EditSettings = lookUpSettings;
      gcRef.Columns.Add(col);
      

      col = new GridColumn()
      {
        FieldName = "IndicatorId",
        Header = "Показатель качества"
      };
      
      lookUpSettings = new LookUpEditSettings
      {
        StyleSettings = new SearchLookUpEditStyleSettings(),
        DisplayMember = "Name",
        ValueMember = "Id",
        ItemsSource = dsQc.QmIdicator
      };
      col.EditSettings = lookUpSettings;
      gcRef.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "ValInfluence",
        Header = "Воздействие"
      };

      TextEditSettings textSetinngs = new TextEditSettings
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
      dsQc.Influence.LoadData();

    }
    #endregion

    #region Command
    public void SelectTypeRef(Object param)
    {
      gcRef.Columns.Clear();
      crTypeRef = (ModuleConst.TypeReferences)Convert.ToInt32(param);

      switch (crTypeRef)
      {
        case ModuleConst.TypeReferences.GroupParam:
          CreateGroupParamRef();
          break;
        case ModuleConst.TypeReferences.Param:
          CreateParamRef();
          break;
        case ModuleConst.TypeReferences.QmIndicator:
          CreateQmIndicatorRef();
          break;
        case ModuleConst.TypeReferences.Influence:
          CreateInfluenceRef();
          break;
      }
    }

    public bool CanShowDefectMap()
    {
      return true;
    }

    public void SaveData()
    {
      switch (crTypeRef)
      {
        case ModuleConst.TypeReferences.GroupParam:
          dsQc.ParamGroup.SaveData();
          break;
        case ModuleConst.TypeReferences.Param:
          dsQc.Param.SaveData();
          break;
        case ModuleConst.TypeReferences.QmIndicator:
          dsQc.QmIdicator.SaveData();
          break;
        case ModuleConst.TypeReferences.Influence:
          dsQc.Influence.SaveData();
          break;
      }
    }
    public bool CanSaveData()
    {
      return dsQc.HasChanges(); ;
    }

    public void DeleteData()
    {
      (gcRef.View as TableView).DeleteRow(gcRef.View.FocusedRowHandle);
    }
    public bool CanDeleteData()
    {
      return ((gcRef.View.IsFocusedView) && (gcRef.View.FocusedRowHandle >= 0)); ;
    }
    #endregion


  }
}
