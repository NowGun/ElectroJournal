using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
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

            FillComoBoxMonth();
            ComboBoxMonth.SelectedIndex = DateTime.Now.Month-1;
            FillTable();
        }

        string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;

        private void FillTable()
        {
            FillText();
            FillStudents();
            FillDates();
        }

        private async void FillStudents()
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1)
            {
                var worksheet = ReoGrid.CurrentWorksheet;
                using (zhirovContext db = new zhirovContext())
                {
                    var days = await db.Groups.Where(p => p.GroupsNameAbbreviated == ((MainWindow)System.Windows.Application.Current.MainWindow).ComboBoxGroup.SelectedItem.ToString()).Select(p => p.Idgroups).ToListAsync();                    
                    var students = await db.Students.Where(p => p.GroupsIdgroups == days[0]).ToListAsync();

                    for (int i = 2; i < students.Count; i++)
                    {
                        worksheet.SetRows(i);
                        worksheet["A" + i] = students[i].StudentsSurname + " " + students[i].StudentsName;
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

                }
            }
        }

        private async void FillDates()
        {
            var worksheet = ReoGrid.CurrentWorksheet;

            using (zhirovContext db = new zhirovContext())
            {
                var days = await db.Dates.Where(p => p.Month == ComboBoxMonth.SelectedIndex + 1 && p.Year == 2022).Select(p => p.Day).ToListAsync();

                for (int i = 1; i < days.Count+1; i++)
                {
                    worksheet.SetCols(i + 1);
                    worksheet[0, i] = days[i - 1];
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
            }
        }

        private void FillText()
        {
            var worksheet = ReoGrid.CurrentWorksheet;

            worksheet["A1"] = "ФИО\\День месяца";
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            //   var workbook = ReoGrid;
            //  workbook.Save(@"table.xls", unvell.ReoGrid.IO.FileFormat.Excel2007);

            SaveJournal();
        }

        private void ReoGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!((e.Text == "н") || (e.Text == "Н") || (e.Text == "а") || (e.Text == "2") || (e.Text == "3") || (e.Text == "4") || (e.Text == "5") || (e.Text == "/") || (e.Text == "А")))
            {
                e.Handled = true;
            }
        }

        private void FillComoBoxMonth()
        {
            for (int i = 0; i < monthNames.Length; i++)
            {
                ComboBoxMonth.Items.Add(monthNames[i]);
            }
        }

        private void ComboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillTable();
        }

        private void SaveJournal()
        {
            DataBaseConn DbUser = new DataBaseConn();
            DataBaseControls DbControls = new DataBaseControls();
            MySqlConnection conn = DataBaseConn.GetDBConnection();

            MySqlCommand command = new MySqlCommand("insert into journal (students_idstudents, disciplines_iddisciplines, teachers_idteachers, journal_date, journal_score) " +
                "value (@students, @disciplines, @teachers, @date, @score)", conn);

            command.Parameters.Add("@students", MySqlDbType.VarChar).Value = 1;
            command.Parameters.Add("@disciplines", MySqlDbType.VarChar).Value = 1;
            command.Parameters.Add("@teachers", MySqlDbType.VarChar).Value = 1;

        }
    }
}