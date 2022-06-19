using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для AcademicYears.xaml
    /// </summary>
    public partial class AcademicYears : Page
    {
        public AcademicYears()
        {
            InitializeComponent();
            FillComboBox();
        }

        private async void ComboBoxSchoolYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxSchoolYears.SelectedIndex != -1)
                {
                    ButtonDeleteSchedule.IsEnabled = true;

                    using zhirovContext db = new();

                    var s = await db.Students.ToListAsync();
                    var j = await db.Journals.Where(j => j.StudyperiodIdstudyperiodNavigation.StudyperiodStart == ComboBoxSchoolYears.SelectedItem.ToString()).ToListAsync();
                    var g = await db.Groups.ToListAsync();
                    var m = await db.Chats.ToListAsync();
                    var p = await db.Presences.Where(p => DateOnly.FromDateTime(p.PresenceDatetime).Year == 2022).ToListAsync();

                    LabelStud.Content = $"Количество студентов: {s.Count}";
                    LabelScore.Content = $"Количество выставленных оценок: {j.Count}";
                    LabelGroups.Content = $"Количество групп: {g.Count}";
                    LabelMessage.Content = $"Количество отправленных сообщений: {m.Count}";
                    LabelStudPos.Content = $"Количество посещений: {p.Count}";
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ComboBoxSchoolYears_SelectionChanged (AcademicYears) | {ex.Message}");
            }
        }
        private void ButtonAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            RootDialog.Show();
            TextBoxYears.Clear();
        }
        private void ButtonDeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            var mb = new WPFUI.Controls.MessageBox();

            mb.ButtonLeftName = "Да";
            mb.ButtonRightName = "Нет";

            mb.ButtonLeftClick += MessageBox_LeftButtonClick;
            mb.ButtonRightClick += MessageBox_RightButtonClick;

            mb.Show("Оповещение", "Вы точно хотите удалить учебный год?\nДанные будут удалены без возможности восстановления.");
        }
        private async void MessageBox_LeftButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using zhirovContext db = new();
                if (ComboBoxSchoolYears.SelectedIndex != -1)
                {
                    var s = await db.Studyperiods.FirstOrDefaultAsync(s => s.StudyperiodStart == ComboBoxSchoolYears.SelectedItem.ToString());

                    if (s != null)
                    {
                        db.Studyperiods.Remove(s);
                        await db.SaveChangesAsync();
                        ButtonDeleteSchedule.IsEnabled = false;
                        FillComboBox();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"MessageBox_LeftButtonClick (AcademicYears) | {ex.Message}");
            }
        }
        private void MessageBox_RightButtonClick(object sender, RoutedEventArgs e) => (sender as WPFUI.Controls.MessageBox)?.Close();
        private async void FillComboBox()
        {
            try
            {
                ComboBoxSchoolYears.Items.Clear();
                using zhirovContext db = new();
                await db.Studyperiods.OrderByDescending(s => s.StudyperiodStart).ForEachAsync(s => ComboBoxSchoolYears.Items.Add(s.StudyperiodStart));
                ComboBoxSchoolYears.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBox (AcademicYears) | {ex.Message}");
            }
        }
        private void RootDialog_ButtonRightClick(object sender, RoutedEventArgs e) => RootDialog.Hide();
        private async void RootDialog_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(TextBoxYears.Text))
                {
                    if (TextBoxYears.Text.Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                    {
                        using zhirovContext db = new();

                        Studyperiod s = new()
                        {
                            StudyperiodStart = TextBoxYears.Text
                        };

                        await db.Studyperiods.AddAsync(s);
                        await db.SaveChangesAsync();
                        RootDialog.Hide();
                        FillComboBox();
                    }
                    else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Введите в формате гггг - гггг");
                }
                else ((MainWindow)Application.Current.MainWindow).Notifications("Уведомление", "Заполните поле");
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"RootDialog_ButtonLeftClick (AcademicYears) | {ex.Message}");
            }
        }
    }
}
