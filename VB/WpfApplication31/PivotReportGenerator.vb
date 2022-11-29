Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports DevExpress.XtraReports.UI
Imports DevExpress.Xpf.PivotGrid
Imports System.Windows.Media
Imports System.Globalization
Imports System.Windows
Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports System.Runtime.InteropServices

Namespace WpfApplication31

    Public Enum ReportGeneratorType
        SinglePage
        FixedColumnWidth
        BestFitColumns
    End Enum

    Public Module PivotReportGenerator

        Public Function GenerateReport(ByVal pivot As DevExpress.Xpf.PivotGrid.PivotGridControl, ByVal kind As WpfApplication31.ReportGeneratorType, ByVal columnWidth As Decimal, ByVal repeatRowHeader As Boolean) As XtraReport
            Dim rep As DevExpress.XtraReports.UI.XtraReport = New DevExpress.XtraReports.UI.XtraReport()
            rep.Landscape = True
            rep.DataSource = WpfApplication31.PivotReportGenerator.FillDataset(pivot)
            rep.DataMember = CType(rep.DataSource, System.Data.DataSet).Tables(CInt((0))).TableName
            Call WpfApplication31.PivotReportGenerator.InitBands(rep)
            Call WpfApplication31.PivotReportGenerator.InitStyles(rep)
            Call WpfApplication31.PivotReportGenerator.InitDetailsBasedonXRTable(rep, kind, System.Convert.ToSingle(columnWidth), repeatRowHeader)
            Return rep
        End Function

        Public Function FillDataset(ByVal pivot As DevExpress.Xpf.PivotGrid.PivotGridControl) As DataSet
            Dim dataSet1 As System.Data.DataSet = New System.Data.DataSet()
            dataSet1.DataSetName = "PivotGridColumns"
            Dim dataTable1 As System.Data.DataTable = New System.Data.DataTable()
            dataSet1.Tables.Add(dataTable1)
            Call WpfApplication31.PivotReportGenerator.FillDatasetColumns(pivot, dataTable1)
            Call WpfApplication31.PivotReportGenerator.FillDatasetExtracted(pivot, dataTable1)
            Return dataSet1
        End Function

'#Region "PreparingDataSet"
        Private Sub FillDatasetExtracted(ByVal pivot As DevExpress.Xpf.PivotGrid.PivotGridControl, ByVal dataTable1 As System.Data.DataTable)
            Dim rowvalues As System.Collections.Generic.List(Of Object) = New System.Collections.Generic.List(Of Object)()
            Dim tempRowText As String = ""
            Dim fieldsInRowArea As System.Collections.Generic.List(Of DevExpress.Xpf.PivotGrid.PivotGridField) = WpfApplication31.PivotReportGenerator.GetFieldsInArea(pivot, DevExpress.Xpf.PivotGrid.FieldArea.RowArea)
            For i As Integer = 0 To pivot.RowCount - 1
                Dim pcea As DevExpress.Xpf.PivotGrid.PivotCellBaseEventArgs = pivot.GetCellInfo(0, i)
                If pcea.RowValueType = DevExpress.Xpf.PivotGrid.FieldValueType.Value Then
                    For Each item As DevExpress.Xpf.PivotGrid.PivotGridField In fieldsInRowArea
                        tempRowText += pcea.GetFieldValue(CType((item), DevExpress.Xpf.PivotGrid.PivotGridField)).ToString() & " | "
                    Next 'add formatting if it's necessary

                    tempRowText = tempRowText.Remove(tempRowText.Length - 3, 3)
                Else
                    tempRowText = pcea.RowValueType.ToString()
                End If

                rowvalues.Clear()
                rowvalues.Add(tempRowText)
                tempRowText = ""
                For j As Integer = 0 To pivot.ColumnCount - 1
                    pcea = pivot.GetCellInfo(j, i)
                    If pcea.Value IsNot Nothing Then
                        rowvalues.Add(pcea.Value)
                    Else
                        rowvalues.Add(System.DBNull.Value)
                    End If
                Next

                dataTable1.Rows.Add(rowvalues.ToArray())
            Next
        End Sub

        Private Sub FillDatasetColumns(ByVal pivot As DevExpress.Xpf.PivotGrid.PivotGridControl, ByVal dataTable1 As System.Data.DataTable)
            dataTable1.Columns.Add("RowFields", GetType(String))
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder()
            Dim fieldsInColumnArea As System.Collections.Generic.List(Of DevExpress.Xpf.PivotGrid.PivotGridField) = WpfApplication31.PivotReportGenerator.GetFieldsInArea(pivot, DevExpress.Xpf.PivotGrid.FieldArea.ColumnArea)
            Dim multipleDataField As Boolean = pivot.GetFieldsByArea(CType((DevExpress.Xpf.PivotGrid.FieldArea.DataArea), DevExpress.Xpf.PivotGrid.FieldArea)).Count > 1
            For i As Integer = 0 To pivot.ColumnCount - 1
                Dim pcea As DevExpress.Xpf.PivotGrid.PivotCellBaseEventArgs = pivot.GetCellInfo(i, 0)
                For Each field As DevExpress.Xpf.PivotGrid.PivotGridField In pcea.GetColumnFields()
                    sb.AppendFormat("{0} | ", field.GetDisplayText(pcea.GetFieldValue(field)))
                Next 'add formatting if it's necessary

                If multipleDataField Then sb.AppendFormat("{0} | ", pcea.DataField)
                If pcea.ColumnValueType = DevExpress.Xpf.PivotGrid.FieldValueType.Value Then
                    sb.Remove(sb.Length - 3, 3)
                Else
                    sb.Append(pcea.ColumnValueType.ToString())
                End If

                dataTable1.Columns.Add(sb.ToString(), GetType(Object))
                sb.Clear()
            Next
        End Sub

        Private Function GetFieldsInArea(ByVal pivot As DevExpress.Xpf.PivotGrid.PivotGridControl, ByVal area As DevExpress.Xpf.PivotGrid.FieldArea) As List(Of DevExpress.Xpf.PivotGrid.PivotGridField)
            Dim fields As System.Collections.Generic.List(Of DevExpress.Xpf.PivotGrid.PivotGridField) = New System.Collections.Generic.List(Of DevExpress.Xpf.PivotGrid.PivotGridField)()
            For i As Integer = 0 To pivot.Fields.Count - 1
                If pivot.Fields(CInt((i))).Area = area Then fields.Add(pivot.Fields(i))
            Next

            Return fields
        End Function

'#End Region
        Public Sub InitBands(ByVal rep As DevExpress.XtraReports.UI.XtraReport)
            ' Create bands
            Dim detail As DevExpress.XtraReports.UI.DetailBand = New DevExpress.XtraReports.UI.DetailBand()
            Dim pageHeader As DevExpress.XtraReports.UI.PageHeaderBand = New DevExpress.XtraReports.UI.PageHeaderBand()
            Dim reportFooter As DevExpress.XtraReports.UI.ReportFooterBand = New DevExpress.XtraReports.UI.ReportFooterBand()
            detail.Height = 20
            reportFooter.Height = 380
            pageHeader.Height = 20
            ' Place the bands onto a report
            rep.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {detail, pageHeader, reportFooter})
        End Sub

        Public Sub InitStyles(ByVal rep As DevExpress.XtraReports.UI.XtraReport)
            ' Create different odd and even styles
            Dim oddStyle As DevExpress.XtraReports.UI.XRControlStyle = New DevExpress.XtraReports.UI.XRControlStyle()
            Dim evenStyle As DevExpress.XtraReports.UI.XRControlStyle = New DevExpress.XtraReports.UI.XRControlStyle()
            ' Specify the odd style appearance
            oddStyle.BackColor = System.Drawing.Color.LightBlue
            oddStyle.StyleUsing.UseBackColor = True
            oddStyle.StyleUsing.UseBorders = False
            oddStyle.Name = "OddStyle"
            ' Specify the even style appearance
            evenStyle.BackColor = System.Drawing.Color.LightPink
            evenStyle.StyleUsing.UseBackColor = True
            evenStyle.StyleUsing.UseBorders = False
            evenStyle.Name = "EvenStyle"
            ' Add styles to report's style sheet
            rep.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {oddStyle, evenStyle})
        End Sub

        Public Sub InitDetailsBasedonXRTable(ByVal rep As DevExpress.XtraReports.UI.XtraReport, ByVal kind As WpfApplication31.ReportGeneratorType, ByVal columnWidth As Single, ByVal repeatRowHeader As Boolean)
            If Not repeatRowHeader OrElse kind = WpfApplication31.ReportGeneratorType.SinglePage Then
                Call WpfApplication31.PivotReportGenerator.InitDetailsBasedonXRTableWithoutRepeatingRowHeader(rep, kind, columnWidth)
            Else
                Call WpfApplication31.PivotReportGenerator.InitDetailsBasedonXRTableRepeatingRowHeader(rep, kind, columnWidth)
            End If
        End Sub

        Private Sub InitDetailsBasedonXRTableRepeatingRowHeader(ByVal rep As DevExpress.XtraReports.UI.XtraReport, ByVal kind As WpfApplication31.ReportGeneratorType, ByVal columnWidth As Single)
            Dim font As DevExpress.Drawing.DXFont = New DevExpress.Drawing.DXFont("Tahoma", 9.75F)
            Dim dataTable As System.Data.DataTable = CType(rep.DataSource, System.Data.DataSet).Tables(0)
            Dim processedPage As Integer = 0
            Dim usablePageWidth As Single = rep.PageWidth - (rep.Margins.Left + rep.Margins.Right)
            Dim columnsWidth As System.Collections.Generic.List(Of Single) = Nothing
            If kind = WpfApplication31.ReportGeneratorType.FixedColumnWidth Then
                columnsWidth = WpfApplication31.PivotReportGenerator.DefineColumnsWidth(columnWidth, dataTable.Columns.Count)
            Else
                columnsWidth = WpfApplication31.PivotReportGenerator.GetColumnsBestFitWidth(dataTable, font, rep.ReportUnit)
            End If

            Dim tableHeader As DevExpress.XtraReports.UI.XRTable = Nothing
            Dim tableDetail As DevExpress.XtraReports.UI.XRTable = Nothing
            Call WpfApplication31.PivotReportGenerator.InitNewTableInstancesAt(rep, font, tableHeader, tableDetail, New System.Drawing.PointF(0, 0))
            tableHeader.BeginInit()
            tableDetail.BeginInit()
            Dim i As Integer = 1
            Call WpfApplication31.PivotReportGenerator.AddCellsToTables(tableHeader, tableDetail, dataTable.Columns(0), columnsWidth(0), True)
            Dim remainingSpace As Single = usablePageWidth - columnsWidth(0)
            Do
                If columnsWidth(i) > remainingSpace Then
                    processedPage += 1
                    tableHeader.WidthF = usablePageWidth - remainingSpace
                    tableDetail.WidthF = usablePageWidth - remainingSpace
                    tableHeader.EndInit()
                    tableDetail.EndInit()
                    Call WpfApplication31.PivotReportGenerator.InitNewTableInstancesAt(rep, font, tableHeader, tableDetail, New System.Drawing.PointF(usablePageWidth * processedPage, 0))
                    tableHeader.BeginInit()
                    tableDetail.BeginInit()
                    Call WpfApplication31.PivotReportGenerator.AddCellsToTables(tableHeader, tableDetail, dataTable.Columns(0), columnsWidth(0), True)
                    remainingSpace = usablePageWidth - columnsWidth(0)
                Else
                    Call WpfApplication31.PivotReportGenerator.AddCellsToTables(tableHeader, tableDetail, dataTable.Columns(i), columnsWidth(i), False)
                    remainingSpace -= columnsWidth(i)
                    i += 1
                End If
            Loop While i < columnsWidth.Count

            tableHeader.WidthF = usablePageWidth - remainingSpace
            tableDetail.WidthF = usablePageWidth - remainingSpace
            tableHeader.EndInit()
            tableDetail.EndInit()
        End Sub

        Public Sub AddCellsToTables(ByVal header As DevExpress.XtraReports.UI.XRTable, ByVal detail As DevExpress.XtraReports.UI.XRTable, ByVal dc As System.Data.DataColumn, ByVal columnWidth As Single, ByVal isFirstColumnInTable As Boolean)
            Dim headerCell As DevExpress.XtraReports.UI.XRTableCell = New DevExpress.XtraReports.UI.XRTableCell()
            headerCell.Text = dc.Caption
            Dim detailCell As DevExpress.XtraReports.UI.XRTableCell = New DevExpress.XtraReports.UI.XRTableCell()
            detailCell.DataBindings.Add("Text", Nothing, dc.Caption)
            headerCell.WidthF = columnWidth
            detailCell.WidthF = columnWidth
            If isFirstColumnInTable Then
                headerCell.Borders = DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top Or DevExpress.XtraPrinting.BorderSide.Bottom
                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top Or DevExpress.XtraPrinting.BorderSide.Bottom
            Else
                headerCell.Borders = DevExpress.XtraPrinting.BorderSide.All
                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.All
            End If

            ' Place the cells into the corresponding tables
            header.Rows(CInt((0))).Cells.Add(headerCell)
            detail.Rows(CInt((0))).Cells.Add(detailCell)
        End Sub

        Public Sub InitNewTableInstancesAt(ByVal report As DevExpress.XtraReports.UI.XtraReport, ByVal font As DevExpress.Drawing.DXFont, <Out> ByRef header As DevExpress.XtraReports.UI.XRTable, <Out> ByRef detail As DevExpress.XtraReports.UI.XRTable, ByVal location As System.Drawing.PointF)
            header = WpfApplication31.PivotReportGenerator.InitXRTable(font, False)
            detail = WpfApplication31.PivotReportGenerator.InitXRTable(font, True)
            header.LocationF = location
            detail.LocationF = location
            Dim headerRow As DevExpress.XtraReports.UI.XRTableRow = New DevExpress.XtraReports.UI.XRTableRow()
            header.Rows.Add(headerRow)
            Dim detailRow As DevExpress.XtraReports.UI.XRTableRow = New DevExpress.XtraReports.UI.XRTableRow()
            detail.Rows.Add(detailRow)
            report.Bands(CType((DevExpress.XtraReports.UI.BandKind.PageHeader), DevExpress.XtraReports.UI.BandKind)).Controls.Add(header)
            report.Bands(CType((DevExpress.XtraReports.UI.BandKind.Detail), DevExpress.XtraReports.UI.BandKind)).Controls.Add(detail)
        End Sub

        Private Function InitXRTable(ByVal font As DevExpress.Drawing.DXFont, ByVal withStyles As Boolean) As XRTable
            Dim table As DevExpress.XtraReports.UI.XRTable = New DevExpress.XtraReports.UI.XRTable()
            table.Font = font
            table.Height = 20
            If withStyles Then
                table.EvenStyleName = "EvenStyle"
                table.OddStyleName = "OddStyle"
            End If

            Return table
        End Function

        Private Function DefineColumnsWidth(ByVal columnWidth As Single, ByVal count As Integer) As List(Of Single)
            Dim columnsWidth As System.Collections.Generic.List(Of Single) = New System.Collections.Generic.List(Of Single)()
            For i As Integer = 0 To count - 1
                columnsWidth.Add(columnWidth)
            Next

            Return columnsWidth
        End Function

        Private Sub InitDetailsBasedonXRTableWithoutRepeatingRowHeader(ByVal rep As DevExpress.XtraReports.UI.XtraReport, ByVal kind As WpfApplication31.ReportGeneratorType, ByVal columnWidth As Single)
            Dim font As DevExpress.Drawing.DXFont = New DevExpress.Drawing.DXFont("Tahoma", 9.75F)
            Dim ds As System.Data.DataSet = CType(rep.DataSource, System.Data.DataSet)
            Dim colCount As Integer = ds.Tables(CInt((0))).Columns.Count
            Dim colWidth As Single = 0
            Dim tableHeader As DevExpress.XtraReports.UI.XRTable = Nothing
            Dim tableDetail As DevExpress.XtraReports.UI.XRTable = Nothing
            Call WpfApplication31.PivotReportGenerator.InitNewTableInstancesAt(rep, font, tableHeader, tableDetail, New System.Drawing.PointF(0, 0))
            Dim columnsWidth As System.Collections.Generic.List(Of Single) = Nothing
            Select Case kind
                Case WpfApplication31.ReportGeneratorType.FixedColumnWidth
                    colWidth = columnWidth
                    tableHeader.WidthF = columnWidth * colCount
                    tableDetail.WidthF = columnWidth * colCount
                Case WpfApplication31.ReportGeneratorType.BestFitColumns
                    columnsWidth = WpfApplication31.PivotReportGenerator.GetColumnsBestFitWidth(ds.Tables(0), font, rep.ReportUnit)
                    colWidth = 0
                    tableHeader.WidthF = WpfApplication31.PivotReportGenerator.GetTotalWidth(columnsWidth)
                    tableDetail.WidthF = tableHeader.Width
                Case Else
                    colWidth =(rep.PageWidth - (rep.Margins.Left + rep.Margins.Right)) / colCount
                    tableHeader.WidthF =(rep.PageWidth - (rep.Margins.Left + rep.Margins.Right))
                    tableDetail.WidthF =(rep.PageWidth - (rep.Margins.Left + rep.Margins.Right))
            End Select

            tableHeader.BeginInit()
            tableDetail.BeginInit()
            ' Create table cells, fill the header cells with text, bind the cells to data
            For i As Integer = 0 To colCount - 1
                Call WpfApplication31.PivotReportGenerator.AddCellsToTables(tableHeader, tableDetail, ds.Tables(CInt((0))).Columns(i), If(kind = WpfApplication31.ReportGeneratorType.BestFitColumns, columnsWidth(i), colWidth), If(i = 0, True, False))
            Next

            tableDetail.EndInit()
            tableHeader.EndInit()
        ' Place the table onto a report's Detail band
        End Sub

        Private Function GetTotalWidth(ByVal columnsWidth As System.Collections.Generic.List(Of Single)) As Single
            Dim i As Single = 0
            For Each colWidth As Single In columnsWidth
                i += colWidth
            Next

            Return i
        End Function

        Private Function GetColumnsBestFitWidth(ByVal dataTable As System.Data.DataTable, ByVal font As DevExpress.Drawing.DXFont, ByVal unit As DevExpress.XtraReports.UI.ReportUnit) As List(Of Single)
            Dim optimalColumnWidth As System.Collections.Generic.List(Of Single) = New System.Collections.Generic.List(Of Single)()
            Dim maxWidth As Single = 0
            Dim tempWidth As Single = 0
            For i As Integer = 1 To dataTable.Rows.Count - 1
                tempWidth = WpfApplication31.PivotReportGenerator.MeasureWidth(dataTable.Rows(CInt((i)))(CInt((0))).ToString(), font, unit)
                maxWidth = If(maxWidth > tempWidth, maxWidth, tempWidth)
            Next

            optimalColumnWidth.Add(maxWidth)
            For i As Integer = 1 To dataTable.Columns.Count - 1
                tempWidth = WpfApplication31.PivotReportGenerator.MeasureWidth(dataTable.Columns(CInt((i))).ColumnName.ToString(), font, unit)
                maxWidth = If(50 > tempWidth, 50, tempWidth)
                optimalColumnWidth.Add(maxWidth)
            Next

            Return optimalColumnWidth
        End Function

        Private Function MeasureWidth(ByVal candidate As String, ByVal font As DevExpress.Drawing.DXFont, ByVal unit As DevExpress.XtraReports.UI.ReportUnit) As Single
            Return DevExpress.XtraReports.UI.BestSizeEstimator.GetBoundsToFitText(CStr((candidate)), CType((New DevExpress.XtraPrinting.BrickStyle() With {.Font = font}), DevExpress.XtraPrinting.BrickStyle), CType((unit), DevExpress.XtraReports.UI.ReportUnit)).Width
        End Function
    End Module
End Namespace
