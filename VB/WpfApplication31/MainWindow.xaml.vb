Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports WpfApplication31.nwindDataSetTableAdapters
Imports DevExpress.Xpf.PivotGrid
Imports DevExpress.Xpf.Editors
Imports DevExpress.XtraReports.UI
Imports DevExpress.Xpf.Printing
Imports System.Windows.Controls

Namespace WpfApplication31
    Partial Public Class MainWindow
        Inherits Window

        Private Class ListBoxValue
            Public Property ReportGeneratorType() As ReportGeneratorType

            Public Overrides Function ToString() As String
                Return ReportGeneratorType.ToString()
            End Function
        End Class

        Private table As New nwindDataSet.CustomerReportsDataTable()
        Private tableAdapter As New CustomerReportsTableAdapter()

        Public Sub New()
            InitializeComponent()
            tableAdapter.Fill(table)
            InitializePivot()
            InitializeListBox()
            InitializeDefaultSettings()
        End Sub
        Private Sub InitializePivot()
            pivotGridControl1.BeginUpdate()
            pivotGridControl1.DataSource = table
            pivotGridControl1.RetrieveFields()
            pivotGridControl1.Fields("CompanyName").Area = FieldArea.RowArea
            pivotGridControl1.Fields("ProductName").Area = FieldArea.RowArea
            pivotGridControl1.Fields("ProductAmount").Area = FieldArea.DataArea

            Dim fieldYear As PivotGridField = pivotGridControl1.Fields("OrderDate")
            fieldYear.Area = FieldArea.ColumnArea
            fieldYear.GroupInterval = FieldGroupInterval.DateYear
            fieldYear.Caption = "Year"
            pivotGridControl1.EndUpdate()
        End Sub
        Private Sub InitializeListBox()
            listBoxEdit1.StyleSettings = New RadioListBoxEditStyleSettings()
            listBoxEdit1.Items.Add(New ListBoxValue() With {.ReportGeneratorType = ReportGeneratorType.SinglePage})
            listBoxEdit1.Items.Add(New ListBoxValue() With {.ReportGeneratorType = ReportGeneratorType.FixedColumnWidth})
            listBoxEdit1.Items.Add(New ListBoxValue() With {.ReportGeneratorType = ReportGeneratorType.BestFitColumns})
            AddHandler listBoxEdit1.SelectedIndexChanged, AddressOf OnListBoxEditSelectedIndexChanged
        End Sub
        Private Sub InitializeDefaultSettings()
            listBoxEdit1.SelectedIndex = 0
            spinEdit1.IsEnabled = False
            checkEdit2.IsEnabled = False
        End Sub
        Private Sub OnListBoxEditSelectedIndexChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Select Case CType(DirectCast(e.Source, ListBoxEdit).SelectedItem, ListBoxValue).ReportGeneratorType
                Case ReportGeneratorType.SinglePage
                    spinEdit1.IsEnabled = False
                    checkEdit2.IsEnabled = False
                Case ReportGeneratorType.FixedColumnWidth
                    spinEdit1.IsEnabled = True
                    checkEdit2.IsEnabled = True
                Case ReportGeneratorType.BestFitColumns
                    spinEdit1.IsEnabled = False
                    checkEdit2.IsEnabled = True
            End Select
        End Sub
        Private Sub OnButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim rep As XtraReport = PivotReportGenerator.GenerateReport(pivotGridControl1, (CType(listBoxEdit1.SelectedItem, ListBoxValue).ReportGeneratorType), spinEdit1.Value, checkEdit2.IsChecked.Value)
            Dim window As New Window()
            Dim grid As New Grid()
            grid.Children.Add(New DocumentPreviewControl() With {.DocumentSource = rep})
            window.Content = grid
            rep.CreateDocument()
            window.ShowDialog()
        End Sub
    End Class
End Namespace
