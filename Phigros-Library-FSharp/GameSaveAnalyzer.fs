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

module GameSaveAnalyzer
open System

open System.Text.RegularExpressions
open GameSave
open AssetsHelper
open Microsoft.FSharp.Core
open PhigrosAPIException
open ScoreUtils
open System.Collections.Generic

type IEnumerable<'T> with
     member public this.Length() = 
        this |> Seq.sumBy (fun _ -> 1)

// Util: Check bool, false -> throw PhigrosAPIException
let CheckIfTrue (reason: string) (value: bool) =
    match value with
     | true -> ()
     | false -> raise(PhigrosAPIException(reason))

// Save analyzing functions.
let GetFullComboCount (diff: Difficulty) (playRecords: PlayRecord seq) = 
    playRecords
     |> Seq.filter (fun pr -> pr.difficulty = diff && pr.isFullCombo = true)
     |> Seq.length

let GetPhiCount (diff: Difficulty) (playRecords: PlayRecord seq) = 
    playRecords
     |> Seq.filter (fun pr -> pr.difficulty = diff && pr.scoreRank = Rank.Phi)
     |> Seq.length
    
let GetBestN n (playRecords: PlayRecord seq) =
    try    
        playRecords
        |> Seq.sortByDescending (fun pr -> pr.singleRKS)
        |> Seq.truncate n
    with
     | :? InvalidOperationException -> playRecords
     | :? ArgumentException -> raise(PhigrosAPIException("Saving is empty. Please play a song first!"))

let GetBestPhi (playRecords: PlayRecord seq) = 
    try
        playRecords
        |> Seq.filter (fun pr -> pr.scoreRank = Rank.Phi)
        |> Seq.sortByDescending (fun pr -> pr.singleRKS)
        |> Seq.head
        |> Some
    with
     | :? ArgumentException -> None
     
let GetRKS (playRecords: seq<PlayRecord>) =
    let phiOption = playRecords
                    |> GetBestPhi
    let b19Sum = playRecords
                 |> GetBestN 19
                 |> Seq.sumBy (fun pr -> pr.singleRKS)
    match phiOption with
     | Some(phi) -> (b19Sum + phi.singleRKS) / 20f
     | None -> b19Sum / 20f
     
let _probeMinRKSInternal rks =
    MathF.Round(rks, 2) + 0.005f
            
let ProbeMinAcc (songId: string) (diff: Difficulty)
                (playRecordList: PlayRecord seq) =
    // No English annotation, just slack off a bit here.
    // 先查询歌曲信息
    let songInformation = songId
                          |> QuerySongInfoFromId
    match songInformation with
     | None -> raise(PhigrosAPIException(String.Format("Song Not Found. Please update API version. Internal ID: {0}", songId)))
     | Some(info) ->
         // 取当前难度谱面定数
         let destChartDiff = info.Charts[diff].Difficulty
         match playRecordList.Length() with
          | x when x < 19 ->
              // 当PlayRecord长度小于19时，返回70%即可
              // 反正也没人闲到只打了十几首ez和hd，连查分图都填不满就来规划推分
              Some(70.00f)     
          | _ ->
              // 取地板
              let best19 = playRecordList
                           |> GetBestN 19
              let rks = playRecordList |> GetRKS
              let floor = best19
                           |> Seq.last
              let floorRKS = floor.singleRKS
              // 当当前歌曲定数小于等于地板RKS，则要想让RKS变化，仅能通过取代Phi位达到。
              match destChartDiff with
               | x when x.Compare(floorRKS, Float32CompareOption.LesserThan) ->
                   match playRecordList
                          |> GetBestPhi with
                    | Some(phi) -> 
                        if destChartDiff.Compare(phi.singleRKS, Float32CompareOption.LesserThan)
                        then None // 若当前有Phi记录且定数超过待查询的曲目定数，则无法推分
                        else Some(100.00f) // 反之则可推分。
                    | None -> Some(100.00f) // 若当前没有Phi记录，直接打到Phi即可推分。
               | _ -> // 反之，则直接替代B19末位/上位即可。
                   let result = match best19
                                       |> Seq.tryFind (fun pr -> pr.songId = songId && pr.difficulty = diff) with
                                 | Some(res) -> res
                                 | None -> best19 |> Seq.last
                   let aimRKS = rks |> _probeMinRKSInternal
                   let rksWithoutResult = rks * 20f - result.singleRKS
                   let aimSingleRKS = aimRKS * 20f - rksWithoutResult
                   aimSingleRKS |> GetAccFromSingleRKS destChartDiff
               
/// 
/// <summary>
/// 推荐推分曲目 
/// </summary>
/// <param name="playRecords">存档游玩记录列表</param>
/// <returns>推荐推分曲目列表</returns>
///
let ProbeSuggestion (playRecords: PlayRecord seq) =
    let result = try
                        let rks = playRecords |> GetRKS // 求RKS
                        playRecords 
                         |> Seq.filter (fun pr -> playRecords
                                                   |> ProbeMinAcc pr.songId pr.difficulty
                                                   |> Option.isSome) // 选择可以推分的曲目
                         |> Seq.sortBy (fun pr -> pr.accuracy) // 根据可推分歌曲的准度排序，准度越差越容易推分
                         |> Seq.sortBy (fun pr -> let diff = pr.difficulty
                                                              |> QueryChartRatingFromId pr.songId
                                                  match diff < rks + 0.5f with
                                                    | true -> 0
                                                    | false -> 1) // 过滤掉定数高于rks 0.5的曲目
                         |> Seq.sortBy (fun pr -> let probedAcc = playRecords
                                                                    |> ProbeMinAcc pr.songId pr.difficulty
                                                                    |> Option.get
                                                  match probedAcc.Compare(100f, Float32CompareOption.Equal) with
                                                    | true -> 1
                                                    | false -> 0)
                         
                 with
                  | _ -> raise(PhigrosAPIException("API Internal error. Please contact maintainer."))
    try
        result
         |> Seq.truncate 10
    with
     | :? ArgumentException -> [] // 列表为空，无法推分
     | :? InvalidOperationException -> result // 可推分曲目小于10首，直接返回
     | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message))
     

// Generic Input Processing
// ID / Name / Alias -> PlayRecord
// Implemented in F# because it's convenient for migration.
 
let _GetPlayRecordFromUserInputInternal (playRecords: PlayRecord list)
                                        (userInput: string list)
                                        (difficulty: Difficulty) =
    let userInputMerged = String.Join(' ', userInput)
    match userInputMerged with
     | IsSongId songId ->
         difficulty |> CheckDiffExists songId
         let info = (songId |> QuerySongInfoFromId).Value
         match playRecords |> List.exists (fun pr -> pr.songId.Equals(userInputMerged) && pr.difficulty.Equals(difficulty)) with
          | true -> playRecords
                    |> List.find (fun pr -> pr.songId.Equals(userInputMerged) && pr.difficulty.Equals(difficulty))
          | false -> PlayRecord(id = info.SongId, name = info.SongName, diff = difficulty, score = 0, isFC = false, acc = 0f)
     | IsName songId | IsAlias songId ->
         match songId.Length with
          | 0 -> raise(PhigrosAPIException("未找到指定歌曲。请检查您的输入。"))
          | 1 ->
              let thisSongId = songId.Head
              difficulty |> CheckDiffExists thisSongId
              let info = (thisSongId |> QuerySongInfoFromId).Value
              match playRecords |> List.exists (fun pr -> pr.songId.Equals(thisSongId) && pr.difficulty.Equals(difficulty)) with
                | true -> playRecords
                            |> List.find (fun pr -> pr.songId.Equals(thisSongId) && pr.difficulty.Equals(difficulty))
                | false -> PlayRecord(id = info.SongId, name = info.SongName, diff = difficulty, score = 0, isFC = false, acc = 0f)
          | _ ->
              let hint = "[ " + String.Join("; ", songId) + " ]"
              raise(PhigrosAPIException(String.Format("根据给定的名称或别名找到大于一首曲目。请重新查询。\n可能的曲目有：{0}", hint)))
     | _ -> raise(PhigrosAPIException("未找到指定歌曲。请检查您的输入。"))
         
let _SliceUserInputDifficulty playRecords (userInput: string list) =
    let difficultyList = [Difficulty.EZ; Difficulty.HD; Difficulty.IN; Difficulty.AT]
    
    let (|IsDiff|_|) (input: string) = match difficultyList |> List.exists (fun diff -> diff.ToString().ToLower().Equals(input.ToLower())) with
                                        | true -> difficultyList
                                                   |> List.choose (fun diff -> if diff.ToString().ToLower().Equals(input.ToLower()) then Some(diff) else None)
                                                   |> List.tryHead
                                        | _ -> None
                           
    match userInput.Length with // If input section's amount >= 2, take out the last section and check if it's an abbr. of a Difficulty. 
     | x when x >= 2 ->
         match userInput[userInput.Length - 1] with
          | IsDiff diff ->
              diff |> _GetPlayRecordFromUserInputInternal playRecords userInput[0 .. userInput.Length - 2]
          | _ -> Difficulty.IN |> _GetPlayRecordFromUserInputInternal playRecords userInput
     | 1 -> Difficulty.IN |> _GetPlayRecordFromUserInputInternal playRecords userInput
     | _ -> raise(PhigrosAPIException("未找到指定歌曲。请检查您的输入。"))
     
  
let GetPlayRecordFromUserInput userInput playRecords =
     [ for str in userInput -> str ]
      |> _SliceUserInputDifficulty playRecords

// Batch query functions.
let GetPlayRecordsBatch (prescribedMinimum: float32) (superiorLimit: float32) (playRecords: PlayRecord seq) =
    (prescribedMinimum <= superiorLimit)
     |> CheckIfTrue "定数范围非法。"
    playRecords
     |> Seq.filter (fun pr -> 
                        match pr.songId |> QuerySongInfoFromId with
                          | Some(songInfo) -> songInfo.Charts[pr.difficulty].Difficulty >= prescribedMinimum && songInfo.Charts[pr.difficulty].Difficulty <= superiorLimit
                          | None -> raise(PhigrosAPIException($"Song not found. Please update API version. Internal ID: {pr.songId}")))
     |> Seq.sortBy (fun pr -> match pr.songId |> QuerySongInfoFromId with
                               | Some(songInfo) -> songInfo.Charts[pr.difficulty].Difficulty
                               | None -> raise(PhigrosAPIException($"Song not found. Please update API version. Internal ID: {pr.songId}")))

// Anti-Cheat functions.
let _checkChallengeModeCheat
    (ignoreCheck: bool)
    (challengeMode: int16) =
    let challengeModeRank, challengeModeRate = challengeMode
                                               |> GetChallengeModeInfo
    match challengeModeRank with
     | ChallengeModeRanking.Rainbow ->
         match challengeModeRate with
          | 48 -> if ignoreCheck // Check Rainbow 48.
                    then ()
                    else raise(PhigrosAPIException("您的存档有作弊嫌疑。请私聊Bot提供可信实力证明，以继续使用查分功能。"))
          | _ -> ()
     | ChallengeModeRanking.Gray ->
         match challengeModeRate with
          | 0 -> ()
          | _ -> raise(PhigrosAPIException("您的存档有作弊行为。"))
     | _ -> ()
     
let _checkSpeedChangeCheat
    (ignoreCheck: bool)
    (playRecord: PlayRecord) =
    match ignoreCheck with
     | true -> ()
     | false ->
        match playRecord.difficulty
              |> QueryChartRatingFromId playRecord.songId with
         | x when x > 15.85f ->
             match playRecord.isFullCombo with
              | true -> if playRecord.accuracy < 98.5f
                        then raise(PhigrosAPIException("您的存档有作弊嫌疑。请私聊Bot提供可信实力证明，以继续使用查分功能。"))
                        else ()
              | false -> ()
         | _ -> ()
         
let _checkRksThreshold (rks: float32) =
    let threshold = 11.0f
    match rks with
     | x when x.Compare(threshold, Float32CompareOption.GreaterThan) -> ()
     | _ -> raise(PhigrosAPIException(String.Format("Ranking score is below the threshold of querying Best19 ({0:F2})", threshold)))
    
type PlayRecord with
    member public this.ToReadableString(playRecords: PlayRecord seq) =
        let suggestHint = match
                            playRecords |> ProbeMinAcc this.songId this.difficulty with
                            | Some(suggest) -> String.Format("推分需要{0:F2}%", suggest)
                            | None -> "无法推分"
        String.Format("<{0} {1:F1}> {2} {3:F2}% {4:F2}： {5}",
                      this.difficulty, this.difficulty |> QueryChartRatingFromId this.songId,
                      this.songName, this.accuracy, this.singleRKS, suggestHint)
        
let FormatPlayRecord (playRecord: PlayRecord) list =
    list |> playRecord.ToReadableString