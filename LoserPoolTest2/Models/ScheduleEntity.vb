Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Imports WpfApplication1.LosersPool.Models
Imports WpfApplication1.JoinPools.Models

Namespace LosersPool.Models
    Public Class ScheduleEntity

        <Key>
        Public Property ScheduleId As Int32
        Public Property GameId As String
        Public Property TimePeriod As String
        Public Property HomeTeam As String
        Public Property AwayTeam As String
        Public Property StartTime As String
        Public Property StartDate As String
        Public Property HomeScore As String
        Public Property AwayScore As String
        Public Property GameTime As String
        Public Property GameDate As String
        Public Property DisplayStatus1 As String
        Public Property DisplayStatus2 As String
        Public Property Status As String
        Public Property Sport As String
        Public Property CronJob As String
        Public Property PoolAlias As String
        Public Property GameCode As String
        Public Property OriginalStartDate As String
        Public Property OriginalStartTime As String
        Public Property StartDateTime As Date
        Public Property RescheduledGame As Boolean
        Public Property MultipleGamesAreScheduled As Boolean
        Public Property MultipleGameNumber As String
        Public Property WinningTeam As String
        Public Property IsHomeTeamWinning As Boolean
        Public Property AreTeamsTied As Boolean
        Public Property seasonPhase As String

    End Class

    Public Class DeletedGame
        <Key>
        Public Property ScheduleId As Int32
        Public Property GameId As String
        Public Property TimePeriod As String
        Public Property HomeTeam As String
        Public Property AwayTeam As String
        Public Property StartTime As String
        Public Property StartDate As String
        Public Property HomeScore As String
        Public Property AwayScore As String
        Public Property GameTime As String
        Public Property GameDate As String
        Public Property DisplayStatus1 As String
        Public Property DisplayStatus2 As String
        Public Property Status As String
        Public Property Sport As String
        Public Property CronJob As String
        Public Property PoolAlias As String
        Public Property GameCode As String
        Public Property OriginalStartDate As String
        Public Property OriginalStartTime As String
        Public Property StartDateTime As Date
        Public Property RescheduledGame As Boolean
        Public Property MultipleGamesAreScheduled As Boolean
        Public Property MultipleGameNumber As String
        Public Property WinningTeam As String
        Public Property IsHomeTeamWinning As Boolean
        Public Property AreTeamsTied As Boolean
        Public Property seasonPhase As String

    End Class

    Public Class TimePeriodSchedule

        Public Property GameId As String
        Public Property TimePeriod As String
        Public Property HomeTeam As String
        Public Property AwayTeam As String
        Public Property HomeTeamImage As String
        Public Property AwayTeamImage As String
        Public Property HomeTeamIcon As String
        Public Property AwayTeamIcon As String
        Public Property StartTime As String
        Public Property StartDate As String
        Public Property HomeScore As String
        Public Property AwayScore As String
        Public Property GameTime As String
        Public Property GameDate As String
        Public Property DisplayStatus1 As String
        Public Property DisplayStatus2 As String
        Public Property Status As String
        Public Property Sport As String
        Public Property PoolAlias As String
        Public Property CronJob As String
        Public Property GameCode As String
        Public Property OriginalStartDate As String
        Public Property OriginalStartTime As String

    End Class


    Public Class ReadScheduleFile

        Public Sub New(filecontrol As String, rootFolder As String, sport As String)

            Dim _dbLoserPool As New LosersPoolContext

            Try
                Dim querySchedule = (From schedule1 In _dbLoserPool.ScheduleEntities
                                     Where schedule1.Sport = sport).ToList

                If querySchedule.Count > 0 Then
                    For Each game1 In querySchedule
                        _dbLoserPool.ScheduleEntities.Remove(game1)
                    Next
                End If
                _dbLoserPool.SaveChanges()

            Catch ex As Exception

            End Try

            System.IO.Directory.SetCurrentDirectory(rootFolder)

            If filecontrol = "onefile" Then
            ElseIf filecontrol = "manyfiles" Then

                Dim schedulefiles = XDocument.Load(".\TestFiles\scheduleDataFileList" + sport + ".xml")


                Dim ScheduleFileList = (From file1 In schedulefiles.Descendants("schedulefiles").Descendants("file")
                                    Select New ScheduleFileXML With {.FilePath = file1.Elements("filepath").Value}).ToList

                For Each schedule1 In ScheduleFileList
                    Dim pathname = schedule1.FilePath
                    ReadScheduleXMLFileAndWriteToScheduleEntities(pathname)
                Next

            End If

        End Sub

        Private Shared Sub ReadScheduleXMLFileAndWriteToScheduleEntities(pathname As String)

            Dim _dbLoserPool = New LosersPoolContext
            Dim _dbPools As New PoolDbContext

            Try
                Using (_dbLoserPool)
                    Using (_dbPools)

                        Dim myschedule = XDocument.Load(pathname)

                        Dim queryScheduleFile = (From schedule1 In myschedule.Descendants("schedule").Descendants("TimeP")
                                                 Select New ScheduleEntity With {.TimePeriod = schedule1.Attribute("TimePeriod"),
                                                                                 .Sport = schedule1.Attribute("Sport")}).Single

                        Dim querySchedule = (From schedule1 In _dbLoserPool.ScheduleEntities
                                             Where schedule1.TimePeriod = queryScheduleFile.TimePeriod And schedule1.Sport = queryScheduleFile.Sport).ToList

                        If querySchedule.Count = 0 Then

                            Dim queryGame = (From gameElement In myschedule.Descendants("schedule").Descendants("TimeP").Descendants("game")
                            Select New ScheduleEntity With {.GameId = gameElement.Attribute("gameNumber"),
                                                            .StartDate = gameElement.Elements("startDate").Value,
                                                            .StartTime = gameElement.Elements("startTime").Value,
                                                            .HomeTeam = gameElement.Elements("homeTeam").Value,
                                                            .AwayTeam = gameElement.Elements("awayTeam").Value}).ToList

                            Dim sport = queryScheduleFile.Sport

                            Dim queryTeams = (From team1 In _dbPools.Teams
                                              Where team1.Sport = sport).ToList

                            For gameNum = 1 To queryGame.Count

                                Dim gameNumber = New ScheduleEntity

                                gameNumber.GameId = queryGame(gameNum - 1).GameId
                                gameNumber.TimePeriod = queryScheduleFile.TimePeriod
                                gameNumber.Sport = queryScheduleFile.Sport
                                gameNumber.StartDate = queryGame(gameNum - 1).StartDate
                                gameNumber.StartTime = queryGame(gameNum - 1).StartTime
                                gameNumber.HomeTeam = queryGame(gameNum - 1).HomeTeam
                                gameNumber.AwayTeam = queryGame(gameNum - 1).AwayTeam

                                _dbLoserPool.ScheduleEntities.Add(gameNumber)
                                _dbLoserPool.SaveChanges()

                            Next

                            Dim thisTimePeriod = queryScheduleFile.TimePeriod
                            Dim querySchedule1 = (From schedule1 In _dbLoserPool.ScheduleEntities
                                                 Where schedule1.TimePeriod = thisTimePeriod And schedule1.Sport = sport
                                                 Select schedule1)

                            For Each team1 In queryTeams

                                Dim byeTeam1 = (From schedule1 In querySchedule1
                                                  Where schedule1.HomeTeam = team1.TeamName Or schedule1.AwayTeam = team1.TeamName).ToList

                                If byeTeam1.Count > 0 Then
                                Else
                                    Dim byeTeam2 As New ByeTeam
                                    byeTeam2.TimePeriod = thisTimePeriod
                                    byeTeam2.TeamName = team1.TeamName
                                    byeTeam2.Sport = sport

                                    _dbLoserPool.ByeTeamsList.Add(byeTeam2)
                                End If
                            Next

                            _dbLoserPool.SaveChanges()

                        End If


                    End Using
                End Using
            Catch ex As Exception

            End Try

        End Sub

    End Class

    Public Class ScheduleFileXML
        Public Property FilePath As String
        Public Property sport As String
    End Class



End Namespace