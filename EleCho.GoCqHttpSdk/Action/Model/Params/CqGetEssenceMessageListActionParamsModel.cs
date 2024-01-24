using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.GoCqHttpSdk.Action.Model.Params
{
    internal class CqGetEssenceMessageListActionParamsModel : CqActionParamsModel
    {
        public CqGetEssenceMessageListActionParamsModel(long group_id, int page)
        {
            this.group_id = group_id;
            this.page = page;
        }

        public long group_id { get; }
        public int page { get; }
    }
}
