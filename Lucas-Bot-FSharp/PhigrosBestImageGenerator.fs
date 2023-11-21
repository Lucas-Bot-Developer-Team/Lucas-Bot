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

module PhigrosBestImageGenerator
open System
open SkiaSharp
open System.IO
open Flurl.Http

open PhigrosAPIHandler
open PhigrosAPIException
open GameSave
open GameSaveAnalyzer
open ScoreUtils
open AssetsHelper

let _specialFontSong =
    Map[("Poseidon.1112vsStar.0", SKTypeface.FromFile("assets/phigros-b19-assets/NotoSans-Regular.ttf"))]

let GetDifficultyColor (difficulty: Difficulty) =
    match difficulty with
     | Difficulty.EZ -> SKColor(byte 8, byte 177, byte 51)
     | Difficulty.HD -> SKColor(byte 6, byte 117, byte 182)
     | Difficulty.IN -> SKColor(byte 206, byte 19, byte 19)
     | _ -> SKColor(byte 55, byte 55, byte 55)

let GetBestInfoArea count : SKRect =
    let row = float32 (count % 5)
    let col = float32 (count / 5)
    SKRect((80f + row * 383.2f), (300f + col * 220f),
                  (80f + row * 383.2f + 373f), (300f + col * 220f + 210f))
    
let GetRainbowShader width =
    let colors = SKColor() |> Array.create 8;
    for i in 0..7 do
        colors[i] <- SKColor.FromHsl(float32 i * 360f / 7f, 100f, 50f);
    SKShader.CreateLinearGradient(SKPoint(0f, 0f), SKPoint(width, 0f),
                                    colors, null, SKShaderTileMode.Repeat);


let rec GetTextWithRestrictedWidth (paint: SKPaint) (width: float32) (text: string) =
    let length = paint.MeasureText(text)
    match length with
     | x when x <= width || text.Length <= 3 -> text
     | _ ->
         match text[text.Length - 3 ..] with
          | "..." -> text[..text.Length - 5] + "..."
                      |> GetTextWithRestrictedWidth paint width
          | _ -> text[..text.Length - 2] + "..."
                  |> GetTextWithRestrictedWidth paint width
                  
let DrawPlayRecordByCount
    (count: int)
    (canvas: SKCanvas)
    (playRecord: PlayRecord) =
    // First, get the destination rectangle.
    let rectangle = count |> GetBestInfoArea
    match playRecord.songId |> QuerySongInfoFromId with
     | Some(songInfo) ->
         use paint = new SKPaint()
         // Load the illustration from file...
         // ... And then draw it.
         use illustrationStream = songInfo.Illustration
                                  |> File.OpenRead
         use illustrationBitmap = SKBitmap.Decode(illustrationStream)
                                   .Resize(SKImageInfo(190, 100), SKFilterQuality.High)
         canvas.DrawBitmap(illustrationBitmap, SKPoint(rectangle.Left + 10f, rectangle.Bottom - 110f), paint)
         
         // Then get the color corresponding to the difficulty.
         let difficultyColor = playRecord.difficulty
                                |> GetDifficultyColor
         paint.Color <- difficultyColor
         canvas.DrawRect(rectangle.Left + 10f, rectangle.Top + 10f, 125f, 25f, paint)
         
         // Draw the rating box.
         let rating = songInfo.Charts[playRecord.difficulty].Difficulty
         paint.IsAntialias <- true
         paint.FilterQuality <- SKFilterQuality.High
         paint.FakeBoldText <- true
         paint.TextSize <- 20f
         paint.Typeface <- SKTypeface.FromFile("assets/phigros-b19-assets/Main.ttf")
         paint.Color <- SKColors.White
         paint.TextAlign <- SKTextAlign.Center
         canvas.DrawText(String.Format("{0} Lv. {1:F1}", playRecord.difficulty, rating), rectangle.Left + 70f, rectangle.Top + 29f, paint)
         // Draw the player's single RKS.
         paint.TextAlign <- SKTextAlign.Left
         canvas.DrawText(String.Format("→   {0:F2}", playRecord.singleRKS), rectangle.Left + 145f, rectangle.Top + 29f, paint)
         // Draw the song's count.
         paint.TextAlign <- SKTextAlign.Right
         canvas.DrawText(String.Format("#{0}", count), rectangle.Right - 10f, rectangle.Top + 27.5f, paint)
         
         // Draw the song's name.
         paint.FakeBoldText <- false
         paint.TextSize <- 40f
         paint.TextAlign <- SKTextAlign.Left
         match _specialFontSong
               |> Map.containsKey playRecord.songId with
           | false ->
               canvas.DrawText(songInfo.SongName |> GetTextWithRestrictedWidth paint 353f, rectangle.Left + 10f, rectangle.Top + 80f, paint)
           | _ ->
               let origTypeFace = paint.Typeface
               paint.Typeface <- _specialFontSong[playRecord.songId]
               canvas.DrawText(songInfo.SongName |> GetTextWithRestrictedWidth paint 353f, rectangle.Left + 10f, rectangle.Top + 80f, paint)
               paint.Typeface <- origTypeFace
         
         // Draw the score.
         paint.TextSize <- 30f
         canvas.DrawText(playRecord.score.ToString("D7"), rectangle.Left + 220f, rectangle.Top + 117.5f, paint)
         
         paint.StrokeWidth <- 3f
         canvas.DrawLine(SKPoint(rectangle.Left + 210f, rectangle.Top + 135f),
                         SKPoint(rectangle.Left + 365f, rectangle.Top + 135f), paint)
         // Get score icon.
         let scoreIconPath = playRecord.scoreRank |> QueryScoreRankIcon playRecord.isFullCombo
         use scoreIconStream = scoreIconPath
                                |> File.OpenRead
         use scoreIconBitmap = SKBitmap.Decode(scoreIconStream)
                                .Resize(SKImageInfo(50, 50), SKFilterQuality.High)
         canvas.DrawBitmap(scoreIconBitmap, SKPoint(rectangle.Left + 205f, rectangle.Bottom - 65f), paint)
         
         // Draw Accuracy.
         paint.FakeBoldText <- true
         paint.TextSize <- 24f
         canvas.DrawText((playRecord.accuracy / 100f).ToString("P2"), rectangle.Left + 260f, rectangle.Bottom - 30f, paint)
     | None -> raise(PhigrosAPIException(String.Format("Song Not Found. Please update API version. Internal ID: {0}", id))) 
    // use illustrationStream = File.OpenRead(playRecord.)
    ()

let GenerateB19ImageAsync
    (ignoreCheck: bool)
    (avatarUri: string)
    (phigrosUser: PhigrosUser) =
    async {
        let! playRecords = phigrosUser.getPlayRecordList()
        playRecords |> GetRKS |> _checkRksThreshold // Check RKS Threshold
        
        let! gameProgress = phigrosUser.getGameProgressAsync()
        // Local data source. For debugging use only.
        // let playRecords = @"C:\Users\Lucas\IdeaProjects\JvavPlayground\save1.zip"
        //                   |> DeserializeGameRecord
        //                   |> ConstructPlayRecordList
        
        // Sample user name. For debug use only.
        // let username = "XsTmRnGpQlKwZfHvYbUeJiOaCdLxNz"
        // let gameProgress = @"C:\Users\Lucas\IdeaProjects\JvavPlayground\save1.zip"
        //                     |> DeserializeGameProgress
        
        // 下载URL中头像
        let! avatarData = 
            match avatarUri.Contains("http") with
             | true -> avatarUri.GetBytesAsync() |> Async.AwaitTask
             | _ -> File.ReadAllBytesAsync(avatarUri) |> Async.AwaitTask
        let avatar = SKBitmap.Decode(avatarData).Resize(SKImageInfo(174, 174), SKFilterQuality.High)
        let mask = SKBitmap.Decode(File.ReadAllBytes("assets/phigros-b19-assets/mask.png")).Resize(SKImageInfo(174, 174), SKFilterQuality.High)
        
        // Check challenge mode. If probability of cheating is detected, it will throw an exception.
        // If ignoreCheck is set, the function will not check challenge mode.
        gameProgress.challengeModeRank |> _checkChallengeModeCheat ignoreCheck
        let rks =  playRecords |> GetRKS
        let filteredB19WithOverflow = playRecords
                                        |> GetBestN 24
        let bestPhi = playRecords
                       |> GetBestPhi
        let! username = phigrosUser.getUserNameAsync()
        
        use image = new SKBitmap(2048, 1440)
        use canvas = new SKCanvas(image)
        canvas.Clear(SKColorF(0f, 0f, 0f, 0f))
        use paint = new SKPaint()
        paint.IsAntialias <- true
        paint.FilterQuality <- SKFilterQuality.High

        // 绘制背景
        use backGroundStream = File.OpenRead("assets/phigros-b19-assets/Background.png")
        use backGroundBitmap = SKBitmap.Decode(backGroundStream) // .Resize(SKImageInfo(2730, 1440), SKFilterQuality.High)

        // 背景高斯模糊+加灰处理
        // paint.ImageFilter <- SKImageFilter.CreateBlur(10f, 10f)
        // canvas.DrawBitmap(backGroundBitmap, SKPoint(-341f, 0f), paint)
        canvas.DrawBitmap(backGroundBitmap, SKPoint(0f, 0f), paint)
        paint.ImageFilter <- null
        paint.Color <- SKColors.Gray.WithAlpha(byte 127)
        // canvas.DrawRect(SKRect(0f, 0f, 2048f, 1440f), paint)
        paint.Color <- SKColors.White
        
        paint.IsStroke <- false
        paint.Color <- SKColors.White
        paint.TextSize <- 30f
        
        // New UI （先在这儿放一句注释，什么时候写完又是个问题）
        //                            ↑ 不过看起来是写完了
        
        paint.IsStroke <- false
        paint.Typeface <- SKTypeface.FromFile("assets/phigros-b19-assets/Title.ttf")
        paint.TextSize <- 25f
        
        // 课题背景标
        let challengeRank, challengeScore = gameProgress.challengeModeRank |> GetChallengeModeInfo
        let challengeModeBackgroundPath = match challengeRank with
                                           | ChallengeModeRanking.Blue -> "assets/phigros-b19-assets/blue.png"
                                           | ChallengeModeRanking.Green -> "assets/phigros-b19-assets/green.png"
                                           | ChallengeModeRanking.Red -> "assets/phigros-b19-assets/red.png"
                                           | ChallengeModeRanking.Gold -> "assets/phigros-b19-assets/gold.png"
                                           | ChallengeModeRanking.Rainbow -> "assets/phigros-b19-assets/rainbow.png"
                                           | _ -> "assets/phigros-b19-assets/gray.png"
        use chalModeStream = File.OpenRead(challengeModeBackgroundPath)
        use chalModeBitmap = SKBitmap.Decode(chalModeStream) // .Resize(SKImageInfo(2730, 1440), SKFilterQuality.High)
        canvas.DrawBitmap(chalModeBitmap, SKPoint(1613f, 39f), paint)
        canvas.DrawText(challengeScore.ToString(), SKPoint(1713f, 110f), paint) // ChalMode
        paint.TextAlign <- SKTextAlign.Right
        canvas.DrawText(String.Format("{0:F3}", rks), SKPoint(1934f, 110f), paint) // RKS
        paint.TextAlign <- SKTextAlign.Left
        
        // 用户名
        paint.TextSize <- 30f
        let truncatedUserName = String.Format("{0}", username)
                                |> GetTextWithRestrictedWidth paint 235f
        paint.TextAlign <- SKTextAlign.Center
        canvas.DrawText(truncatedUserName, SKPoint(1818f, 148f), paint)
        paint.TextAlign <- SKTextAlign.Left
        
        // Data数量
        let dataSize, dataAmount = gameProgress.money |> GetDataAmount
        paint.TextSize <- 25f
        canvas.DrawText("DATA", SKPoint(1698f, 188f), paint)
        paint.TextAlign <- SKTextAlign.Right
        canvas.DrawText(String.Format("{0:F2} {1}", dataSize, dataAmount.ToString()), SKPoint(1924f, 186f), paint)
        paint.TextAlign <- SKTextAlign.Left
       
        
        // let playTime = DateTime(2023, 9, 1, 12, 34, 56)
        let! playTime = phigrosUser.getSaveUpdateTimeAsync()
        paint.TextSize <- 30f
        canvas.DrawText(playTime.ToString("yyyy-MM-dd"), SKPoint(1505f, 236f), paint)
        paint.TextAlign <- SKTextAlign.Right
        canvas.DrawText(playTime.ToString("HH:mm:ss"), SKPoint(1927f, 236f), paint)
        paint.TextAlign <- SKTextAlign.Left
        
        use blendBitmap = new SKBitmap(174, 174)
        use blendCanvas = new SKCanvas(blendBitmap)
        blendCanvas.Clear()
        use blendPaint = new SKPaint()
        // canvas.DrawBitmap(avatar, SKPoint(1509f, 51f), blendPaint)
        blendCanvas.DrawBitmap(avatar, SKPoint(0f, 0f), blendPaint)
        blendPaint.BlendMode <- SKBlendMode.DstIn
        blendCanvas.DrawBitmap(mask, SKPoint(0f, 0f), blendPaint)
        canvas.DrawBitmap(blendBitmap, SKPoint(1512f, 52f), paint)
        
        paint.Typeface <- SKTypeface.FromFile("assets/phigros-b19-assets/Main.ttf")
        
        // If the play record has a "Phi" record, then place it in #0.
        // Otherwise, draw a hint.
        match bestPhi with
          | Some(phiRecord) ->
              phiRecord
               |> DrawPlayRecordByCount 0 canvas
          | _ ->
             paint.TextAlign <- SKTextAlign.Center
             paint.TextSize <- 30f
             canvas.DrawText("您当前没有AP过歌曲。", 260f, 360f, paint)
             canvas.DrawText("AP任意歌曲", 260f, 420f, paint)
             canvas.DrawText("可以大幅提升RKS！", 260f, 480f, paint)
             
        filteredB19WithOverflow
         |> Seq.iteri (fun count playRecord ->
             playRecord |> _checkSpeedChangeCheat ignoreCheck
             playRecord |> DrawPlayRecordByCount (count + 1) canvas)
        
        // 输出到本地图像
        // let path = Path.ChangeExtension(Path.GetTempFileName(), ".png")
        // use imageStream = new SKFileWStream(path)
        // image.Encode(imageStream, SKEncodedImageFormat.Png, 100) |> ignore
        use imageStream = new MemoryStream()
        image.Encode(imageStream, SKEncodedImageFormat.Png, 50) |> ignore
        return imageStream.GetBuffer()
        
        // return path 
    }