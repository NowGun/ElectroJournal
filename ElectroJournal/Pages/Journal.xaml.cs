using ElectroJournal.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Graphics;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal : Page
    {
        public Journal()
        {
            InitializeComponent();


            FillTable();
        }

        DataBase DbUser = new DataBase();
        DataBaseControls DbControls = new DataBaseControls();
        MySqlConnection conn = DataBase.GetDBConnection();

        private void FillTable()
        {
            FillText();
            FillStudents();
            FillDates();
           
        }

        private void FillStudents()
        {
            var worksheet = ReoGrid.CurrentWorksheet;

            //

            MySqlCommand command = new MySqlCommand("SELECT `students_name`, `students_surname` FROM `students` where groups_idgroups = 21", conn);

            conn.Open();

            MySqlDataReader reader = command.ExecuteReader();



            for (int i = 2; reader.Read(); i++)
            {
                worksheet.SetRows(i);
                worksheet["A" + i] = reader.GetString(1) + " " + reader.GetString(0);
                worksheet.AutoFitColumnWidth(0, false);

                unvell.ReoGrid.Cell? cell = worksheet.Cells["A" + i];
                cell.IsReadOnly = true;

                worksheet.SetRangeStyles("A1:A" + i, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.FontSize | PlainStyleFlag.FontName,
                    FontName = "Segoe UI",
                    FontSize = 14,
                });
            }

            conn.Close();
        }

        private void FillDates()
        {
            var worksheet = ReoGrid.CurrentWorksheet;


            MySqlCommand command = new MySqlCommand("SELECT `day` FROM `dates` where  `month` = 2 and `year` = 2022", conn);

            conn.Open();

            MySqlDataReader reader = command.ExecuteReader();



            for (int i = 1; reader.Read(); i++)
            {
                worksheet.SetCols(i+1);
                worksheet[0, i] = reader.GetString(0);
                ReoGrid.DoAction(new SetColumnsWidthAction(1, i, 30));

                unvell.ReoGrid.Cell? cell = worksheet.Cells[0, i];
                cell.IsReadOnly = true;

                worksheet.SetRangeStyles("B1:BP150", new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.HorizontalAlign,
                    HAlign = ReoGridHorAlign.Center,
                });

                worksheet.SetRangeStyles("B1:BP150", new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.FontSize | PlainStyleFlag.FontName,
                    FontName = "Segoe UI",
                    FontSize = 14,
                });
            }

            conn.Close();
        }

        private void FillText()
        {
            var worksheet = ReoGrid.CurrentWorksheet;

            worksheet["A1"] = "ФИО\\День месяца";
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            var workbook = ReoGrid;
            workbook.Save(@"C:\Users\nowgu\Desktop\table.xls", unvell.ReoGrid.IO.FileFormat.Excel2007);
        }

        
    }
}
