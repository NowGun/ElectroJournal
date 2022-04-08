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
using SFTPService;
using Renci.SshNet;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Renci.SshNet.Async;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

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
            CardUser.Visibility = Visibility.Hidden;
            LoadData();
            ListViewTeachersRefresh();

            ListViewTeachers.SelectedIndex = 0;
        }

        List<int> idTeachers = new List<int>();

        private static string? path;

        public System.Windows.Threading.DispatcherTimer timer2 = new();

        private string firstname;

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            using (zhirovContext db = new zhirovContext())
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Idteachers == Properties.Settings.Default.UserID);

                if (teacher != null)
                {
                    teacher.TeachersPhone = TextBoxPhone.Text;
                    teacher.TeachersMail = TextBoxMail.Text;
                    teacher.TeachersImage = $@"http://193.33.230.80/public_html/imagesEJ/{Properties.Settings.Default.UserID}Photo{System.IO.Path.GetExtension(path)}";
                    await db.SaveChangesAsync();

                    try
                    {
                        using (SftpClient client = new SftpClient(new PasswordConnectionInfo(Properties.Settings.Default.Server, Properties.Settings.Default.UserName, Properties.Settings.Default.Password)))
                        {
                            client.Connect();
                            if (client.IsConnected)
                            {
                                var fileStream = new FileStream($@"{path}", FileMode.Open);

                                if (path != null)
                                {                                  
                                    await client.UploadAsync(fileStream, $@"/var/www/daniil-server/public_html/imagesEJ/{Properties.Settings.Default.UserID}Photo{System.IO.Path.GetExtension(path)}");
                                    client.Disconnect();
                                    client.Dispose();
                                    LoadData();
                                    ((MainWindow)Application.Current.MainWindow).Notifications("Сообщение", "Данные обновлены");
                                }
                                fileStream.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            ProgressBar.Visibility = Visibility.Hidden;
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
                LabelDeletePhoto.Visibility = Visibility.Visible;
                ButtonSave.IsEnabled = true;
                path = myDialog.FileName;

                BitmapImage bitmap = new BitmapImage();
                
                //var stream = File.OpenRead(path);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                //bitmap.StreamSource = stream;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
/*                stream.Close();
                stream.Dispose();*/

                PersonPicture.ProfilePicture = bitmap;
            }
        }

        public void ButtonLogoutUser_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl sc = new();

            ((MainWindow)Application.Current.MainWindow).TextBoxLogin.Text = "";
            ((MainWindow)Application.Current.MainWindow).TextBoxPassword.Password = "";
            sc.CompletionLogin();
            ((MainWindow)Application.Current.MainWindow).GridLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).GridMenu.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).Frame.Visibility = Visibility.Hidden;
            ((MainWindow)Application.Current.MainWindow).GridNLogin.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).LabelScheduleCall.Content = "";
            ((MainWindow)Application.Current.MainWindow).timer2.Stop();
            ((MainWindow)Application.Current.MainWindow).ButtonShowLogin.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).animLabel = true;
            ((MainWindow)Application.Current.MainWindow).AnimLog(true);
        }

        private async void LoadData()
        {
            try
            {
                var anim = (Storyboard)FindResource("AnimLoad");
                var anim2 = (Storyboard)FindResource("AnimLoadComplete");
                anim.Begin();

                using (zhirovContext db = new zhirovContext())
                {
                    var teachers = await db.Teachers.Where(p => p.Idteachers == Properties.Settings.Default.UserID).FirstOrDefaultAsync();

                    string FIO = teachers.TeachersSurname + " " + teachers.TeachersName + " " + teachers.TeachersPatronymic;

                    if (!String.IsNullOrWhiteSpace(teachers.TeachersImage))
                    {
                        LabelDeletePhoto.Visibility = Visibility.Visible;
                        var myImage = new System.Windows.Controls.Image();

                        var stringPath = $@"{teachers.TeachersImage}";

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                        bitmapImage.EndInit();
                        PersonPicture.ProfilePicture = bitmapImage;

                        ((MainWindow)Application.Current.MainWindow).RefreshImage(teachers.TeachersImage);
                    }
                    else
                    {
                        LabelDeletePhoto.Visibility = Visibility.Hidden;
                        PersonPicture.ProfilePicture = null;
                        PersonPicture.DisplayName = $"{teachers.TeachersName} {teachers.TeachersSurname}";
                    }

                    TextBlockFIO.Content = FIO;
                    PasswordBoxPassword.Password = teachers.TeachersPassword;
                    LabelIDUser.Content = "Id: " + teachers.Idteachers;
                    TextBoxPhone.Text = teachers.TeachersPhone;
                    TextBoxMail.Text = teachers.TeachersMail;

                    ButtonSave.IsEnabled = false;
                }
                anim2.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private async void LabelDeletePhoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            using (zhirovContext db = new())
            {
                var teacher = await db.Teachers.Where(p => p.Idteachers == Properties.Settings.Default.UserID).FirstOrDefaultAsync();

                if (!String.IsNullOrWhiteSpace(teacher.TeachersImage))
                {
                    teacher.TeachersImage = "";
                    await db.SaveChangesAsync();

                    try 
                    {
                        using (SftpClient client = new SftpClient(new PasswordConnectionInfo(Properties.Settings.Default.Server, Properties.Settings.Default.UserName, Properties.Settings.Default.Password)))
                        {
                            client.Connect();
                            if (client.IsConnected)
                            {
                                var fileStream = new FileStream($@"{path}", FileMode.Open);

                                if (path != null)
                                {
                                    client.DeleteFile($@"/var/www/daniil-server/public_html/imagesEJ/{Properties.Settings.Default.UserID}Photo{System.IO.Path.GetExtension(path)}");
                                    client.Disconnect();
                                    client.Dispose();
                                    ((MainWindow)Application.Current.MainWindow).RefreshImage(teacher.TeachersImage);
                                }
                                fileStream.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    { 
                        MessageBox.Show(ex.Message);
                    }                    
                }
                PersonPicture.ProfilePicture = null;
                PersonPicture.DisplayName = $"{teacher.TeachersName} {teacher.TeachersSurname}";
            }
            LabelDeletePhoto.Visibility = Visibility.Hidden;
        }


        private async void ListViewTeachersRefresh()
        {
            try
            {
                ListViewTeachers.Items.Clear();
                idTeachers.Clear();
                ObservableCollection<UsersListBox> co1 = new ObservableCollection<UsersListBox>();

                co1.Add(new UsersListBox
                {
                    textFIO = $"Моя страница"
                });

                using (zhirovContext db = new zhirovContext())
                {
                    await db.Teachers.Where(t => t.Idteachers != Properties.Settings.Default.UserID).OrderBy(t => t.TeachersSurname).ForEachAsync(t =>
                    {
                        if (!String.IsNullOrWhiteSpace(t.TeachersImage))
                        {
                            var myImage = new System.Windows.Controls.Image();

                            var stringPath = $@"{t.TeachersImage}";

                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                            bitmapImage.EndInit();

                            co1.Add(new UsersListBox
                            {

                                image = bitmapImage,
                                textFIO = $"{t.TeachersSurname} {t.TeachersName}"
                            });
                        }
                        else
                        {
                            co1.Add(new UsersListBox
                            {

                                imageDN = $"{t.TeachersSurname} {t.TeachersName}",
                                textFIO = $"{t.TeachersSurname} {t.TeachersName}"
                            });
                        }

                        idTeachers.Add((int)t.Idteachers);
                        ListViewTeachers.ItemsSource = co1;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private async void ListViewTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListViewTeachers.SelectedItem != null)
                {
                    if (ListViewTeachers.SelectedIndex != 0)
                    {
                        CardUser.Visibility = Visibility.Visible;
                        grid.Visibility = Visibility.Hidden;

                        using (zhirovContext db = new zhirovContext())
                        {
                            var teachers = await db.Teachers.Where(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex - 1]).ToListAsync();

                            foreach (var t in teachers)
                            {
                                TextBlockName.Content = t.TeachersSurname + " " + t.TeachersName;

                                if (!String.IsNullOrWhiteSpace(t.TeachersImage))
                                {
                                    var myImage = new System.Windows.Controls.Image();
                                    var stringPath = $@"{t.TeachersImage}";

                                    firstname = t.TeachersName;
                                    timer2.Tick += new EventHandler(ListBoxMessageRefresh);
                                    timer2.Interval = new TimeSpan(0, 0, 1);
                                    timer2.Start();

                                    BitmapImage bitmapImage = new BitmapImage();
                                    bitmapImage.BeginInit();
                                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                    bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                                    bitmapImage.EndInit();
                                    PersonPictureUser.ProfilePicture = bitmapImage;
                                }
                                else
                                {
                                    PersonPictureUser.ProfilePicture = null;
                                    PersonPictureUser.DisplayName = $"{t.TeachersName} {t.TeachersSurname}";
                                }
                                ListBoxMessageRefresh2();
                            }
                        }
                    }
                    else if (ListViewTeachers.SelectedIndex == 0)
                    {
                        timer2.Stop();
                        CardUser.Visibility = Visibility.Hidden;
                        grid.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (zhirovContext db = new())
                {
                    Chat chat = new Chat
                    {
                        TeachersFrom = (uint)Properties.Settings.Default.UserID,
                        TeachersTo = (uint)idTeachers[ListViewTeachers.SelectedIndex - 1],
                        ChatText = TextBoxMessage.Text
                    };

                    await db.Chats.AddAsync(chat);
                    await db.SaveChangesAsync();

                    //ListBoxMessageRefresh();
                    //ListBoxMessageRefresh2();
                    /*                ListBoxMessage.Items.MoveCurrentToLast();
                                    ListBoxMessage.ScrollIntoView(ListBoxMessage.Items.CurrentItem);
                                    ListBoxMessage.ScrollIntoView();*/
                    TextBoxMessage.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void ListBoxMessageRefresh(object sender, EventArgs e)
        {
            try 
            {
                ObservableCollection<MessageListBox> co1 = new ObservableCollection<MessageListBox>();
                using (zhirovContext db = new zhirovContext())
                {
                    await db.Chats.Where(c => (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == idTeachers[ListViewTeachers.SelectedIndex - 1]) ||
                    (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == idTeachers[ListViewTeachers.SelectedIndex - 1])).ForEachAsync(c =>
                    {
                        if (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == idTeachers[ListViewTeachers.SelectedIndex - 1])
                        {
                            co1.Add(new MessageListBox
                            {
                                name = firstname,
                                text = c.ChatText,
                                time = Convert.ToString(c.ChatDate)
                            });
                        }
                        else if (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == idTeachers[ListViewTeachers.SelectedIndex - 1])
                        {
                            co1.Add(new MessageListBox
                            {
                                name = Properties.Settings.Default.FirstName,
                                text = c.ChatText,
                                time = Convert.ToString(c.ChatDate)
                            });
                        }
                    });
                    //idTeachers.Add((int)t.Idteachers);
                    ListBoxMessage.ItemsSource = co1;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }

        private async void ListBoxMessageRefresh2()
        {
            try
            {
                IList<MessageListBox> co1 = new List<MessageListBox>();
                co1.Clear();
                using (zhirovContext db = new zhirovContext())
                {
                    await db.Chats.Where(c => (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == idTeachers[ListViewTeachers.SelectedIndex - 1]) ||
                    (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == idTeachers[ListViewTeachers.SelectedIndex - 1])).ForEachAsync(c =>
                    {
                        if (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == idTeachers[ListViewTeachers.SelectedIndex - 1])
                        {
                            co1.Add(new MessageListBox
                            {
                                name = firstname,
                                text = c.ChatText,
                                time = Convert.ToString(c.ChatDate)
                            });
                        }
                        else if (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == idTeachers[ListViewTeachers.SelectedIndex - 1])
                        {
                            co1.Add(new MessageListBox
                            {
                                name = Properties.Settings.Default.FirstName,
                                text = c.ChatText,
                                time = Convert.ToString(c.ChatDate)
                            });
                        }

                    });
                    //idTeachers.Add((int)t.Idteachers);
                    ListBoxMessage.ItemsSource = co1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                timer2.Stop();
                CardUser.Visibility = Visibility.Hidden;
                grid.Visibility = Visibility.Visible;
            }            
        }
    }

    public class UsersListBox
    {
        public BitmapImage? image { get; set; }
        public string? imageDN { get; set; }
        public string? textFIO { get; set; }
    }

    public class MessageListBox
    {
        public string? name { get; set; }
        public string? text { get; set; }
        public string? time { get; set; }
    }
}
