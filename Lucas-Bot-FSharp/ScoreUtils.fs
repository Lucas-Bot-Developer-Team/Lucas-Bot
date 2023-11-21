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

module ScoreUtils
open System

///
/// <summary> 什么年代了，还在抽传统香烟 </summary>
/// 
type Float32CompareOption =
 | Equal = 0
 | NotEqual = 1
 | GreaterThan = 2
 | LesserThan = 3
 
type Single with
  ///
  /// <summary> Compare float32 with accurate equality judgement. </summary>
  /// <param name="y"> The right hand side of comparing. </param>
  /// <param name="option"> Float32 compare option. </param>
  /// <returns> The compare result. </returns>
  /// 
  member x.Compare(y: float32, option: Float32CompareOption) =
   match option with
    | Float32CompareOption.Equal -> MathF.Abs(x - y) < 1e-3f
    | Float32CompareOption.NotEqual-> MathF.Abs(x - y) > 1e-3f
    | Float32CompareOption.GreaterThan -> x > y || MathF.Abs(x - y) < 1e-3f
    | Float32CompareOption.LesserThan -> x < y || MathF.Abs(x - y) < 1e-3f
    | _ -> raise(NotImplementedException())
    
// 枚举类型定义，方便处理

type Difficulty = 
 | EZ = 0
 | HD = 1
 | IN = 2
 | AT = 3

type Rank = 
 | False
 | C
 | B 
 | A
 | S
 | V
 | Phi
 
type ChallengeModeRanking =
 | Gray = 0
 | Green = 1
 | Blue = 2
 | Red = 3
 | Gold = 4
 | Rainbow = 5
 
type DataAmountUnit =
 | KB = 0
 | MB = 1
 | GB = 2
 | TB = 3
 | PB = 4

let GetDifficulty x = 
    match x with
     | 0 -> Difficulty.EZ
     | 1 -> Difficulty.HD
     | 2 -> Difficulty.IN
     | _ -> Difficulty.AT
     
let GetChallengeModeRanking x =
    match x with
     | 0 -> ChallengeModeRanking.Gray
     | 1 -> ChallengeModeRanking.Green
     | 2 -> ChallengeModeRanking.Blue
     | 3 -> ChallengeModeRanking.Red
     | 4 -> ChallengeModeRanking.Gold
     | _ -> ChallengeModeRanking.Rainbow
     
     

let GetScoreRank x = 
    match x with
     | a when a <= 699999 -> Rank.False
     | a when a >= 700000 && a <= 819999 -> Rank.C
     | a when a >= 820000 && a <= 879999 -> Rank.B
     | a when a >= 880000 && a <= 919999 -> Rank.A
     | a when a >= 920000 && a <= 959999 -> Rank.S
     | a when a >= 960000 && a <= 999999 -> Rank.V
     | _ -> Rank.Phi

let GetBit x index = (x >>> index &&& 1) = 1 

let rec _GetDifficultyInfoInternal loopCount x =
    match loopCount with
     | x when x < 0 -> []
     | newLoopCount ->
        let isCleared = loopCount
                         |> GetBit x
        let info = match isCleared with
                    | true -> [loopCount |> GetDifficulty]
                    | _ -> []
        x
         |> _GetDifficultyInfoInternal (newLoopCount - 1)
         |> List.append info
         |> List.sort

let GetDifficultyInfo x = x |> _GetDifficultyInfoInternal 3

let GetSingleRKS (acc: float32) (rating: float32) =
    // Calculate single song's Ranking Score.
    // The rules are as follows, and it doesn't need to explain.
    match acc with
     | x when x.Compare(100f, Float32CompareOption.Equal) -> rating 
     | x when x < 70.0f -> 0f
     | _ -> MathF.Pow(((acc - 55.0f) / 45.0f), 2.0f) * rating
     
let GetAccFromSingleRKS (songRating: float32) (singleRKS: float32)  =
    let threshold = songRating / 9.0f
    match singleRKS with
     | x when x.Compare(threshold, Float32CompareOption.GreaterThan) ->
         let result = MathF.Sqrt(singleRKS / songRating) * 45f + 55f
         if result > 100f then None else Some(result)
     | _ -> None
     
let GetChallengeModeInfo (info: int16) =
    let grade = (int info) / 100
                |> GetChallengeModeRanking
    let rank = (int info) % 100
    grade, rank
    
let rec _GetDataAmountInternal (result: float32) (data: int16 array) =
   match data |> Array.tryFind (fun x -> not (x = int16 0)) with
    | None -> (result, 4 - data.Length |> enum<DataAmountUnit>)
    | Some _ -> _GetDataAmountInternal (float32 result / 1024f + float32 data[0]) data[1..]
    
let GetDataAmount data = data
                          |> _GetDataAmountInternal 0f