using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectroJournal.Pages.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для Disciplines.xaml
    /// </summary>
    public partial class Disciplines : Page
    {
        public Disciplines()
        {
            InitializeComponent();
            //FillListBoxDisciplines();
        }

        List<int> idDisp = new List<int>();

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxFullName.Text))
            {
                if (ListBoxDiscipline.SelectedItem != null)
                {
                    using (zhirovContext db = new zhirovContext())
                    {
                        Discipline? disp = await db.Disciplines.FirstOrDefaultAsync(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]);

                        if (disp != null)
                        {
                            disp.DisciplinesName = TextBoxFullName.Text;
                            disp.DisciplinesNameAbbreviated = TextBoxName.Text;
                            disp.DisciplinesIndex = TextBoxIndex.Text;

                            await db.SaveChangesAsync();

                            FillListBoxDisciplines();
                            ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Уведомление", "Сохранено");
                        }
                    }
                }
                else
                {
                    using (zhirovContext db = new())
                    {

                        Discipline disp = new Discipline
                        {
                            DisciplinesName = TextBoxFullName.Text,
                            DisciplinesNameAbbreviated = TextBoxName.Text,
                            DisciplinesIndex = TextBoxIndex.Text
                        };

                        await db.Disciplines.AddAsync(disp);
                        await db.SaveChangesAsync();

                        TextBoxFullName.Clear();
                        TextBoxName.Clear();
                        TextBoxIndex.Clear();
                        FillListBoxDisciplines();
                        ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Данные сохранены");
                    }
                }
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");
            }
        }
        private async void FillListBoxDisciplines()
        {
            ListBoxDiscipline.Items.Clear();
            idDisp.Clear();

            using (zhirovContext db = new zhirovContext())
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
        }
        private async void ListBoxDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonDelete.IsEnabled = true;

            if (ListBoxDiscipline.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
                    var disp = await db.Disciplines.Where(p => p.Iddisciplines == idDisp[ListBoxDiscipline.SelectedIndex]).FirstOrDefaultAsync();

                    TextBoxFullName.Text = disp.DisciplinesName;
                    TextBoxName.Text = disp.DisciplinesNameAbbreviated;
                    TextBoxIndex.Text = disp.DisciplinesIndex;
                }
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
            if (ListBoxDiscipline.Items.Count == 0)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
            }
            else if (ListBoxDiscipline.SelectedItem != null)
            {
                using (zhirovContext db = new zhirovContext())
                {
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
        }
        private void ListBoxDiscipline_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteDisp();
            }
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteDisp();
        }
        private void ComboBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBoxDisciplines();
        }
    }
}
