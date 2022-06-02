using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Disciplines.xaml
    /// </summary>
    public partial class Disciplines : Page
    {
        public Disciplines()
        {
            InitializeComponent();
        }

        List<int> idDisp = new List<int>();

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TextBoxName.Text))
                {
                    using zhirovContext db = new();

                    if (ListBoxDiscipline.SelectedItem != null)
                    {
                        Discipline? disp = await db.Disciplines.FirstOrDefaultAsync(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]);

                        if (disp != null)
                        {
                            disp.DisciplinesName = TextBoxFullName.Text;
                            disp.DisciplinesNameAbbreviated = TextBoxName.Text;
                            disp.DisciplinesIndex = TextBoxIndex.Text;
                        }
                    }
                    else
                    {
                        Discipline disp = new Discipline
                        {
                            DisciplinesName = TextBoxFullName.Text,
                            DisciplinesNameAbbreviated = TextBoxName.Text,
                            DisciplinesIndex = TextBoxIndex.Text
                        };

                        await db.Disciplines.AddAsync(disp);
                        await db.SaveChangesAsync();
                    }
                    TextBoxFullName.Clear();
                    TextBoxName.Clear();
                    TextBoxIndex.Clear();
                    TextBoxFullName.Focus();
                    FillListBoxDisciplines();
                }
                else ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonSave_Click | {ex.Message}");
            }
        }
        private async void FillListBoxDisciplines()
        {
            try
            {
                ListBoxDiscipline.Items.Clear();
                idDisp.Clear();

                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    switch (ComboBoxSorting.SelectedIndex)
                    {
                        case 0:
                            await db.Disciplines.OrderBy(t => t.DisciplinesNameAbbreviated).ForEachAsync(t =>
                            {
                                ListBoxDiscipline.Items.Add($"{t.DisciplinesNameAbbreviated}");
                                idDisp.Add((int)t.Iddisciplines);
                            });
                            break;
                        case 1:
                            await db.Disciplines.OrderByDescending(t => t.DisciplinesNameAbbreviated).ForEachAsync(t =>
                            {
                                ListBoxDiscipline.Items.Add($"{t.DisciplinesNameAbbreviated}");
                                idDisp.Add((int)t.Iddisciplines);
                            });
                            break;
                    }
                }
                else
                {
                    await db.Disciplines.OrderBy(t => t.DisciplinesNameAbbreviated).Where(t => EF.Functions.Like(t.DisciplinesNameAbbreviated, $"%{SearchBox.Text}%")).ForEachAsync(t =>
                    {
                        ListBoxDiscipline.Items.Add($"{t.DisciplinesNameAbbreviated}");
                        idDisp.Add((int)t.Iddisciplines);
                    });
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBoxDisciplines | {ex.Message}");
            }
        }
        private async void ListBoxDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ButtonDelete.IsEnabled = true;

                if (ListBoxDiscipline.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    var disp = await db.Disciplines.Where(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]).FirstOrDefaultAsync();

                    if (disp != null)
                    {
                        TextBoxFullName.Text = disp.DisciplinesName;
                        TextBoxName.Text = disp.DisciplinesNameAbbreviated;
                        TextBoxIndex.Text = disp.DisciplinesIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxDiscipline_SelectionChanged | {ex.Message}");
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ListBoxDiscipline.SelectedItem = null;
            TextBoxFullName.Clear();
            TextBoxName.Clear();
            TextBoxIndex.Clear();
        }
        private async void DeleteDisp()
        {
            try
            {
                if (ListBoxDiscipline.Items.Count == 0)
                {
                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                }
                else if (ListBoxDiscipline.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    Discipline? disp = await db.Disciplines.FirstOrDefaultAsync(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]);

                    if (disp != null)
                    {
                        db.Disciplines.Remove(disp);
                        await db.SaveChangesAsync();

                        ListBoxDiscipline.Items.Clear();
                        FillListBoxDisciplines();
                        TextBoxFullName.Clear();
                        TextBoxName.Clear();
                        TextBoxIndex.Clear();
                        ButtonDelete.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"DeleteDisp | {ex.Message}");
            }
        }
        private void ListBoxDiscipline_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteDisp();
            }
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e) => DeleteDisp();
        private void ComboBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxDisciplines();
        private void SearchBox_PreviewKeyUp(object sender, KeyEventArgs e) => FillListBoxDisciplines();
    }
}
