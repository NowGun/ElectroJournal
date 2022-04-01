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
                    await db.SaveChangesAsync();

                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные обновлены");
                }
            }
        }

        private void ButtonResetPassword_Click(object sender, RoutedEventArgs e)
        {
            new Windows.ChangePassword().ShowDialog();
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
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Изображения(*.jpg;*.png)|*.JPG;*.PNG" + "|Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                //TextBoxPath.Text = myDialog.FileName;
                //path = myDialog.FileName;
            }
        }

        public void ButtonLogoutUser_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).GridNLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).LabelScheduleCall.Content = "";
            ((MainWindow)Application.Current.MainWindow).timer2.Stop();
            ((MainWindow)Application.Current.MainWindow).animLabel = true;
            ((MainWindow)Application.Current.MainWindow).AnimLog(true);
        }

        private async void LoadData()
        {
            using (zhirovContext db = new zhirovContext())
            {
                var teachers = await db.Teachers.Where(p => p.Idteachers == Properties.Settings.Default.UserID).ToListAsync();

                foreach (var t in teachers)
                {
                    string FIO = t.TeachersSurname + " " + t.TeachersName + " " + t.TeachersPatronymic;

                    //PersonPicture.DisplayName = $"{t.TeachersName} {t.TeachersSurname}";
                    

                    
                        var myImage = new Image();
                        var stringPath = @"https://thefilmstage.com/wp-content/uploads/2020/07/Feels-Good-Man-1.jpg";

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                    bitmapImage.EndInit();

                    myImage.Source = bitmapImage;

                       /* Uri imageUri = new Uri(stringPath, UriKind.Absolute);
                        BitmapImage imageBitmap = new BitmapImage(imageUri);
                        myImage.Source = imageBitmap;*/

                        PersonPicture.ProfilePicture = bitmapImage;
                    
                    

                    



                    TextBlockFIO.Content = FIO;
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
