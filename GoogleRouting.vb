Imports Cadcorp.SIS.GisLink.Library
Imports Cadcorp.SIS.GisLink.Library.Constants
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.IO
Imports System.Net

Public Class GoogleRouting

    Private lat() As Double
    Private lng() As Double

    Public Sub New()

    End Sub

    Public Sub Routing()
        Try

            decodePolyline("uxhuBxrc|QPA@?@?B?B@@B@D@HDFD@B@H@JBB@DBFJ")
            Exit Sub

            Dim x, y, z As Double
            Loader.SIS.OpenSel(0)
            Loader.SIS.CreateListFromSelection("lLine")
            ReDim lat(Loader.SIS.GetGeomNumPt(0) - 1)
            ReDim lng(Loader.SIS.GetGeomNumPt(0) - 1)
            For i = 0 To Loader.SIS.GetGeomNumPt(0) - 1
                Loader.SIS.OpenList("lLine", 0)
                Loader.SIS.SplitPos(x, y, z, Loader.SIS.GetGeomPt(0, i))
                Loader.SIS.CreatePoint(x, y, z, "", 0, 1)
                lat(i) = Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oLat#")
                lng(i) = Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oLon#")
                Loader.SIS.DeleteItem()
            Next

            Dim sUrl = "http://maps.googleapis.com/maps/api/directions/json?origin=" & lat(0).ToString & "," & lng(0).ToString & "&destination=" & lat(1).ToString & "," & lng(1).ToString & "&sensor=false"
            Dim data = New System.Net.WebClient().DownloadString(sUrl)
            Dim response As Byte() = Encoding.Unicode.GetBytes(data)
            Dim deserialiser = New DataContractJsonSerializer(GetType(googleRoute))
            Dim ms = New MemoryStream(response)
            Dim results As googleRoute
            results = deserialiser.ReadObject(ms)

            Dim steps = results.routes(0).legs(0).steps.Length
            For i = 0 To steps - 1
                Loader.SIS.CreatePoint(0, 0, 0, "", 0, 1)
                Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLat#", results.routes(0).legs(0).steps(i).start_location.lat)
                Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLon#", results.routes(0).legs(0).steps(i).start_location.lng)
                Loader.SIS.UpdateItem()
                Loader.SIS.AddToList("lPoints")
            Next

            Loader.SIS.CreatePoint(0, 0, 0, "", 0, 1)
            Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLat#", results.routes(0).legs(0).steps(steps - 1).end_location.lat)
            Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLon#", results.routes(0).legs(0).steps(steps - 1).end_location.lng)
            Loader.SIS.UpdateItem()
            Loader.SIS.AddToList("lPoints")

            Loader.SIS.OpenList("lPoints", 0)
            Loader.SIS.MoveTo(Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_ox#"), Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oy#"), 0)
            For i = 1 To Loader.SIS.GetListSize("lPoints") - 1
                Loader.SIS.OpenList("lPoints", i)
                Loader.SIS.LineTo(Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_ox#"), Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oy#"), 0)
            Next
            Loader.SIS.UpdateItem()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Public Sub decodePolyline(ByVal polyline As String)

        Dim polylinechars As Char() = polyline.ToCharArray()
        Dim currentLat As Integer = 0
        Dim currentLng As Integer = 0
        Dim next5bits As Integer
        Dim sum As Integer
        Dim shifter As Integer
        Dim index As Integer = 0

        While index < polylinechars.Length
            sum = 0
            shifter = 0
            Do
                index += 1
                next5bits = AscW(polylinechars(index - 1)) - 63
                sum = sum Or (next5bits And 31) << shifter
                shifter += 5
            Loop While next5bits >= 32 AndAlso index < polylinechars.Length

            If index >= polylinechars.Length Then
                Exit While
            End If

            currentLat += If((sum And 1) = 1, Not (sum >> 1), (sum >> 1))

            'calculate next longitude
            sum = 0
            shifter = 0
            Do
                index += 1
                next5bits = AscW(polylinechars(index - 1)) - 63
                sum = sum Or (next5bits And 31) << shifter
                shifter += 5
            Loop While next5bits >= 32 AndAlso index < polylinechars.Length

            If index >= polylinechars.Length AndAlso next5bits >= 32 Then
                Exit While
            End If

            currentLng += If((sum And 1) = 1, Not (sum >> 1), (sum >> 1))

            Loader.SIS.CreatePoint(0, 0, 0, "", 0, 1)
            Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLat#", Convert.ToDouble(currentLat) / 100000.0)
            Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLon#", Convert.ToDouble(currentLng) / 100000.0)
            Loader.SIS.UpdateItem()
            Loader.SIS.AddToList("lPoints")

        End While

        Loader.SIS.OpenList("lPoints", 0)
        Loader.SIS.MoveTo(Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_ox#"), Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oy#"), 0)
        For i = 1 To Loader.SIS.GetListSize("lPoints") - 1
            Loader.SIS.OpenList("lPoints", i)
            Loader.SIS.LineTo(Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_ox#"), Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oy#"), 0)
        Next
        Loader.SIS.UpdateItem()

    End Sub

End Class

Public Class googleRoute

    Public Property status As String
        Get
            Return m_status
        End Get
        Set(ByVal value As String)
            m_status = value
        End Set
    End Property
    Private m_status As String

    Public Property routes() As routes()
        Get
            Return m_routes
        End Get
        Set(ByVal value As routes())
            m_routes = value
        End Set
    End Property
    Private m_routes As routes()

End Class

Public Class routes

    Public Property legs() As legs()
        Get
            Return m_legs
        End Get
        Set(ByVal value As legs())
            m_legs = value
        End Set
    End Property
    Private m_legs As legs()

End Class

Public Class legs

    Public Property steps() As steps()
        Get
            Return m_steps
        End Get
        Set(ByVal value As steps())
            m_steps = value
        End Set
    End Property
    Private m_steps As steps()

End Class

Public Class steps

    Public Property start_location As start_location
        Get
            Return m_start_location
        End Get
        Set(ByVal value As start_location)
            m_start_location = value
        End Set
    End Property
    Private m_start_location As start_location

    Public Property end_location As start_location
        Get
            Return m_end_location
        End Get
        Set(ByVal value As start_location)
            m_end_location = value
        End Set
    End Property
    Private m_end_location As start_location

End Class

Public Class start_location

    Public Property lat As String
        Get
            Return m_lat
        End Get
        Set(ByVal value As String)
            m_lat = value
        End Set
    End Property
    Private m_lat As String

    Public Property lng As String
        Get
            Return m_lng
        End Get
        Set(ByVal value As String)
            m_lng = value
        End Set
    End Property
    Private m_lng As String

End Class

Public Class end_location

    Public Property lat As String
        Get
            Return m_lat
        End Get
        Set(ByVal value As String)
            m_lat = value
        End Set
    End Property
    Private m_lat As String

    Public Property lng As String
        Get
            Return m_lng
        End Get
        Set(ByVal value As String)
            m_lng = value
        End Set
    End Property
    Private m_lng As String

End Class