Public Class Indexer
    Public Shared Sub indexIt(ByVal youtubeObj As YoutubeObject, ByRef db As Database)
        Dim desc As String = youtubeObj.title & " " & youtubeObj.description
        Dim split() As String = desc.Split(" ")
        Dim s As String
        For Each s In split
            db.AddWord(youtubeObj.videoid, s.Replace("\n", " ").Replace("(", " ").Replace(")", " "))
        Next
    End Sub
End Class
