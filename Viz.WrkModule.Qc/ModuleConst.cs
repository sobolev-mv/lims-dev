﻿namespace Viz.WrkModule.Qc
{

  public static class ModuleConst
  {
    public const string ModuleId = "177001";

    public enum TypeReferences
    {
      GroupParam = 0,
      Param = 1,
      QmIndicator = 2,
      Influence = 3
    };
    
    //Группы 1ур.
    internal enum AccL1Gr
    {
      ShiftRptUo = 10001
    }

    //Группы 2ур.
    internal enum AccL2Gr
    {
      ShiftRptUo = 13000
    }

    //Кнопки запуска отчетов
    internal enum AccRunControl
    {
      ShiftRptUo = 16000
    }

    public const string ScriptsFolder = "\\Scripts";
    public const string PrintLabelParamConfig = "\\Config\\PrintLabelParam.config";

  }
}