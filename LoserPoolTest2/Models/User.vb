Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Namespace LosersPool.Models

    Public Class UserChoices

        <Key>
        Public Property ListId As Int32
        Public Property UserID As String
        Public Property UserName As String
        Public Property TimePeriod As String
        Public Property ConfirmationNumber As Int32
        Public Property UserPick As String

        Public Property Contender As Boolean

        Public Property Team1Available As Boolean
        Public Property Team2Available As Boolean
        Public Property Team3Available As Boolean
        Public Property Team4Available As Boolean
        Public Property Team5Available As Boolean
        Public Property Team6Available As Boolean
        Public Property Team7Available As Boolean
        Public Property Team8Available As Boolean
        Public Property Team9Available As Boolean
        Public Property Team10Available As Boolean
        Public Property Team11Available As Boolean
        Public Property Team12Available As Boolean
        Public Property Team13Available As Boolean
        Public Property Team14Available As Boolean
        Public Property Team15Available As Boolean
        Public Property Team16Available As Boolean
        Public Property Team17Available As Boolean
        Public Property Team18Available As Boolean
        Public Property Team19Available As Boolean
        Public Property Team20Available As Boolean
        Public Property Team21Available As Boolean
        Public Property Team22Available As Boolean
        Public Property Team23Available As Boolean
        Public Property Team24Available As Boolean
        Public Property Team25Available As Boolean
        Public Property Team26Available As Boolean
        Public Property Team27Available As Boolean
        Public Property Team28Available As Boolean
        Public Property Team29Available As Boolean
        Public Property Team30Available As Boolean
        Public Property Team31Available As Boolean
        Public Property Team32Available As Boolean

        Public Property AdministrationPick As Boolean

        Public Property PoolAlias As String
        Public Property Sport As String
        Public Property CronJob As String

        Public Property PickedGameCode As String
        Public Property UserPickPostponed As String
        Public Property UserIsWinning As Boolean
        Public Property UserIsTied As Boolean

        Public Overridable Property PossibleUserPicks As New List(Of String)

    End Class

    Public Class UserPick
        <Key>
        Public Property UserPickId As Int32
        Public Property UserID As String
        Public Property CronJobName As String
        Public Property PoolAlias As String
        Public Property TimePeriod As String
        Public Property UserPick1 As String
        Public Property GameCode As String
        Public Property UserPickPostponed As String
        Public Property PickIsWinning As Boolean
        Public Property PickIsTied As Boolean
    End Class

    Public Class UserChoiceList

        Public Sub New(filepath As String, wNumber As String, rootFolder As String)
            Dim _dbLoserPool As New LosersPoolContext
            Try
                Using (_dbLoserPool)

                    Dim queryUser1 = (From users1 In _dbLoserPool.UserChoicesList).ToList

                    If _dbLoserPool.UserChoicesList.Count >= 1 Then
                        Exit Sub
                    End If

                    _dbLoserPool.SaveChanges()

                    System.IO.Directory.SetCurrentDirectory(rootFolder)

                    Dim userChoicesXDocument = XDocument.Load(filepath)

                    Dim DailyPossibleChoicesForAllUsers = (From timePeriod1 In userChoicesXDocument.Descendants("UserChoicesList").Descendants("TimeP").Descendants("User")
                                                            Select New UserChoices With {.UserID = timePeriod1.Attribute("UserId").Value,
                                                                                            .TimePeriod = timePeriod1.Attribute("TimePeriod").Value,
                                                                                            .UserName = timePeriod1.Elements("UserName").Value,
                                                                                            .ConfirmationNumber = CInt(timePeriod1.Elements("ConfirmationNumber").Value),
                                                                                            .UserPick = timePeriod1.Elements("UserPick").Value,
                                                                                            .Team1Available = CBool(timePeriod1.Elements("Team1Available").Value),
                                                                                            .Team2Available = CBool(timePeriod1.Elements("Team2Available").Value),
                                                                                            .Team3Available = CBool(timePeriod1.Elements("Team3Available").Value),
                                                                                            .Team4Available = CBool(timePeriod1.Elements("Team4Available").Value),
                                                                                            .Team5Available = CBool(timePeriod1.Elements("Team5Available").Value),
                                                                                            .Team6Available = CBool(timePeriod1.Elements("Team6Available").Value),
                                                                                            .Team7Available = CBool(timePeriod1.Elements("Team7Available").Value),
                                                                                            .Team8Available = CBool(timePeriod1.Elements("Team8Available").Value),
                                                                                            .Team9Available = CBool(timePeriod1.Elements("Team9Available").Value),
                                                                                            .Team10Available = CBool(timePeriod1.Elements("Team10Available").Value),
                                                                                            .Team11Available = CBool(timePeriod1.Elements("Team11Available").Value),
                                                                                            .Team12Available = CBool(timePeriod1.Elements("Team12Available").Value),
                                                                                            .Team13Available = CBool(timePeriod1.Elements("Team13Available").Value),
                                                                                            .Team14Available = CBool(timePeriod1.Elements("Team14Available").Value),
                                                                                            .Team15Available = CBool(timePeriod1.Elements("Team15Available").Value),
                                                                                            .Team16Available = CBool(timePeriod1.Elements("Team16Available").Value),
                                                                                            .Team17Available = CBool(timePeriod1.Elements("Team17Available").Value),
                                                                                            .Team18Available = CBool(timePeriod1.Elements("Team18Available").Value),
                                                                                            .Team19Available = CBool(timePeriod1.Elements("Team19Available").Value),
                                                                                            .Team20Available = CBool(timePeriod1.Elements("Team20Available").Value),
                                                                                            .Team21Available = CBool(timePeriod1.Elements("Team21Available").Value),
                                                                                            .Team22Available = CBool(timePeriod1.Elements("Team22Available").Value),
                                                                                            .Team23Available = CBool(timePeriod1.Elements("Team23Available").Value),
                                                                                            .Team24Available = CBool(timePeriod1.Elements("Team24Available").Value),
                                                                                            .Team25Available = CBool(timePeriod1.Elements("Team25Available").Value),
                                                                                            .Team26Available = CBool(timePeriod1.Elements("Team26Available").Value),
                                                                                            .Team27Available = CBool(timePeriod1.Elements("Team27Available").Value),
                                                                                            .Team28Available = CBool(timePeriod1.Elements("Team28Available").Value),
                                                                                            .Team29Available = CBool(timePeriod1.Elements("Team29Available").Value),
                                                                                            .Team30Available = CBool(timePeriod1.Elements("Team30Available").Value),
                                                                                            .Team31Available = CBool(timePeriod1.Elements("Team31Available").Value),
                                                                                            .Team32Available = CBool(timePeriod1.Elements("Team32Available").Value),
                                                                                            .Contender = CBool(timePeriod1.Elements("Contender").Value),
                                                                                            .PoolAlias = timePeriod1.Elements("PoolAlias").Value,
                                                                                            .Sport = timePeriod1.Elements("Sport").Value}).ToList

                    For Each user1 In DailyPossibleChoicesForAllUsers
                        Dim user2 = New UserChoices
                        user2.ListId = _dbLoserPool.UserChoicesList.Count + 1
                        user2.UserID = user1.UserID
                        user2.UserName = user1.UserName
                        user2.TimePeriod = user1.TimePeriod
                        user2.ConfirmationNumber = user1.ConfirmationNumber
                        user2.Contender = user1.Contender
                        user2.UserPick = user1.UserPick
                        user2.Team1Available = user1.Team1Available
                        user2.Team2Available = user1.Team2Available
                        user2.Team3Available = user1.Team3Available
                        user2.Team4Available = user1.Team4Available
                        user2.Team5Available = user1.Team5Available
                        user2.Team6Available = user1.Team6Available
                        user2.Team7Available = user1.Team7Available
                        user2.Team8Available = user1.Team8Available
                        user2.Team9Available = user1.Team9Available
                        user2.Team10Available = user1.Team10Available
                        user2.Team11Available = user1.Team11Available
                        user2.Team12Available = user1.Team12Available
                        user2.Team13Available = user1.Team13Available
                        user2.Team14Available = user1.Team14Available
                        user2.Team15Available = user1.Team15Available
                        user2.Team16Available = user1.Team16Available
                        user2.Team17Available = user1.Team17Available
                        user2.Team18Available = user1.Team18Available
                        user2.Team19Available = user1.Team19Available
                        user2.Team20Available = user1.Team20Available
                        user2.Team21Available = user1.Team21Available
                        user2.Team22Available = user1.Team22Available
                        user2.Team23Available = user1.Team23Available
                        user2.Team24Available = user1.Team24Available
                        user2.Team25Available = user1.Team25Available
                        user2.Team26Available = user1.Team26Available
                        user2.Team27Available = user1.Team27Available
                        user2.Team28Available = user1.Team28Available
                        user2.Team29Available = user1.Team29Available
                        user2.Team30Available = user1.Team30Available
                        user2.Team31Available = user1.Team31Available
                        user2.Team32Available = user1.Team32Available
                        user2.PoolAlias = user1.PoolAlias
                        user2.Sport = user1.Sport
                        _dbLoserPool.UserChoicesList.Add(user2)
                    Next

                    _dbLoserPool.SaveChanges()

                    Dim dummy = "dummy"
                End Using
            Catch ex As Exception

            End Try

        End Sub
    End Class

    Public Class UserStatus

        Public Property UserName As String
        Public Property IsUserWinning As Boolean
        Public Property IsUserTied As Boolean
    End Class

    Public Class PickStatus
        Public Property UserId As String
        Public Property UserName As String
        Public Property GameCode As String
        Public Property IsPickWinning As Boolean
        Public Property IsPickTied As Boolean
    End Class



End Namespace
