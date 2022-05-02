﻿using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Events;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal
    {
        public Journal()
        {
            InitializeComponent();

            FillComoBoxMonth();
            ComboBoxMonth.SelectedIndex = DateTime.Now.Month - 1;
            //FillTable();
            FillComboBoxDisp();
            FillComboBoxYears();
            // FillScore();
            var ws = ReoGrid.Worksheets[0];
            ws.CellDataChanged += rgrid_AfterCellEdit;
        }

        private int stuud;
        private int daysTable;
        string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

        private void FillTable()
        {
            var anim = (Storyboard)FindResource("AnimOpenLoad");
            anim.Begin();

            FillText();
            FillStudents();
            FillDates();
            FillScore();
            SettingSheet();
        }
        private async void FillScore()
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1 && ComboBoxDisp.SelectedIndex != -1)
            {
                var worksheet = ReoGrid.CurrentWorksheet;
                using (zhirovContext db = new())
                {
                    if (ComboBoxYears.SelectedItem != null)
                    {
                        for (int i = 1; i <= stuud; i++)
                        {
                            for (int j = 1; j < daysTable; j++)
                            {
                                if (!String.IsNullOrWhiteSpace(ReoGrid.CurrentWorksheet.Cells[i, 0].DisplayText))
                                {
                                    var period = await db.Studyperiods.Where(t => t.StudyperiodStart == ComboBoxYears.SelectedItem.ToString()).FirstOrDefaultAsync();

                                    var disp = await db.Disciplines.Where(d => d.DisciplinesNameAbbreviated == ComboBoxDisp.SelectedItem.ToString()).FirstOrDefaultAsync();
                                    string[] student2 = ReoGrid.CurrentWorksheet.Cells[i, 0].DisplayText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    var student = await db.Students.Where(s => s.StudentsName == student2[1] && s.StudentsSurname == student2[0]).FirstOrDefaultAsync();

                                    var scoreList = await db.Journals
                                        .Where(s => s.StudyperiodIdstudyperiod == period.Idstudyperiod &&
                                        s.JournalMonth == Convert.ToString(ComboBoxMonth.SelectedIndex + 1) &&
                                        s.DisciplinesIddisciplines == disp.Iddisciplines &&
                                        s.StudentsIdstudents == student.Idstudents &&
                                        s.JournalDay == ReoGrid.CurrentWorksheet.Cells[0, j].DisplayText)
                                        .FirstOrDefaultAsync();

                                    if (scoreList != null)
                                    {
                                        worksheet[i, j] = scoreList.JournalScore;
                                    }
                                }

                            }
                        }
                    }

                    /*if (ComboBoxYears.SelectedItem != null)
                    {
                        var period = await db.Studyperiods.Where(t => t.StudyperiodStart == ComboBoxYears.SelectedItem.ToString()).FirstOrDefaultAsync();
                        var disp = await db.Disciplines.Where(d => d.DisciplinesNameAbbreviated == ComboBoxDisp.SelectedItem.ToString()).FirstOrDefaultAsync();
                        var scoreList = await db.Journals
                            .Where(s => s.StudyperiodIdstudyperiod == period.Idstudyperiod && 
                            s.JournalMonth == Convert.ToString(ComboBoxMonth.SelectedIndex + 1) &&
                            s.DisciplinesIddisciplines == disp.Iddisciplines)
                            .ToListAsync();
                        foreach (var t in scoreList)
                        {
                            for (int i = 1; i < stuud; i++)
                            {
                                for (int j = 1; j < daysTable; j++)
                                {

                                    string[] student2 = ReoGrid.CurrentWorksheet.Cells[i, 0].DisplayText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    var student = await db.Students.Where(s => s.StudentsName == student2[1] && s.StudentsSurname == student2[0]).FirstOrDefaultAsync();                                    

                                    if (t.StudentsIdstudents == student.Idstudents && t.JournalDay == ReoGrid.CurrentWorksheet.Cells[0, j].DisplayText)
                                    {
                                        worksheet[i, j] = t.JournalScore;
                                    }
                                }
                            }
                        }
                    }*/
                }
            }
            var anim = (Storyboard)FindResource("AnimCloseLoad");
            anim.Begin();
        }
        private async void FillStudents()
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow).ComboBoxGroup.SelectedIndex != -1)
            {
                var worksheet = ReoGrid.CurrentWorksheet;
                using (zhirovContext db = new zhirovContext())
                {
                    var days = await db.Groups.Where(p => p.GroupsNameAbbreviated == ((MainWindow)System.Windows.Application.Current.MainWindow).ComboBoxGroup.SelectedItem.ToString()).Select(p => p.Idgroups).ToListAsync();
                    var students = await db.Students.Where(p => p.GroupsIdgroups == days[0]).OrderBy(p => p.StudentsSurname).ToListAsync();
                    stuud = students.Count;
                    for (int i = 2; i - 2 < stuud; i++)
                    {
                        worksheet.SetRows(i);
                        worksheet["A" + i] = students[i - 2].StudentsSurname + " " + students[i - 2].StudentsName;
                        worksheet.AutoFitColumnWidth(0, false);

                        unvell.ReoGrid.Cell? cell = worksheet.Cells["A" + i];
                        cell.IsReadOnly = true;

                        /*worksheet.SetRangeStyles("A1:A" + i, new WorksheetRangeStyle
                        {
                            Flag = PlainStyleFlag.FontSize | PlainStyleFlag.FontName | PlainStyleFlag.TextColor | PlainStyleFlag.LineColor | PlainStyleFlag.Padding,
                            FontName = "Segoe UI",
                            FontSize = 13,
                            TextColor = Colors.Black,
                            Padding = new PaddingValue(10),
                        });*/
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
                daysTable = days.Count + 1;
                for (int i = 1; i < daysTable; i++)
                {
                    worksheet.SetCols(days.Count + 1);
                    worksheet[0, i] = days[i - 1];
                    ReoGrid.DoAction(new SetColumnsWidthAction(1, i, 30));

                    unvell.ReoGrid.Cell? cell = worksheet.Cells[0, i];
                    cell.IsReadOnly = true;

                }
            }
            /*worksheet.SetRangeStyles("B1:BP150", new WorksheetRangeStyle
            {
                Flag = PlainStyleFlag.HorizontalAlign | PlainStyleFlag.FontSize | PlainStyleFlag.FontName | PlainStyleFlag.TextColor,
                HAlign = ReoGridHorAlign.Center,
                FontName = "Segoe UI",
                FontSize = 13,
                TextColor = Colors.Black
            });*/
        }
        private void FillText()
        {
            var worksheet = ReoGrid.CurrentWorksheet;

            worksheet["A1"] = "ФИО\\День месяца";
            /*worksheet.SetRangeStyles("A1:A1", new WorksheetRangeStyle
            {
                Flag = PlainStyleFlag.HorizontalAlign | PlainStyleFlag.FontSize | PlainStyleFlag.FontName | PlainStyleFlag.TextColor,
                HAlign = ReoGridHorAlign.Center,
                FontName = "Segoe UI",
                FontSize = 13,
                TextColor = Colors.Black
            });*/
        }
        private void SettingSheet()
        {

            var worksheet = ReoGrid.Worksheets[0];
            ReoGrid.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
            worksheet.SetSettings(WorksheetSettings.View_ShowHeaders, false);
            if (Properties.Settings.Default.Theme == 1)
            {
                ReoGrid.ControlStyle = new ControlAppearanceStyle(Colors.Black, Colors.WhiteSmoke, false);
            }
            else
            {
                ReoGrid.ControlStyle = new ControlAppearanceStyle(Colors.Gray, Colors.Black, false);
            }
        }
        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            //   var workbook = ReoGrid;
            //  workbook.Save(@"table.xls", unvell.ReoGrid.IO.FileFormat.Excel2007);

            //SaveJournal();
        }
        private void ReoGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!((e.Text == "н") || (e.Text == "Н") || (e.Text == "а") || (e.Text == "2") || (e.Text == "3") || (e.Text == "4") || (e.Text == "5") || (e.Text == "/") || (e.Text == "А")))
            {
                e.Handled = true;
            }
        }
        private async void FillComboBoxDisp()
        {
            ComboBoxDisp.Items.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Disciplines.OrderBy(t => t.DisciplinesNameAbbreviated).ForEachAsync(t =>
                {
                    ComboBoxDisp.Items.Add(t.DisciplinesNameAbbreviated);
                });
            }
            //ComboBoxDisp.SelectedIndex = 1;
        }
        private async void FillComboBoxYears()
        {
            ComboBoxYears.Items.Clear();

            using (zhirovContext db = new zhirovContext())
            {
                await db.Studyperiods.OrderBy(t => t.StudyperiodStart).ForEachAsync(t =>
                {
                    ComboBoxYears.Items.Add(t.StudyperiodStart);
                });
            }
            ComboBoxYears.SelectedIndex = 0;
        }
        private void FillComoBoxMonth()
        {
            foreach (var month in monthNames)
            {
                ComboBoxMonth.Items.Add(month);
            }
        }
        private void ComboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillTable();
        }
        private async void SaveJournal(string students,
            string disciplines,
            int teachers,
            string studyPeriod,
            string score,
            string time)
        {
            using (zhirovContext db = new())
            {
                string[] FIO = students.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] time2 = time.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                var student = await db.Students.Where(t => t.StudentsName == FIO[1] && t.StudentsSurname == FIO[0]).FirstOrDefaultAsync();
                var disp = await db.Disciplines.Where(t => t.DisciplinesNameAbbreviated == disciplines).FirstOrDefaultAsync();
                var period = await db.Studyperiods.Where(t => t.StudyperiodStart == studyPeriod).FirstOrDefaultAsync();

                if (student != null && disp != null & period != null && !String.IsNullOrWhiteSpace(score))
                {
                    DataBase.Journal journal = new DataBase.Journal
                    {
                        StudentsIdstudents = student.Idstudents,
                        DisciplinesIddisciplines = disp.Iddisciplines,
                        TeachersIdteachers = (uint)teachers,
                        StudyperiodIdstudyperiod = period.Idstudyperiod,
                        JournalYear = time2[0],
                        JournalMonth = time2[1],
                        JournalDay = time2[2],
                        JournalScore = score
                    };

                    await db.Journals.AddAsync(journal);
                    await db.SaveChangesAsync();
                }
            }
        }
        private void ReoGrid_WorkbookLoaded(object sender, EventArgs e)
        {
            FillTable();
        }
        void rgrid_AfterCellEdit(object sender, CellEventArgs e)
        {
            string[] poz = ReoGrid.CurrentWorksheet.SelectionRange.ToString().Split(new char[] { ':' });
            string score = poz[0];

            char poz3 = score[1];

            string poz4 = score.Last().ToString();

            int stud = int.Parse(Regex.Match(score, @"\d+").Value);

            string poz6 = Regex.Replace(score, @"[^A-Z]+", string.Empty);

            if (ComboBoxDisp.SelectedItem != null && !String.IsNullOrWhiteSpace(ReoGrid.CurrentWorksheet.Cells[stud - 1, 0].DisplayText))
            {
                SaveJournal(ReoGrid.CurrentWorksheet.Cells[stud - 1, 0].DisplayText, ComboBoxDisp.SelectedItem.ToString(), Properties.Settings.Default.UserID, ComboBoxYears.SelectedItem.ToString(), ReoGrid.CurrentWorksheet.Cells[score].DisplayText, $"{CheckYear()}.{ComboBoxMonth.SelectedIndex + 1}.{ReoGrid.CurrentWorksheet.Cells[$"{poz6}1"].DisplayText}");
            }

            LabelTest.Content = $"оценка {ReoGrid.CurrentWorksheet.Cells[score].DisplayText} фио {ReoGrid.CurrentWorksheet.Cells[stud - 1, 0].DisplayText} дата {ReoGrid.CurrentWorksheet.Cells[$"{poz6}1"].DisplayText} позиция {score}";
        }
        private string CheckYear()
        {
            string[] year = ComboBoxYears.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (ComboBoxMonth.SelectedIndex <= 6)
            {
                return year[1];
            }
            else if (ComboBoxMonth.SelectedIndex >= 8)
            {
                return year[0];
            }

            return null;
        }
        private void ComboBoxDisp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxDisp.SelectedItem != null)
            {
                FillTable();
            }
        }
    }
}