
Imports System.Drawing.Printing
Public Class PrintHelper

    Friend TextToBePrinted As List(Of String)
    Friend PrtLine As Integer
    Friend DestinationPrinter As String
    Friend FontFamily As String
    Friend FontSize As String

    Public MyString As String = ""
    Public MyPages As String()

    Public Sub prt(ByVal text As List(Of String), ByVal printer As String, docName As String)
        TextToBePrinted = text
        Form1.LogBox.AppendText(String.Format("Using {0}/{1} on printer {2}" & vbCrLf, FontFamily, FontSize, DestinationPrinter))
        Dim prn As New Printing.PrintDocument
        prn.PrintController = New System.Drawing.Printing.StandardPrintController

        Using (prn)
                prn.PrinterSettings.PrinterName = DestinationPrinter
                prn.DefaultPageSettings.Landscape = True
                prn.DefaultPageSettings.Margins.Top = 0
                prn.DefaultPageSettings.Margins.Bottom = 0
                prn.DefaultPageSettings.Margins.Left = 0
                prn.DefaultPageSettings.Margins.Right = 0
                prn.OriginAtMargins = True

                If DestinationPrinter.Contains("PDF") Then
                    prn.PrinterSettings.PrintFileName = docName & ".pdf"
                    prn.PrinterSettings.PrintToFile = True
                End If
                For x = 0 To TextToBePrinted.Count - 1
                    If x <> TextToBePrinted.Count - 1 Then
                        MyString = MyString & TextToBePrinted(x) & vbCrLf
                    Else
                        MyString = MyString & TextToBePrinted(x)
                    End If
                Next

                ' now we're back to one huge ass string again, so now lets split it out into pages?

                MyPages = MyString.Split(vbFormFeed)

                ' Now a bit of document analysis
                Dim TotalLinesToPrint As Long = TextToBePrinted.Count
                Dim TotalPagesFound As Integer = MyPages.Count

                If TotalPagesFound = 1 Then
                    If TotalLinesToPrint > 66 Then
                        Form1.LogBox.AppendText("Unpaginated document found.  Paginating." & vbCrLf)
                        ' We'll do this by starting at the first line, and inserting
                        ' a formfeed character at the beginning of each 67th line.
                        ' Then rebuilding the big string, and splitting that out
                        ' into pages, just like we did before.  Probably bad coding
                        ' practice, but I can't think of any way of determining the
                        ' pagination without doing it and looking.

                        ' Now we go back to TextToBePrinted
                        Dim ThisLine As Integer = 0
                        For x = 0 To TextToBePrinted.Count - 1
                            ' We're only messing around with every 67th line
                            ' until the very end, then we'll insert a vbFormfeed
                            ' at the END of the last line.  Doing this will 
                            ' hopefully bring us in line with the VMS PRINT 
                            ' command, thus allowing us to dump raw data to
                            ' the printer bypassing the print queue.
                            Dim HoldingString As String = TextToBePrinted(x)
                            If x = 66 Then        ' Remember, we're zero based, so this is really 67
                                HoldingString = vbFormFeed & HoldingString
                                ' Now replace it.
                                TextToBePrinted(x) = HoldingString
                                ' Set ThisLine back to 0
                                ThisLine = 0
                            End If
                            ThisLine += 1
                        Next
                        ' Okay, now insert the final vbFormfeed at the END
                        ' of the LAST line.
                        TextToBePrinted(TextToBePrinted.Count - 1) += vbFormFeed
                        ' The final formfeed isn't actually necessary, but it's done
                        ' here to conform to the way VMS Queue's operate.  In either
                        ' case, Windows will print the final page and eject anyway.
                        ' You'll notice we ignore the last page if it's blank regardless.

                        ' Now let's prepare the document again.
                        For x = 0 To TextToBePrinted.Count - 1
                            If x <> TextToBePrinted.Count - 1 Then
                                MyString = MyString & TextToBePrinted(x) & vbCrLf
                            Else
                                MyString = MyString & TextToBePrinted(x)
                            End If
                        Next

                        ' now we're back to one huge ass string again, so now lets split it out into pages?

                        MyPages = MyString.Split(vbFormFeed)
                    End If
                End If

                AddHandler prn.PrintPage, AddressOf Me.PrintPageHandler
                prn.Print()
                RemoveHandler prn.PrintPage, AddressOf Me.PrintPageHandler
            End Using

    End Sub

    Private Sub PrintPageHandler(ByVal Sender As Object, ByVal args As Printing.PrintPageEventArgs)

        Dim thisPage As String
        Static PageNumber As Integer
        Dim myFont As New Font(FontFamily, FontSize)
        Dim H As Integer = args.MarginBounds.Height
        Dim W As Integer = args.MarginBounds.Width
        Dim newFont As Font = GetFontSize(myFont, 66, args)
        'Form1.LogBox.AppendText(String.Format("FINAL FONT IS {0} in {1} points.", newFont.Name, newFont.Size))
        myFont = newFont
        If myFont.Size <> (newFont.Size) Then
            Stop
        End If
        thisPage = MyPages(PageNumber)
        Form1.LogBox.AppendText("Printing page " & PageNumber & vbCrLf)
        args.Graphics.DrawString(thisPage, myFont, Brushes.Black, 25, 25)
        PageNumber = PageNumber + 1
        Form1.LogBox.AppendText("Setting next pagenumber to " & PageNumber & vbCrLf)
        ' Try to print the next page.  In some cases, depending on how the queue is defined
        ' the last page will be entirely empty (as in no data whatsoever, not even a blank line)
        ' and will throw an object outside bounds error.  The following try/catch attempts to 
        ' trap that.
        Try
            If MyPages(PageNumber) = "" Then
                Form1.LogBox.AppendText("The last page is blank, not printing it." & vbCrLf)
                PageNumber = PageNumber + 1
            End If
        Catch ex As Exception
            Form1.LogBox.AppendText("EXCPTN: Document contains a blank that has not been stored.  Ignoring it." & vbCrLf)
        End Try
        If PageNumber > MyPages.Count - 1 Then
            args.HasMorePages = False
        Else
            args.HasMorePages = True
        End If

    End Sub

    Public Function GetFontSize(ByRef thisFont As Font, LinesPerPage As Integer, ByRef args As PrintPageEventArgs) As Font
        Dim da As Graphics = args.Graphics
        'da.PageUnit = GraphicsUnit.Point
        Dim height As Integer = args.PageSettings.PrintableArea.Height
        Dim width As Integer = args.PageSettings.PrintableArea.Width
        Dim mySize As New SizeF
        Dim MeasuredLines As Integer = 0
        Dim myNewFontSize As Single = 0
        For i = 5 To 12 Step 0.25
            Dim testFont As New Font(thisFont.Name, i)
            mySize = args.Graphics.MeasureString("WWWMMMgggyyyZZZ", testFont)
            ' Ok, now we have a height
            MeasuredLines = (width / mySize.Height) ' Wft.... Landscape mode doesn't alter these measurements?
            'Form1.LogBox.AppendText(String.Format("Sizing font with size {0} produces {1} lines per page.", i, MeasuredLines))
            If MeasuredLines < 66 Then
                myNewFontSize = i - 0.01
                Exit For
            End If
        Next i
        'Form1.LogBox.AppendText(String.Format("Using {0,8} type size", myNewFontSize))
        'Stop
        If myNewFontSize = 0 Then
            Form1.LogBox.AppendText("Unable to adjust font size.  Why?" & vbCrLf)
        End If
        thisFont = New Font(thisFont.Name, myNewFontSize)
        Return thisFont
    End Function
End Class
