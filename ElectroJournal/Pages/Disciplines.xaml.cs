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
            FillComboBoxGroups();
            FillComboBoxCourse();
            FillListBoxCycle();
        }

        List<int> idDisp = new List<int>();
        List<int> idCycle = new List<int>();
        private bool changeCycle = false;

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TextBoxFullName.Text) && ComboBoxCours.SelectedIndex != -1 && ComboBoxGroup.SelectedIndex != -1 && ListBoxCycle.SelectedIndex != -1)
                {
                    using zhirovContext db = new();
                    
                    if (ListBoxDiscipline.SelectedItem != null)
                    {
                        string[] name = ListBoxCycle.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        Discipline? disp = await db.Disciplines
                            .Include(d => d.CycleIdcycleNavigation)
                            .FirstOrDefaultAsync(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]);
                        var c = await db.Cycles.FirstOrDefaultAsync(c => c.CyclуName == name[1].Trim() && c.CycleIndex == name[0].Trim());
                        if (disp != null)
                        {
                            disp.DisciplinesName = TextBoxFullName.Text;
                            disp.DisciplinesNameAbbreviated = TextBoxName.Text;
                            disp.DisciplinesIndex = TextBoxIndex.Text;
                            disp.DisciplinesHours = int.Parse(NumberBoxHours.Text);
                            disp.CycleIdcycle = c.Idcycle;

                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var g = await db.Groups.FirstOrDefaultAsync(g => g.GroupsNameAbbreviated == ComboBoxGroup.SelectedItem.ToString());
                        var c = await db.Courses.FirstOrDefaultAsync(c => c.CourseName == ComboBoxCours.SelectedItem.ToString());

                        Discipline disp = new Discipline
                        {
                            DisciplinesName = TextBoxFullName.Text,
                            DisciplinesNameAbbreviated = TextBoxName.Text,
                            DisciplinesIndex = TextBoxIndex.Text,
                            DisciplinesHours = int.Parse(NumberBoxHours.Text),
                            GroupsIdgroups = g.Idgroups,
                            CourseIdcourse = c.Idcourse,
                            CycleIdcycle = (uint)idCycle[ListBoxCycle.SelectedIndex]
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

                if (ComboBoxCours.SelectedIndex != -1 && ComboBoxGroup.SelectedIndex != -1)
                {
                    /*if (String.IsNullOrWhiteSpace(SearchBox.Text))
                    {
                        await db.Disciplines
                            .Where(t => t.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ComboBoxGroup.SelectedItem.ToString()
                            && t.CourseIdcourseNavigation.CourseName == ComboBoxCours.SelectedItem.ToString())
                            .OrderBy(t => t.DisciplinesName)
                            .ForEachAsync(t =>
                        {
                            ListBoxDiscipline.Items.Add($"{t.DisciplinesName}");
                            idDisp.Add((int)t.Iddisciplines);
                        });
                    }
                    else
                    {
                        await db.Disciplines.OrderBy(t => t.DisciplinesName)
                            .Where(t => t.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ComboBoxGroup.SelectedItem.ToString()
                            && t.CourseIdcourseNavigation.CourseName == ComboBoxCours.SelectedItem.ToString()
                            && (EF.Functions.Like(t.DisciplinesNameAbbreviated, $"%{SearchBox.Text}%")
                            || EF.Functions.Like(t.DisciplinesIndex, $"%{SearchBox.Text}%")
                            || EF.Functions.Like(t.DisciplinesName, $"%{SearchBox.Text}%")))
                            .ForEachAsync(t =>
                            {
                                ListBoxDiscipline.Items.Add($"{t.DisciplinesName}");
                                idDisp.Add((int)t.Iddisciplines);
                            });
                    }*/
                    if (String.IsNullOrWhiteSpace(SearchBox.Text))
                    {
                        await db.Disciplines
                            .OrderBy(t => t.DisciplinesName)
                            .ForEachAsync(t =>
                        {
                            ListBoxDiscipline.Items.Add($"{t.DisciplinesName}");
                            idDisp.Add((int)t.Iddisciplines);
                        });
                    }
                    else
                    {
                        await db.Disciplines.OrderBy(t => t.DisciplinesName)
                            .Where(t => EF.Functions.Like(t.DisciplinesNameAbbreviated, $"%{SearchBox.Text}%")
                            || EF.Functions.Like(t.DisciplinesIndex, $"%{SearchBox.Text}%")
                            || EF.Functions.Like(t.DisciplinesName, $"%{SearchBox.Text}%"))
                            .ForEachAsync(t =>
                            {
                                ListBoxDiscipline.Items.Add($"{t.DisciplinesName}");
                                idDisp.Add((int)t.Iddisciplines);
                            });
                    }
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
                    var disp = await db.Disciplines
                        .Include(p =>  p.CycleIdcycleNavigation)
                        .Where(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex])
                        .FirstOrDefaultAsync();

                    if (disp != null)
                    {
                        TextBoxFullName.Text = disp.DisciplinesName;
                        TextBoxName.Text = disp.DisciplinesNameAbbreviated;
                        TextBoxIndex.Text = disp.DisciplinesIndex;
                        NumberBoxHours.Text = disp.DisciplinesHours.ToString();
                        ListBoxCycle.SelectedItem = $"{disp.CycleIdcycleNavigation.CycleIndex} - {disp.CycleIdcycleNavigation.CyclуName}";
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxDiscipline_SelectionChanged | {ex.Message}");
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => ClearValue();
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
                    Discipline? disp = await db.Disciplines
                        .FirstOrDefaultAsync(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]);

                    if (disp != null)
                    {
                        db.Disciplines.Remove(disp);
                        await db.SaveChangesAsync();

                        FillListBoxDisciplines();
                        ClearValue();
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
        private void SearchBox_PreviewKeyUp(object sender, KeyEventArgs e) => FillListBoxDisciplines();
        private void Page_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D)
            {
                ClearValue();
                TextBoxFullName.Focus();
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SearchBox.Clear();
                SearchBox.Focus();
            }
        }
        private async void FillComboBoxGroups()
        {
            try
            {
                ComboBoxGroup.Items.Clear();
                using zhirovContext db = new();
                await db.Groups.OrderBy(g => g.CourseIdcourseNavigation.CourseName).ForEachAsync(g =>
                {
                    ComboBoxGroup.Items.Add(g.GroupsNameAbbreviated);
                });
            }
            catch (Exception ex)
            {

            }
        }
        private async void FillComboBoxCourse()
        {
            try
            {
                ComboBoxCours.Items.Clear();
                using zhirovContext db = new();
                await db.Courses.OrderBy(g => g.CourseName).ForEachAsync(g =>
                {
                    ComboBoxCours.Items.Add(g.CourseName);
                });
            }
            catch (Exception ex)
            {

            }
        }
        private async void FillListBoxCycle()
        {
            try
            {
                idCycle.Clear();
                ListBoxCycle.Items.Clear();
                using zhirovContext db = new();
                await db.Cycles.OrderBy(c => c.CyclуName).ForEachAsync(c => 
                {
                    ListBoxCycle.Items.Add(string.IsNullOrWhiteSpace(c.CycleIndex) ? $"{c.CyclуName}" : $"{c.CycleIndex} - {c.CyclуName}");
                    idCycle.Add((int)c.Idcycle);
                });
            }
            catch (Exception ex)
            {

            }
        }
        private void ClearValue()
        {
            ListBoxDiscipline.SelectedItem = null;
            TextBoxFullName.Clear();
            TextBoxName.Clear();
            TextBoxIndex.Clear();
        }
        private void RootDialog_ButtonRightClick(object sender, RoutedEventArgs e) => RootDialog.Hide();
        private async void RootDialog_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using zhirovContext db = new();

                if (!changeCycle)
                {
                    Cycle c = new()
                    {
                        CyclуName = TextBoxCycleName.Text,
                        CycleIndex = TextBoxCycleindex.Text,
                    };

                    await db.Cycles.AddAsync(c);
                    await db.SaveChangesAsync();
                }
                else
                {
                    Cycle? c = await db.Cycles.FirstOrDefaultAsync(c => c.Idcycle == idCycle[ListBoxCycle.SelectedIndex]);

                    if (c != null)
                    {
                        c.CyclуName = TextBoxCycleName.Text;
                        c.CycleIndex = TextBoxIndex.Text;

                        await db.SaveChangesAsync();
                    }
                }
                FillListBoxCycle();
                RootDialog.Hide();
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"RootDialog_ButtonLeftClick | {ex.Message}");
            }
        }
        private async void IconChangeCycle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ListBoxCycle.SelectedIndex != -1)
                {
                    changeCycle = true;

                    using zhirovContext db = new();
                    var c = await db.Cycles.FirstOrDefaultAsync(c => c.Idcycle == idCycle[ListBoxCycle.SelectedIndex]);

                    if (c != null)
                    {
                        TextBoxCycleindex.Text = c.CycleIndex;
                        TextBoxCycleName.Text = c.CyclуName;
                    }

                    RootDialog.Show();
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"IconChangeHousing_PreviewMouseLeftButtonUp | {ex.Message}");
            }
        }
        private async void IconDeleteCycle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ListBoxCycle.Items.Count == 0)
                {
                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                }
                else if (ListBoxCycle.SelectedItem != null)
                {
                    using zhirovContext db = new();
                    Cycle? c = await db.Cycles.FirstOrDefaultAsync(c => c.Idcycle == idCycle[ListBoxCycle.SelectedIndex]);

                    if (c != null)
                    {
                        db.Cycles.Remove(c);
                        await db.SaveChangesAsync();

                        FillListBoxCycle();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"IconDeleteHousing_PreviewMouseLeftButtonUp | {ex.Message}");
            }
        }
        private void IconAddCycle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            changeCycle = false;
            TextBoxCycleindex.Clear();
            TextBoxCycleName.Clear();
            RootDialog.Show();
        }
        private void ComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxDisciplines();
        private void ComboBoxCours_SelectionChanged(object sender, SelectionChangedEventArgs e) => FillListBoxDisciplines();
    }
}
