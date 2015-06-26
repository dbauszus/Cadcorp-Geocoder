Imports Cadcorp.SIS.GisLink.Library
Imports Cadcorp.SIS.GisLink.Library.Constants
Imports System.IO
Imports System.Windows.Forms

<GisLinkProgram("Geocoder")> _
Public Class Loader

    Private Shared APP As SisApplication
    Private Shared _sis As MapModeller

    Public Shared Property SIS As MapModeller
        Get
            If _sis Is Nothing Then _sis = APP.TakeoverMapManager
            Return _sis
        End Get
        Set(ByVal value As MapModeller)
            _sis = value
        End Set
    End Property

    Public Sub New(ByVal SISApplication As SisApplication)
        APP = SISApplication

        Dim group As SisRibbonGroup = APP.RibbonGroup
        group.Text = "Geocoder"

        Dim BtnGoogleFreeSearch As SisRibbonButton = New SisRibbonButton("", New SisClickHandler(AddressOf GoogleFreeSearch))
        BtnGoogleFreeSearch.LargeImage = True
        BtnGoogleFreeSearch.Icon = My.Resources.googlegeocode
        BtnGoogleFreeSearch.Help = "Google Free Search"
        BtnGoogleFreeSearch.MinSelection = -1
        group.Controls.Add(BtnGoogleFreeSearch)

        Dim BtnGoogleRoute As SisRibbonButton = New SisRibbonButton("", New SisClickHandler(AddressOf GoogleRouting))
        BtnGoogleRoute.LargeImage = True
        BtnGoogleRoute.Icon = My.Resources.googlegeocode
        BtnGoogleRoute.Help = "Google Routing"
        BtnGoogleRoute.MinSelection = -1
        group.Controls.Add(BtnGoogleRoute)

    End Sub

    Private Sub GoogleFreeSearch(ByVal sender As Object, ByVal e As SisClickArgs)
        e.MapModeller.Dispose()
        Try
            Dim googleGeocodeDialog As New GoogleGeocode()
            googleGeocodeDialog.StartPosition = FormStartPosition.CenterScreen
            googleGeocodeDialog.TopMost = True
            googleGeocodeDialog.Show()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GoogleRouting(ByVal sender As Object, ByVal e As SisClickArgs)
        Try
            Loader.SIS = e.MapModeller
            Dim googleRouting As New GoogleRouting()
            googleRouting.Routing()
            Loader.SIS.Dispose()
            Loader.SIS = Nothing
        Catch ex As Exception
        End Try
    End Sub

End Class