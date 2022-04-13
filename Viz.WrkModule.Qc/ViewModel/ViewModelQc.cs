using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
    private GridControl gcParamChr;
    private GridControl gcParamChrOpt;
    private GridControl gcFocused;

    private ModuleConst.TypeReferences crTypeRef;
    private DataRow paramDataRow = null;
    private int prevMasterRowHandle = -1;
    private Int64 paramIdKeyVal; 


    #endregion

    #region Public Property
    #endregion

    #region Protected Method
    #endregion

    #region Private Method
    private void ParamItemChanged(object sender, CurrentItemChangedEventArgs args)
    {
      //btnXSamplesRowChanged.CommandParameter = (sender as DevExpress.Xpf.Grid.GridViewBase).Grid.GetRow(e.RowData.RowHandle.Value);
      if (args.NewItem != null)
      {
        paramDataRow = (args.NewItem as DataRowView).Row;
        paramIdKeyVal = Convert.ToInt64(this.paramDataRow["Id"]);
        this.dsQc.ParamChr.LoadData(paramIdKeyVal);
        this.dsQc.ParamChrOpt.LoadData(paramIdKeyVal);
      }
      else
        paramDataRow = null;
    }

    private void MasterRowExpanded(object sender, RowEventArgs e)
    {
      GridControl gcDetail = (sender as GridControl).GetDetail(e.RowHandle) as GridControl;

      if ((prevMasterRowHandle >= 0) && e.RowHandle != prevMasterRowHandle)
        (sender as GridControl).CollapseMasterRow(prevMasterRowHandle);
      
      gcDetail.ItemsSource = dsQc.ParamChr;
      prevMasterRowHandle = e.RowHandle;
    }

    private void FocusedViewChanged(object sender, FocusedViewChangedEventArgs e)
    {
      var detailGrid = (e.NewView.DataControl).OwnerDetailDescriptor as DataControlDetailDescriptor;

      if (detailGrid == null)
      {
        gcFocused = null;
        return;
      }

      gcFocused = e.NewView.DataControl as GridControl;

      var tag = (ModuleConst.TypeParamsGc)Convert.ToInt32(detailGrid.DataControl.Tag);

      if (tag == ModuleConst.TypeParamsGc.GcParamChrOpt)
        e.NewView.DataControl.ItemsSource = dsQc.ParamChrOpt;
    }

    private void ParamChrNewRow(object sender, DataTableNewRowEventArgs e)
    {
      e.Row["ParamId"] = paramIdKeyVal;
    }


    void CreateGroupParamRef()
    {
      (gcRef.View as TableView).AllowMasterDetail = true;
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

      this.gcRef.CurrentItemChanged += ParamItemChanged;
      this.gcRef.MasterRowExpanded += MasterRowExpanded;
      this.gcRef.View.FocusedViewChanged += FocusedViewChanged;

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
        FieldName = "InCalc",
        Header = "Участвует в расчете"
      };
      var checkSettings = new CheckEditSettings();
      col.EditSettings = checkSettings;
      gcRef.Columns.Add(col);


      //Создаем Detail Grids
      DataControlDetailDescriptor dataControlDetail1 = new DataControlDetailDescriptor();
      //dataControlDetail1.ItemsSourcePath = "ParamChr";
      gcParamChr = new GridControl();
      gcParamChr.Tag = 1;
      dataControlDetail1.DataControl = gcParamChr;
      gcParamChr.View.DetailHeaderContent = "Допустимые значения параметров";
      (gcParamChr.View as TableView).ShowGroupPanel = false;
      (gcParamChr.View as TableView).NewItemRowPosition = NewItemRowPosition.Bottom;
      (gcParamChr.View as TableView).NavigationStyle = GridViewNavigationStyle.Cell;
      (gcParamChr.View as TableView).AllowEditing = true;
      
      col = new GridColumn()
      {
        FieldName = "ParamId",
        Header = "ID",
        ReadOnly = true
      };
      gcParamChr.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "Thickness",
        Header = "Толщина"
      };
      lookUpSettings = new LookUpEditSettings
      {
        StyleSettings = new SearchLookUpEditStyleSettings(),
        DisplayMember = "TextDispaly",
        ValueMember = "Thickness",
        ItemsSource = dsQc.Thickness
      };
      col.EditSettings = lookUpSettings;
      gcParamChr.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "MinVal",
        Header = "Мин. значение"
      };
      var textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "n4",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;
      gcParamChr.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "MaxVal",
        Header = "Макс. значение"
      };
      textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "n4",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;
      gcParamChr.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "LogVal",
        Header = "Логическое значение"
      };
      checkSettings = new CheckEditSettings();
      col.EditSettings = checkSettings;
      gcParamChr.Columns.Add(col);

      DataControlDetailDescriptor dataControlDetail2 = new DataControlDetailDescriptor();
      //dataControlDetail.ItemsSourcePath = "Orders";
      gcParamChrOpt = new GridControl();
      gcParamChrOpt.Tag = 2;
      dataControlDetail2.DataControl = gcParamChrOpt;
      gcParamChrOpt.View.DetailHeaderContent = "Оптимальные значения параметров";
      (gcParamChrOpt.View as TableView).ShowGroupPanel = false;
      (gcParamChrOpt.View as TableView).NewItemRowPosition = NewItemRowPosition.Bottom;
      

      col = new GridColumn()
      {
        FieldName = "ParamId",
        Header = "ID",
        ReadOnly = true
      };
      gcParamChrOpt.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "Thickness",
        Header = "Толщина"
      };
      lookUpSettings = new LookUpEditSettings
      {
        StyleSettings = new SearchLookUpEditStyleSettings(),
        DisplayMember = "TextDispaly",
        ValueMember = "Thickness",
        ItemsSource = dsQc.Thickness
      };
      col.EditSettings = lookUpSettings;
      gcParamChrOpt.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "MinVal",
        Header = "Мин. значение"
      };
      textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "n4",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;
      gcParamChrOpt.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "MaxVal",
        Header = "Макс. значение"
      };
      textSetinngs = new TextEditSettings
      {
        MaskType = MaskType.Numeric,
        Mask = "n4",
        MaskIgnoreBlank = false,
        MaskUseAsDisplayFormat = true,
      };
      col.EditSettings = textSetinngs;
      gcParamChrOpt.Columns.Add(col);

      col = new GridColumn()
      {
        FieldName = "LogVal",
        Header = "Логическое значение"
      };
      checkSettings = new CheckEditSettings();
      col.EditSettings = checkSettings;
      gcParamChrOpt.Columns.Add(col);
      
      //ContentDetailDescriptor contentDetail = new ContentDetailDescriptor();
      //contentDetail.ContentTemplate = (DataTemplate)FindResource("EmployeeNotes");
      //contentDetail.HeaderContent = "Notes";

      TabViewDetailDescriptor tabDetail = new TabViewDetailDescriptor();
      tabDetail.DetailDescriptors.Add(dataControlDetail1);
      tabDetail.DetailDescriptors.Add(dataControlDetail2);
      //tabDetail.DetailDescriptors.Add(contentDetail);

      gcRef.DetailDescriptor = tabDetail;
    }

    void CreateQmIndicatorRef()
    {
      gcRef.ItemsSource = dsQc.QmIndicator;
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
      dsQc.QmIndicator.LoadData();

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
        ItemsSource = dsQc.QmIndicator
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
      dsQc.QmIndicator.LoadData();
      dsQc.Influence.LoadData();
      dsQc.Thickness.LoadData();
      
      dsQc.ParamChr.TableNewRow += ParamChrNewRow;
      dsQc.ParamChrOpt.TableNewRow += ParamChrNewRow;
    }

 
    #endregion

    #region Command
    public void SelectTypeRef(Object param)
    {
      gcRef.Columns.Clear();
      this.gcRef.CurrentItemChanged -= ParamItemChanged;
      this.gcRef.MasterRowExpanded -= MasterRowExpanded;
      this.gcRef.View.FocusedViewChanged -= FocusedViewChanged;

      //(gcRef.DetailDescriptor as TabViewDetailDescriptor)?.DetailDescriptors.Clear();
      gcRef.DetailDescriptor = null;
      

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
          dsQc.ParamChr.SaveData();
          dsQc.ParamChrOpt.SaveData();
          break;
        case ModuleConst.TypeReferences.QmIndicator:
          dsQc.QmIndicator.SaveData();
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
      if (crTypeRef != ModuleConst.TypeReferences.Param)
        (gcRef.View as TableView).DeleteRow(gcRef.View.FocusedRowHandle);
      else if ((gcFocused == null) && (crTypeRef == ModuleConst.TypeReferences.Param))
        (gcRef.View as TableView).DeleteRow(gcRef.View.FocusedRowHandle);
      else if ((gcFocused != null) && (crTypeRef == ModuleConst.TypeReferences.Param))
        (gcFocused.View as TableView).DeleteRow(gcFocused.View.FocusedRowHandle);
    }
    public bool CanDeleteData()
    {
      if ((gcRef.View.IsFocusedView) && (gcRef.View.FocusedRowHandle >= 0) && (crTypeRef != ModuleConst.TypeReferences.Param))
        return true;
      else if ((gcFocused == null) && (gcRef.View.IsFocusedView) && (gcRef.View.FocusedRowHandle >= 0) && (crTypeRef == ModuleConst.TypeReferences.Param))
        return true;
      else if ((gcFocused != null) && (gcFocused.View.IsFocusedView) && (gcFocused.View.FocusedRowHandle >= 0) && (crTypeRef == ModuleConst.TypeReferences.Param))
        return true;
      else
        return false;
    }
    #endregion


  }
}
