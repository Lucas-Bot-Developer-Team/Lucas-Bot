//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//               佛祖保佑         永无BUG
//
//
//

module GameSave
open System
open ScoreUtils
open AssetsHelper

type ScoreRecord(acc: float32, score: int, isFC: bool) = 
    member val accuracy = acc with get 
    member val score = score with get
    member val isFullCombo = isFC with get
    member val scoreRank = score |> GetScoreRank with get

type SongRecord(id: string, scoreInfos: Map<Difficulty, ScoreRecord>) = 
    member val songId = id with get
    member val scoreInfos = scoreInfos with get

type GameRecord(ver: int, songs: List<SongRecord>) =
    member val version = ver with get
    member val songRecords = songs with get

type PlayRecord(id: string, name: string, diff: Difficulty, score: int, isFC: bool, acc: float32) =
    member val accuracy = acc with get 
    member val score = score with get
    member val isFullCombo = isFC with get
    member val scoreRank = score |> GetScoreRank with get
    member val songName = name with get
    member val songId = id with get
    member val difficulty = diff with get
    member val singleRKS = diff 
                        |> QueryChartRatingFromId id
                        |> GetSingleRKS acc
                        
type GameProgress(ver: int) =
    member val ver = ver with get
    member val isFirstRun = false with get, set
    member val isLegacyChapterFinished = false with get, set
    member val alreadyShowCollectionTip = false with get, set
    member val alreadyShowAutoUnlockINTip = false with get, set
    member val completed = "" with get, set
    member val songUpdateInfo = byte 0 with get, set
    member val challengeModeRank = int16 0 with get, set
    member val money = int16 0 |> Array.create 5 with get, set
    member val unlockFlagOfSpasmodic = byte 0 with get, set
    member val unlockFlagOfIgallta = byte 0 with get, set
    member val unlockFlagOfRrharil = byte 0 with get, set
    member val flagOfSongRecordKey = byte 0 with get, set
    member val randomVersionUnlocked = byte 0 with get, set
    member val chapter8UnlockBegin = false with get, set
    member val chapter8UnlockSecondPhase = false with get, set
    member val chapter8Passed = false with get, set
    member val chapter8SongUnlocked = byte 0 with get, set

type UserInfo(ver: int) = 
    member val ver = ver with get
    member val displayUserId = false with get, set
    member val selfIntroduction = "" with get, set
    member val avatar = "" with get, set 
    member val background = "" with get, set