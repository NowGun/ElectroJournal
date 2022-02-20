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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElectroJournal.Windows;
using MySql.Data.MySqlClient;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using ElectroJournal.Classes;
using ElectroJournal.DataBase;
using Microsoft.EntityFrameworkCore;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        public Users()
        {
            InitializeComponent();
            LoadData();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            using (zhirovContext db = new zhirovContext())
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == Properties.Settings.Default.UserID);

                if (teacher != null)
                {
                    teacher.TeachersPhone = TextBoxPhone.Text;
                    teacher.TeachersMail = TextBoxMail.Text;
                    db.SaveChanges();

                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные обновлены");
                }
            }
        }

        private void ButtonResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            //((MainWindow)Application.Current.MainWindow).AnimLog(true);

            new Windows.ResetPassword().ShowDialog();
        }

        private void LabelChangePhoto_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void LabelChangePhoto_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void LabelChangePhoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        }

        public void ButtonLogoutUser_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).GridNLogin.Visibility = Visibility.Visible;
            //((MainWindow)Application.Current.MainWindow).AnimLog(true);
        }

        private async void LoadData()
        {
            using (zhirovContext db = new zhirovContext())
            {
                var teachers = await db.Teachers.Where(p => p.Idteachers == Properties.Settings.Default.UserID).ToListAsync();

                foreach (var t in teachers)
                {
                    string FIO = t.TeachersSurname + " " + t.TeachersName + " " + t.TeachersPatronymic;

                    TextBlockFIO.Content = t.TeachersSurname + " " + t.TeachersName + " " + t.TeachersPatronymic;
                    PasswordBoxPassword.Password = t.TeachersPassword;
                    LabelIDUser.Content = "Id: " + t.Idteachers;
                    TextBoxPhone.Text = t.TeachersPhone;
                    TextBoxMail.Text = t.TeachersMail;

                    ButtonSave.IsEnabled = false;
                }
            }
        }

        private void TextBoxPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ButtonSave.IsEnabled = true;
        }

        private void TextBoxMail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ButtonSave.IsEnabled = true;
        }
    }
}
