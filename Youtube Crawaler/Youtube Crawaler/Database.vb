Imports System.Data
Imports System.Data.SqlClient
Public Class Database
    Private Shared connector As Database
    Private connection As SqlConnection
    Private Sub New()
        Dim connStr As String = IO.File.ReadAllText(Application.StartupPath & "\connectionString.txt")
        connection = New SqlConnection(connStr)
        connection.Open()
    End Sub
    Public Shared Function getConnector() As Database
        If connector Is Nothing Then
            connector = New Database()
        End If
        Return connector
    End Function
    Public Function getSqlConnection() As SqlConnection
        Return connection
    End Function
    Public Sub AddVideo(ByVal videoid As String, ByVal title As String, ByVal desc As String, ByVal imageUrl As String, ByVal videoUrl As String)
        Dim add As SqlCommand = New SqlCommand("videoadd", connection)
        add.CommandType = CommandType.StoredProcedure
        add.Parameters.AddWithValue("videoid", videoid)
        add.Parameters.AddWithValue("title", title)
        add.Parameters.AddWithValue("descptn", desc)
        add.Parameters.AddWithValue("imageurl", imageUrl)
        add.Parameters.AddWithValue("videourl", videoUrl)
        add.ExecuteNonQuery()
    End Sub
    Public Sub AddWord(ByVal videoid As String, ByVal word As String)
        If word.Length > 40 Or Trim(word) = "" Then
            Return
        End If
        Dim add As SqlCommand = New SqlCommand("indexer", connection)
        add.CommandType = CommandType.StoredProcedure
        add.Parameters.AddWithValue("videoid", videoid)
        add.Parameters.AddWithValue("word", Trim(word))
        add.ExecuteNonQuery()
    End Sub
End Class
