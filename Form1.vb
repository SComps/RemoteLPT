' *** REMOTELPT/Scott Johnson westdalefarmer@gmail.com


Imports System.Configuration
Imports System.Data.Common
Imports System.Diagnostics.SymbolStore
Imports System.Drawing.Printing
Imports System.Net.Configuration
Imports System.Net.Security
Imports System.Security
Imports System.Security.Permissions
Imports System.Threading
Imports Microsoft.Win32
Imports nsoftware.IPWorks

Public Class Form1

    Dim myRemoteHost As String = ""
    Dim myRemotePort As String = ""
    Dim Settled As Boolean = False
    Dim ConnectedAt As DateTime = #00:00#
    Dim LastData As DateTime = #00:00#
    Dim CollectingDocument As Boolean = False
    Dim myDocument As New List(Of String)
    Public WithEvents pd As New PrintDocument
    Public DefaultPrinter As String = pd.PrinterSettings.PrinterName
    Public DefaultPage As PageSettings = pd.DefaultPageSettings
    Public DefaultSettings As PrinterSettings = pd.PrinterSettings
    Public FontList As List(Of String)
    Public DefaultFontFamily As String = "Consolas"
    Public DefaultFontSize As Single = 7.15
    Public SelectedPrinter As String
    Public SelectedFontFamily As String
    Public SelectedFontSize As Single
    Public DocumentBuffer As String = ""
    Public regKey As RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software\RemoteLPT", True)
    Public PDFPath As String = System.Environment.CurrentDirectory

    Sub LoadRegistry()
        If regKey Is Nothing Then
            regKey = Registry.CurrentUser.CreateSubKey("Software\RemoteLPT", True)
        End If
        Dim regRemoteHost As String = regKey.GetValue("Remote_Host", "")
        Dim regPort As String = regKey.GetValue("Remote_Port", "")
        Dim regPrinter As String = regKey.GetValue("Output_Device", "")
        Dim regFont As String = regKey.GetValue("Output_Font", "")
        Dim regFontSize As String = regKey.GetValue("Font_Size", "")
        Dim regReconnect As Boolean = regKey.GetValue("Auto_Reconnect", False)
        Dim regLinesPage As Integer = regKey.GetValue("Lines_Page", 66)
        Dim regShowData As Boolean = regKey.GetValue("Show_Data", True)
        Dim regUpperOnly As Boolean = regKey.GetValue("Upper_Only", False)
        Dim regPDFPath As String = regKey.GetValue("PDF_Path", System.Environment.CurrentDirectory)

        If regRemoteHost <> "" Then myRemoteHost = regRemoteHost

        If regPort <> "" Then myRemotePort = regPort

        If regPrinter <> "" Then DefaultPrinter = regPrinter

        If regFont <> "" Then DefaultFontFamily = regFont

        If regFontSize <> "" Then DefaultFontSize = regFontSize

        If regLinesPage <> 0 Then txtLinesPage.Text = regLinesPage

        If regPDFPath <> "" Then
            PDFPath = regPDFPath
            LogBox.AppendText(String.Format("Setting document storage directory to: {0}" & vbCrLf, PDFPath))
        End If

        verboseLog.Checked = regShowData

        UpperOnly.Checked = regUpperOnly

        retryConnect.Checked = regReconnect

    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadRegistry()


        LogBox.AppendText("Initializing printer..." & vbCrLf)
        Dim myPrinters As List(Of String) = GetPrinters()
        Printers.Items.Clear()
        For Each p As String In myPrinters
            Printers.Items.Add(p)
        Next
        Dim printerItem As Integer = 0
        For Each p As String In Printers.Items
            If p = DefaultPrinter Then
                Printers.SelectedIndex = printerItem
                Exit For
            End If
            printerItem += 1
        Next
        FontList = EnumerateFonts()
        InstalledFonts.Items.Clear()

        For Each s As String In FontList
            InstalledFonts.Items.Add(s)
        Next
        Dim FontItem As Integer = 0
        For Each ff As String In InstalledFonts.Items
            If ff = DefaultFontFamily Then
                InstalledFonts.SelectedIndex = FontItem
            End If
            FontItem += 1
        Next
        txtFontSize.Text = DefaultFontSize
        If myRemoteHost = "" Then
            LogBox.AppendText("Remote host not defined, Assuming 192.168.1.24." & vbCrLf)
            myRemoteHost = "192.168.1.24"
        End If
        If myRemotePort = "" Then
            LogBox.AppendText("Remote port not defined.  Assuming 9110" & vbCrLf)
            myRemotePort = 9110
        End If
        txtRemoteHost.Text = myRemoteHost
        txtRemotePort.Text = myRemotePort
        connTimer.Stop()
        retryTimer.Start()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnRemote.Click
        If RemPort.Connected = False Then
            If myRemoteHost <> txtRemoteHost.Text.Trim Then
                myRemoteHost = txtRemoteHost.Text.Trim
                LogBox.AppendText("Setting remote host to " & myRemoteHost & vbCrLf)
            End If
            If myRemotePort <> txtRemotePort.Text.Trim Then
                myRemotePort = txtRemotePort.Text.Trim
                LogBox.AppendText("Setting remote port to " & myRemotePort & vbCrLf)
            End If
            Dim txt As String = "Tring to connect to {0} on port {1}" & vbCrLf
            LogBox.AppendText(String.Format(txt, myRemoteHost, myRemotePort))
            RemPort.RemoteHost = myRemoteHost
            RemPort.RemotePort = myRemotePort

            RemPort.Connect(myRemoteHost, myRemotePort)
            regKey.SetValue("Remote_Host", myRemoteHost)
            regKey.SetValue("Remote_Port", myRemotePort)
        Else
            RemPort.Disconnect()
            LogBox.AppendText("Disconnecting From Remote host" & vbCrLf)
            Settled = False
            ' We were connected, but now we're not.  Must resettle the
            ' connection, FreeAXP in particular outputs a banner that
            ' we don't want to print.
        End If

    End Sub


    Private Sub RemPort_OnConnected(sender As Object, e As IpportConnectedEventArgs) Handles RemPort.OnConnected
        LogBox.AppendText("Connected to remote port" & vbCrLf)
        btnRemote.Text = "Stop"
        ConnectedAt = Now
        LastData = Now
        RemPort.EOL = ""
        connTimer.Start()
    End Sub

    Private Sub RemPort_OnDataIn(sender As Object, e As IpportDataInEventArgs) Handles RemPort.OnDataIn
        Dim CountIn As Integer = e.TextB.Length
        'LogBox.AppendText(String.Format("{0} bytes received in this block." & vbCrLf, CountIn))
        Dim InboundData As String = e.Text

        If (Settled = True) And (CollectingDocument = False) Then
            CollectingDocument = True
            LogBox.AppendText("*****" & vbCrLf)
            LogBox.AppendText("COLLECTING DOCUMENT FOR PRINTING" & vbCrLf)
            DocumentBuffer = ""       ' Clear the document Buffer
        End If

        If CollectingDocument Then
            If UpperOnly.Checked Then
                InboundData = InboundData.ToUpper
            End If
            DocumentBuffer = DocumentBuffer & InboundData      ' We're not concerned about line endings at this point.
            If verboseLog.Checked Then
                Dim outText As String = "{0}"
                LogBox.AppendText(String.Format(outText, InboundData))
            End If
        End If
        LastData = Now
    End Sub

    Private Sub RemPort_OnDisconnected(sender As Object, e As IpportDisconnectedEventArgs) Handles RemPort.OnDisconnected
        LogBox.AppendText("Disconnected from remote host" & vbCrLf)
        btnRemote.Text = "Start"
        connTimer.Stop()
    End Sub

    Private Sub connTimer_Tick(sender As Object, e As EventArgs) Handles connTimer.Tick
        connTimer.Stop()
        RemPort.DoEvents()
        If RemPort.Connected = True Then
            Dim Elapsed As TimeSpan = Now.Subtract(ConnectedAt)
            Dim SettleTime As TimeSpan = Now.Subtract(LastData)
            If Settled = True Then
                If CollectingDocument = True Then
                    If SettleTime.Seconds > 2 Then
                        ' We've heard nothing from the remote for 5 seconds.
                        CollectingDocument = False
                        LogBox.AppendText("Document ready to print." & vbCrLf)
                        myDocument = SplitBuffer()
                        Dim outDoc As List(Of String) = SeparateLines(myDocument)

                        SpoolJob(outDoc)
                    End If
                End If
            End If
            If (SettleTime.Seconds > 5) And (Not Settled) Then      ' Once connected 2 seconds should be fine, 5 initial
                Settled = True
                LogBox.AppendText("Connection Settled." & vbCrLf)
            End If

            Dim stat As String = "Connected {0} days, {1} hours, {2} minutes, {3} seconds."
            connStatus.Text = String.Format(stat, Elapsed.Days, Elapsed.Hours, Elapsed.Minutes, Elapsed.Seconds)
        Else
            connStatus.Text = "Not connected."
        End If

        connTimer.Start()
    End Sub

    Private Sub LogBox_TextChanged(sender As Object, e As EventArgs) Handles LogBox.TextChanged
        LogBox.ScrollToCaret()

    End Sub

    Private Sub SpoolJob(job As List(Of String))
        Dim ph As New PrintHelper
        Dim docname As String = String.Format("{0}\{1}_{2}", PDFPath, myRemoteHost.Replace(".", "-"), Now.Ticks)
        Me.LogBox.AppendText(docname)
        LogBox.AppendText("Set output file to " & docname & vbCrLf)
        ph.DestinationPrinter = SelectedPrinter
        ph.FontFamily = SelectedFontFamily
        ph.FontSize = txtFontSize.Text
        ph.prt(job, SelectedPrinter, docname)
        myDocument.Clear()
        If SelectedPrinter.Contains("PDF") Then
            LogBox.AppendText(String.Format("Printed output to {0}" & vbCrLf, docname & ".pdf"))
        End If
    End Sub


    Private Function GetPrinters() As List(Of String)
        Dim myPrinters As PrinterSettings.StringCollection
        Dim PrinterList As New List(Of String)
        myPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters
        For Each p As String In myPrinters
            PrinterList.Add(p)
        Next
        Return PrinterList
    End Function

    Private Sub retryTimer_Tick(sender As Object, e As EventArgs) Handles retryTimer.Tick
        retryTimer.Enabled = False

        If Not RemPort.Connected Then
            If retryConnect.Checked Then
                RemPort.RemoteHost = txtRemoteHost.Text
                RemPort.RemotePort = txtRemotePort.Text
                RemPort.Connected = True
            End If
        End If
        retryTimer.Enabled = True
    End Sub

    Private Function EnumerateFonts() As List(Of String)
        Dim Fonts As New List(Of String)
        Dim FontList() As FontFamily = FontFamily.Families
        For Each ff As FontFamily In FontList
            'LogBox.AppendText(String.Format("Enumerating font {0}" & vbCrLf, ff.Name))
            Fonts.Add(ff.Name)
        Next
        Return Fonts
    End Function

    Private Sub Printers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Printers.SelectedIndexChanged
        SelectedPrinter = Printers.SelectedItem
        LogBox.AppendText(String.Format("Selected printer {0}" & vbCrLf, SelectedPrinter))
        regKey.SetValue("Output_Device", SelectedPrinter)
        If SelectedPrinter.Contains("PDF") Then
            btnPDFPath.Visible = True

        End If
    End Sub

    Private Sub InstalledFonts_SelectedIndexChanged(sender As Object, e As EventArgs) Handles InstalledFonts.SelectedIndexChanged
        SelectedFontFamily = InstalledFonts.SelectedItem
        LogBox.AppendText(String.Format("Selected {0} as output font." & vbCrLf, SelectedFontFamily))
        regKey.SetValue("Output_Font", SelectedFontFamily)
        regKey.SetValue("Font_Size", txtFontSize.Text)
    End Sub

    Private Sub retryConnect_CheckedChanged(sender As Object, e As EventArgs) Handles retryConnect.CheckedChanged
        If regKey Is Nothing Then Exit Sub
        regKey.SetValue("Auto_Reconnect", retryConnect.Checked)

    End Sub

    Private Sub verboseLog_CheckedChanged(sender As Object, e As EventArgs) Handles verboseLog.CheckedChanged
        If regKey Is Nothing Then Exit Sub
        regKey.SetValue("Show_Data", verboseLog.Checked)
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles txtLinesPage.TextChanged
        If txtFontSize.Text = "" Then txtFontSize.Text = DefaultFontSize
        If regKey Is Nothing Then Exit Sub
        regKey.SetValue("Font_Size", CSng(txtFontSize.Text))
    End Sub

    Private Sub txtRemoteHost_TextChanged(sender As Object, e As EventArgs) Handles txtRemoteHost.TextChanged
        If regKey Is Nothing Then Exit Sub
        regKey.SetValue("Remote_Host", txtRemoteHost.Text)
    End Sub

    Private Sub txtRemotePort_TextChanged(sender As Object, e As EventArgs) Handles txtRemotePort.TextChanged
        If regKey Is Nothing Then Exit Sub
        regKey.SetValue("Remote_Port", txtRemotePort.Text)
    End Sub

    Public Function SplitBuffer() As List(Of String)
        Dim myLines As String() = DocumentBuffer.Split(vbCrLf)
        myDocument.Clear()
        For Each l As String In myLines
            l = l.Replace(vbCr, "")
            l = l.Replace(vbLf, "")
            myDocument.Add(l)
        Next

        Return myDocument
    End Function
    Public Function SeparateLines(inDoc As List(Of String)) As List(Of String)
        Dim outDoc As New List(Of String)
        Dim crlfPos As Integer = 0
        Dim LineCount As Integer = inDoc.Count
        LogBox.AppendText(String.Format("Processing {0} lines of the input document, seeking individual lines" & vbCrLf, LineCount))
        For thisLine As Integer = 0 To LineCount - 1 ' The list is 0 based, so subtract 1 from the count for the last element.
            Dim individualLines As String()
            individualLines = inDoc(thisLine).Split(vbCrLf)

            Dim subLineCount As Integer = individualLines.Count

            ' It shouldn't really happen, but JUST IN CASE there's more than one line present here.
            For Each inLine As String In individualLines
                inLine = inLine.Replace(vbCrLf, "")     ' Strip off the CRLF, we don't need it anymore
                outDoc.Add(String.Format("{0}", inLine))
            Next
        Next thisLine

        Return outDoc
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnPrinter.Click
        Dim resp As DialogResult
        resp = dlgPrint.ShowDialog
        Stop
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btnPage.Click
        Dim resp As DialogResult
        resp = dlgPage.ShowDialog
        Stop
    End Sub

    Private Sub UpperOnly_CheckedChanged(sender As Object, e As EventArgs) Handles UpperOnly.CheckedChanged
        regKey.SetValue("Upper_Only", UpperOnly.Checked)
    End Sub

    Private Sub btnPDFPath_Click(sender As Object, e As EventArgs) Handles btnPDFPath.Click
        BrowsePDFPath.SelectedPath = PDFPath
        Dim result As New DialogResult
        result = BrowsePDFPath.ShowDialog
        If result = DialogResult.OK Then
            PDFPath = BrowsePDFPath.SelectedPath
            regKey.SetValue("PDF_Path", PDFPath)
        End If

    End Sub
End Class

