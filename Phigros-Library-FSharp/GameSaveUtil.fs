(*
________                                             ______________ _____                                   ______________________                        
__  ___/___  _____________ ___________________       ___  __ \__  /____(_)______ _____________________      ___  ____/_  ___/__  /_______ _______________ 
_____ \_  / / /__  /_  __ `/_  __ \_  __ \  _ \________  /_/ /_  __ \_  /__  __ `/_  ___/  __ \_  ___/________  /_   _____ \__  __ \  __ `/_  ___/__  __ \
____/ // /_/ /__  /_/ /_/ /_  / / /  / / /  __//_____/  ____/_  / / /  / _  /_/ /_  /   / /_/ /(__  )_/_____/  __/   ____/ /_  / / / /_/ /_  /   __  /_/ /
/____/ \__,_/ _____/\__,_/ /_/ /_//_/ /_/\___/       /_/     /_/ /_//_/  _\__, / /_/    \____//____/        /_/      /____/ /_/ /_/\__,_/ /_/    _  .___/ 
                                                                         /____/                                                                  /_/
*)

module Phigros_Library_FSharp.GameSaveUtil
open GameSave
open ScoreUtils
open AssetsHelper
open PhigrosAPIException
open Org.BouncyCastle.Crypto.Parameters
open Org.BouncyCastle.Security
open System
open System.IO
open System.Text
open SharpCompress.Archives

let gameRecordVersion = 1
let gameProgressVersion = 3
let userInfoVersion = 1

let _ConstructIntFromLEB128 (data: byte array) = 
    let firstByte = (int data[0]) &&& 255
    match 7 |> GetBit firstByte with
     | false -> data[1..], int (firstByte &&& 127), 1
     | true ->
              let secondByte = (int data[1]) &&& 127
              let value = (secondByte <<< 7) ||| (firstByte &&& 127)
              data[2..], value, 2

let _checkPlayRecordInternal (playRecord: PlayRecord) =
    match playRecord.scoreRank with
     | Phi ->
         match playRecord.isFullCombo with
          | true -> ()
          | false -> raise(PhigrosAPIException("您的存档有作弊行为。"))
     | _ -> ()

let _ConstructStringInternal (data: byte array) =
    // let length = int data[0]
    let rest, length, firstByteLength = data |> _ConstructIntFromLEB128
    // printfn $"Constructed str = {rest[..length] |> Encoding.UTF8.GetString}; length = {length}"
    rest[..length - 1] |> Encoding.UTF8.GetString, length, firstByteLength
    // if data[0] >= (byte 128)
    //  then (data[2..length + 1] |> Encoding.UTF8.GetString, length + 1)
    //  else (data[1..length] |> Encoding.UTF8.GetString, length)
    
let rec _ConstructShortArrayFromLEB128Internal (result: int16 array) count consumption (data: byte array) =
    match count with
     | x when x <= 0 -> result, consumption
     | _ ->
         let firstByte = (int data[0]) &&& 255
         match 7 |> GetBit firstByte with 
          | false -> data[1..]
                     |> _ConstructShortArrayFromLEB128Internal
                            ([| int16 (firstByte &&& 127) |] |> Array.append result)
                            (count - 1)
                            (consumption + 1)
          | true ->
              let secondByte = (int data[1]) &&& 127
              let value = (secondByte <<< 7) ||| (firstByte &&& 127)
              data[2..]
                |> _ConstructShortArrayFromLEB128Internal
                    ([| int16 value |] |> Array.append result)
                    (count - 1)
                    (consumption + 2)

let ConstructShortArrayFromLEB128 count (data: byte array) =
    data 
    |> _ConstructShortArrayFromLEB128Internal [|  |] count 0


let ValidatesBuffer version (buffer: byte array) =
    match int buffer[0] with
      | x when x = version -> buffer[1..]
      | otherVersion ->
         printfn $"Version Not match! Expect %d{version} but get %d{otherVersion}" 
         match otherVersion with
          | x when x < version -> 
            raise(PhigrosAPIException("Phigros version too low. Please update your Phigros version."))
          | _ -> 
            raise(PhigrosAPIException("Phigros API version too low. Please contact maintainer."))

let ReadAllBytesFromFile version path = 
    let buffer = File.ReadAllBytes(path)
    buffer |> ValidatesBuffer version
    
let rec _ReadAllBytesFromZipEntryInternal name (entries: List<IArchiveEntry>) : byte array =
    match entries with
     | [] -> raise(PhigrosAPIException("Saving is corrupted. Please re-upload saving or contact maintainer."))
     | curr::rest ->
         match curr.Key with
          | x when x = name ->
              use stream = curr.OpenEntryStream()
              let buffer = byte 0 |> Array.create (int curr.Size)
              stream.Read(buffer, 0, buffer.Length) |> ignore
              buffer
          | _ ->
              rest |> _ReadAllBytesFromZipEntryInternal name
            
let ReadAllBytesFromZipEntry version name (path: string) =
    let archive = ArchiveFactory.Open(path)
    // use zipArchive = ZipFile.OpenRead(path)
    let zipEntryList = [ for entry in archive.Entries -> entry ]
    zipEntryList
    |> _ReadAllBytesFromZipEntryInternal name
    |> ValidatesBuffer version

let DecryptSavingRaw (data: byte array) =
    // Decrypt the raw saving. Saving's encryption is AES/CBC/PKCS5Padding.
    // Because. NET does NOT implement the AES/CBC/PKCS5Padding algorithm, BouncyCastle library is used instead.
    
    // Init cipher, key and iv.
    let key = Convert.FromBase64String("6Jaa0qVAJZuXkZCLiOa/Ax5tIZVu+taKUN1V1nqwkks=")
    let iv = Convert.FromBase64String("Kk/wisgNYwcAV8WVGMgyUw==")
    let cipherParameters = ParametersWithIV(KeyParameter(key), iv)
    let cipher = CipherUtilities.GetCipher("AES/CBC/PKCS5Padding")
    cipher.Init(false, cipherParameters)

    // Decrypt the data using given parameters.
    let decryptedData = byte 0
                        |> Array.create (cipher.GetOutputSize(data.Length))
    try
        let processed = cipher.ProcessBytes(data, 0, data.Length, decryptedData, 0)
        cipher.DoFinal(decryptedData, processed) |> ignore
        decryptedData
    with
        // If an exception is generated, wrap the exception message as PhigrosAPIException...
        // ...and throw it to the upper layer for processing.
        | e -> raise(PhigrosAPIException(e.GetType().Name.ToString() + ": " + e.Message))

let ConstructScoreRecord isFC (data: byte array) =
    // Small function which is used to construct a score record.
    // Reduce code duplication.
    let score = data[0..3] 
                |> BitConverter.ToInt32
    let accuracy = data[4..7]
                   |> BitConverter.ToSingle
    ScoreRecord(accuracy, score, isFC)

let rec _ConstructSongRecordInternal 
    (data: byte array) 
    (fcList: List<Difficulty>)
    (clearList: List<Difficulty>)
    (songRecord: Map<Difficulty, ScoreRecord>) =
    match clearList with
     | [] -> songRecord
     | diff::rest ->
        match fcList 
              |> List.tryFind (fun x -> x = diff) with
         | Some _ -> (diff, data[0..7] |> ConstructScoreRecord true)
                      |> songRecord.Add 
         | None -> (diff, data[0..7] |> ConstructScoreRecord false)
                    |> songRecord.Add 
        |> _ConstructSongRecordInternal data[8..] fcList rest

let ConstructSongRecord (data: byte array) = 
    let songId, idLength, _ = data |> _ConstructStringInternal
    let songInfoRawLength = int data[idLength + 1]
    let clearInfo = int data[idLength + 2] |> GetDifficultyInfo
    let fcInfo = int data[idLength + 3] |> GetDifficultyInfo
    let songInfoRaw = data[idLength + 4 .. idLength + songInfoRawLength + 1]
    let songRecordMap = Map.empty
                        |> _ConstructSongRecordInternal songInfoRaw fcInfo clearInfo 
    (SongRecord(songId, songRecordMap), idLength + songInfoRawLength + 2)
    
let rec _ConstructGameRecordInternal gameRecordLength (gameRecord: List<SongRecord>) (data: byte array) = 
    match gameRecordLength with
     | x when x = 0 -> gameRecord
     | _ ->
        let record, consumption = data |> ConstructSongRecord
        data[consumption..]
        |> _ConstructGameRecordInternal (gameRecordLength - 1) (gameRecord @ [record])

let ConstructGameRecord (data: byte array) =
    let restData, length, consumption = data |> _ConstructIntFromLEB128
    GameRecord(gameRecordVersion, restData
                                   |> _ConstructGameRecordInternal length List.empty)
    
    (*let gameRecordLength = int data[0]
    match gameRecordLength >= 128 with
     | true -> GameRecord(gameRecordVersion, data[2..]
                                      |> _ConstructGameRecordInternal gameRecordLength List.empty)
     | _ -> GameRecord(gameRecordVersion, data[1..]
                                      |> _ConstructGameRecordInternal gameRecordLength List.empty)*)
let DeserializeGameRecord path =
    try   
        path
         |> ReadAllBytesFromZipEntry gameRecordVersion "gameRecord"
         |> DecryptSavingRaw
         |> ConstructGameRecord
    with
     | :? PhigrosAPIException as e -> raise(e)
     | e ->
         printfn $"%s{e.ToString()}"
         raise(PhigrosAPIException("Saving is corrupted. Please re-upload saving or contact maintainer."))

let rec _ConstructPlayRecordListInternal (listPlayRecord: List<PlayRecord>) (listSongRecord: List<SongRecord>) = 
    match listSongRecord with
     | [] -> listPlayRecord
     | curr::rest ->
        let mutable newPlayRecordList = []
        for info: Difficulty in curr.scoreInfos.Keys do
            match curr.songId with 
            | "Introduction" -> ()
            | _ -> let newPlayRecord = PlayRecord(match curr.songId |> QuerySongInfoFromId with
                                                                     | Some(x) -> curr.songId, x.SongName, info, curr.scoreInfos[info].score, curr.scoreInfos[info].isFullCombo, curr.scoreInfos[info].accuracy
                                                                     | None -> raise (PhigrosAPIException(String.Format("Song Not Found. Please update API version. Internal ID: {0}", curr.songId))))
                   newPlayRecord |> _checkPlayRecordInternal
                   newPlayRecordList <- newPlayRecordList @ [newPlayRecord]
        rest
        |> _ConstructPlayRecordListInternal (listPlayRecord @ newPlayRecordList)

let ConstructPlayRecordList (gameRecord: GameRecord) =
    gameRecord.songRecords
    |> _ConstructPlayRecordListInternal []
    
let ConstructGameProgress ver (data: byte array) =
    let gameProgress = GameProgress(ver)
    let firstByte = int data[0]
    gameProgress.isFirstRun <- 0 |> GetBit firstByte
    gameProgress.isLegacyChapterFinished <- 1 |> GetBit firstByte
    gameProgress.alreadyShowCollectionTip <- 2 |> GetBit firstByte
    gameProgress.alreadyShowAutoUnlockINTip <- 3 |> GetBit firstByte
    let completed, completedLength, _ = data[1..] |> _ConstructStringInternal
    gameProgress.completed <- completed
    gameProgress.songUpdateInfo <- data[2 + completedLength]
    gameProgress.challengeModeRank <- data[3 + completedLength .. 4 + completedLength] |> BitConverter.ToInt16
    let money, moneyLength = data[5 + completedLength..] |> ConstructShortArrayFromLEB128 5
    gameProgress.money <- money
    let newLength = completedLength + moneyLength
    gameProgress.unlockFlagOfSpasmodic <- data[5 + newLength]
    gameProgress.unlockFlagOfIgallta <- data[6 + newLength]
    gameProgress.unlockFlagOfRrharil <- data[7 + newLength]
    gameProgress.flagOfSongRecordKey <- data[8 + newLength]
    gameProgress.randomVersionUnlocked <- data[9 + newLength]
    gameProgress.chapter8UnlockBegin <- 0 |> GetBit (int data[10 + newLength])
    gameProgress.chapter8UnlockSecondPhase <- 1 |> GetBit (int data[10 + newLength])
    gameProgress.chapter8Passed <- 2 |> GetBit (int data[10 + newLength])
    gameProgress.chapter8SongUnlocked <- data[11 + newLength]
    gameProgress
    
let DeserializeGameProgress (path: string) =
    try
        path
        |> ReadAllBytesFromZipEntry gameProgressVersion "gameProgress"
        |> DecryptSavingRaw
        |> ConstructGameProgress gameProgressVersion
    with
     | :? PhigrosAPIException as e -> raise(e)
     | _ -> raise(PhigrosAPIException("Saving is corrupted. Please re-upload saving or contact maintainer."))

let ConstructUserInfo ver (data: byte array) =
    let userInfo = UserInfo(ver)
    userInfo.displayUserId <- Int32.IsPositive(int data[0])
    let introduction, introLength, introByteLength = data[1..] |> _ConstructStringInternal
    userInfo.selfIntroduction <- introduction
    let avatar, avatarLength, _ = data[1 + introByteLength + introLength..] |> _ConstructStringInternal
    userInfo.avatar <- avatar
    let background, backgroundLength, _ = data[2 + introByteLength + introLength + avatarLength..] |> _ConstructStringInternal
    userInfo.background <- background
    userInfo

let DeserializeUserInfo (path: string) =
    try
        path
        |> ReadAllBytesFromZipEntry userInfoVersion "user"
        |> DecryptSavingRaw
        |> ConstructUserInfo userInfoVersion
    with
     | :? PhigrosAPIException as e -> raise(e)
     | e -> 
        printfn $"{e.Message}\n{e.StackTrace}"
        raise(PhigrosAPIException("Saving is corrupted. Please re-upload saving or contact maintainer."))