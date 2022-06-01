using EJAdmin.DataBaseClasses;
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

namespace EJAdmin.Pages
{
    /// <summary>
    /// Логика взаимодействия для ControlEJ.xaml
    /// </summary>
    public partial class ControlEJ : Page
    {
        public ControlEJ()
        {
            InitializeComponent();
            LoadInfo();
            FillListBox();
        }

        private async void FillListBox()
        {
            try
            {
                ListBoxUniver.Items.Clear();
                using electrojournalContext db = new();
                await db.Educationals.OrderBy(d => d.Name).ForEachAsync(d => ListBoxUniver.Items.Add(d.Name));
            }
            catch
            {

            }
        }
        private async void UpdateVersion()
        {
            try
            {
                using electrojournalContext db = new();

                DataBaseClasses.Version? v = await db.Versions.FirstOrDefaultAsync(v => v.Idversion == 1);

                if (v != null)
                {
                    v.VersionName = TextBoxVersion.Text;
                    await db.SaveChangesAsync();
                }
            }
            catch
            {

            }
        }
        private async void LoadInfo()
        {
            using electrojournalContext db = new();

            DataBaseClasses.Version? v = await db.Versions.FirstOrDefaultAsync(v => v.Idversion == 1);
            if (v != null)
            {
                TextBoxVersion.Text = v.VersionName;
            }
        }
        private void TextBoxVersion_LostFocus(object sender, RoutedEventArgs e) => UpdateVersion();
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TextBoxDB.Clear();
            TextBoxUniver.Clear();
            ListBoxUniver.SelectedIndex = -1;
            ButtonDel.IsEnabled = false;
        }
        private async void ListBoxUniver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ButtonDel.IsEnabled = true;
                using electrojournalContext db = new();
                Educational? d = await db.Educationals.FirstOrDefaultAsync(d => d.Name == ListBoxUniver.SelectedItem.ToString());

                if (d != null)
                {
                    TextBoxUniver.Text = d.Name;
                    TextBoxDB.Text = d.NameDb;
                }
            }
            catch
            {

            }
        }
        private async void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using electrojournalContext db = new();
                Educational? d = await db.Educationals.FirstOrDefaultAsync(d => d.Name == ListBoxUniver.SelectedItem.ToString());

                if (d != null)
                {
                    db.Educationals.Remove(d);
                    await db.SaveChangesAsync();
                    FillListBox();
                    TextBoxDB.Clear();
                    TextBoxUniver.Clear();
                    ButtonDel.IsEnabled = false;
                }
            }
            catch
            {

            }
        }
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(TextBoxUniver.Text) && !String.IsNullOrWhiteSpace(TextBoxDB.Text))
                {
                    using electrojournalContext db = new();

                    if (ListBoxUniver.SelectedIndex == -1)
                    {
                        Educational d = new Educational
                        {
                            Name = TextBoxUniver.Text,
                            NameDb = TextBoxDB.Text,
                        };

                        await db.Educationals.AddAsync(d);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        Educational? d = await db.Educationals.FirstOrDefaultAsync(d => d.Name == ListBoxUniver.SelectedItem.ToString());

                        if (d != null)
                        {
                            d.Name = TextBoxUniver.Text;
                            d.NameDb = TextBoxDB.Text;

                            await db.SaveChangesAsync();
                        }
                    }

                    FillListBox();
                    TextBoxDB.Clear();
                    TextBoxUniver.Clear();
                    ButtonDel.IsEnabled = false;
                }
            }
            catch 
            {

            }
        }
    }
}