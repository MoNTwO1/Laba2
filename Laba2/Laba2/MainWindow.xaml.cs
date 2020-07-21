using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace Laba2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : System.Windows.Window
    {
        private PagingCollectionView _cview;
        ObservableCollection<Threat> threats = new ObservableCollection<Threat>();
        public int page = 0;

        /* public static string pathToFile = @"C:\Users\MoNTwO1\Desktop\thrlist.xlsx";*/
        public readonly string pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\thrlist.xlsx";
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(pathToFile))
            {
                MessageBox.Show("Искомый файл не обнаружен, будет произведена предварительная загрузка");
                MessageBox.Show("Пожалуста, подождите");
                WebClient webClient = new WebClient();
                webClient.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", pathToFile);
            }
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorksheet;
            Microsoft.Office.Interop.Excel.Range xlRange;
            int xlRow;
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkbook = xlApp.Workbooks.Open(pathToFile);
            xlWorksheet = xlWorkbook.Worksheets["Sheet"];
            xlRange = xlWorksheet.UsedRange;

            for (xlRow = 3; xlRow <= xlRange.Rows.Count; xlRow++)
            {


                if (xlRange.Cells[xlRow, 1].Text != "")
                {
                    threats.Add(new Threat(xlRange.Cells[xlRow, 1].Text, xlRange.Cells[xlRow, 2].Text, xlRange.Cells[xlRow, 3].Text, xlRange.Cells[xlRow, 4].Text, xlRange.Cells[xlRow, 5].Text, xlRange.Cells[xlRow, 6].Text, xlRange.Cells[xlRow, 7].Text, xlRange.Cells[xlRow, 8].Text));

                }
            }

            this._cview = new PagingCollectionView(threats, 15);
            this.DataContext = this._cview;


        }
        private void OnNextClicked(object sender, RoutedEventArgs e)
        {
            
            this._cview.MoveToNextPage();
            page++;
            
            
        }

        private void OnPreviousClicked(object sender, RoutedEventArgs e)
        {
            this._cview.MoveToPreviousPage();
            page--;
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            const int a = 15;
            int num; 
            num = InfoGrid.SelectedIndex + 1;
            MessageBox.Show(threats[num + a*page - 1].Id + "\n" + threats[num + a * page -1].Name + "\n" + threats[num + a * page -1 ].Decsription + "\n" + threats[num + a * page-1].Source + "\n" + threats[num + a * page-1].Obj + "\n" + threats[num + a * page-1].Сonfidentiality + "\n" + threats[num + a * page-1].Access + "\n" + threats[num + a * page-1].IncludeDateThreat);
        }

        

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Threat> oldThreats = new ObservableCollection<Threat>(threats);
            if (!File.Exists(pathToFile))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", pathToFile);
            }
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorksheet;
            Microsoft.Office.Interop.Excel.Range xlRange;
            
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkbook = xlApp.Workbooks.Open(pathToFile);
            xlWorksheet = xlWorkbook.Worksheets["Sheet"];
            xlRange = xlWorksheet.UsedRange;

            for (int i = 0; i < threats.Count; i++)
            {
                if (xlRange.Cells[i + 3, 1].Text != "")
                {
                    int a;
                    int b;
                    int c;
                    if (threats[i].Сonfidentiality == "Да")
                    {
                        a = 1;
                    }
                    else
                    {
                        a = 0;
                    }
                    if (threats[i].Access == "Да")
                    {
                        b = 1;
                    }
                    else
                    {
                        b = 0;
                    }
                    if (threats[i].IncludeDateThreat == "Да")
                    {
                        c = 1;
                    }
                    else
                    {
                        c = 0;
                    }
                    if (threats[i].Id.Split('.')[1] != xlRange.Cells[i + 3, 1].Text ||
                        threats[i].Name != xlRange.Cells[i + 3, 2].Text ||
                        threats[i].Decsription != xlRange.Cells[i + 3, 3].Text ||
                        threats[i].Source != xlRange.Cells[i + 3, 4].Text ||
                        threats[i].Obj != xlRange.Cells[i + 3, 5].Text ||
                        a.ToString() != xlRange.Cells[i + 3, 6].Text ||
                        b.ToString() != xlRange.Cells[i + 3, 7].Text ||
                        c.ToString() != xlRange.Cells[i + 3, 8].Text)
                    {
                        threats.RemoveAt(i);
                        threats.Insert(i, new Threat(xlRange.Cells[i + 3, 1].Text,
                        xlRange.Cells[i + 3, 2].Text,
                        xlRange.Cells[i + 3, 3].Text,
                        xlRange.Cells[i + 3, 4].Text,
                        xlRange.Cells[i + 3, 5].Text,
                        xlRange.Cells[i + 3, 6].Text,
                        xlRange.Cells[i + 3, 7].Text,
                        xlRange.Cells[i + 3, 8].Text));
                    }
                }
                else
                {
                    break;
                }
            }
            uint countNewThreats = 0;
            for (int i = threats.Count + 3; xlRange.Cells[i, 1].Text != ""; i++)
            {
                countNewThreats++;
                threats.Add(new Threat(xlRange.Cells[i, 1].Text,
                        xlRange.Cells[i, 2].Text,
                        xlRange.Cells[i, 3].Text,
                        xlRange.Cells[i, 4].Text,
                        xlRange.Cells[i, 5].Text,
                        xlRange.Cells[i, 6].Text,
                        xlRange.Cells[i, 7].Text,
                        xlRange.Cells[i, 8].Text));
            }
            InfoGrid.Items.Refresh();
            var oldDifferences = oldThreats.Except(threats);
            var newDifferences = threats.Except(oldThreats);
            MessageBox.Show("Количество измененных записей " + oldDifferences.Count().ToString());
            waslabel.IsEnabled = true;
            becamelabel.IsEnabled = true;
            was.IsEnabled = true;
            was.ItemsSource = oldDifferences;
            became.IsEnabled = true;
            became.ItemsSource = newDifferences;
            xlWorkbook.Close();
            xlApp.Quit();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file (*.txt)|*.txt";
           
            string localDataBase = "";
            foreach (var threat in threats)
            {
                localDataBase += threat.ToString() + "\n----------------\n";
            }
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, localDataBase);
            }
        }
    }
    public class PagingCollectionView : CollectionView
    {
        private readonly IList _innerList;
        private readonly int _itemsPerPage;

        private int _currentPage = 1;

        public PagingCollectionView(IList innerList, int itemsPerPage)
            : base(innerList)
        {
            this._innerList = innerList;
            this._itemsPerPage = itemsPerPage;
        }

        public override int Count
        {
            get
            {
                if (this._innerList.Count == 0) return 0;
                if (this._currentPage < this.PageCount) // page 1..n-1
                {
                    return this._itemsPerPage;
                }
                else // page n
                {
                    var itemsLeft = this._innerList.Count % this._itemsPerPage;
                    if (0 == itemsLeft)
                    {
                        return this._itemsPerPage; // exactly itemsPerPage left
                    }
                    else
                    {
                        // return the remaining items
                        return itemsLeft;
                    }
                }
            }
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            set
            {
                this._currentPage = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
            }
        }

        public int ItemsPerPage { get { return this._itemsPerPage; } }

        public int PageCount
        {
            get
            {
                return (this._innerList.Count + this._itemsPerPage - 1)
                    / this._itemsPerPage;
            }
        }

        private int EndIndex
        {
            get
            {
                var end = this._currentPage * this._itemsPerPage - 1;
                return (end > this._innerList.Count) ? this._innerList.Count : end;
            }
        }

        private int StartIndex
        {
            get { return (this._currentPage - 1) * this._itemsPerPage; }
        }

        public override object GetItemAt(int index)
        {
            var offset = index % (this._itemsPerPage);
            return this._innerList[this.StartIndex + offset];
        }

        public void MoveToNextPage()
        {
            
          
            if (this._currentPage < this.PageCount)
            {
                this.CurrentPage += 1;
            }
            this.Refresh();
        }

        public void MoveToPreviousPage()
        {
            if (this._currentPage > 1)
            {
                this.CurrentPage -= 1;
            }
            this.Refresh();
        }
    }




}



