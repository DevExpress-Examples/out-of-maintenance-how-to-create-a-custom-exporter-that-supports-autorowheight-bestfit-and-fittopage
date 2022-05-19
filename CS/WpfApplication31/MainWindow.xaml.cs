using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApplication31.nwindDataSetTableAdapters;
using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.Windows.Controls;

namespace WpfApplication31 {
    public partial class MainWindow : Window {
        class ListBoxValue {
            public ReportGeneratorType ReportGeneratorType { get; set; }

            public override string ToString() {
                return ReportGeneratorType.ToString();
            }
        }

        nwindDataSet.CustomerReportsDataTable table = new nwindDataSet.CustomerReportsDataTable();
        CustomerReportsTableAdapter tableAdapter = new CustomerReportsTableAdapter();

        public MainWindow() {
            InitializeComponent();
            tableAdapter.Fill(table);
            InitializePivot();
            InitializeListBox();
            InitializeDefaultSettings();
        }
        void InitializePivot() {
            pivotGridControl1.BeginUpdate();
            pivotGridControl1.DataSource = table;
            pivotGridControl1.RetrieveFields();
            pivotGridControl1.Fields["CompanyName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["ProductName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["ProductAmount"].Area = FieldArea.DataArea;

            PivotGridField fieldYear = pivotGridControl1.Fields["OrderDate"];
            fieldYear.Area = FieldArea.ColumnArea;
            (fieldYear.DataBinding as DataSourceColumnBinding).GroupInterval = FieldGroupInterval.DateYear;
            fieldYear.Caption = "Year";
            pivotGridControl1.EndUpdate();
        }
        void InitializeListBox() {
            listBoxEdit1.StyleSettings = new RadioListBoxEditStyleSettings();
            listBoxEdit1.Items.Add(new ListBoxValue() { ReportGeneratorType = ReportGeneratorType.SinglePage });
            listBoxEdit1.Items.Add(new ListBoxValue() { ReportGeneratorType = ReportGeneratorType.FixedColumnWidth });
            listBoxEdit1.Items.Add(new ListBoxValue() { ReportGeneratorType = ReportGeneratorType.BestFitColumns });
            listBoxEdit1.SelectedIndexChanged += OnListBoxEditSelectedIndexChanged;
        }
        void InitializeDefaultSettings() {
            listBoxEdit1.SelectedIndex = 0;
            spinEdit1.IsEnabled = false;
            checkEdit2.IsEnabled = false;
        }
        void OnListBoxEditSelectedIndexChanged(object sender, RoutedEventArgs e) {
            switch(((ListBoxValue)((ListBoxEdit)e.Source).SelectedItem).ReportGeneratorType) {
                case ReportGeneratorType.SinglePage:
                    spinEdit1.IsEnabled = false;
                    checkEdit2.IsEnabled = false;
                    break;
                case ReportGeneratorType.FixedColumnWidth:
                    spinEdit1.IsEnabled = true;
                    checkEdit2.IsEnabled = true;
                    break;
                case ReportGeneratorType.BestFitColumns:
                    spinEdit1.IsEnabled = false;
                    checkEdit2.IsEnabled = true;
                    break;
            }
        }
        void OnButtonClick(object sender, RoutedEventArgs e) {
            XtraReport rep = PivotReportGenerator.GenerateReport(pivotGridControl1, (((ListBoxValue)listBoxEdit1.SelectedItem).ReportGeneratorType), spinEdit1.Value, checkEdit2.IsChecked.Value);
            Window window = new Window();
            Grid grid = new Grid();
            grid.Children.Add(new DocumentPreviewControl() { DocumentSource = rep });
            window.Content = grid;
            rep.CreateDocument();
            window.ShowDialog();
        }
    }
}
