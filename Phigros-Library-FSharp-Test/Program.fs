// For more information see https://aka.ms/fsharp-console-apps
open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Text.RegularExpressions
open PhigrosAPIException
open PhigrosBestImageGenerator
open PhigrosAPIHandler
open GameSaveAnalyzer
open ScoreUtils
open AssetsHelper
open SkiaSharp
open YamlDotNet
open YamlDotNet.RepresentationModel

(*
try
    let songId = "Rrharil.TeamGrimoire.0"
    let diff = Difficulty.AT
    let playRecords = PhigrosUser("v6n7d2y37xidanbfow7ola7az").getPlayRecordList()
                        |> Async.RunSynchronously
    let result = playRecords
                 |> ProbeMinAcc songId diff
    let info = playRecords |> List.find (fun pr -> pr.songId = songId && pr.difficulty = diff)
    let resultStr =
        match result with
         | Some(acc) -> String.Format("{0} {1} ({2:F2}%): 推分需要{3:F2}%", info.songName, diff, info.accuracy, acc)
         | None -> String.Format("{0} {1} ({2:F2}%): 无法推分", info.songName, diff, info.accuracy)
    resultStr |> printfn "%s"
with
 | :? PhigrosAPIException as e -> printfn $"%s{e.Message}"
 *)
 
let mapListToStr (list: string list) =
    let strBuilder = new StringBuilder("[ ")
    list
     |> List.iter (fun str ->
         strBuilder.Append(str) |> ignore
         strBuilder.Append("; ") |> ignore)
    strBuilder.Append("]").ToString()
    
try
    (*
    let gameProgress = PhigrosUser("v6n7d2y37xidanbfow7ola7az").getGameProgressAsync()
                        |> Async.RunSynchronously
    let data, unit = gameProgress.money |> GetDataAmount
    printfn $"%f{data} %s{unit.ToString()}"*)
    // let playRecords =  PhigrosUser("pv6cbgy92eu81uvckbpfanynb").getPlayRecordList()
    //                    |> Async.RunSynchronously
    // printfn "Best19："
    // playRecords
    //  |> GetBestN 19
    //  |> List.iter (fun pr -> printfn $"%s{pr.ToReadableString(playRecords)}")
    // let data = PhigrosUser("v6n7d2y37xidanbfow7ola7az") // .getPlayRecordList()
    //              |> GenerateB19ImageAsync true "http://q.qlogo.cn/headimg_dl?dst_uin=1443937075&spec=640&img_type=png"
    //              |> Async.RunSynchronously
    // printfn $"%d{data.Length / 1024 / 1024}"
    // 0 |> ignore            
    // path |> printfn "%s"
    // let bitmap = File.ReadAllBytes(path) |> SKBitmap.Decode
    // use imageStream = new SKBas
    // bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100) |> ignore
    // PhigrosUser("v6n7d2y37xidanbfow7ola7az").getGameProgressAsync() |> Async.RunSynchronously |> ignore
    
    // 通过Alias查询SongId
    // let alias = "粪"
    // let songIds = alias
    //                |> QuerySongIdFromAlias
    // printfn $"%s{alias} -> %s{String.Join(' ', songIds)}"
    
    // let playRecords = PhigrosUser("v6n7d2y37xidanbfow7ola7az").getPlayRecordList()
    //                     |> Async.RunSynchronously
    // let pr = GetPlayRecordFromUserInput ["痉挛"; "hd"] playRecords 
    // printfn $"%s{pr.ToReadableString(playRecords)}"
    // _initAvatarDict()
    // 通过SongId反查别名
    // let songId = "もぺもぺ.LeaF.0"
    // let regex = Regex("(.*)\.(.*)\.0")
    // printfn $"SongId %s{songId} match regex? %s{regex.Match(songId).Success.ToString()}"
    // printfn $"Alias %s{alias} match regex? %s{regex.Match(alias).Success.ToString()}"
    // let aliases = songId
    //               |> ReverseQueryAliasFromSongId
    // printfn $"%s{songId} -> [ %s{String.Join(';', aliases)} ]"

    let userInfo = PhigrosUser("v6n7d2y37xidanbfow7ola7az").getUserInfoAsync()
                    |> Async.RunSynchronously
    printfn $"selfIntroduction = %s{userInfo.selfIntroduction}"
    printfn $"avatar = %s{userInfo.avatar}"
    printfn $"backGround = %s{userInfo.background}"
with
 | :? PhigrosAPIException as e -> printfn $"%s{e.Message}\n%s{e.Data0}\n%s{e.StackTrace}"