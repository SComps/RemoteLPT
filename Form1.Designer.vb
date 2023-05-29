<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.lblRemote = New System.Windows.Forms.Label()
        Me.txtRemoteHost = New System.Windows.Forms.TextBox()
        Me.txtRemotePort = New System.Windows.Forms.TextBox()
        Me.lblHost = New System.Windows.Forms.Label()
        Me.lblPort = New System.Windows.Forms.Label()
        Me.retryConnect = New System.Windows.Forms.CheckBox()
        Me.btnRemote = New System.Windows.Forms.Button()
        Me.lblLocalPrinter = New System.Windows.Forms.Label()
        Me.Printers = New System.Windows.Forms.ComboBox()
        Me.btnPrinter = New System.Windows.Forms.Button()
        Me.btnPage = New System.Windows.Forms.Button()
        Me.InstalledFonts = New System.Windows.Forms.ComboBox()
        Me.lblFont = New System.Windows.Forms.Label()
        Me.txtLinesPage = New System.Windows.Forms.TextBox()
        Me.lblLinesPage = New System.Windows.Forms.Label()
        Me.txtFontSize = New System.Windows.Forms.TextBox()
        Me.lblSize = New System.Windows.Forms.Label()
        Me.LogBox = New System.Windows.Forms.RichTextBox()
        Me.lblLog = New System.Windows.Forms.Label()
        Me.RemPort = New nsoftware.IPWorks.Ipport(Me.components)
        Me.connTimer = New System.Windows.Forms.Timer(Me.components)
        Me.strip = New System.Windows.Forms.StatusStrip()
        Me.connStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.verboseLog = New System.Windows.Forms.CheckBox()
        Me.retryTimer = New System.Windows.Forms.Timer(Me.components)
        Me.dlgPage = New System.Windows.Forms.PageSetupDialog()
        Me.dlgPrint = New System.Windows.Forms.PrintDialog()
        Me.UpperOnly = New System.Windows.Forms.CheckBox()
        Me.btnPDFPath = New System.Windows.Forms.Button()
        Me.BrowsePDFPath = New System.Windows.Forms.FolderBrowserDialog()
        Me.strip.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblRemote
        '
        Me.lblRemote.AutoSize = True
        Me.lblRemote.Location = New System.Drawing.Point(12, 9)
        Me.lblRemote.Name = "lblRemote"
        Me.lblRemote.Size = New System.Drawing.Size(47, 13)
        Me.lblRemote.TabIndex = 0
        Me.lblRemote.Text = "Remote:"
        '
        'txtRemoteHost
        '
        Me.txtRemoteHost.Location = New System.Drawing.Point(53, 34)
        Me.txtRemoteHost.Name = "txtRemoteHost"
        Me.txtRemoteHost.Size = New System.Drawing.Size(100, 20)
        Me.txtRemoteHost.TabIndex = 1
        '
        'txtRemotePort
        '
        Me.txtRemotePort.Location = New System.Drawing.Point(191, 34)
        Me.txtRemotePort.Name = "txtRemotePort"
        Me.txtRemotePort.Size = New System.Drawing.Size(50, 20)
        Me.txtRemotePort.TabIndex = 2
        '
        'lblHost
        '
        Me.lblHost.AutoSize = True
        Me.lblHost.Location = New System.Drawing.Point(15, 37)
        Me.lblHost.Name = "lblHost"
        Me.lblHost.Size = New System.Drawing.Size(32, 13)
        Me.lblHost.TabIndex = 3
        Me.lblHost.Text = "Host:"
        '
        'lblPort
        '
        Me.lblPort.AutoSize = True
        Me.lblPort.Location = New System.Drawing.Point(159, 37)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(29, 13)
        Me.lblPort.TabIndex = 4
        Me.lblPort.Text = "Port:"
        '
        'retryConnect
        '
        Me.retryConnect.AutoSize = True
        Me.retryConnect.Location = New System.Drawing.Point(15, 60)
        Me.retryConnect.Name = "retryConnect"
        Me.retryConnect.Size = New System.Drawing.Size(121, 17)
        Me.retryConnect.TabIndex = 5
        Me.retryConnect.Text = "Retry on disconnect"
        Me.retryConnect.UseVisualStyleBackColor = True
        '
        'btnRemote
        '
        Me.btnRemote.Location = New System.Drawing.Point(297, 32)
        Me.btnRemote.Name = "btnRemote"
        Me.btnRemote.Size = New System.Drawing.Size(75, 23)
        Me.btnRemote.TabIndex = 6
        Me.btnRemote.Text = "Start"
        Me.btnRemote.UseVisualStyleBackColor = True
        '
        'lblLocalPrinter
        '
        Me.lblLocalPrinter.AutoSize = True
        Me.lblLocalPrinter.Location = New System.Drawing.Point(15, 98)
        Me.lblLocalPrinter.Name = "lblLocalPrinter"
        Me.lblLocalPrinter.Size = New System.Drawing.Size(66, 13)
        Me.lblLocalPrinter.TabIndex = 7
        Me.lblLocalPrinter.Text = "Local Printer"
        '
        'Printers
        '
        Me.Printers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.Printers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.Printers.FormattingEnabled = True
        Me.Printers.Location = New System.Drawing.Point(18, 114)
        Me.Printers.Name = "Printers"
        Me.Printers.Size = New System.Drawing.Size(247, 21)
        Me.Printers.TabIndex = 8
        '
        'btnPrinter
        '
        Me.btnPrinter.Location = New System.Drawing.Point(18, 141)
        Me.btnPrinter.Name = "btnPrinter"
        Me.btnPrinter.Size = New System.Drawing.Size(75, 23)
        Me.btnPrinter.TabIndex = 9
        Me.btnPrinter.Text = "Printer "
        Me.btnPrinter.UseVisualStyleBackColor = True
        '
        'btnPage
        '
        Me.btnPage.Location = New System.Drawing.Point(99, 141)
        Me.btnPage.Name = "btnPage"
        Me.btnPage.Size = New System.Drawing.Size(75, 23)
        Me.btnPage.TabIndex = 10
        Me.btnPage.Text = "Page"
        Me.btnPage.UseVisualStyleBackColor = True
        '
        'InstalledFonts
        '
        Me.InstalledFonts.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.InstalledFonts.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.InstalledFonts.FormattingEnabled = True
        Me.InstalledFonts.Location = New System.Drawing.Point(21, 187)
        Me.InstalledFonts.Name = "InstalledFonts"
        Me.InstalledFonts.Size = New System.Drawing.Size(247, 21)
        Me.InstalledFonts.TabIndex = 11
        '
        'lblFont
        '
        Me.lblFont.AutoSize = True
        Me.lblFont.Location = New System.Drawing.Point(15, 167)
        Me.lblFont.Name = "lblFont"
        Me.lblFont.Size = New System.Drawing.Size(28, 13)
        Me.lblFont.TabIndex = 12
        Me.lblFont.Text = "Font"
        '
        'txtLinesPage
        '
        Me.txtLinesPage.Location = New System.Drawing.Point(271, 114)
        Me.txtLinesPage.Name = "txtLinesPage"
        Me.txtLinesPage.Size = New System.Drawing.Size(50, 20)
        Me.txtLinesPage.TabIndex = 13
        Me.txtLinesPage.Text = "66"
        '
        'lblLinesPage
        '
        Me.lblLinesPage.AutoSize = True
        Me.lblLinesPage.Location = New System.Drawing.Point(327, 117)
        Me.lblLinesPage.Name = "lblLinesPage"
        Me.lblLinesPage.Size = New System.Drawing.Size(62, 13)
        Me.lblLinesPage.TabIndex = 14
        Me.lblLinesPage.Text = "Lines/Page"
        '
        'txtFontSize
        '
        Me.txtFontSize.Location = New System.Drawing.Point(309, 188)
        Me.txtFontSize.Name = "txtFontSize"
        Me.txtFontSize.Size = New System.Drawing.Size(50, 20)
        Me.txtFontSize.TabIndex = 15
        '
        'lblSize
        '
        Me.lblSize.AutoSize = True
        Me.lblSize.Location = New System.Drawing.Point(273, 191)
        Me.lblSize.Name = "lblSize"
        Me.lblSize.Size = New System.Drawing.Size(30, 13)
        Me.lblSize.TabIndex = 16
        Me.lblSize.Text = "Size:"
        '
        'LogBox
        '
        Me.LogBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogBox.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LogBox.Location = New System.Drawing.Point(18, 263)
        Me.LogBox.Name = "LogBox"
        Me.LogBox.Size = New System.Drawing.Size(396, 237)
        Me.LogBox.TabIndex = 18
        Me.LogBox.Text = ""
        Me.LogBox.WordWrap = False
        '
        'lblLog
        '
        Me.lblLog.AutoSize = True
        Me.lblLog.Location = New System.Drawing.Point(15, 247)
        Me.lblLog.Name = "lblLog"
        Me.lblLog.Size = New System.Drawing.Size(25, 13)
        Me.lblLog.TabIndex = 19
        Me.lblLog.Text = "Log"
        '
        'RemPort
        '
        Me.RemPort.About = "IPWorks 2020 [Build 7239]"
        Me.RemPort.InvokeThrough = Me
        '
        'connTimer
        '
        Me.connTimer.Interval = 250
        '
        'strip
        '
        Me.strip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.connStatus})
        Me.strip.Location = New System.Drawing.Point(0, 507)
        Me.strip.Name = "strip"
        Me.strip.Size = New System.Drawing.Size(426, 22)
        Me.strip.TabIndex = 20
        Me.strip.Text = "StatusStrip1"
        '
        'connStatus
        '
        Me.connStatus.Name = "connStatus"
        Me.connStatus.Size = New System.Drawing.Size(411, 17)
        Me.connStatus.Spring = True
        Me.connStatus.Text = "Not Connected"
        Me.connStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'verboseLog
        '
        Me.verboseLog.AutoSize = True
        Me.verboseLog.Checked = True
        Me.verboseLog.CheckState = System.Windows.Forms.CheckState.Checked
        Me.verboseLog.Location = New System.Drawing.Point(297, 243)
        Me.verboseLog.Name = "verboseLog"
        Me.verboseLog.Size = New System.Drawing.Size(112, 17)
        Me.verboseLog.TabIndex = 21
        Me.verboseLog.Text = "Show printed data"
        Me.verboseLog.UseVisualStyleBackColor = True
        '
        'retryTimer
        '
        Me.retryTimer.Interval = 5000
        '
        'dlgPrint
        '
        Me.dlgPrint.UseEXDialog = True
        '
        'UpperOnly
        '
        Me.UpperOnly.AutoSize = True
        Me.UpperOnly.Location = New System.Drawing.Point(21, 214)
        Me.UpperOnly.Name = "UpperOnly"
        Me.UpperOnly.Size = New System.Drawing.Size(94, 17)
        Me.UpperOnly.TabIndex = 22
        Me.UpperOnly.Text = "No lower case"
        Me.UpperOnly.UseVisualStyleBackColor = True
        '
        'btnPDFPath
        '
        Me.btnPDFPath.Location = New System.Drawing.Point(180, 141)
        Me.btnPDFPath.Name = "btnPDFPath"
        Me.btnPDFPath.Size = New System.Drawing.Size(75, 23)
        Me.btnPDFPath.TabIndex = 23
        Me.btnPDFPath.Text = "PDF Path"
        Me.btnPDFPath.UseVisualStyleBackColor = True
        Me.btnPDFPath.Visible = False
        '
        'Form1
        '
        Me.AcceptButton = Me.btnRemote
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(426, 529)
        Me.Controls.Add(Me.btnPDFPath)
        Me.Controls.Add(Me.UpperOnly)
        Me.Controls.Add(Me.verboseLog)
        Me.Controls.Add(Me.strip)
        Me.Controls.Add(Me.lblLog)
        Me.Controls.Add(Me.LogBox)
        Me.Controls.Add(Me.lblSize)
        Me.Controls.Add(Me.txtFontSize)
        Me.Controls.Add(Me.lblLinesPage)
        Me.Controls.Add(Me.txtLinesPage)
        Me.Controls.Add(Me.lblFont)
        Me.Controls.Add(Me.InstalledFonts)
        Me.Controls.Add(Me.btnPage)
        Me.Controls.Add(Me.btnPrinter)
        Me.Controls.Add(Me.Printers)
        Me.Controls.Add(Me.lblLocalPrinter)
        Me.Controls.Add(Me.btnRemote)
        Me.Controls.Add(Me.retryConnect)
        Me.Controls.Add(Me.lblPort)
        Me.Controls.Add(Me.lblHost)
        Me.Controls.Add(Me.txtRemotePort)
        Me.Controls.Add(Me.txtRemoteHost)
        Me.Controls.Add(Me.lblRemote)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Remote Terminal LPT"
        Me.strip.ResumeLayout(False)
        Me.strip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblRemote As Label
    Friend WithEvents txtRemoteHost As TextBox
    Friend WithEvents txtRemotePort As TextBox
    Friend WithEvents lblHost As Label
    Friend WithEvents lblPort As Label
    Friend WithEvents retryConnect As CheckBox
    Friend WithEvents btnRemote As Button
    Friend WithEvents lblLocalPrinter As Label
    Friend WithEvents Printers As ComboBox
    Friend WithEvents btnPrinter As Button
    Friend WithEvents btnPage As Button
    Friend WithEvents InstalledFonts As ComboBox
    Friend WithEvents lblFont As Label
    Friend WithEvents txtLinesPage As TextBox
    Friend WithEvents lblLinesPage As Label
    Friend WithEvents txtFontSize As TextBox
    Friend WithEvents lblSize As Label
    Friend WithEvents LogBox As RichTextBox
    Friend WithEvents lblLog As Label
    Friend WithEvents RemPort As nsoftware.IPWorks.Ipport
    Friend WithEvents connTimer As Timer
    Friend WithEvents strip As StatusStrip
    Friend WithEvents connStatus As ToolStripStatusLabel
    Friend WithEvents verboseLog As CheckBox
    Friend WithEvents retryTimer As Timer
    Friend WithEvents dlgPage As PageSetupDialog
    Friend WithEvents dlgPrint As PrintDialog
    Friend WithEvents UpperOnly As CheckBox
    Friend WithEvents btnPDFPath As Button
    Friend WithEvents BrowsePDFPath As FolderBrowserDialog
End Class
