Imports System.Net.WebClient
Imports System.Text.RegularExpressions
Imports System.Data.SqlClient
Public Class Form1
    Dim isRunning As Boolean = False
    Dim uri As String
    Dim content As String
    Dim db As Database
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click

        Timer1.Start()
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If isRunning = False Then
            refreshList()
            RunThread()
        End If
    End Sub
    Public Sub RunThread()
        isRunning = True
        Dim t As New Threading.Thread(AddressOf Crawl)
        t.Start()
    End Sub
    Public Sub refreshList()
        Dim connection As SqlConnection = db.getSqlConnection()
        Dim sqlCmd As SqlCommand = New SqlCommand("SELECT * FROM initialurl", connection)
        Dim reader As SqlDataReader = sqlCmd.ExecuteReader()
        ListBox1.Items.Clear()
        While reader.Read()
            ListBox1.Items.Add(reader.Item("videourl"))
        End While
        If ListBox1.Items.Count <= 0 Then
            ListBox1.Items.Add(url.Text)
        End If
        reader.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Dim youtubeObj As YoutubeObject = Youtube.getYoutubeObject("S1mdUPp2HLA")
        db = Database.getConnector()
        refreshList()
    End Sub

    Private Delegate Sub AppendTextBoxDelegate(ByVal TB As RichTextBox, ByVal txt As String)

    Private Sub AppendTextBox(ByVal TB As RichTextBox, ByVal txt As String)
        If TB.InvokeRequired Then
            TB.Invoke(New AppendTextBoxDelegate(AddressOf AppendTextBox), New Object() {TB, txt})
        Else
            TB.Text = txt
        End If
    End Sub
    Private Delegate Sub RemoveItemDelegate(ByVal TB As ListBox)
    Private Sub RemoveItems(ByVal TB As ListBox)
        If TB.InvokeRequired Then
            TB.Invoke(New RemoveItemDelegate(AddressOf RemoveItems), New Object() {TB})
        Else
            TB.Items.RemoveAt(0)
        End If
    End Sub

    Private Delegate Sub AddItemDelegate(ByVal TB As ListBox, ByVal s As String)
    Private Sub AddItems(ByVal TB As ListBox, ByVal s As String)
        If TB.InvokeRequired Then
            TB.Invoke(New AddItemDelegate(AddressOf AddItems), New Object() {TB, s})
        Else
            TB.Items.Add(s)
        End If
    End Sub

    Public Sub Crawl()
        uri = CStr(Invoke(New Func(Of String)(Function() Me.ListBox1.GetItemText(Me.ListBox1.Items(0)))))

        If (Not uri.StartsWith("http://")) Then
            uri = "http://" & uri
        End If
        Dim wc As System.Net.WebClient
        wc = New System.Net.WebClient()
        content = wc.DownloadString(uri)
        Dim con2 As String = content
        AppendTextBox(RichTextBox1, content)
        

        Dim youtubeObj As YoutubeObject = Youtube.getYoutubeObject(uri.Replace("http://www.youtube.com/watch?v=", ""))

        AppendTextBox(RichTextBox2, youtubeObj.description)
        For Each i As Match In Regex.Matches(con2, "/watch[?]v=(.*)""")
            Dim videoid As String = i.Groups(1).Value.Split("""")(0)
            If videoid.Contains("&") Then
                videoid = videoid.Split("&")(0)
            End If
            db.AddVideo(videoid, "", "", "", "http://www.youtube.com/watch?v=" & videoid)
        Next
        db.AddVideo(youtubeObj.videoid, youtubeObj.title, youtubeObj.description, youtubeObj.imageurl, youtubeObj.videourl)
        Indexer.indexIt(youtubeObj, db)
        isRunning = False
        
    End Sub
End Class
