Imports System.Data.SqlClient
Partial Class _Default
    Inherits System.Web.UI.Page

    Dim conn As SqlConnection

    Protected Sub ImageButton1_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton1.Click
        If conn Is Nothing Then
            conn = New SqlConnection("Server=NICE\SQLEXPRESS; Database=bits; Trusted_Connection=True;")
            conn.Open()
        End If
        Dim query As String = Trim(TextBox1.Text)
        If query = "" Then
            Return
        End If
        Dim word() As String = query.Split(" ")

        Dim rank As New Dictionary(Of String, Double)

        Dim w As String
        For Each w In word
            'SELECT *, count/totalword as rel FROM FinalUrls Where word = w
            Dim sqlCmd As New SqlCommand("SELECT *, count/totalword as rel FROM FinalUrls Where word = '" & w & "'", conn)
            Dim reader As SqlDataReader = sqlCmd.ExecuteReader
            While reader.Read
                Try
                    If rank.Item(reader("videoid")) = Double.NaN Then
                        rank.Add(reader("videoid"), 0)
                    End If
                Catch ex As Exception
                    rank.Add(reader("videoid"), 0)
                End Try
                Dim r As Double = rank.Item(reader("videoid")) + Math.Log(1 + reader("count") / reader("totalWord"))
                rank.Remove(reader("videoid"))
                rank.Add(reader("videoid"), r)
            End While
            reader.Close()
        Next
        Dim db_search As String = ""
        For Each r As KeyValuePair(Of String, Double) In rank
            Dim key As String = r.Key
            Dim value As Double = r.Value
            db_search &= "'" & key & "'" & ","
            'rank.Remove(key)
            'rank.Add(key, value)
        Next
        display.Text = "No Result Found!!! :("
        If Trim(db_search) = "" Then
            Return
        End If
        db_search = db_search.Substring(0, db_search.Length - 1)
        'SELECT * FROM youtubevideo WHERE videoid IN (db_search)
        Dim sqlCmd2 As New SqlCommand("SELECT * FROM youtubevideo WHERE videoid IN (" & db_search & ")", conn)
        Dim reader2 As SqlDataReader = sqlCmd2.ExecuteReader
        display.Text = ""
        Dim template As String = "<tr><td width='30%'>IMAGE</TD><td>DETAILS</td></tr>"
        template = template.Replace("IMAGE", "<IMG SRC='image_url'/>")
        template = template.Replace("DETAILS", "<table border='0'><tr>title</tr><tr>desc</tr></table>")
        template = template.Replace("title", "<a href='video_url' target='_test'>ttle</a>")
        While reader2.Read
            display.Text &= template.Replace("image_url", reader2("imageurl")).Replace("desc", reader2("description")).Replace("video_url", reader2("videourl")).Replace("ttle", reader2("title"))
        End While
        display.Text = "<table border='0'>" & display.Text & "</table>"
        reader2.Close()
        Dim x As Int16
    End Sub
End Class
