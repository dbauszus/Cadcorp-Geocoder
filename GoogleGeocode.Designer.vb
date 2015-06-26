<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GoogleGeocode
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GoogleGeocode))
        Me.TB_ADDRESS = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'TB_ADDRESS
        '
        Me.TB_ADDRESS.Location = New System.Drawing.Point(9, 10)
        Me.TB_ADDRESS.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TB_ADDRESS.Name = "TB_ADDRESS"
        Me.TB_ADDRESS.Size = New System.Drawing.Size(889, 22)
        Me.TB_ADDRESS.TabIndex = 2
        '
        'GoogleGeocode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(916, 48)
        Me.Controls.Add(Me.TB_ADDRESS)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "GoogleGeocode"
        Me.Text = "Google Address Search"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TB_ADDRESS As System.Windows.Forms.TextBox
End Class
