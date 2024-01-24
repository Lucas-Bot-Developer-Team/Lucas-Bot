﻿using EleCho.GoCqHttpSdk.DataStructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.GoCqHttpSdk
{
    /// <summary>
    /// 好友信息
    /// </summary>
    public record class CqFriend
    {
        internal CqFriend(CqFriendModel model)
        {
            UserId = model.user_id;
            Nickname = model.nickname;
            Remark = model.remark;
        }

        /// <summary>
        /// 用户 QQ
        /// </summary>
        public long UserId { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; }
    }
}
