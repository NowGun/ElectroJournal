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
using System.Windows.Shapes;
using System.Net.Mail;
using System.Net;
using MySql.Data.MySqlClient;
using System.Data;
using ElectroJournal.Classes;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using Microsoft.Win32;
using ElectroJournal.DataBase;
using System.Collections.ObjectModel;
using ElectroJournal.Classes.DataBaseEJ;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            //RectangleLoad.Visibility = Visibility.Hidden;
            TitleBar.CloseActionOverride = CloseActionOverride;
            ListBoxBugsRefresh();
            ComboBoxRefresh();
        }

        private string path;
        List<int> idBugs = new List<int>();
        private bool _isDarkTheme = false;

        private void CloseActionOverride(WPFUI.Controls.TitleBar titleBar, Window window)
        {
            ((MainWindow)Application.Current.MainWindow).ThemeCheck();
            this.Close();
        }
        private void MainWindow_Completed(object sender, EventArgs e)
        {
        }
        private async void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarSend.Visibility = Visibility.Visible;
            ListBoxBugs.SelectedItem = null;

            string text = new TextRange(RRTBname.Document.ContentStart, RRTBname.Document.ContentEnd).Text;
            int chapter = 24;

            if (ComboBoxChapter.SelectedItem != null)
            {
                chapter = ComboBoxChapter.SelectedIndex + 1;
            }
            else chapter = 24;

            if (!string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(TextBoxTitle.Text))
            {
                ButtonSend.IsEnabled = false;

                using (ejContext db = new())
                {
                    Bugreporter bg = new Bugreporter
                    {
                        BugreporterTitle = TextBoxTitle.Text,
                        BugreporterMessage = text,
                        StatusIdstatus = 1,
                        ChapterIdchapter = (uint)chapter
                    };
                    await db.Bugreporters.AddAsync(bg);
                    await db.SaveChangesAsync();
                }

                ListBoxBugsRefresh();
                SendMessage(text, path, TextBoxTitle.Text);

                TextBoxPath.Text = "";
                TextBoxTitle.Text = "";
                RRTBname.Document.Blocks.Clear();
            }
            else
            {
                Notifications("Ошибка", "Заполните поле");
            }
        }
        async private void SendMessage (string text, string path, string title)
        {
            bool a = false;

            MailAddress from = new MailAddress("mail@techno-review.ru", "Новый баг");
            MailAddress to = new MailAddress("nowgun98@gmail.com");
            MailMessage m = new MailMessage(from, to);

            if (!String.IsNullOrEmpty(path))
            {
                Attachment file = new Attachment(path);
                m.Attachments.Add(file);
            }

            m.Subject = title;
            m.Body = text;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("connect.smtp.bz", 2525);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("zhirowdaniil@gmail.com", "CB1W3lAeBwQ6");
            smtp.EnableSsl = true;

            await Task.Run(() =>
            {
                try
                {
                    smtp.Send(m);
                }
                catch (SmtpException)
                {
                    a = true;
                }
            });

            if (!a)
            {
                Notifications("Успешно", "Сообщение отправлено");
                ButtonSend.IsEnabled = true;
                ProgressBarSend.Visibility = Visibility.Hidden;
            }
            else if (a)
            {
                Notifications("Ошибка", "Отсутствует подключение к интернету");
                ButtonSend.IsEnabled = true;
                ProgressBarSend.Visibility = Visibility.Hidden;
            }
        }
        private void Notifications (string message, string title)
        {
            RootSnackbar.Title = message;
            RootSnackbar.Content = title;
            //RootSnackbar.Icon = WPFUI.Common.Icon.MailError16;
            RootSnackbar.Expand();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }
        public void ThemeCheck()
        {
            int theme = Properties.Settings.Default.Theme;

            _isDarkTheme = theme == 1;
            WPFUI.Theme.Manager.Switch(theme == 1 ? WPFUI.Theme.Style.Dark : WPFUI.Theme.Style.Light);

            ApplyBackgroundEffect();
        }
        private void ApplyBackgroundEffect()
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            WPFUI.Background.Manager.Remove(windowHandle);

            if (_isDarkTheme)
            {
                WPFUI.Background.Manager.ApplyDarkMode(windowHandle);
            }
            else
            {
                WPFUI.Background.Manager.RemoveDarkMode(windowHandle);
            }

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                this.Background = System.Windows.Media.Brushes.Transparent;
                WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, windowHandle);
            }
        }
        private void ButtonBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Изображения(*.jpg;*.png)|*.JPG;*.PNG" + "|Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                TextBoxPath.Text = myDialog.FileName;
                path = myDialog.FileName;


                /*var stringPath = $@"{teachers.TeachersImage}";

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.UriSource = new Uri(stringPath, UriKind.Absolute);
                bitmapImage.EndInit();
                Image.fback*/
            }
        }
        private async void ListBoxBugsRefresh()
        {
            try
            {
                idBugs.Clear();
                ObservableCollection<BugsListBox> list = new();
                using (ejContext db = new())
                {
                    var bugs = await db.Bugreporters.Include(u => u.StatusIdstatusNavigation).ToListAsync();

                    foreach (var bu in bugs)
                    {
                        list.Add( new BugsListBox {
                            btitle = bu.BugreporterTitle,
                            btext = bu.BugreporterMessage,
                            bstatus = bu.StatusIdstatusNavigation?.StatusName,
                        });

                        idBugs.Add((int)bu.Idbugreporter);
                    }
                    ListBoxBugs.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ListBoxBugsRefresh");
            }
        }
        private async void ListBoxBugs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxBugs.SelectedItem != null)
            {
                using (ejContext db = new())
                {
                    var bu = await db.Bugreporters.Where(b => b.Idbugreporter == idBugs[ListBoxBugs.SelectedIndex]).Include(c => c.ChapterIdchapterNavigation).FirstOrDefaultAsync();

                    TextBoxTitle.Text = bu.BugreporterTitle;
                    RRTBname.Document.Blocks.Clear();
                    RRTBname.Document.Blocks.Add(new Paragraph(new Run(bu.BugreporterMessage)));
                    ComboBoxChapter.SelectedItem = bu.ChapterIdchapterNavigation?.ChapterName;
                }
            }
        }
        private async void ComboBoxRefresh()
        {
            ComboBoxChapter.Items.Clear();

            using (ejContext db = new())
            {
                await db.Chapters.OrderBy(c => c.Idchapter).ForEachAsync(c =>
                {
                    ComboBoxChapter.Items.Add(c.ChapterName);
                });
            }
        }
    }

    public class BugsListBox
    {
        public string? btitle { get; set; }
        public string? btext { get; set; }
        public string? bstatus { get; set; }
    }
}
