Imports Cadcorp.SIS.GisLink.Library
Imports Cadcorp.SIS.GisLink.Library.Constants
Imports System.Windows.Forms
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.IO
Imports System.Net


Public Class GoogleGeocode

    Public Sub New()
        InitializeComponent()
        Try
            Loader.SIS.Dispose()
            Loader.SIS = Nothing
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private geocodeResponse As GoogleGeocodeResponse

    Private Sub TB_ADDRESS_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TB_ADDRESS.KeyUp
        Try
            If e.KeyCode = Keys.Enter Then
                geocodeResponse = Nothing
                Dim nOverlay = Loader.SIS.GetInt(SIS_OT_WINDOW, 0, "_nOverlay&")
                Dim llX, llY, urX, urY, z As Double
                Loader.SIS.SplitExtent(llX, llY, z, urX, urY, z, Loader.SIS.GetViewExtent())
                Loader.SIS.CreateInternalOverlay("tmp", nOverlay)
                Loader.SIS.SetDatasetPrj(Loader.SIS.GetInt(SIS_OT_OVERLAY, nOverlay, "_nDataset&"), "Latitude/Longitude.OGC.WGS_1984")
                Loader.SIS.SetInt(SIS_OT_WINDOW, 0, "_nDefaultOverlay&", nOverlay)
                Loader.SIS.CreatePoint(llX, llY, z, "", 0, 1)
                Dim llLat As Double = Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oLat#")
                Dim llLon As Double = Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oLon#")
                Loader.SIS.CreatePoint(urX, urY, z, "", 0, 1)
                Dim urLat As Double = Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oLat#")
                Dim urLon As Double = Loader.SIS.GetFlt(SIS_OT_CURITEM, 0, "_oLon#")
                Loader.SIS.RemoveOverlay(nOverlay)

                geocodeResponse = Geocode(TB_ADDRESS.Text, llLat, llLon, urLat, urLon)

                SetLocation()

                Loader.SIS.Dispose()
                Loader.SIS = Nothing
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
            Loader.SIS.Dispose()
            Loader.SIS = Nothing
        End Try

    End Sub


    Private Function Geocode(ByVal sAddress As String, ByVal llLat As Double, ByVal llLon As Double, ByVal urLat As Double, ByVal urLon As Double) As GoogleGeocodeResponse
        Try
            Dim sUrl = "http://maps.googleapis.com/maps/api/geocode/json?address=" & System.Uri.EscapeUriString(sAddress) & "&components=country:MX&bounds=" & llLat & "," & llLon & "|" & urLat & "," & urLon & "&sensor=false"
            Dim data = New System.Net.WebClient().DownloadString(sUrl)
            Dim response As Byte() = Encoding.Unicode.GetBytes(data)
            Dim deserialiser = New DataContractJsonSerializer(GetType(GoogleGeocodeResponse))
            Dim ms = New MemoryStream(response)
            Return deserialiser.ReadObject(ms)

        Catch ex As Exception
            Return Nothing
            MsgBox(ex.ToString)
        End Try

    End Function


    Private Sub SetLocation()
        Try

            Dim nOverlay = Loader.SIS.GetInt(SIS_OT_WINDOW, 0, "_nOverlay&")
            Loader.SIS.CreateInternalOverlay(GeocodeResponse.results(0).formatted_address, nOverlay)
            Loader.SIS.SetFlt(SIS_OT_OVERLAY, nOverlay, "GPointScaleOverride#", -1)

            nOverlay += 1
            Loader.SIS.CreateInternalOverlay("tmp", nOverlay)
            Loader.SIS.SetDatasetPrj(Loader.SIS.GetInt(SIS_OT_OVERLAY, nOverlay, "_nDataset&"), "Latitude/Longitude.OGC.WGS_1984")
            Loader.SIS.SetInt(SIS_OT_WINDOW, 0, "_nDefaultOverlay&", nOverlay)
            Loader.SIS.CreatePoint(0, 0, 0, "Marker", 0, 1)
            Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLat#", GeocodeResponse.results(0).geometry.location.lat)
            Loader.SIS.SetFlt(SIS_OT_CURITEM, 0, "_oLon#", GeocodeResponse.results(0).geometry.location.lng)

            For i = 0 To GeocodeResponse.results(0).address_components.Length - 1
                If Not GeocodeResponse.results(0).address_components(i).types.Length = 0 Then
                    Select Case GeocodeResponse.results(0).address_components(i).types(0)
                        Case "route"
                            Loader.SIS.SetStr(SIS_OT_CURITEM, 0, "route$", GeocodeResponse.results(0).address_components(i).long_name)
                        Case "locality"
                            Loader.SIS.SetStr(SIS_OT_CURITEM, 0, "locality$", GeocodeResponse.results(0).address_components(i).long_name)
                        Case "administrative_area_level_2"
                            Loader.SIS.SetStr(SIS_OT_CURITEM, 0, "administrative_area_level_2$", GeocodeResponse.results(0).address_components(i).long_name)
                        Case "administrative_area_level_1"
                            Loader.SIS.SetStr(SIS_OT_CURITEM, 0, "administrative_area_level_1$", GeocodeResponse.results(0).address_components(i).long_name)
                        Case "postal_code_prefix"
                            Loader.SIS.SetStr(SIS_OT_CURITEM, 0, "postal_code_prefix$", GeocodeResponse.results(0).address_components(i).long_name)
                    End Select
                End If
            Next

            Loader.SIS.UpdateItem()
            Loader.SIS.AddToList("Location")
            Loader.SIS.SetInt(SIS_OT_WINDOW, 0, "_nDefaultOverlay&", nOverlay - 1)
            Loader.SIS.CopyListItems("Location")
            Loader.SIS.EmptyList("Location")
            Loader.SIS.CreateListFromOverlay(nOverlay - 1, "Overlay")
            Loader.SIS.SelectList("Overlay")
            Loader.SIS.DoCommand("AComZoomSelect")
            Loader.SIS.RemoveOverlay(nOverlay)
            Loader.SIS.Dispose()
            Loader.SIS = Nothing

        Catch ex As Exception
            MsgBox(ex.ToString)
            Loader.SIS.Dispose()
            Loader.SIS = Nothing
        End Try

    End Sub

End Class


Public Class GoogleGeocodeResponse

    Public Property status() As String
        Get
            Return m_status
        End Get
        Set(ByVal value As String)
            m_status = value
        End Set
    End Property
    Private m_status As String

    Public Property results() As results()
        Get
            Return m_results
        End Get
        Set(ByVal value As results())
            m_results = value
        End Set
    End Property
    Private m_results As results()

End Class


Public Class results

    Public Property formatted_address() As String
        Get
            Return m_formatted_address
        End Get
        Set(ByVal value As String)
            m_formatted_address = value
        End Set
    End Property
    Private m_formatted_address As String

    Public Property geometry() As geometry
        Get
            Return m_geometry
        End Get
        Set(ByVal value As geometry)
            m_geometry = value
        End Set
    End Property
    Private m_geometry As geometry

    Public Property address_components() As address_component()
        Get
            Return m_address_components
        End Get
        Set(ByVal value As address_component())
            m_address_components = value
        End Set
    End Property
    Private m_address_components As address_component()

End Class

Public Class geometry

    Public Property location() As location
        Get
            Return m_location
        End Get
        Set(ByVal value As location)
            m_location = value
        End Set
    End Property
    Private m_location As location

End Class

Public Class location

    Public Property lat() As String
        Get
            Return m_lat
        End Get
        Set(ByVal value As String)
            m_lat = value
        End Set
    End Property
    Private m_lat As String

    Public Property lng() As String
        Get
            Return m_lng
        End Get
        Set(ByVal value As String)
            m_lng = value
        End Set
    End Property
    Private m_lng As String

End Class

Public Class address_component

    Public Property long_name() As String
        Get
            Return m_long_name
        End Get
        Set(ByVal value As String)
            m_long_name = value
        End Set
    End Property
    Private m_long_name As String

    Public Property types() As String()
        Get
            Return m_types
        End Get
        Set(ByVal value As String())
            m_types = value
        End Set
    End Property
    Private m_types As String()

End Class