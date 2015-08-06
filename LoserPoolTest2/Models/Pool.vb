Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Imports WpfApplication1.JoinPools.Models

Public Class Pool

    <Key>
    Public Property PoolId As String

    <Required>
    Public Property PoolName As String

    Public Property Sport As String
    Public Property timePeriodName As String
    Public Property timePeriodIncrement As String

End Class
Public Class SeedPools
    Public Sub New()
        Dim _dbPools2 As New PoolDbContext

        Try

            Using (_dbPools2)

                Dim querypool1 = (From user1 In _dbPools2.Pools).ToList

                If querypool1.Count >= 1 Then
                    Exit Sub
                End If

                Dim pool1 As New Pool
                pool1.PoolId = "pool1"
                pool1.PoolName = "LoserPool"

                _dbPools2.Pools.Add(pool1)

                Dim pool2 As New Pool
                pool2.PoolId = "pool2"
                pool2.PoolName = "PlayoffPool"

                _dbPools2.Pools.Add(pool2)

                _dbPools2.SaveChanges()
            End Using
        Catch ex As Exception

        End Try

    End Sub

End Class



