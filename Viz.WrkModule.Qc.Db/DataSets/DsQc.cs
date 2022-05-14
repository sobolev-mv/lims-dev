using System;
using System.Data;
using System.Collections.Generic;
using Smv.Data.Oracle;
using Devart.Data.Oracle;

namespace Viz.WrkModule.Qc.Db.DataSets
{
  public sealed partial class DsQc : DataSet
  {
    public ParamGroupDataTable ParamGroup { get; private set; }
    public ParamDataTable Param { get; private set; }
    public QmIndicatorDataTable QmIndicator { get; private set; }
    public InfluenceDataTable Influence { get; private set; }
    public ParamChrDataTable ParamChr { get; private set; }
    public ThicknessDataTable Thickness { get; private set; }
    public ParamChrOptDataTable ParamChrOpt { get; private set; }
    public ParamLnkDataTable ParamLnk { get; private set; }
    public StsDataTable Sts { get; private set; }
    public DsQc() : base()
    {
      this.DataSetName = "DsQc";

      this.ParamGroup = new ParamGroupDataTable("ParamGroup");
      this.Tables.Add(this.ParamGroup);

      this.Param = new ParamDataTable("Param");
      this.Tables.Add(this.Param);

      this.QmIndicator = new QmIndicatorDataTable("QmIndicator");
      this.Tables.Add(this.QmIndicator);

      this.Influence = new InfluenceDataTable("Influence");
      this.Tables.Add(this.Influence);

      this.ParamChr = new ParamChrDataTable("ParamChr");
      this.Tables.Add(this.ParamChr);

      this.Thickness = new ThicknessDataTable("Thickness");
      this.Tables.Add(this.Thickness);

      this.ParamChrOpt = new ParamChrOptDataTable("ParamChrOpt");
      this.Tables.Add(this.ParamChrOpt);

      this.ParamLnk = new ParamLnkDataTable("ParamLnk");
      this.Tables.Add(this.ParamLnk);

      this.Sts = new StsDataTable("Sts");
      this.Tables.Add(this.Sts);

    }
  }

}
