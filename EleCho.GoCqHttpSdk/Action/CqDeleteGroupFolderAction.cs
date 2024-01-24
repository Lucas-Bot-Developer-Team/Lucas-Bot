﻿using EleCho.GoCqHttpSdk.Action.Model.Params;


namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 删除群文件夹操作
    /// </summary>
    public class CqDeleteGroupFolderAction : CqAction
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="folderId"></param>
        public CqDeleteGroupFolderAction(long groupId, string folderId)
        {
            GroupId = groupId;
            FolderId = folderId;
        }

        /// <summary>
        /// 操作类型: 删除群文件夹
        /// </summary>
        public override CqActionType ActionType => CqActionType.DeleteGroupFolder;

        /// <summary>
        /// 群号
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 文件夹 ID
        /// </summary>
        public string FolderId { get; set; }


        internal override CqActionParamsModel GetParamsModel()
        {
            return new CqDeleteGroupFolderActionParamsModel(GroupId, FolderId);
        }
    }
}