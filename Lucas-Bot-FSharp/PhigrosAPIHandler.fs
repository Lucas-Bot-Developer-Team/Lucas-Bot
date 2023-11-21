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

module PhigrosAPIHandler
open System
open Flurl.Http
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open PhigrosAPIException
open GameSave
open GameSaveUtil
open System.IO
open System.Threading.Tasks

let apiBaseUrl = "https://rak3ffdi.cloud.tds1.tapapis.cn/1.1"

type String with
    member this.IsNotEmpty() =
        match this.Length with
         | 0 -> false
         | _ -> true
     

let WrapFSharpAsync<'T> (asyncResult: Async<'T>): Task<'T> =
    asyncResult |> Async.StartAsTask

let _getInfoInternalAsync (apiBaseUrl: string) (sessionToken: string) (pathSegment: string) (key: string) =
    async {
            let! result = apiBaseUrl
                           .WithHeader("X-LC-Id", "rAK3FfdieFob2Nn8Am")
                           .WithHeader("X-LC-Key","Qr9AEqtuoSVS3zeD6iVbM4ZC0AtkJcQ89tywVyi0")
                           .WithHeader("User-Agent","LeanCloud-CSharp-SDK/1.0.3")
                           .WithHeader("Accept","application/json")
                           .WithHeader("X-LC-Session", sessionToken)
                           .AppendPathSegment(pathSegment)
                           .GetStringAsync()
                          |> Async.AwaitTask
            let json = JsonConvert.DeserializeObject<JToken>(result)
            printfn $"%s{result}"
            return json[key]
        }

type PhigrosUser(sessionToken: string) =
    // Player's Session Token. The unique identifier for each player in the server's database.
    let sessionToken = sessionToken
    member val saveFile = "" with get, set
              
    // Get user's nickname from server, cause the user's nickname is not stored locally.
    member this.getUserNameAsync() =
        async {
            try
                let! rawResult = "nickname"
                                 |> _getInfoInternalAsync apiBaseUrl sessionToken "users/me"
                let username = rawResult.ToString()
                return username
            with
             | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message))
                    return ""
       }
    
    member this.getGameSaveUrlAsync() =
       async {
            try
                let! result = "results"
                               |> _getInfoInternalAsync apiBaseUrl sessionToken "classes/_GameSave"
                let gameFileNode = result[0]["gameFile"]
                let url = gameFileNode["url"].ToString()
                return url
            with
             | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message))
                    return ""
       }
    member this.getSaveUpdateTimeAsync() =
        async {
            try
                let! result = "results"
                               |> _getInfoInternalAsync apiBaseUrl sessionToken "classes/_GameSave"
                let time = (result[0].["gameFile"]["updatedAt"]).ToString()
                return DateTime.Parse(time).AddHours(8)
            with
             | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message))
                    return DateTime()
       } 

    member this.getSaveZipAsync() =
        try
            async {
                match this.saveFile.IsNotEmpty() && File.Exists(this.saveFile) with
                 | true -> return this.saveFile
                 | _ ->
                     let! gameSaveUrl = this.getGameSaveUrlAsync()
                     use! rawZipStream = gameSaveUrl.GetStreamAsync() |> Async.AwaitTask
                     let receivedLength = rawZipStream.Length
                     let rawBytes = byte 0 |> Array.create (int receivedLength)
                     rawZipStream.Read(rawBytes, 0, int receivedLength) |> ignore
                     let tmpZipFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".zip")
                     use outputStream = File.OpenWrite(tmpZipFilePath)
                     outputStream.Write(rawBytes, 0, int receivedLength)
                     printfn $"Save zip path = %s{tmpZipFilePath}"
                     this.saveFile <- tmpZipFilePath
                     return tmpZipFilePath
            }
        with
         | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message))
         
    member this.getGameRecordAsync() =
        async {
            try
                let! saveZipPath = this.getSaveZipAsync()
                let gameRecord = saveZipPath
                                  |> DeserializeGameRecord
                return gameRecord
            with
            | :? PhigrosAPIException as e -> raise(e)
                                             return GameRecord(0, [])
            | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message)); return GameRecord(0, [])
        }
         
    member this.getGameProgressAsync() : Async<GameProgress> =
        async {
            try
                let! saveZipPath = this.getSaveZipAsync()
                let gameProgress = saveZipPath
                                    |> DeserializeGameProgress
                return gameProgress
            with
            | :? PhigrosAPIException as e -> raise(e); return GameProgress(0)
            | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message)); return GameProgress(0)
        }

    member this.getUserInfoAsync() : Async<UserInfo> =
        async {
            try
                let! saveZipPath = this.getSaveZipAsync()
                let userInfo = saveZipPath
                                |> DeserializeUserInfo
                return userInfo
            with
            | :? PhigrosAPIException as e -> raise(e); return UserInfo(0)
            | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message)); return UserInfo(0)
        }

    
    member this.getPlayRecordList() : Async<List<PlayRecord>> =
        async {
            try
                let! gameRecord = this.getGameRecordAsync()
                return gameRecord
                        |> ConstructPlayRecordList
            with
            | :? PhigrosAPIException as e -> raise(e); return []
            | e -> raise(PhigrosAPIException(e.GetType().ToString() + ": " + e.Message)); return []
        }