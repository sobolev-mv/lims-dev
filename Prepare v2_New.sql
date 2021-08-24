-- PREPARE
DECLARE
BEGIN
  FOR viEC IN ( SELECT *
                  FROM ERP_Control c
                 WHERE c.ControlId  > 46936528 -- критерий отбора
                   AND c.State = 'prepare'
                   AND c.MessageType IS NULL
                   AND c.Action IS NULL
                   AND EXISTS (SELECT *
                                 FROM ERP_GMData d
                                WHERE c.ControlId = d.ControlId)
                 ORDER BY c.ControlId)
  LOOP
    DBMS_Output.put_line( 'ControlId = ' || viEC.ControlId);
    
    viEC.MessageType  := 'GMSnd';
    viEC.Action       := 'GMSnd';
    viEC.Remark       := 'Resent by script '||TRUNC(SYSDATE)||' prepared->send v2';
    
    -- Fill ERP PO if missing
    IF viEC.ERPFa_Nr IS NULL THEN
      SELECT MAX(ERPFa_Nr)
        INTO viEC.ERPFa_Nr
        FROM ERP_Control
       WHERE MessageType  = 'GMSnd'
         AND Action       = 'GMSnd'
         AND SubsystemId  = viEC.SubsystemId
         AND Subsystem    = viEC.Subsystem
         AND Fa_Nr        = viEC.Fa_Nr;
      
      glo.assert(viEC.ERPFa_Nr IS NOT NULL, 'PO Id is not found!');
      
      -- Also fill to GMData
      FOR viGMD IN (SELECT * FROM ERP_GMData
                     WHERE ControlId = viEC.ControlId
                       AND ERPPOId IS NULL)
      LOOP
        viGMD.ERPPOId := viEC.ERPFa_Nr;
        tabERP_GMData.upd(viGMD);
      END LOOP;
    END IF;
    
    tabERP_Control.upd(viEC);
    
    -- Clean SAPT tables
    DELETE SAPT_SAPHEADER       WHERE ControlId = viEC.ControlId;
    DELETE SAPT_GMSnd           WHERE ControlId = viEC.ControlId;
    DELETE SAPT_GM_Code         WHERE ControlId = viEC.ControlId;
    DELETE SAPT_GM_Head_01      WHERE ControlId = viEC.ControlId;
    DELETE SAPT_GM_Item_Create  WHERE ControlId = viEC.ControlId;

    kmpERPUtilsPrj.resend2ERPforGen(viEC.ControlId);
  END LOOP;
  
  -- Synchronous generation to TCQ
  FOR viIpc IN (
    SELECT *
      FROM Glo_Ipc
     WHERE TRUNC(DtAngelegt) = TRUNC(SYSDATE)
       AND MsgCode = 'GEN_TLG_SAP'
     ORDER BY 1)
  LOOP
    kmpSAPGenPrj.dispatch(pi_Ide       => viIpc.IpcNr,
                          pi_Code      => viIpc.MsgCode,
                          pi_Data      => viIpc.MsgBuffer,
                          pi_Sender    => viIpc.Sender,
                          pi_Prio      => viIpc.Prioritaet,
                          pi_DTCreated => viIpc.DtAngelegt);
    
    DELETE Glo_Ipc WHERE IpcNr = viIpc.IpcNr;
  END LOOP;
END;
/
