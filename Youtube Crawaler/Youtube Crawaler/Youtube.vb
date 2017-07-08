Public Class Youtube
    Public Shared Function getYoutubeObject(ByVal videoid As String)
        Dim wc As System.Net.WebClient
        wc = New System.Net.WebClient()
        Dim content As String = wc.DownloadString("https://www.googleapis.com/youtube/v3/videos?part=id%2Csnippet&id=" & videoid & "&key=AIzaSyAmSdAaATbHy_WdR5Hk7P_1rUAsGIUzzDk")

        Dim title_start As Integer = content.IndexOf("""title"": """)
        Dim title_end As Integer = content.IndexOf("""", title_start + 10)
        Dim title As String = content.Substring(title_start + 10, title_end - title_start - 10)

        Dim desc_start As Integer = content.IndexOf("""description""")
        Dim desc_end As Integer = content.IndexOf("""", desc_start + 16)
        Dim desc As String = content.Substring(desc_start + 16, desc_end - desc_start - 16)

        Dim imageurl_start As Integer = content.IndexOf("""url"": """)
        Dim imageurl_end As Integer = content.IndexOf("""", imageurl_start + 8)
        Dim imageurl As String = content.Substring(imageurl_start + 8, imageurl_end - imageurl_start - 8)

        Return New YoutubeObject(title, desc, videoid, "http://www.youtube.com/watch?v=" & videoid, imageurl)
    End Function
End Class

Public Class YoutubeObject
    Sub New(ByVal ttl As String, ByVal desc As String, ByVal vid As String, ByVal vurl As String, ByVal imgurl As String)
        title = ttl
        description = desc
        videoid = vid
        videourl = vurl
        imageurl = imgurl
    End Sub
    Public videoid, videourl, imageurl, title, description As String
End Class
