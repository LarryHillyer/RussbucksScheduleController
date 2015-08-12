Imports System
Imports System.Math
Imports System.Data
Imports System.Linq
Imports System.Xml.Linq
Imports System.Globalization

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Path

Imports System.Threading
Imports System.Threading.Tasks

Imports WpfApplication1.JoinPools.Models
Imports WpfApplication1.LosersPool.Models

Class MainWindow


    Private Sports As New Dictionary(Of String, String)
    Private TeamNicknameCollection As New Dictionary(Of String, String)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim _dbApp As New ApplicationDbContext
        Dim _dbPools As New PoolDbContext

        Try
            Using (_dbApp)
                Using (_dbPools)

                    Dim queryParameters = (From param1 In _dbApp.AppFolders).Single

                    System.IO.Directory.SetCurrentDirectory(queryParameters.scheduleCronJobFolder)
                    Dim rootFolder = queryParameters.scheduleCronJobFolder

                    Dim cronJobName = My.Application.CronJobName1

                    DeleteData(cronJobName)

                    MW1.Title = "Schedule Cron Job - " + cronJobName

                    Dim queryCronJob = (From cronJob2 In _dbApp.CronJobs
                                        Where cronJob2.CronJobName = cronJobName).Single

                    Dim queryCronJobPools = (From cronJobpool1 In _dbApp.CronJobPools
                                             Where cronJobpool1.CronJobName = cronJobName).ToList

                    Dim queryPoolParams3 = (From poolParam3 In _dbPools.PoolParameters
                                            Where poolParam3.CronJob = queryCronJob.CronJobName).ToList

                    If queryCronJob.SelectedSport = "baseball" Then

                        Dim queryTeams = (From team1 In _dbPools.Teams
                                          Where team1.Sport = "baseball" And team1.TeamName <> "dummy").ToList

                        For Each team1 In queryTeams
                            TeamNicknameCollection.Add(team1.NickName, team1.TeamName)
                        Next

                        Sports.Add("baseball", "baseball")

                    ElseIf queryCronJob.SelectedSport = "football" Then
                        Sports.Add("football", "football")
                    End If


                    Dim sport = queryCronJob.SelectedSport
                    Dim filename = "\PythonScheduleScrape.bat"
                    Dim scrapedFilesFolder = queryParameters.scrapedScheduleFilesFolder

                    Dim sport1 = ""
                    Dim timePeriod1 = ""
                    If sport = "baseball" Then
                        sport1 = "mlb"
                        timePeriod1 = "day"
                        filename = "PythonScheduleScrape.bat"
                    ElseIf sport = "football" Then
                        sport1 = "nfl"
                        timePeriod1 = "week"
                        filename = "NFLPythonScheduleScrape.bat"
                    End If

                    Dim scheduleScrapeDate = DateTime.Parse(queryCronJob.SelectedSeasonStartGameDate)
                    Dim seasonStart = scheduleScrapeDate.ToString("MM/dd/yy")

                    Dim scheduleScrapeEndDate = DateTime.Parse(queryCronJob.SelectedSeasonEndDate)
                    Dim seasonEnd = scheduleScrapeEndDate.ToString("MM/dd/yy")

                    Dim sT1 As New ScheduleThread

                    sT1.filename = filename
                    sT1.rootFolder = rootFolder
                    sT1.scrapedFilesFolder = scrapedFilesFolder
                    sT1.scheduleScrapeDate = scheduleScrapeDate
                    sT1.scheduleScrapeEndDate = scheduleScrapeEndDate
                    sT1.sport = sport
                    sT1.sport1 = sport1
                    sT1.TeamNicknameCollection = TeamNicknameCollection
                    sT1.cronJobName = cronJobName

                    Dim scheduleScrapeThread As New Thread(AddressOf sT1.ScheduleScrape)
                    scheduleScrapeThread.IsBackground = True
                    scheduleScrapeThread.Start()

                End Using
            End Using

        Catch ex As Exception

        End Try

    End Sub

    Private Sub DeleteData(cronJobName As String)



        Dim _dbLoserPool4 As New LosersPoolContext
        Dim _dbPools4 As New PoolDbContext
        Try
            Using (_dbLoserPool4)
                Using (_dbPools4)

                    Dim queryUsersChoices = (From user1 In _dbLoserPool4.UserChoicesList
                                             Where user1.CronJob = cronJobName).ToList

                    If queryUsersChoices.Count > 0 Then
                        For Each user1 In queryUsersChoices
                            _dbLoserPool4.UserChoicesList.Remove(user1)
                        Next
                    End If

                    Dim queryTimePeriods = (From user1 In _dbLoserPool4.ScheduleTimePeriods
                                            Where user1.CronJob = cronJobName).ToList

                    If queryTimePeriods.Count > 0 Then
                        For Each timeperiod1 In queryTimePeriods
                            _dbLoserPool4.ScheduleTimePeriods.Remove(timeperiod1)
                        Next
                    End If

                    Dim querySchedule = (From game1 In _dbLoserPool4.ScheduleEntities
                                         Where game1.CronJob = cronJobName).ToList

                    If querySchedule.Count > 0 Then
                        For Each game1 In querySchedule
                            _dbLoserPool4.ScheduleEntities.Remove(game1)
                        Next
                    End If

                    Dim queryLosers = (From game1 In _dbLoserPool4.LoserList
                                       Where game1.CronJob = cronJobName).ToList

                    If queryLosers.Count > 0 Then
                        For Each loser1 In queryLosers
                            _dbLoserPool4.LoserList.Remove(loser1)
                        Next
                    End If

                    Dim queryByeTeams = (From game1 In _dbLoserPool4.ByeTeamsList
                                         Where game1.CronJob = cronJobName).ToList

                    If queryByeTeams.Count > 0 Then
                        For Each byeteam1 In queryByeTeams
                            _dbLoserPool4.ByeTeamsList.Remove(byeteam1)
                        Next
                    End If

                    Dim queryCurrentScoringUpdate = (From game1 In _dbLoserPool4.CurrentScoringUpdates
                                                     Where game1.CronJobName = cronJobName).ToList

                    If queryCurrentScoringUpdate.Count > 0 Then
                        For Each cSU1 In queryCurrentScoringUpdate
                            _dbLoserPool4.CurrentScoringUpdates.Remove(cSU1)
                        Next
                    End If

                    Dim queryScoringUpdate = (From game1 In _dbLoserPool4.ScoringUpdates
                                              Where game1.CronJobName = cronJobName).ToList

                    If queryScoringUpdate.Count > 0 Then
                        For Each sU1 In queryScoringUpdate
                            _dbLoserPool4.ScoringUpdates.Remove(sU1)
                        Next
                    End If

                    Dim queryPostponedGames = (From game1 In _dbLoserPool4.PostponedGames
                                               Where game1.CronJobName = cronJobName).ToList

                    If queryPostponedGames.Count > 0 Then
                        For Each game1 In queryPostponedGames
                            _dbLoserPool4.PostponedGames.Remove(game1)
                        Next
                    End If

                    Dim queryDeletedGames = (From game1 In _dbLoserPool4.DeletedGames
                               Where game1.CronJob = cronJobName).ToList

                    If queryDeletedGames.Count > 0 Then
                        For Each game1 In queryDeletedGames
                            _dbLoserPool4.DeletedGames.Remove(game1)
                        Next
                    End If

                    Dim queryUserPicks = (From qUP1 In _dbLoserPool4.UserPicks
                                          Where qUP1.CronJobName = cronJobName).ToList

                    If queryUserPicks.Count > 0 Then
                        For Each game1 In queryUserPicks
                            _dbLoserPool4.UserPicks.Remove(game1)
                        Next
                    End If

                    _dbLoserPool4.SaveChanges()

                    Dim queryTotalSchedule = (From game1 In _dbPools4.ScheduledGames
                             Where game1.CronJob = cronJobName).ToList

                    If queryTotalSchedule.Count > 0 Then
                        For Each game1 In queryTotalSchedule
                            _dbPools4.ScheduledGames.Remove(game1)
                        Next
                    End If


                    'Dim queryTotalMLBSchedule = (From game1 In _dbPools4.MLBScheduledGames
                    'Where game1.CronJob = cronJobName).ToList

                    'If queryTotalMLBSchedule.Count > 0 Then
                    'For Each game1 In queryTotalMLBSchedule
                    '_dbPools4.MLBScheduledGames.Remove(game1)
                    'Next
                    'End If

                    'Dim queryTotalNLFSchedule = (From game1 In _dbPools4.NFLScheduledGames
                    '                                Where game1.CronJob = cronJobName).ToList

                    'If queryTotalNLFSchedule.Count > 0 Then
                    'For Each game1 In queryTotalNLFSchedule
                    '_dbPools4.NFLScheduledGames.Remove(game1)
                    'Next
                    'End If

                    _dbPools4.SaveChanges()

                End Using
            End Using
        Catch ex As Exception

        End Try

    End Sub

End Class

Public Class ScheduleThread

    Public Property filename As String
    Public Property rootFolder As String
    Public Property scheduleScrapeDate As Date
    Public Property scheduleScrapeEndDate As Date
    Public Property scrapedFilesFolder As String
    Public Property sport As String
    Public Property sport1 As String
    Public Property TeamNicknameCollection As Dictionary(Of String, String)
    Public Property cronJobName As String

    Public Sub ScheduleScrape()

        Dim _dbPools1 As New PoolDbContext
        Dim _dbApp1 As New ApplicationDbContext

        Try
            Using (_dbPools1)
                Using (_dbApp1)

                    Dim queryParameters = (From param1 In _dbApp1.AppFolders).Single

                    System.IO.Directory.SetCurrentDirectory(queryParameters.scheduleCronJobFolder)

                    If sport = "baseball" Then
                        sport1 = "mlb"
                        Dim pythonScrape = "MLBScheduleScrape1.py"

                        While Me.scheduleScrapeEndDate >= Me.scheduleScrapeDate
                            Dim scheduleDate = scheduleScrapeDate.ToString("MM/dd/yy")

                            ScheduleScrape(sport, sport1, pythonScrape, scheduleDate, cronJobName, filename, scrapedFilesFolder)

                            Me.scheduleScrapeDate = Me.scheduleScrapeDate.AddDays(1)

                            Thread.Sleep(2000)

                        End While

                    ElseIf sport = "football" Then

                        sport1 = "nfl"

                        Dim queryCronJobs = (From qCJ In _dbApp1.CronJobs
                                             Where qCJ.CronJobName = cronJobName).Single

                        Dim pythonScrape = ""
                        If queryCronJobs.CronJobIsPreseason = False Then
                            pythonScrape = "NFLSchedulescrape1.py"

                            For weekNum = 1 To 17
                                ScheduleScrape(sport, sport1, pythonScrape, CStr(weekNum), cronJobName, filename, scrapedFilesFolder)
                                Thread.Sleep(2000)
                            Next
                        Else
                            pythonScrape = "NFLSchedulescrapePreseason.py"
                            For weekNum = 2 To 5
                                ScheduleScrape(sport, sport1, pythonScrape, CStr(weekNum), cronJobName, filename, scrapedFilesFolder)
                                Thread.Sleep(2000)
                            Next

                        End If



                    End If

                End Using
            End Using
        Catch ex As Exception

        End Try
    End Sub

    Private Shared Sub ScheduleScrape(sport As String, league As String, pythonScrape As String, timePeriodParameter As String, cronJobName As String, filename As String, scrapedFilesFolder As String)

        Dim _dbApp2 As New ApplicationDbContext
        Dim _dbPools2 As New PoolDbContext

        Try
            Using (_dbApp2)
                Using (_dbPools2)
                    File.Delete(".\ScrapedFiles\" + filename)
                    Thread.Sleep(2000)
                    File.AppendAllText(".\ScrapedFiles\" + filename, "C:\python27\python " + " " + ".\ScrapedFiles\" + pythonScrape + _
                                       " " + league + " " + sport + " " + timePeriodParameter + " " + scrapedFilesFolder)

                    Dim cnt = 1
RestartScrape:
                    Dim mySchedule As New XDocument
                    Dim filename1 As String
                    Try
                        Dim myProcess As Process
                        myProcess = Process.Start(".\ScrapedFiles\" + filename)

                        While Not myProcess.HasExited
                            'myProcess.Refresh()
                            Thread.Sleep(100)
                            Dim dummy = "dummy"
                        End While

                        filename1 = ".\ScrapedFiles\" + "schedule" + sport + ".xml"

                        Try
                            mySchedule = XDocument.Load(filename1)

                        Catch ex As Exception

                            If cnt > 1 Then
                                Exit Sub
                            End If
                            cnt += 1
                            Thread.Sleep(10000)
                            GoTo RestartScrape

                        End Try
                    Catch ex As Exception

                        If cnt > 1 Then
                            Exit Sub
                        End If
                        cnt += 1
                        GoTo RestartScrape

                    End Try

                    Dim queryGame = (From game In mySchedule.Descendants("score").Descendants("game")
                                Select New ScheduledGame With {.HomeTeam = game.Attribute("hometeam").Value,
                                .AwayTeam = game.Attribute("awayteam").Value,
                                .GameCode = game.Attribute("gamecode").Value,
                                .HomeScore = game.Element("homescore").Value,
                                .AwayScore = game.Element("awayscore").Value,
                                .GameDate = game.Element("gamedate").Value,
                                .GameTime = game.Element("gametime").Value,
                                .Status = game.Element("status").Value,
                                .DisplayStatus1 = game.Element("display_status1").Value,
                                .DisplayStatus2 = game.Element("display_status2").Value}).ToList


                    For Each game In queryGame
                        game.HomeTeam = game.HomeTeam
                        game.AwayTeam = game.AwayTeam
                        game.GameDate = game.GameDate
                        game.StartDate = game.GameDate
                        game.OriginalStartDate = game.GameDate
                        game.StartTime = game.GameTime
                        game.OriginalStartTime = game.GameTime
                        Try
                            game.StartDateTime = DateTime.Parse(game.GameDate + " " + game.GameTime)
                        Catch ex As Exception
                            game.StartDateTime = DateTime.Parse(game.GameDate + " " + "9:00 AM")
                        End Try

                        game.CronJob = cronJobName
                    Next

                    _dbPools2.SaveChanges()

                    Dim queryCronJobs = (From qCJ In _dbApp2.CronJobs
                                        Where qCJ.CronJobName = cronJobName).Single

                    Dim queryGame1 = (From game In queryGame
                                      Order By game.StartDateTime).ToList

                    Dim cnt1 = 1

                    For Each game In queryGame1

                        If game.Status = "Pre-Game" Then
                            game.GameId = "game" + CStr(cnt1)
                            game.Sport = sport
                            If queryCronJobs.CronJobIsPreseason = False Then
                                game.IsPreseason = False
                            Else
                                game.IsPreseason = True
                            End If

                            cnt1 += 1
                            _dbPools2.ScheduledGames.Add(game)
                        End If
                    Next

                    _dbPools2.SaveChanges()

                    For Each game In queryGame1
                        Dim queryMultipleGames = (From game1 In queryGame Where game1.HomeTeam = game.HomeTeam And _
                        game1.AwayTeam = game1.AwayTeam).ToList

                        If queryMultipleGames.Count = 1 Then
                            game.MultipleGamesAreScheduled = False
                            _dbPools2.SaveChanges()
                        ElseIf queryMultipleGames.Count > 1 Then
                            If game.MultipleGamesAreScheduled = False Then
                                Dim cnt2 = 1
                                For Each game1 In queryMultipleGames
                                    game1.MultipleGamesAreScheduled = True
                                    game1.MultipleGameNumber = CStr(cnt2)
                                    cnt2 += 1
                                    _dbPools2.SaveChanges()
                                Next
                            End If
                        End If
                    Next

                    File.Delete(filename1)

                End Using
            End Using
        Catch ex As Exception

        End Try
    End Sub
End Class

