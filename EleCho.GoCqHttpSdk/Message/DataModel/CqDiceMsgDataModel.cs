﻿#pragma warning disable CS8618


namespace EleCho.GoCqHttpSdk.Message.DataModel
{
    internal record class CqDiceMsgDataModel : CqMsgDataModel
    {
        public static CqDiceMsgDataModel FromCqCode(CqCode code)
        {
            return new CqDiceMsgDataModel();
        }
    }
}