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

module AssetsHelper
open System.Collections.Generic
open System.IO
open System
open ScoreUtils
open PhigrosAPIException
open YamlDotNet.RepresentationModel
open System.Text.RegularExpressions

type ChartInformation() = 
    member val Level = 0 with get, set
    member val Difficulty = 0f with get, set
    member val Charter = "" with get, set


type SongInformation() =
    member val SongId = "" with get, set
    member val SongName = "" with get, set
    member val Illustration = "" with get, set
    member val Composer = "" with get, set
    member val Illustrator = "" with get, set
    member val Charts = Dictionary<Difficulty, ChartInformation>() with get, set

// Internal dictionary. Should not be directly used by other modules.
let _songIdNameDict = Dictionary<string, string>()
let _songInformationDict = Dictionary<string, SongInformation>()
let _difficultyDict = Dictionary<string, float32 array>()
let _avatarDict = Dictionary<string, string>()
let _aliasesDict = Dictionary<string, string list>() // Aliases Dictionary

let _GenericQueryFromDict<'T>
    initializer
    (dict: Dictionary<string, 'T>)
    (key: string) =
    try
        initializer()
        match [for dictKey in dict.Keys -> dictKey]
              |> List.exists (fun k -> k.ToLower().Equals(key.ToLower())) with
         | true ->
             let key = [for dictKey in dict.Keys -> dictKey]
                        |> List.find (fun (k: string) -> k.ToLower().Equals(key.ToLower()))
             Some(dict[key])
         | _ -> None
    with
     | e -> printfn $"%s{e.GetType().Name}\n%s{e.Message}\n%s{e.StackTrace}"; raise(PhigrosAPIException(e.GetType().Name + ": " + e.Message))

let _initDifficultyDict() =
    match _difficultyDict.Count with // Determines whether the internal dictionary is initialized or not.
     | x when x = 0 -> // Not initialized. Do necessary initialization.
        let diffFile = File.OpenText("assets/phigros-ingame-statistics/difficulty.csv")
        let diffList = diffFile.ReadToEnd().Split("\n")
        for diff in diffList do
            let diffInfo = diff.Split(",")
            let diffArr = [| for i in diffInfo[1..] -> i |> Single.Parse |]
            _difficultyDict.Add(diffInfo[0], diffArr)
     | _ -> ()

let QueryChartRatingFromId id diff =
    _initDifficultyDict()
    match _difficultyDict.ContainsKey(id) with
     | true -> _difficultyDict[id][int diff]
     | false -> raise(PhigrosAPIException(String.Format("Song Not Found. Please update API version. Internal ID: {0}", id)))
     
let _initSongInformation() = 
    match _songInformationDict.Count with
     | x when x = 0 -> // Initialize song information by extracted assets from Phigros apk/obb.
        let file = File.OpenText("assets/phigros-ingame-statistics/info.csv")
        let infos = file.ReadToEnd().Trim().Split("\n")
        for info: String in infos do
            let splitInfo = info.Trim().Split("\\")
            let information = SongInformation()
            information.SongId <- splitInfo[0]
            information.SongName <- splitInfo[1]
            information.Composer <- splitInfo[2]
            information.Illustration <- String.Format("assets/Illustration/{0}.png", splitInfo[0])
            information.Illustrator <- splitInfo[3]
            // Initialize each difficulty's rating.
            // Each song always contains EZ/HD/IN difficulty;
            // Meanwhile, some songs contain AT difficulty. 
            for i in [Difficulty.EZ; Difficulty.HD; Difficulty.IN; Difficulty.AT] do
                try
                    let chartInfo = ChartInformation()
                    chartInfo.Charter <- splitInfo[4 + int i]
                    chartInfo.Difficulty <- i |> QueryChartRatingFromId information.SongId
                    chartInfo.Level <- int (chartInfo.Difficulty |> MathF.Floor)
                    information.Charts.Add(i, chartInfo)
                with
                 // IndexOutOfRangeException: Current song doesn't have AT difficulty.
                 // Otherwise, it's an internal error and we should throw the exception to the upper layer. 
                 | :? IndexOutOfRangeException -> ()
                 | error -> raise(PhigrosAPIException(error.Message))
                    
            _songInformationDict.Add(information.SongId, information)
            _songIdNameDict.Add(information.SongId, information.SongName)
     | _ -> ()
     
let QuerySongInfoFromId songId = 
    songId
     |> _GenericQueryFromDict _initSongInformation _songInformationDict
     
let _initAvatarDict() =
    match _avatarDict.Count with
     | x when x = 0 ->
        let avatarInfoFile = File.OpenText("assets/phigros-ingame-statistics/avatar.csv")
        let avatarInfoList = avatarInfoFile.ReadToEnd().Trim().Split("\n")
        for avatar in avatarInfoList do
            let avatarInfo = avatar.Split(",")
            // printfn $"Key = {avatarInfo[1].Trim()} -> Value = assets/avatar/{avatarInfo[0].Trim()}.png"
            _avatarDict.Add(avatarInfo[0].Trim(), $"assets/avatar/{avatarInfo[0].Trim()}.png")
     | _ -> ()
     
let QueryScoreRankIcon isFC rank =
    match isFC with
     | false ->
         match rank with
          | Rank.False -> "assets/phigros-b19-assets/F15F.png"
          | Rank.C -> "assets/phigros-b19-assets/C15C.png"
          | Rank.B -> "assets/phigros-b19-assets/B15B.png"
          | Rank.A -> "assets/phigros-b19-assets/A15A.png"
          | Rank.S -> "assets/phigros-b19-assets/S15S.png"
          | _ -> "assets/phigros-b19-assets/V15V.png"
     | _ ->
         match rank with
          | Rank.Phi -> "assets/phigros-b19-assets/phi15phi.png"
          | _ -> "assets/phigros-b19-assets/V15FC.png"
          
let _initAliasesDict() =
    match _aliasesDict.Count with
     | x when x = 0 ->
         let aliasesFile = File.OpenText("assets/phigros-ingame-statistics/aliases.yaml")
         let yamlStream = YamlStream()
         yamlStream.Load(aliasesFile)
         let rootNode: YamlMappingNode = downcast yamlStream.Documents[0].RootNode
         [ for kvPair in rootNode.Children -> kvPair ]
            |> List.iter(fun kvPair ->
                            let valueNode: YamlSequenceNode = downcast kvPair.Value
                            _aliasesDict.Add(kvPair.Key.ToString(), [for value in valueNode.Children -> value.ToString()]))
     | _ -> ()
     
let QuerySongIdFromAlias alias =
    alias
     |> _GenericQueryFromDict _initAliasesDict _aliasesDict 

let QueryAvatarPathFromName (avatarName: String) = 
    Some($"assets/avatar/{avatarName.Trim()}.png")
     
let ReverseQueryAliasFromSongId songId =
    _initAliasesDict()
    let aliasesList = List<string>()
    [ for kvPair in _aliasesDict -> kvPair ]
     |> List.iter(fun kvPair ->
         match kvPair.Value |> List.contains songId with
          | true -> aliasesList.Add(kvPair.Key)
          | false -> ())
    [ for alias in aliasesList -> alias ]
    
let CheckDiffExists (songId: string) (diff: Difficulty) =
    let info = QuerySongInfoFromId songId
    match info.Value.Charts.ContainsKey(diff) with
     | false -> raise(PhigrosAPIException("指定的难度不存在。"))
     | true -> ()

type GetPlayRecordInputType =
 | SongName 
 | SongId
 | Alias
 
let (|IsSongId|_|) input =
    let result = Regex("(.*)\.(.*)\.0").Match(input)
    if result.Success
        then
            if _songInformationDict.ContainsKey(input) then Some(input) else None
        else None

let (|IsName|_|) (input: string) =
    let queryResult =  [ for kvPair in _songInformationDict -> kvPair ]
                        |> List.choose (fun info -> if info.Value.SongName.ToLower().Equals(input.ToLower()) then Some(info.Key) else None)
    if queryResult.IsEmpty
        then None
        else Some(queryResult)
        
let (|IsAlias|_|) input =
    QuerySongIdFromAlias input
    
    
let QuerySongInfoFromInput (input: string) =
    _initAliasesDict()
    _initSongInformation()
    _initDifficultyDict()
    match input with
     | IsSongId songId -> songId |> QuerySongInfoFromId |> Option.get
     | IsAlias songId | IsName songId ->
       match songId.Length with
          | 0 -> raise(PhigrosAPIException("未找到指定歌曲。请检查您的输入。"))
          | 1 ->
              let thisSongId = songId.Head
              thisSongId |> QuerySongInfoFromId |> Option.get
          | _ ->
              let hint = "[ " + String.Join("; ", songId) + " ]"
              raise(PhigrosAPIException(String.Format("根据给定的名称或别名找到大于一首曲目。请重新查询。\n可能的曲目有：{0}", hint)))
     | _ -> raise(PhigrosAPIException("未找到指定歌曲。请检查您的输入。"))