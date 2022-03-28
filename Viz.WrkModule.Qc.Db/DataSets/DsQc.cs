using System;
using System.Data;
using System.Collections.Generic;
using Smv.Data.Oracle;
using Devart.Data.Oracle;

namespace Viz.WrkModule.Qc.Db
{
  public sealed class DsQc : DataSet
  {
    public ParamGroupDataTable ParamGroup { get; private set; }
    public ParamDataTable Param { get; private set; }
    public QmIdicatorDataTable QmIdicator { get; private set; }
    public InfluenceDataTable Influence { get; private set; }

    public DsQc() : base()
    {
      this.DataSetName = "DsQc";

      this.ParamGroup = new ParamGroupDataTable("ParamGroup");
      this.Tables.Add(this.ParamGroup);

      this.Param = new ParamDataTable("Param");
      this.Tables.Add(this.Param);

      this.QmIdicator = new QmIdicatorDataTable("QmIdicator");
      this.Tables.Add(this.QmIdicator);

      this.Influence = new InfluenceDataTable("Influence");
      this.Tables.Add(this.Influence);

    }

    public class ParamGroupDataTable : DataTable
    {
      protected readonly OracleDataAdapter adapter;

      public ParamGroupDataTable(string tblName) : base()
      {
        this.TableName = tblName;
        adapter = new OracleDataAdapter();

        var col = new DataColumn("Id", typeof(int), null, MappingType.Element)
        {
          AllowDBNull = false,
          AutoIncrement = true,
          AutoIncrementStep = -1
        };
        col.Caption = "ID";
        this.Columns.Add(col);

        col = new DataColumn("Name", typeof(string), null, MappingType.Element) { AllowDBNull = false };
        col.Caption = "Наименование";
        this.Columns.Add(col);

        this.Constraints.Add(new UniqueConstraint("Pk_" + tblName, new[] { this.Columns["Id"] }, true));
        this.Columns["Id"].Unique = true;

        adapter.TableMappings.Clear();
        var dtm = new System.Data.Common.DataTableMapping("VIZ_PRN.QMF_PARAM_GROUP", tblName);
        dtm.ColumnMappings.Add("ID", "Id");
        dtm.ColumnMappings.Add("NAME", "Name");

        adapter.TableMappings.Add(dtm);

        //Select Command
        adapter.SelectCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "SELECT ID, NAME FROM VIZ_PRN.QMF_PARAM_GROUP ORDER BY ID",
          CommandType = CommandType.Text
        };

        //Insert Command
        adapter.InsertCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "INSERT INTO VIZ_PRN.QMF_PARAM_GROUP(ID, NAME) " +
            "VALUES(:PID, :PNAME)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        var param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.String,
          OracleDbType = OracleDbType.VarChar,
          Direction = ParameterDirection.Input,
          ParameterName = "PNAME",
          SourceColumn = "NAME",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        //Update Command
        adapter.UpdateCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "UPDATE VIZ_PRN.QMF_PARAM_GROUP SET NAME = :PNAME " +
            "WHERE (ID = :Original_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.String,
          OracleDbType = OracleDbType.VarChar,
          Direction = ParameterDirection.Input,
          ParameterName = "PNAME",
          SourceColumn = "NAME",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_ID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.UpdateCommand.Parameters.Add(param);

        //DeleteCommand
        adapter.DeleteCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "DELETE VIZ_PRN.QMF_PARAM_GROUP WHERE (ID = :Original_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_ID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.DeleteCommand.Parameters.Add(param);

      }

      public int LoadData()
      {
        return Odac.LoadDataTable(this, adapter, true, null);
      }

      public int SaveData()
      {
        return Odac.SaveChangedData(this, adapter);
      }

    }

    public class ParamDataTable : DataTable
    {
      protected readonly OracleDataAdapter adapter;

      public ParamDataTable(string tblName) : base()
      {
        this.TableName = tblName;
        adapter = new OracleDataAdapter();

        var col = new DataColumn("Id", typeof(Int64), null, MappingType.Element)
        {
          AllowDBNull = false,
          AutoIncrement = false,
          AutoIncrementStep = -1
        };
        this.Columns.Add(col);

        col = new DataColumn("GroupId", typeof(int), null, MappingType.Element)
        {
          AllowDBNull = false,
          AutoIncrement = true,
          AutoIncrementStep = -1
        };
        this.Columns.Add(col);

        col = new DataColumn("Name", typeof(string), null, MappingType.Element) { AllowDBNull = false };
        this.Columns.Add(col);

        col = new DataColumn("MinVal", typeof(double), null, MappingType.Element);
        this.Columns.Add(col);

        col = new DataColumn("MaxVal", typeof(double), null, MappingType.Element);
        this.Columns.Add(col);

        col = new DataColumn("LogVal", typeof(int), null, MappingType.Element);
        this.Columns.Add(col);

        col = new DataColumn("InCalc", typeof(int), null, MappingType.Element);
        this.Columns.Add(col);
        
        this.Constraints.Add(new UniqueConstraint("Pk_" + tblName, new[] { this.Columns["Id"] }, true));
        this.Columns["Id"].Unique = true;

        adapter.TableMappings.Clear();
        var dtm = new System.Data.Common.DataTableMapping("VIZ_PRN.QMF_PARAM", tblName);
        dtm.ColumnMappings.Add("ID", "Id");
        dtm.ColumnMappings.Add("GROUP_ID", "GroupId");
        dtm.ColumnMappings.Add("NAME", "Name");
        dtm.ColumnMappings.Add("MIN_VAL", "MinVal");
        dtm.ColumnMappings.Add("MAX_VAL", "MaxVal");
        dtm.ColumnMappings.Add("LOG_VAL", "LogVal");
        dtm.ColumnMappings.Add("IN_CALC", "InCalc");
        adapter.TableMappings.Add(dtm);

        //Select Command
        adapter.SelectCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "SELECT ID, GROUP_ID, NAME, MIN_VAL, MAX_VAL, LOG_VAL, IN_CALC FROM VIZ_PRN.QMF_PARAM ORDER BY ID, GROUP_ID",
          CommandType = CommandType.Text
        };

        //Insert Command
        adapter.InsertCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "INSERT INTO VIZ_PRN.QMF_PARAM(ID, GROUP_ID, NAME, MIN_VAL, MAX_VAL, LOG_VAL, IN_CALC) " +
            "VALUES(:PID, :PGROUP_ID, :PNAME, :PMIN_VAL, :PMAX_VAL, :PLOG_VAL, :PIN_CALC)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        var param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          ParameterName = "PID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PGROUP_ID",
          SourceColumn = "GROUP_ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.String,
          OracleDbType = OracleDbType.VarChar,
          Direction = ParameterDirection.Input,
          ParameterName = "PNAME",
          SourceColumn = "NAME",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PMIN_VAL",
          SourceColumn = "MIN_VAL",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PMAX_VAL",
          SourceColumn = "MAX_VAL",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PLOG_VAL",
          SourceColumn = "LOG_VAL",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PIN_CALC",
          SourceColumn = "IN_CALC",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        //Update Command
        adapter.UpdateCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "UPDATE VIZ_PRN.QMF_PARAM SET GROUP_ID = :PGROUP_ID, NAME = :PNAME, MIN_VAL = :PMIN_VAL, MAX_VAL = :PMAX_VAL, LOG_VAL = :PLOG_VAL, IN_CALC = :PIN_CALC " +
            "WHERE (ID = :Original_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PGROUP_ID",
          SourceColumn = "GROUP_ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.String,
          OracleDbType = OracleDbType.VarChar,
          Direction = ParameterDirection.Input,
          ParameterName = "PNAME",
          SourceColumn = "NAME",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PMIN_VAL",
          SourceColumn = "MIN_VAL",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PMAX_VAL",
          SourceColumn = "MAX_VAL",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PLOG_VAL",
          SourceColumn = "LOG_VAL",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int32,
          OracleDbType = OracleDbType.Integer,
          Direction = ParameterDirection.Input,
          ParameterName = "PIN_CALC",
          SourceColumn = "IN_CALC",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_ID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.UpdateCommand.Parameters.Add(param);

        //DeleteCommand
        adapter.DeleteCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "DELETE VIZ_PRN.QMF_PARAM WHERE (ID = :Original_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_ID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.DeleteCommand.Parameters.Add(param);

      }

      public int LoadData()
      {
        return Odac.LoadDataTable(this, adapter, true, null);
      }

      public int SaveData()
      {
        return Odac.SaveChangedData(this, adapter);
      }

    
    }

    public class QmIdicatorDataTable : DataTable
    {
      protected readonly OracleDataAdapter adapter;

      public QmIdicatorDataTable(string tblName) : base()
      {
        this.TableName = tblName;
        adapter = new OracleDataAdapter();

        var col = new DataColumn("Id", typeof(Int64), null, MappingType.Element)
        {
          AllowDBNull = false,
          AutoIncrement = true,
          AutoIncrementStep = -1
        };
        this.Columns.Add(col);

        col = new DataColumn("Name", typeof(string), null, MappingType.Element) { AllowDBNull = false };
        this.Columns.Add(col);

        col = new DataColumn("Tou", typeof(double), null, MappingType.Element);
        this.Columns.Add(col);
        
        this.Constraints.Add(new UniqueConstraint("Pk_" + tblName, new[] { this.Columns["Id"] }, true));
        this.Columns["Id"].Unique = true;

        adapter.TableMappings.Clear();
        var dtm = new System.Data.Common.DataTableMapping("VIZ_PRN.QMF_INDICATOR", tblName);
        dtm.ColumnMappings.Add("ID", "Id");
        dtm.ColumnMappings.Add("NAME", "Name");
        dtm.ColumnMappings.Add("TOU", "Tou");
        adapter.TableMappings.Add(dtm);

        //Select Command
        adapter.SelectCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "SELECT ID, NAME, TOU FROM VIZ_PRN.QMF_INDICATOR ORDER BY ID",
          CommandType = CommandType.Text
        };

        //Insert Command
        adapter.InsertCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "INSERT INTO VIZ_PRN.QMF_INDICATOR(ID, NAME, TOU) " +
            "VALUES(:PID, :PNAME, :PTOU)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        var param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          ParameterName = "PID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.String,
          OracleDbType = OracleDbType.VarChar,
          Direction = ParameterDirection.Input,
          ParameterName = "PNAME",
          SourceColumn = "NAME",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PTOU",
          SourceColumn = "TOU",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        //Update Command
        adapter.UpdateCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "UPDATE VIZ_PRN.QMF_INDICATOR SET NAME = :PNAME, TOU = :PTOU " +
            "WHERE (ID = :Original_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.String,
          OracleDbType = OracleDbType.VarChar,
          Direction = ParameterDirection.Input,
          ParameterName = "PNAME",
          SourceColumn = "NAME",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PTOU",
          SourceColumn = "TOU",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_ID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.UpdateCommand.Parameters.Add(param);

        //DeleteCommand
        adapter.DeleteCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "DELETE VIZ_PRN.QMF_INDICATOR WHERE (ID = :Original_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_ID",
          SourceColumn = "ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.DeleteCommand.Parameters.Add(param);
      }

      public int LoadData()
      {
        return Odac.LoadDataTable(this, adapter, true, null);
      }

      public int SaveData()
      {
        return Odac.SaveChangedData(this, adapter);
      }

    }

    public class InfluenceDataTable : DataTable
    {
      protected readonly OracleDataAdapter adapter;

      public InfluenceDataTable(string tblName) : base()
      {
        this.TableName = tblName;
        adapter = new OracleDataAdapter();

        var col = new DataColumn("ParamId", typeof(Int64), null, MappingType.Element)
        {
          AllowDBNull = false,
          AutoIncrement = false,
          AutoIncrementStep = -1
        };
        this.Columns.Add(col);

        col = new DataColumn("IndicatorId", typeof(Int64), null, MappingType.Element)
        {
          AllowDBNull = false,
          AutoIncrement = false,
          AutoIncrementStep = -1
        };
        this.Columns.Add(col);

        col = new DataColumn("ValInfluence", typeof(double), null, MappingType.Element);
        this.Columns.Add(col);

        this.Constraints.Add(new UniqueConstraint("Pk_" + tblName, new[] { this.Columns["ParamId"], this.Columns["IndicatorId"] }, true));
        
        adapter.TableMappings.Clear();
        var dtm = new System.Data.Common.DataTableMapping("VIZ_PRN.QMF_INFLUENCE", tblName);
        dtm.ColumnMappings.Add("PARAM_ID", "ParamId");
        dtm.ColumnMappings.Add("INDICATOR_ID", "IndicatorId");
        dtm.ColumnMappings.Add("VAL_INFLUENCE", "ValInfluence");
        adapter.TableMappings.Add(dtm);

        //Select Command
        adapter.SelectCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText = "SELECT PARAM_ID, INDICATOR_ID, VAL_INFLUENCE FROM VIZ_PRN.QMF_INFLUENCE ORDER BY INDICATOR_ID",
          CommandType = CommandType.Text
        };

        //Insert Command
        adapter.InsertCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "INSERT INTO VIZ_PRN.QMF_INFLUENCE(PARAM_ID, INDICATOR_ID, VAL_INFLUENCE) " +
            "VALUES(:PPARAM_ID, :PINDICATOR_ID, :PVAL_INFLUENCE)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        var param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          ParameterName = "PPARAM_ID",
          SourceColumn = "PARAM_ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          ParameterName = "PINDICATOR_ID",
          SourceColumn = "INDICATOR_ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PVAL_INFLUENCE",
          SourceColumn = "VAL_INFLUENCE",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.InsertCommand.Parameters.Add(param);

        //Update Command
        adapter.UpdateCommand = new OracleCommand
        {
          Connection = Odac.DbConnection,
          CommandText =
            "UPDATE VIZ_PRN.QMF_INFLUENCE SET VAL_INFLUENCE = :PVAL_INFLUENCE " +
            "WHERE (PARAM_ID = :Original_PARAM_ID) AND (INDICATOR_ID = :Original_INDICATOR_ID)",
          CommandType = CommandType.Text,
          PassParametersByName = true,
          UpdatedRowSource = UpdateRowSource.None
        };

        param = new OracleParameter
        {
          DbType = DbType.Double,
          OracleDbType = OracleDbType.Number,
          Direction = ParameterDirection.Input,
          ParameterName = "PVAL_INFLUENCE",
          SourceColumn = "VAL_INFLUENCE",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Current
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_PARAM_ID",
          SourceColumn = "PARAM_ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.UpdateCommand.Parameters.Add(param);

        param = new OracleParameter
        {
          DbType = DbType.Int64,
          OracleDbType = OracleDbType.Int64,
          Direction = ParameterDirection.Input,
          IsNullable = false,
          ParameterName = "Original_INDICATOR_ID",
          SourceColumn = "INDICATOR_ID",
          SourceColumnNullMapping = false,
          SourceVersion = DataRowVersion.Original
        };
        adapter.UpdateCommand.Parameters.Add(param);
      }

      public int LoadData()
      {
        return Odac.LoadDataTable(this, adapter, true, null);
      }

      public int SaveData()
      {
        return Odac.SaveChangedData(this, adapter);
      }
      
    }

  }

}
