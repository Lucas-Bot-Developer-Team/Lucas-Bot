﻿#pragma warning disable CS8618


namespace EleCho.GoCqHttpSdk.Message.DataModel
{
    internal record class CqAnonymousMsgDataModel : CqMsgDataModel
    {
        public static CqAnonymousMsgDataModel FromCqCode(CqCode code)
        {
            return new CqAnonymousMsgDataModel();
        }
    }
}