using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            FillListBoxDisciplines();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {    
            /*
            using (zhirovContext db = new zhirovContext())
            {
                Discipline discipline = new Discipline { DisciplinesNameAbbreviated = TextBoxName.Text };

                db.Disciplines.Add(discipline);
                await db.SaveChangesAsync();
                FillListBoxDisciplines();
            }*/
        }

        private async void FillListBoxDisciplines()
        {/*
            using (zhirovContext db = new zhirovContext())
            {
                await db.Disciplines.ForEachAsync(p => 
                {
                    ListBoxDisciplines.Items.Add(p.DisciplinesNameAbbreviated);
                });

                
            }*/
        }
    }
}
