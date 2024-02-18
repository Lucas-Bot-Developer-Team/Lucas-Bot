(*
________                                             ______________ _____                                   ______________________                        
__  ___/___  _____________ ___________________       ___  __ \__  /____(_)______ _____________________      ___  ____/_  ___/__  /_______ _______________ 
_____ \_  / / /__  /_  __ `/_  __ \_  __ \  _ \________  /_/ /_  __ \_  /__  __ `/_  ___/  __ \_  ___/________  /_   _____ \__  __ \  __ `/_  ___/__  __ \
____/ // /_/ /__  /_/ /_/ /_  / / /  / / /  __//_____/  ____/_  / / /  / _  /_/ /_  /   / /_/ /(__  )_/_____/  __/   ____/ /_  / / / /_/ /_  /   __  /_/ /
/____/ \__,_/ _____/\__,_/ /_/ /_//_/ /_/\___/       /_/     /_/ /_//_/  _\__, / /_/    \____//____/        /_/      /____/ /_/ /_/\__,_/ /_/    _  .___/ 
                                                                         /____/                                                                  /_/
*)

module Phigros_Library_FSharp.GameSave
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