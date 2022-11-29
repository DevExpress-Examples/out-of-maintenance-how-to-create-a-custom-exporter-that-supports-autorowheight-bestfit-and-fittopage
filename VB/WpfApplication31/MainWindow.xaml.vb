Imports System.Windows
Imports WpfApplication31.nwindDataSetTableAdapters
Imports DevExpress.Xpf.PivotGrid
Imports DevExpress.Xpf.Editors
Imports DevExpress.XtraReports.UI
Imports DevExpress.Xpf.Printing
Imports System.Windows.Controls

Namespace WpfApplication31

    Public Partial Class MainWindow
        Inherits Window

        Private Class ListBoxValue

            Public Property ReportGeneratorType As ReportGeneratorType

            Public Overrides Function ToString() As String
                Return ReportGeneratorType.ToString()
            End Function
        End Class

        Private table As nwindDataSet.CustomerReportsDataTable = New nwindDataSet.CustomerReportsDataTable()

        Private tableAdapter As CustomerReportsTableAdapter = New CustomerReportsTableAdapter()

        Public Sub New()
            Me.InitializeComponent()
            tableAdapter.Fill(table)
            InitializePivot()
            InitializeListBox()
            InitializeDefaultSettings()
        End Sub

        Private Sub InitializePivot()
            Me.pivotGridControl1.BeginUpdate()
            Me.pivotGridControl1.DataSource = table
            Me.pivotGridControl1.RetrieveFields()
            Me.pivotGridControl1.Fields("CompanyName").Area = FieldArea.RowArea
            Me.pivotGridControl1.Fields("ProductName").Area = FieldArea.RowArea
            Me.pivotGridControl1.Fields("ProductAmount").Area = FieldArea.DataArea
            Dim fieldYear As PivotGridField = Me.pivotGridControl1.Fields("OrderDate")
            fieldYear.Area = FieldArea.ColumnArea
            TryCast(fieldYear.DataBinding, DataSourceColumnBinding).GroupInterval = FieldGroupInterval.DateYear
            fieldYear.Caption = "Year"
            Me.pivotGridControl1.EndUpdate()
        End Sub

        Private Sub InitializeListBox()
            Me.listBoxEdit1.StyleSettings = New RadioListBoxEditStyleSettings()
            Me.listBoxEdit1.Items.Add(New ListBoxValue() With {.ReportGeneratorType = ReportGeneratorType.SinglePage})
            Me.listBoxEdit1.Items.Add(New ListBoxValue() With {.ReportGeneratorType = ReportGeneratorType.FixedColumnWidth})
            Me.listBoxEdit1.Items.Add(New ListBoxValue() With {.ReportGeneratorType = ReportGeneratorType.BestFitColumns})
            AddHandler Me.listBoxEdit1.SelectedIndexChanged, AddressOf Me.OnListBoxEditSelectedIndexChanged
        End Sub

        Private Sub InitializeDefaultSettings()
            Me.listBoxEdit1.SelectedIndex = 0
            Me.spinEdit1.IsEnabled = False
            Me.checkEdit2.IsEnabled = False
        End Sub

        Private Sub OnListBoxEditSelectedIndexChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Select Case CType(CType(e.Source, ListBoxEdit).SelectedItem, ListBoxValue).ReportGeneratorType
                Case ReportGeneratorType.SinglePage
                    Me.spinEdit1.IsEnabled = False
                    Me.checkEdit2.IsEnabled = False
                Case ReportGeneratorType.FixedColumnWidth
                    Me.spinEdit1.IsEnabled = True
                    Me.checkEdit2.IsEnabled = True
                Case ReportGeneratorType.BestFitColumns
                    Me.spinEdit1.IsEnabled = False
                    Me.checkEdit2.IsEnabled = True
            End Select
        End Sub

        Private Sub OnButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim rep As XtraReport = PivotReportGenerator.GenerateReport(Me.pivotGridControl1, (CType(Me.listBoxEdit1.SelectedItem, ListBoxValue).ReportGeneratorType), Me.spinEdit1.Value, Me.checkEdit2.IsChecked.Value)
            Dim window As Window = New Window()
            Dim grid As Grid = New Grid()
            grid.Children.Add(New DocumentPreviewControl() With {.DocumentSource = rep})
            window.Content = grid
            rep.CreateDocument()
            window.ShowDialog()
        End Sub
    End Class
End Namespace
