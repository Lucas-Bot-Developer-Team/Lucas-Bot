﻿using System;
using System.Threading.Tasks;
using EleCho.GoCqHttpSdk.Post;

namespace EleCho.GoCqHttpSdk
{

    // 注意：这个文件是自动生成的，不要手动修改

    /// <summary>
    /// 上报会话的相关拓展方法
    /// </summary>
    public static class CqPostSessionExtensions
    {
        private static ICqPostSession Use<IContext>(ICqPostSession session, Func<IContext, Func<Task>, Task> middleware) where IContext : CqPostContext
        {
            session.PostPipeline.Use(async (context, next) =>
            {
                if (context is IContext specifiedContext)
                {
                    await middleware.Invoke(specifiedContext, next);
                }
                else
                {
                    await next.Invoke();
                }
            });

            return session;
        }

        private static ICqPostSession Use<IContext>(ICqPostSession session, Action<IContext, Func<Task>> middleware) where IContext : CqPostContext
        {
            session.PostPipeline.Use(async (context, next) =>
            {
                if (context is IContext specifiedContext)
                {
                    middleware.Invoke(specifiedContext, next);
                }
                else
                {
                    await next.Invoke();
                }
            });

            return session;
        }

        private static ICqPostSession Use<IContext>(ICqPostSession session, Func<IContext, Task> middleware) where IContext : CqPostContext
        {
            session.PostPipeline.Use(async (context, next) =>
            {
                if (context is IContext specifiedContext)
                {
                    await middleware.Invoke(specifiedContext);
                    await next.Invoke();
                }
                else
                {
                    await next.Invoke();
                }
            });

            return session;
        }

        private static ICqPostSession Use<IContext>(ICqPostSession session, Action<IContext> middleware) where IContext : CqPostContext
        {
            session.PostPipeline.Use(async (context, next) =>
            {
                if (context is IContext specifiedContext)
                {
                    middleware.Invoke(specifiedContext);
                    await next.Invoke();
                }
                else
                {
                    await next.Invoke();
                }
            });

            return session;
        }

        /// <summary>
        /// 使用一个中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">中间件</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseMiddleware(this ICqPostSession session, CqPostMiddleware middleware)
        {
            session.PostPipeline.Use(middleware.Execute);
            return session;
        }

        /// <summary>
        /// 使用一个插件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="plugin">插件</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePlugin(this ICqPostSession session, CqPostPlugin plugin)
        {
            session.PostPipeline.Use(plugin.Execute);
            return session;
        }

        /// <summary>
        /// 使用任何一个中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">中间件</param>
        /// <returns>传入的上报会话</returns>
        public static void UseAny(this ICqPostSession session, Func<CqPostContext, Func<Task>, Task> middleware) => Use(session, middleware);

        #region Message
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessage(this ICqPostSession session, Func<CqGroupMessagePostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessage(this ICqPostSession session, Action<CqGroupMessagePostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessage(this ICqPostSession session, Func<CqGroupMessagePostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessage(this ICqPostSession session, Action<CqGroupMessagePostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateMessage(this ICqPostSession session, Func<CqPrivateMessagePostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateMessage(this ICqPostSession session, Action<CqPrivateMessagePostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateMessage(this ICqPostSession session, Func<CqPrivateMessagePostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateMessage(this ICqPostSession session, Action<CqPrivateMessagePostContext> middleware) => Use(session, middleware);
        #endregion Message

        #region SelfMessage
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupSelfMessage(this ICqPostSession session, Func<CqGroupSelfMessagePostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupSelfMessage(this ICqPostSession session, Action<CqGroupSelfMessagePostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupSelfMessage(this ICqPostSession session, Func<CqGroupSelfMessagePostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupSelfMessage(this ICqPostSession session, Action<CqGroupSelfMessagePostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateSelfMessage(this ICqPostSession session, Func<CqPrivateSelfMessagePostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateSelfMessage(this ICqPostSession session, Action<CqPrivateSelfMessagePostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateSelfMessage(this ICqPostSession session, Func<CqPrivateSelfMessagePostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePrivateSelfMessage(this ICqPostSession session, Action<CqPrivateSelfMessagePostContext> middleware) => Use(session, middleware);
        #endregion

        #region Notice
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseClientStatusChanged(this ICqPostSession session, Func<CqClientStatusChangedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseClientStatusChanged(this ICqPostSession session, Action<CqClientStatusChangedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseClientStatusChanged(this ICqPostSession session, Func<CqClientStatusChangedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseClientStatusChanged(this ICqPostSession session, Action<CqClientStatusChangedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendAdded(this ICqPostSession session, Func<CqFriendAddedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendAdded(this ICqPostSession session, Action<CqFriendAddedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendAdded(this ICqPostSession session, Func<CqFriendAddedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendAdded(this ICqPostSession session, Action<CqFriendAddedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendMessageRecalled(this ICqPostSession session, Func<CqFriendMessageRecalledPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendMessageRecalled(this ICqPostSession session, Action<CqFriendMessageRecalledPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendMessageRecalled(this ICqPostSession session, Func<CqFriendMessageRecalledPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendMessageRecalled(this ICqPostSession session, Action<CqFriendMessageRecalledPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupAdministratorChanged(this ICqPostSession session, Func<CqGroupAdministratorChangedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupAdministratorChanged(this ICqPostSession session, Action<CqGroupAdministratorChangedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupAdministratorChanged(this ICqPostSession session, Func<CqGroupAdministratorChangedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupAdministratorChanged(this ICqPostSession session, Action<CqGroupAdministratorChangedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupEssenceChanged(this ICqPostSession session, Func<CqGroupEssenceChangedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupEssenceChanged(this ICqPostSession session, Action<CqGroupEssenceChangedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupEssenceChanged(this ICqPostSession session, Func<CqGroupEssenceChangedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupEssenceChanged(this ICqPostSession session, Action<CqGroupEssenceChangedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupFileUploaded(this ICqPostSession session, Func<CqGroupFileUploadedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupFileUploaded(this ICqPostSession session, Action<CqGroupFileUploadedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupFileUploaded(this ICqPostSession session, Func<CqGroupFileUploadedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupFileUploaded(this ICqPostSession session, Action<CqGroupFileUploadedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupLuckyKingNoticed(this ICqPostSession session, Func<CqGroupLuckyKingNoticedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupLuckyKingNoticed(this ICqPostSession session, Action<CqGroupLuckyKingNoticedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupLuckyKingNoticed(this ICqPostSession session, Func<CqGroupLuckyKingNoticedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupLuckyKingNoticed(this ICqPostSession session, Action<CqGroupLuckyKingNoticedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberBanChanged(this ICqPostSession session, Func<CqGroupMemberBanChangedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberBanChanged(this ICqPostSession session, Action<CqGroupMemberBanChangedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberBanChanged(this ICqPostSession session, Func<CqGroupMemberBanChangedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberBanChanged(this ICqPostSession session, Action<CqGroupMemberBanChangedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberDecreased(this ICqPostSession session, Func<CqGroupMemberDecreasedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberDecreased(this ICqPostSession session, Action<CqGroupMemberDecreasedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberDecreased(this ICqPostSession session, Func<CqGroupMemberDecreasedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberDecreased(this ICqPostSession session, Action<CqGroupMemberDecreasedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberHonorChanged(this ICqPostSession session, Func<CqGroupMemberHonorChangedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberHonorChanged(this ICqPostSession session, Action<CqGroupMemberHonorChangedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberHonorChanged(this ICqPostSession session, Func<CqGroupMemberHonorChangedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberHonorChanged(this ICqPostSession session, Action<CqGroupMemberHonorChangedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberIncreased(this ICqPostSession session, Func<CqGroupMemberIncreasedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberIncreased(this ICqPostSession session, Action<CqGroupMemberIncreasedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberIncreased(this ICqPostSession session, Func<CqGroupMemberIncreasedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberIncreased(this ICqPostSession session, Action<CqGroupMemberIncreasedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberNicknameChanged(this ICqPostSession session, Func<CqGroupMemberNicknameChangedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberNicknameChanged(this ICqPostSession session, Action<CqGroupMemberNicknameChangedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberNicknameChanged(this ICqPostSession session, Func<CqGroupMemberNicknameChangedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberNicknameChanged(this ICqPostSession session, Action<CqGroupMemberNicknameChangedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberTitleChangeNoticed(this ICqPostSession session, Func<CqGroupMemberTitleChangeNoticedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberTitleChangeNoticed(this ICqPostSession session, Action<CqGroupMemberTitleChangeNoticedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberTitleChangeNoticed(this ICqPostSession session, Func<CqGroupMemberTitleChangeNoticedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMemberTitleChangeNoticed(this ICqPostSession session, Action<CqGroupMemberTitleChangeNoticedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessageRecalled(this ICqPostSession session, Func<CqGroupMessageRecalledPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessageRecalled(this ICqPostSession session, Action<CqGroupMessageRecalledPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessageRecalled(this ICqPostSession session, Func<CqGroupMessageRecalledPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupMessageRecalled(this ICqPostSession session, Action<CqGroupMessageRecalledPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseOfflineFileUploaded(this ICqPostSession session, Func<CqOfflineFileUploadedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseOfflineFileUploaded(this ICqPostSession session, Action<CqOfflineFileUploadedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseOfflineFileUploaded(this ICqPostSession session, Func<CqOfflineFileUploadedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseOfflineFileUploaded(this ICqPostSession session, Action<CqOfflineFileUploadedPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePoked(this ICqPostSession session, Func<CqPokedPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePoked(this ICqPostSession session, Action<CqPokedPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePoked(this ICqPostSession session, Func<CqPokedPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UsePoked(this ICqPostSession session, Action<CqPokedPostContext> middleware) => Use(session, middleware);
        #endregion Notice

        #region Request
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendRequest(this ICqPostSession session, Func<CqFriendRequestPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendRequest(this ICqPostSession session, Action<CqFriendRequestPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendRequest(this ICqPostSession session, Func<CqFriendRequestPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseFriendRequest(this ICqPostSession session, Action<CqFriendRequestPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupRequest(this ICqPostSession session, Func<CqGroupRequestPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupRequest(this ICqPostSession session, Action<CqGroupRequestPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupRequest(this ICqPostSession session, Func<CqGroupRequestPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseGroupRequest(this ICqPostSession session, Action<CqGroupRequestPostContext> middleware) => Use(session, middleware);
        #endregion Request

        #region MetaEvent
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseHeartbeat(this ICqPostSession session, Func<CqHeartbeatPostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseHeartbeat(this ICqPostSession session, Action<CqHeartbeatPostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseHeartbeat(this ICqPostSession session, Func<CqHeartbeatPostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseHeartbeat(this ICqPostSession session, Action<CqHeartbeatPostContext> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 带有下一个中间件参数的中间件. async (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseLifecycle(this ICqPostSession session, Func<CqLifecyclePostContext, Func<Task>, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 带有下一个中间件参数的中间件. (context, next) => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseLifecycle(this ICqPostSession session, Action<CqLifecyclePostContext, Func<Task>> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">异步, 不带有下一个中间件参数的中间件. async context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseLifecycle(this ICqPostSession session, Func<CqLifecyclePostContext, Task> middleware) => Use(session, middleware);
        /// <summary>
        /// 使用一个能处理特定上报的中间件
        /// </summary>
        /// <param name="session">上报会话</param>
        /// <param name="middleware">同步, 不带有下一个中间件参数的中间件. context => { }</param>
        /// <returns>传入的上报会话</returns>
        public static ICqPostSession UseLifecycle(this ICqPostSession session, Action<CqLifecyclePostContext> middleware) => Use(session, middleware);
        #endregion MetaEvent
    }
}