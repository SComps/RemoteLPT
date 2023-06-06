' *** REMOTELPT/Scott Johnson westdalefarmer@gmail.com


Imports System.Drawing.Printing
Imports Microsoft.Win32
Imports nsoftware.IPWorks
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Net.Http
Imports System.Threading
Imports Microsoft.SqlServer.Server
Imports System.Runtime.InteropServices.ComTypes
Imports Microsoft.VisualBasic.Logging

Public Class Form1

    Enum Source
        REMTELNET = 1
        REMFLATFILE = 2
    End Enum




    Dim Initialized As Boolean = False
    Dim myRemoteHost As String = ""
    Dim myRemotePort As String = ""
    Dim Settled As Boolean = False
    Dim ConnectedAt As DateTime = #00:00#
    Dim LastData As DateTime = #00:00#
    Dim CollectingDocument As Boolean = False
    Dim myDocument As New List(Of String)
    Dim WithEvents pd As New PrintDocument
    Dim DefaultPrinter As String = pd.PrinterSettings.PrinterName
    Dim DefaultPage As PageSettings = pd.DefaultPageSettings
    Dim DefaultSettings As PrinterSettings = pd.PrinterSettings
    Dim FontList As List(Of String)
    Dim DefaultFontFamily As String = "Consolas"
    Dim DefaultFontSize As Single = 7.15
    Dim SelectedPrinter As String
    Dim SelectedFontFamily As String
    Dim SelectedFontSize As Single
    Dim DocumentBuffer As String = ""
    Dim regKey As RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software\RemoteLPT", True)
    Dim PDFPath As String = System.Environment.CurrentDirectory
    Dim SourceType As Integer = 1
    Dim FlatFile As String = ""
    Dim FlatFileHighWater As ULong = 0
    Dim InStream As StreamReader
    Dim lastMaxOffset As Long
    Dim fileSettle As Boolean


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
        Dim regSourceType As Integer = regKey.GetValue("SourceType", 1)
        Dim regFlatFilename As String = regKey.GetValue("FlatFilename", "")
        Dim regIgnoreExisting As Boolean = regKey.GetValue("FileIgnoreExisting", True)

        If regRemoteHost <> "" Then myRemoteHost = regRemoteHost

        If regPort <> "" Then myRemotePort = regPort

        If regPrinter <> "" Then DefaultPrinter = regPrinter

        If regFont <> "" Then DefaultFontFamily = regFont

        If regFontSize <> "" Then DefaultFontSize = regFontSize

        If regLinesPage <> 0 Then txtLinesPage.Text = regLinesPage

        If regFlatFilename <> "" Then
            txtFlatFile.Text = regFlatFilename
        End If
        If regPDFPath <> "" Then
            PDFPath = regPDFPath
            LogBox.AppendText(String.Format("Setting document storage directory to: {0}" & vbCrLf, PDFPath))
        End If

        verboseLog.Checked = regShowData

        UpperOnly.Checked = regUpperOnly

        retryConnect.Checked = regReconnect

        SourceType = regSourceType

        chkIgnoreExisting.Checked = regIgnoreExisting

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

        If SourceType = Source.REMTELNET Then
            optRemoteTelnet.Checked = True
            If myRemoteHost = "" Then
                LogBox.AppendText("Remote host not defined, Assuming 192.168.1.24." & vbCrLf)
                myRemoteHost = "192.168.1.24"
            End If
            If myRemotePort = "" Then
                LogBox.AppendText("Remote port not defined.  Assuming 9110" & vbCrLf)
                myRemotePort = 9110
            End If
        End If
        If SourceType = Source.REMFLATFILE Then
            optFlatFile.Checked = True
            If FlatFile = "" Then
                LogBox.AppendText("No remote printer file defined." & vbCrLf)
            Else
                ' Process the input file.
                ' See if it exists
                Dim IExist As Boolean = CheckFile(FlatFile)
                If Not IExist Then
                    LogBox.AppendText("Remote printer file does not exist." & vbCrLf)
                    LogBox.AppendText("No printing will take place.  Make sure your emulator" & vbCrLf)
                    LogBox.AppendText("creates the file, then restart this application." & vbCrLf)
                Else
                    InStream = New StreamReader(New FileStream(FlatFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    If chkIgnoreExisting.Checked Then
                        lastMaxOffset = InStream.BaseStream.Length
                        LogBox.AppendText(String.Format("Not printing existing {0} bytes of data." & vbCrLf, lastMaxOffset))
                    End If
                End If
            End If
        End If
        If SourceType = Source.REMTELNET Then
            txtRemoteHost.Text = myRemoteHost
            txtRemotePort.Text = myRemotePort
            connTimer.Stop()
            retryTimer.Start()
        Else
            bufferTimer.Start()
            connTimer.Start()
            retryTimer.Stop()
        End If
        Initialized = True
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
        If SourceType = Source.REMTELNET Then
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
        End If

        If SourceType = Source.REMFLATFILE Then
            Dim currentLength As Long = InStream.BaseStream.Length
            If fileSettle Then
                If currentLength > lastMaxOffset Then
                    connStatus.Text = "Collecting document."
                    LogBox.AppendText("New data waiting in output file." & vbCrLf)
                    Dim Reading As Boolean = True
                    Dim thisLine As String = ""
                    InStream.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin)
                    CollectingDocument = True

                    While (InStream.Peek() >= 0)
                        thisLine = InStream.ReadLine()
                        DocumentBuffer = DocumentBuffer & thisLine & vbCrLf
                        LogBox.AppendText(thisLine & vbCrLf)
                    End While
                    lastMaxOffset = InStream.BaseStream.Position
                    LastData = Now
                Else
                    connStatus.Text = ("Max offset is current." & vbCrLf)
                End If

                CollectingDocument = False
                LogBox.AppendText("Document ready to print." & vbCrLf)
                myDocument = SplitBuffer()
                Dim outDoc As List(Of String) = SeparateLines(myDocument)
                DocumentBuffer = ""
                myDocument.Clear()
                SpoolJob(outDoc)
                outDoc.Clear()
                CollectingDocument = False
                fileSettle = False
            End If
        End If
        connTimer.Start()
    End Sub

    Private Sub LogBox_TextChanged(sender As Object, e As EventArgs) Handles LogBox.TextChanged
        LogBox.ScrollToCaret()

    End Sub

    Private Sub SpoolJob(job As List(Of String))
        Dim ph As New PrintHelper
        Dim docname As String
        If SourceType = Source.REMTELNET Then
            docname = String.Format("{0}\{1}_{2}", PDFPath, myRemoteHost.Replace(".", "-"), Now.Ticks)
        Else
            docname = String.Format("{0}\{1}_{2}", PDFPath, "FILE_PRINTER", Now.Ticks)
        End If
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

    Private Function CheckFile(FName As String) As Boolean
        Return File.Exists(FName)
    End Function

    Private Sub optRemoteTelnet_CheckedChanged(sender As Object, e As EventArgs) Handles optRemoteTelnet.CheckedChanged
        If Not Initialized Then Exit Sub

        If optRemoteTelnet.Checked Then
            SourceType = Source.REMTELNET
            LogBox.AppendText("Selecting Remote Telnet connection." & vbCrLf)
        Else
            SourceType = Source.REMFLATFILE
        End If
        regKey.SetValue("SourceType", SourceType)
    End Sub

    Private Sub optFlatFile_CheckedChanged(sender As Object, e As EventArgs) Handles optFlatFile.CheckedChanged
        If Not Initialized Then Exit Sub

        If optFlatFile.Checked Then
            SourceType = Source.REMFLATFILE
            LogBox.AppendText("Selecting Remote flat file." & vbCrLf)
        Else
            SourceType = Source.REMTELNET
        End If
        regKey.SetValue("SourceType", SourceType)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Dim resp As DialogResult
        dlgFlatFile.AddExtension = True
        dlgFlatFile.InitialDirectory = regKey.GetValue("FlatFileDirectory", System.Environment.CurrentDirectory)
        dlgFlatFile.DefaultExt = ".txt"
        dlgFlatFile.SupportMultiDottedExtensions = True
        dlgFlatFile.Multiselect = False
        dlgFlatFile.AutoUpgradeEnabled = True
        dlgFlatFile.Filter = "txt files (*.txt)|*.txt|prn files (*.prn)|*.prn|output files (*.out)|*.out|All files (*.*)|*.*"
        dlgFlatFile.FileName = txtFlatFile.Text
        resp = dlgFlatFile.ShowDialog()
        If resp = DialogResult.OK Then
            txtFlatFile.Text = dlgFlatFile.FileName
            regKey.SetValue("FlatFilename", dlgFlatFile.FileName)
            Dim thisPath As String = dlgFlatFile.FileName.Replace(dlgFlatFile.SafeFileName, "")
            regKey.SetValue("FlatFileDirectory", thisPath)
        End If
    End Sub

    Private Sub txtFlatFile_TextChanged(sender As Object, e As EventArgs) Handles txtFlatFile.TextChanged
        FlatFile = txtFlatFile.Text
    End Sub

    Private Sub chkIgnoreExisting_CheckedChanged(sender As Object, e As EventArgs) Handles chkIgnoreExisting.CheckedChanged
        regKey.SetValue("FileIgnoreExisting", chkIgnoreExisting.Checked)
    End Sub

    Private Sub bufferTimer_Tick(sender As Object, e As EventArgs) Handles bufferTimer.Tick
        Static BufferCount As Long
        Static oldFileLength As Long
        bufferTimer.Stop()
        Dim fileLength As Long = InStream.BaseStream.Length
        bufferLabel.Text = String.Format("{2}-{0}/{1}", fileLength, lastMaxOffset, BufferCount)
        If fileLength > lastMaxOffset Then
            If BufferCount = 0 Then
                LogBox.AppendText("File length greater than last offset." & vbCrLf)
            End If
            If oldFileLength = fileLength Then
                    BufferCount += 1
                    If BufferCount > 250 Then
                        BufferCount = 0
                        fileSettle = True
                    End If
                Else
                    BufferCount = 0
                End If
                oldFileLength = fileLength
            End If
            bufferTimer.Start()
    End Sub
End Class

