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
            DataContext = this;
            datA = new ObservableCollection<UsersListBox>();
            LoadData();
            ListViewTeachersRefresh();
            ListViewTeachers.SelectedIndex = 0;
        }

        public System.Windows.Threading.DispatcherTimer timer2 = new();
        List<int> idTeachers = new List<int>();
        private static string? path;
        private string? firstname;
        private bool checkLastM = true;

        public ObservableCollection<UsersListBox> datA { get; set; }
        public UsersListBox SelectedData { get; set; }

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

            sc.ExitUser();
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
                datA.Clear();
                idTeachers.Clear();

                string status = "Hidden";
                string lmess = "";

                datA.Add(new UsersListBox
                {
                    online = "Hidden",
                    textFIO = $"Моя страница"
                });

                using (zhirovContext db = new())
                {
                    ProgressBarTeachers2.Visibility = Visibility.Visible;

                    var t2 = await db.Teachers.Where(t => t.Idteachers != Properties.Settings.Default.UserID).OrderBy(t => t.TeachersSurname).ToListAsync();

                    foreach (var t in t2)
                    {
                        Chat? chat = await db.Chats.Where(c => (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == t.Idteachers) || (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == t.Idteachers)).OrderByDescending(c => c.Idchat).FirstOrDefaultAsync();

                        if (t.TeachersStatus == 1) status = "Visible"; 
                        else status = "Hidden";

                        if (chat != null)
                        {
                            if (chat.TeachersTo == Properties.Settings.Default.UserID && chat.TeachersFrom == t.Idteachers)
                            {
                                lmess = chat.ChatText;
                            }
                            else if (chat.TeachersFrom == Properties.Settings.Default.UserID && chat.TeachersTo == t.Idteachers)
                            {
                                lmess = $"Вы: {chat.ChatText}";
                            }
                        }
                        else lmess = "";

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

                            datA.Add(new UsersListBox
                            {
                                online = status,
                                lastMessage = lmess,
                                image = bitmapImage,
                                textFIO = $"{t.TeachersSurname} {t.TeachersName}"
                            });
                        }
                        else
                        {
                            datA.Add(new UsersListBox
                            {
                                online = status,
                                lastMessage = lmess,
                                imageDN = $"{t.TeachersSurname} {t.TeachersName}",
                                textFIO = $"{t.TeachersSurname} {t.TeachersName}"
                            });
                        }
                        idTeachers.Add((int)t.Idteachers);
                    }
                    ProgressBarTeachers2.Visibility = Visibility.Hidden;
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
                        var anim = (Storyboard)FindResource("AnimMessage");
                        anim.Begin();
                        ListBoxMessageRefresh2();
                        CardUser.Visibility = Visibility.Visible;
                        grid.Visibility = Visibility.Hidden;

                        using (zhirovContext db = new zhirovContext())
                        {
                            var teachers = await db.Teachers.Where(p => p.Idteachers == idTeachers[ListViewTeachers.SelectedIndex - 1]).ToListAsync();

                            foreach (var t in teachers)
                            {
                                TextBlockName.Content = t.TeachersSurname + " " + t.TeachersName;
                                firstname = t.TeachersName;

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
                                    PersonPictureUser.ProfilePicture = bitmapImage;
                                }
                                else
                                {
                                    PersonPictureUser.ProfilePicture = null;
                                    PersonPictureUser.DisplayName = $"{t.TeachersName} {t.TeachersSurname}";
                                }
                                
                                timer2.Tick += new EventHandler(ListBoxMessageRefresh);
                                timer2.Interval = new TimeSpan(0, 0, 1);
                                timer2.Start();
                            }
                        }
                    }
                    else if (ListViewTeachers.SelectedIndex == 0)
                    {
                        var anim = (Storyboard)FindResource("AnimOpenLK");
                        anim.Begin();
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
                if (!String.IsNullOrWhiteSpace(TextBoxMessage.Text))
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

                        TextBoxMessage.Clear();
                        checkLastM = true;
                        ListBoxMessageRefresh2();
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private async void CheckNewMess(int index)
        {
            try
            {
                string lmess = "";
                string status = "Hidden";

                using (zhirovContext db = new())
                {
                    Teacher? t2 = await db.Teachers.Where(t => t.Idteachers == index).FirstOrDefaultAsync();
                    Chat? chat = await db.Chats.Where(c => (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == index) ||
                    (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == index)).OrderByDescending(c => c.Idchat).FirstOrDefaultAsync();

                    if (t2.TeachersStatus == 1) status = "Visible";
                    else status = "Hidden";

                    if (chat != null)
                    {
                        if (chat.TeachersTo == Properties.Settings.Default.UserID && chat.TeachersFrom == index)
                        {
                            lmess = chat.ChatText;
                        }
                        else if (chat.TeachersFrom == Properties.Settings.Default.UserID && chat.TeachersTo == index)
                        {
                            lmess = $"Вы: {chat.ChatText}";
                        }
                    }
                    else lmess = "";

                    SelectedData.lastMessage = lmess;
                    SelectedData.online = status;
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
                int index = idTeachers[ListViewTeachers.SelectedIndex - 1];
                ObservableCollection<MessageListBox> co1 = new ObservableCollection<MessageListBox>();
                using (zhirovContext db = new zhirovContext())
                {
                    if (ListViewTeachers.SelectedIndex != 0)
                    {
                        var a2 = await db.Chats.Where(c => (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == index) ||
                            (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == index)).ToListAsync();

                        foreach (var c in a2)
                        {
                            if (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == index)
                            {
                                co1.Add(new MessageListBox
                                {
                                    name = firstname,
                                    text = c.ChatText,
                                    time = Convert.ToString(c.ChatDate)
                                });

                            }
                            else if (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == index)
                            {
                                co1.Add(new MessageListBox
                                {
                                    name = Properties.Settings.Default.FirstName,
                                    text = c.ChatText,
                                    time = Convert.ToString(c.ChatDate),
                                    hAligment = "Right",
                                    flowStack = "RightToLeft"
                                });
                            }
                        }

                        ListBoxMessage.ItemsSource = co1;

                        if (checkLastM)
                        {
                            ListBoxMessage.Items.MoveCurrentToLast();
                            ListBoxMessage.ScrollIntoView(ListBoxMessage.Items.CurrentItem);
                        }
                        CheckNewMess(index);
                    }
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
                int index = idTeachers[ListViewTeachers.SelectedIndex - 1];
                ObservableCollection<MessageListBox> co1 = new ObservableCollection<MessageListBox>();
                using (zhirovContext db = new zhirovContext())
                {
                    if (ListViewTeachers.SelectedIndex != 0)
                    {
                        var c2 = await db.Chats.Where(c => (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == index) ||
                        (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == index)).ToListAsync();

                        foreach (var c in c2)
                        {
                            if (c.TeachersTo == Properties.Settings.Default.UserID && c.TeachersFrom == index)
                            {
                                co1.Add(new MessageListBox
                                {
                                    name = firstname,
                                    text = c.ChatText,
                                    time = Convert.ToString(c.ChatDate)
                                });
                            }
                            else if (c.TeachersFrom == Properties.Settings.Default.UserID && c.TeachersTo == index)
                            {
                                co1.Add(new MessageListBox
                                {
                                    name = Properties.Settings.Default.FirstName,
                                    text = c.ChatText,
                                    time = Convert.ToString(c.ChatDate),
                                    hAligment = "Right",
                                    flowStack = "RightToLeft"
                                });
                            }
                        }

                        ListBoxMessage.ItemsSource = co1;

                        if (checkLastM)
                        {
                            ListBoxMessage.Items.MoveCurrentToLast();
                            ListBoxMessage.ScrollIntoView(ListBoxMessage.Items.CurrentItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ListViewTeachers.SelectedIndex = 0;
                timer2.Stop();
                CardUser.Visibility = Visibility.Hidden;
                grid.Visibility = Visibility.Visible;
            }            
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            timer2.Stop();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //ListViewTeachersRefresh2();
        }
        private void ListBoxMessage_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                checkLastM = false;
            }
        }
    }

    public class UsersListBox : NotifyPropertyChanged
    {
        string? lMessage;
        string? cOnline;
        public BitmapImage? image { get; set; }
        public string? imageDN { get; set; }
        public string? textFIO { get; set; }
        public string? online
        {
            get => cOnline;

            set
            {
                cOnline = value;
                RaisePropertyChanged("online");
            }
        }
        public string? lastMessage
        { 
            get => lMessage; 

            set
            {
                lMessage = value;
                RaisePropertyChanged("lastMessage");
            }

        }
    }
    public class MessageListBox
    {
        public string? name { get; set; }
        public string? text { get; set; }
        public string? time { get; set; }
        public string? hAligment { get; set; }
        public string? flowStack { get; set; }
    }
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
