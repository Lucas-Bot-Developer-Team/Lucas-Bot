//      ___          ___          ___          ___          ___          ___          ___     
//     /\  \        /\__\        /\  \        /\  \        /\__\        /\__\        /\  \    
//    /::\  \      /:/  /        \:\  \      /::\  \      /::|  |      /::|  |      /::\  \   
//   /:/\ \  \    /:/  /          \:\  \    /:/\:\  \    /:|:|  |     /:|:|  |     /:/\:\  \  
//  _\:\~\ \  \  /:/  /  ___       \:\  \  /::\~\:\  \  /:/|:|  |__  /:/|:|  |__  /::\~\:\  \ 
// /\ \:\ \ \__\/:/__/  /\__\_______\:\__\/:/\:\ \:\__\/:/ |:| /\__\/:/ |:| /\__\/:/\:\ \:\__\
// \:\ \:\ \/__/\:\  \ /:/  /\::::::::/__/\/__\:\/:/  /\/__|:|/:/  /\/__|:|/:/  /\:\~\:\ \/__/
//  \:\ \:\__\   \:\  /:/  /  \:\~~\~~         \::/  /     |:/:/  /     |:/:/  /  \:\ \:\__\  
//   \:\/:/  /    \:\/:/  /    \:\  \          /:/  /      |::/  /      |::/  /    \:\ \/__/  
//    \::/  /      \::/  /      \:\__\        /:/  /       /:/  /       /:/  /      \:\__\    
//     \/__/        \/__/        \/__/        \/__/        \/__/        \/__/        \/__/    

using Phigros_Library_FSharp;

namespace Lucas_Bot_OneBot.Modules.Phigros.Services;

using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

using static PhigrosAPIException;
using static PhigrosBestImageGenerator;
using static PhigrosAPIHandler;
using static GameSaveAnalyzer;

using Entities;
using Core;
using Helpers;

public static class PhigrosModuleHelper
{
    private enum PhigrosModuleOperationState
    {
        SUCCESS,
        ERR_NOT_BOUND,
        ERR_INTERNAL,
        ERR_INSTRUCTION_FORMAT
    }

    public static async void BestImageProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        logger.Info($"{CommandBuilder.DefaultCommandSuffix}b19指令被唤起，使用者：{commandInfo.SenderId}");
        var bestImageGenerationState = PhigrosModuleOperationState.SUCCESS;
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
        logger.Info("从数据库中查询SessionToken");
        var queryFailedReason = "";
        if (!await queryResult.AnyAsync())
            bestImageGenerationState = PhigrosModuleOperationState.ERR_NOT_BOUND;

        if (bestImageGenerationState == PhigrosModuleOperationState.SUCCESS)
        {
            logger.Info("已查询到SessionToken, 准备进入FSharp层");
            try
            {
                await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                    commandInfo.GroupId, commandInfo.MessageId,
                    new CqMessage("已知晓您的请求。正在生成 Best19 查分图，耗时可能较长，请耐心等待。"));
            }
            catch (Exception ex)
            {
                logger.Error("出现异常：", ex);
                bestImageGenerationState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
            var sessionToken = queryResult.First()["sessionToken"].ToString();
            var phigrosUser = new PhigrosUser(sessionToken);

            try
            {
                var avatarUri = ""; //await Utilities.GetAvatarUri(commandInfo.SenderId, commandInfo.GroupId ?? 0);
                var avatarQueryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
                if (await avatarQueryResult.AnyAsync())
                {
                    var useGameAvatar = queryResult.First();
                    if (useGameAvatar.TryGetElement("useGameAvatar", out var avatar) && avatar.Value == true)
                    {
                        var userInfo = await WrapFSharpAsync(phigrosUser.getUserInfoAsync());
                        logger.Info($"avatar = {userInfo.avatar}");
                        avatarUri = AssetsHelper.QueryAvatarPathFromName(userInfo.avatar).Value;
                    }
                    else
                    {
                        if (Program.GetDeployedPlatformType() 
                            is PlatformType.LAGRANGE_CORE
                            or PlatformType.OPEN_SHAMROCK)
                        {
                            avatarUri = Utilities.GetAvatarUri(commandInfo.SenderId);
                        }
                        else
                        {
                            
                            var tapAvatar = await WrapFSharpAsync(phigrosUser.getTapTapAvatarAsync());
                            logger.Info($"avatar = {tapAvatar}");
                            avatarUri = tapAvatar;
                        }
                    }
                }
                var best19ImageData =
                    await WrapFSharpAsync(GenerateB19ImageAsync(false, avatarUri,
                        phigrosUser));
                logger.Info("图片生成成功，已从FSharp层退出");

                switch (commandInfo.MessageType)
                {
                    case CqMessageType.Group:
                        await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                            commandInfo.GroupId, commandInfo.MessageId,
                            new CqMessage(new CqAtMsg(commandInfo.SenderId), CqImageMsg.FromBytes(best19ImageData)),
                            withReply: false);
                        break;
                    default:
                        await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                            commandInfo.GroupId, commandInfo.MessageId,
                            new CqMessage(CqImageMsg.FromBytes(best19ImageData)),
                            withReply: false);
                        break;
                }
            }
            catch (PhigrosAPIException e)
            {
                logger.Error("FSharp层出现异常", e);
                queryFailedReason = $"[PhigrosAPIException]\n{e.Data0}";
                bestImageGenerationState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
            catch (Exception e)
            {
                logger.Error("出现其他异常：", e);
                queryFailedReason = $"[{e.GetType()}]\n{e.Message}";
                bestImageGenerationState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
        }

        var hintMessage = bestImageGenerationState switch
        {
            PhigrosModuleOperationState.SUCCESS => "",
            PhigrosModuleOperationState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            PhigrosModuleOperationState.ERR_INTERNAL => queryFailedReason,
            PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT => throw new ArgumentOutOfRangeException(nameof(commandInfo)),
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };
        logger.Info($"b19查询状态：{bestImageGenerationState}");

        if (bestImageGenerationState == PhigrosModuleOperationState.SUCCESS) return;
        try
        {
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }
    }

    public static async void BestsProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        var bestsState = PhigrosModuleOperationState.SUCCESS;
        var hintMessage = "";

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}bests指令被唤起，使用者：{commandInfo.SenderId}");
        if (commandInfo.Parameters.Count != 1)
            bestsState = PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT;
        var bestsCount = 0;
        if (bestsState == PhigrosModuleOperationState.SUCCESS
            && (!int.TryParse(commandInfo.Parameters[0], out bestsCount) || bestsCount < 10 || bestsCount > 99))
            bestsState = PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT;

        logger.Info("通过指令格式校验");
        if (bestsState == PhigrosModuleOperationState.SUCCESS)
        {
            var collection = Utilities.GetCollection("phi", "sessionToken");
            var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
            logger.Info("从数据库中查询SessionToken");
            if (!await queryResult.AnyAsync())
                bestsState = PhigrosModuleOperationState.ERR_NOT_BOUND;
            if (bestsState == PhigrosModuleOperationState.SUCCESS)
            {
                logger.Info("已查询到SessionToken, 准备进入FSharp层");
                var sessionToken = queryResult.First()["sessionToken"].ToString();
                var phigrosUser = new PhigrosUser(sessionToken);

                try
                {
                    var playRecords = await WrapFSharpAsync(phigrosUser.getPlayRecordList());
                    var bestN = GetBestN(bestsCount, playRecords);
                    var bestPhi = GetBestPhi(playRecords);
                    var stringBuilder = new StringBuilder();

                    try
                    {
                        var phi = bestPhi.Value;
                        stringBuilder.Append("*" + FormatPlayRecord(phi, playRecords) + "\n");
                    }
                    catch (NullReferenceException)
                    {
                    } // Swallow exception

                    var i = 1;
                    foreach (var playRecord in bestN)
                    {
                        stringBuilder.Append(
                            $"{i}：{FormatPlayRecord(playRecord, playRecords)}{(i++ == bestsCount ? '\0' : '\n')}");
                    }

                    hintMessage = stringBuilder.ToString();
                    logger.Info("玩家信息查询成功，已从FSharp层退出");
                }
                catch (PhigrosAPIException e)
                {
                    logger.Error("FSharp层出现异常", e);
                    hintMessage = $"[PhigrosAPIException]\n{e.Data0}";
                    bestsState = PhigrosModuleOperationState.ERR_INTERNAL;
                }
                catch (Exception e)
                {
                    logger.Error("出现其他异常：", e);
                    hintMessage = $"[{e.GetType()}]\n{e.Message}";
                    bestsState = PhigrosModuleOperationState.ERR_INTERNAL;
                }
            }
        }

        hintMessage = bestsState switch
        {
            PhigrosModuleOperationState.SUCCESS => hintMessage,
            PhigrosModuleOperationState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            PhigrosModuleOperationState.ERR_INTERNAL => hintMessage,
            PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT => $"指令格式错误，应为{CommandBuilder.DefaultCommandSuffix}bests <待查询的Bests数量(介于10-99之间)>",
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            if (bestsState == PhigrosModuleOperationState.SUCCESS)
            {
                try
                {
                    await Program.HttpSession.SendPrivateMessageAsync(commandInfo.SenderId, new CqMessage(hintMessage));
                    if (commandInfo.MessageType == CqMessageType.Group)
                        await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                            commandInfo.GroupId, commandInfo.MessageId,
                            new CqMessage("消息过长，请在私聊中查看（如果您没有收到私聊消息，请检查是否允许陌生人私聊或添加好友）"));
                }
                catch (Exception)
                {
                    hintMessage = "私聊消息发送失败，请检查是否允许陌生人私聊或添加好友";
                    bestsState = PhigrosModuleOperationState.ERR_INTERNAL;
                }
            }

            if (bestsState != PhigrosModuleOperationState.SUCCESS)
                await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                    commandInfo.GroupId, commandInfo.MessageId,
                    new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"bests 查询状态：{bestsState.ToString()}");
    }

    public static async void SuggestProber(Command commandInfo)
    {
        var logger = Program.Logger;
        var suggestsState = PhigrosModuleOperationState.SUCCESS;
        var hintMessage = "";

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}suggest指令被唤起，使用者：{commandInfo.SenderId}");
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
        logger.Info("从数据库中查询SessionToken");
        if (!await queryResult.AnyAsync())
            suggestsState = PhigrosModuleOperationState.ERR_NOT_BOUND;
        if (suggestsState == PhigrosModuleOperationState.SUCCESS)
        {
            logger.Info("已查询到SessionToken, 准备进入FSharp层");
            var sessionToken = queryResult.First()["sessionToken"].ToString();
            var phigrosUser = new PhigrosUser(sessionToken);

            try
            {
                var playRecords = await WrapFSharpAsync(phigrosUser.getPlayRecordList());
                var probeN = ProbeSuggestion(playRecords);
                var probedPlayRecords = probeN as GameSave.PlayRecord[] ?? probeN.ToArray();
                var count = probedPlayRecords.Length;
                var stringBuilder = new StringBuilder("[推荐推分曲目]\n");

                if (count != 0)
                {
                    var i = 1;
                    foreach (var playRecord in probedPlayRecords)
                    {
                        stringBuilder.Append(
                            $"{FormatPlayRecord(playRecord, playRecords)}{(i++ == count ? '\0' : '\n')}");
                    }
                }
                else
                {
                    stringBuilder.Append(" 无推荐推分曲目");
                }

                hintMessage = stringBuilder.ToString();
                logger.Info("玩家信息查询成功，已从FSharp层退出");
            }
            catch (PhigrosAPIException e)
            {
                logger.Error("FSharp层出现异常", e);
                hintMessage = $"[PhigrosAPIException]\n{e.Data0}";
                suggestsState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
            catch (Exception e)
            {
                logger.Error("出现其他异常：", e);
                hintMessage = $"[{e.GetType()}]\n{e.Message}";
                suggestsState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
        }

        hintMessage = suggestsState switch
        {
            PhigrosModuleOperationState.SUCCESS => hintMessage,
            PhigrosModuleOperationState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            PhigrosModuleOperationState.ERR_INTERNAL => hintMessage,
            PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT => throw new ArgumentOutOfRangeException(nameof(commandInfo)),
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"suggests 查询状态：{suggestsState}");
    }

    public static async void InfoProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        var suggestsState = PhigrosModuleOperationState.SUCCESS;
        var hintMessage = "";

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}info指令被唤起，使用者：{commandInfo.SenderId}");
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
        logger.Info("从数据库中查询SessionToken");
        if (!await queryResult.AnyAsync())
            suggestsState = PhigrosModuleOperationState.ERR_NOT_BOUND;
        if (suggestsState == PhigrosModuleOperationState.SUCCESS)
        {
            logger.Info("已查询到SessionToken, 准备进入FSharp层");
            var sessionToken = queryResult.First()["sessionToken"].ToString();
            var phigrosUser = new PhigrosUser(sessionToken);

            try
            {
                var username = await WrapFSharpAsync(phigrosUser.getUserNameAsync());
                var playRecords = await WrapFSharpAsync(phigrosUser.getPlayRecordList());
                var rks = GetRKS(playRecords);
                var gameProgress = await WrapFSharpAsync(phigrosUser.getGameProgressAsync());
                var chalMode = ScoreUtils.GetChallengeModeInfo(gameProgress.challengeModeRank);
                var updateTime = await WrapFSharpAsync(phigrosUser.getSaveUpdateTimeAsync());
                hintMessage = $"\n昵称: {username}\n" +
                    $"RKS: {rks:F3}\n" +
                    $"课题模式: {chalMode.Item1} {chalMode.Item2}\n" +
                    $"存档更新时间: {updateTime:yyyy-MM-dd HH:mm:ss}";
                logger.Info("玩家信息查询成功，已从FSharp层退出");
            }
            catch (PhigrosAPIException e)
            {
                logger.Error("FSharp层出现异常", e);
                hintMessage = $"[PhigrosAPIException]\n{e.Data0}";
                suggestsState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
            catch (Exception e)
            {
                logger.Error("出现其他异常：", e);
                hintMessage = $"[{e.GetType()}]\n{e.Message}";
                suggestsState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
        }

        hintMessage = suggestsState switch
        {
            PhigrosModuleOperationState.SUCCESS => hintMessage,
            PhigrosModuleOperationState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            PhigrosModuleOperationState.ERR_INTERNAL => hintMessage,
            PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT => throw new ArgumentOutOfRangeException(nameof(commandInfo)),
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"info 查询状态：{suggestsState}");
    }

    public static async void AccuracyProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        var suggestsState = PhigrosModuleOperationState.SUCCESS;
        var hintMessage = "";

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}acc指令被唤起，使用者：{commandInfo.SenderId}");
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
        logger.Info("从数据库中查询SessionToken");
        if (!await queryResult.AnyAsync())
            suggestsState = PhigrosModuleOperationState.ERR_NOT_BOUND;
        if (suggestsState == PhigrosModuleOperationState.SUCCESS)
        {
            logger.Info("已查询到SessionToken, 准备进入FSharp层");
            var sessionToken = queryResult.First()["sessionToken"].ToString();
            var phigrosUser = new PhigrosUser(sessionToken);

            try
            {
                var playRecords = await WrapFSharpAsync(phigrosUser.getPlayRecordList());
                var playRecord = GetPlayRecordFromUserInput(commandInfo.Parameters, playRecords);
                hintMessage = FormatPlayRecord(playRecord, playRecords);
                logger.Info("玩家信息查询成功，已从FSharp层退出");
            }
            catch (PhigrosAPIException e)
            {
                logger.Error("FSharp层出现异常", e);
                hintMessage = $"[PhigrosAPIException]\n{e.Data0}";
                suggestsState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
            catch (Exception e)
            {
                logger.Error("出现其他异常：", e);
                hintMessage = $"[{e.GetType()}]\n{e.Message}";
                suggestsState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
        }

        hintMessage = suggestsState switch
        {
            PhigrosModuleOperationState.SUCCESS => hintMessage,
            PhigrosModuleOperationState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            PhigrosModuleOperationState.ERR_INTERNAL => hintMessage,
            PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT => throw new ArgumentOutOfRangeException(nameof(commandInfo)),
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                commandInfo.GroupId, commandInfo.MessageId,
                new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"acc 查询状态：{suggestsState}");
    }

    public static async void BatchProcessor(Command commandInfo)
    {
        var logger = Program.Logger;
        var batchState = PhigrosModuleOperationState.SUCCESS;
        var hintMessage = "";
        var prescribedMinimum = 0f;
        var superiorLimit = prescribedMinimum;
        var count = 0;

        if (commandInfo.Parameters.Count != 1 && commandInfo.Parameters.Count != 2) 
        {
            batchState = PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT;
        }
        else
        {
            try
            {
                prescribedMinimum = float.Parse(commandInfo.Parameters[0]);
                superiorLimit = prescribedMinimum;
                if (commandInfo.Parameters.Count == 2)
                {
                    superiorLimit = float.Parse(commandInfo.Parameters[1]);
                }
            }
            catch (Exception ex)
            {
                logger.Error("发生异常: ", ex);
                hintMessage = ex.Message;
                batchState = PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT;
            }
        }

        logger.Info($"{CommandBuilder.DefaultCommandSuffix}batch指令被唤起，使用者：{commandInfo.SenderId}");
        var collection = Utilities.GetCollection("phi", "sessionToken");
        var queryResult = collection.Find(Builders<BsonDocument>.Filter.Eq("qq", commandInfo.SenderId.ToString()));
        logger.Info("从数据库中查询SessionToken");
        if (!await queryResult.AnyAsync())
            batchState = PhigrosModuleOperationState.ERR_NOT_BOUND;
        if (batchState == PhigrosModuleOperationState.SUCCESS)
        {
            logger.Info("已查询到SessionToken, 准备进入FSharp层");
            var sessionToken = queryResult.First()["sessionToken"].ToString();
            var phigrosUser = new PhigrosUser(sessionToken);

            try
            {
                var playRecords = await WrapFSharpAsync(phigrosUser.getPlayRecordList());
                var batchRecords = GetPlayRecordsBatch(prescribedMinimum, superiorLimit, playRecords);
                var records = batchRecords as GameSave.PlayRecord[] ?? batchRecords.ToArray();
                count = records.Length;
                var stringBuilder = new StringBuilder($"[批量查分结果]\n");

                if (count != 0)
                {
                    var i = 1;
                    foreach (var playRecord in records)
                    {
                        stringBuilder.Append(
                            $"{FormatPlayRecord(playRecord, playRecords)}{(i++ == count ? '\0' : '\n')}");
                    }
                }
                else
                {
                    stringBuilder.Append("该定数范围内您未游玩过歌曲");
                }

                hintMessage = stringBuilder.ToString();
                logger.Info("玩家信息查询成功，已从FSharp层退出");
            }
            catch (PhigrosAPIException e)
            {
                logger.Error("FSharp层出现异常", e);
                hintMessage = $"[PhigrosAPIException]\n{e.Data0}";
                batchState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
            catch (Exception e)
            {
                logger.Error("出现其他异常：", e);
                hintMessage = e.Message;
                batchState = PhigrosModuleOperationState.ERR_INTERNAL;
            }
        }

        hintMessage = batchState switch
        {
            PhigrosModuleOperationState.SUCCESS => hintMessage,
            PhigrosModuleOperationState.ERR_NOT_BOUND => $"您未绑定。请先使用{CommandBuilder.DefaultCommandSuffix}bind指令完成绑定。",
            PhigrosModuleOperationState.ERR_INTERNAL => hintMessage,
            PhigrosModuleOperationState.ERR_INSTRUCTION_FORMAT => $"指令格式错误，应为{CommandBuilder.DefaultCommandSuffix}batch 定数下限 <定数上限（可选）>",
            _ => throw new ArgumentOutOfRangeException(nameof(commandInfo))
        };

        try
        {
            if (batchState == PhigrosModuleOperationState.SUCCESS)
            {
                try
                {
                    if (count > 15)
                    {
                        await Program.HttpSession.SendPrivateMessageAsync(commandInfo.SenderId, new CqMessage(hintMessage));
                        if (commandInfo.MessageType == CqMessageType.Group)
                            await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                                commandInfo.GroupId, commandInfo.MessageId,
                                new CqMessage("消息过长，请在私聊中查看（如果您没有收到私聊消息，请检查是否允许陌生人私聊或添加好友）"));
                    }
                    else
                    {
                        await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                            commandInfo.GroupId, commandInfo.MessageId,
                            new CqMessage(hintMessage));
                    }
                }
                catch (Exception)
                {
                    hintMessage = "私聊消息发送失败，请检查是否允许陌生人私聊或添加好友";
                    batchState = PhigrosModuleOperationState.ERR_INTERNAL;
                }
            }

            if (batchState != PhigrosModuleOperationState.SUCCESS)
                await Program.HttpSession.GenericReplyMessageAsync(commandInfo.MessageType, commandInfo.SenderId,
                    commandInfo.GroupId, commandInfo.MessageId,
                    new CqMessage(hintMessage));
        }
        catch (Exception ex)
        {
            logger.Error("出现异常：", ex);
        }

        logger.Info($"batch 查询状态：{batchState}");
    }
}