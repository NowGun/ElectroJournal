using ElectroJournal.Classes;
using ElectroJournal.Classes.DataBaseEF;
using ElectroJournal.DataBase;
using ElectroJournal.Windows;
using Microsoft.EntityFrameworkCore;
using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;
using PCSC.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Students.xaml
    /// </summary>
    public partial class Students : Page
    {
        public Students()
        {
            InitializeComponent();

            FillComboBoxGroups();
            FillComboBoxCourse();
            ComboBoxGroups.SelectedIndex = 0;
            ButtonDeleteNumber.Visibility = Visibility.Collapsed;
        }

        List<int> idStudents = new();
        List<int> idGroups = new();

        public delegate void CardReadHandler(string id);
        public event CardReadHandler CardRead = delegate { };

        private ISCardContext _cardContext { get; set; }
        private ISCardMonitor _monitor { get; set; }
        private string _readerName { get; set; }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBar.Visibility = Visibility.Visible;

                if (!string.IsNullOrWhiteSpace(TextBoxStudentsFIO.Text) && ListBoxGroups.SelectedIndex != -1)
                {
                    if (TextBoxStudentsFIO.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                    {
                        using zhirovContext db = new();
                        string[] FIO = TextBoxStudentsFIO.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        DateTime? date = !String.IsNullOrEmpty(DatePickerDateBirthday.Text) ? DateTime.Parse(DatePickerDateBirthday.Text) : null;

                        if (ListBoxStudents.SelectedItem != null)
                        {
                            Student? student = await db.Students.FirstOrDefaultAsync(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]);
                            Smartcard? smart = await db.Smartcards.FirstOrDefaultAsync(s => s.StudentId == idStudents[ListBoxStudents.SelectedIndex]);

                            if (student != null)
                            {
                                student.StudentsName = FIO[1];
                                student.StudentsSurname = FIO[0];
                                student.StudentsPatronymic = FIO[2];
                                student.StudentsParentPhone = TextBoxParentPhone.Text;
                                student.StudentsPhone = TextBoxStudentsPhone.Text;
                                student.StudentsParent = TextBoxParentFIO.Text;
                                student.StudentsResidence = TextBoxStudentsResidence.Text;
                                student.StudentsBirthday = DatePickerDateBirthday.Text != null ? date : null;
                                student.GroupsIdgroups = (uint)idGroups[ListBoxGroups.SelectedIndex];

                                if (smart != null) smart.SmartcardIdentifier = LabelCardId.Content.ToString().Length == 8 ? (string)LabelCardId.Content : smart.SmartcardIdentifier;

                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            Student students = new Student
                            {
                                StudentsName = FIO[1],
                                StudentsSurname = FIO[0],
                                StudentsPatronymic = FIO[2],
                                StudentsParentPhone = TextBoxParentPhone.Text,
                                StudentsPhone = TextBoxStudentsPhone.Text,
                                StudentsParent = TextBoxParentFIO.Text,
                                StudentsResidence = TextBoxStudentsResidence.Text,
                                StudentsBirthday = DatePickerDateBirthday.Text != null ? date : null,
                                GroupsIdgroups = (uint)idGroups[ListBoxGroups.SelectedIndex],
                            };

                            await db.Students.AddAsync(students);
                            await db.SaveChangesAsync();

                            if (LabelCardId.Content.ToString().Length == 8)
                            {
                                var s = await db.Students.OrderByDescending(s => s.Idstudents).FirstOrDefaultAsync();
                                if (s != null)
                                {
                                    Smartcard smart = new Smartcard
                                    {
                                        StudentId = s.Idstudents,
                                        SmartcardIdentifier = (string)LabelCardId.Content,
                                    };

                                    await db.Smartcards.AddAsync(smart);
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        ClearValue();
                    }
                    else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Поле ФИО должно быть в формате: Фамилия - Имя - Отчество");
                }
                else ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Заполните поля помеченные *");

                ProgressBar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ButtonSave_Click(students) | {ex.Message}");
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["AnimChangeTextNumber"]).Begin();
            StartScan();
            TextBoxParentFIO.Clear();
            TextBoxParentPhone.Clear();
            TextBoxStudentsFIO.Clear();
            TextBoxStudentsPhone.Clear();
            TextBoxStudentsResidence.Clear();
            DatePickerDateBirthday.Text = null;
            ButtonDeleteNumber.Visibility = Visibility.Collapsed;
            ButtonChangeNumber.Visibility = Visibility.Collapsed;
            ListBoxStudents.SelectedIndex = -1;
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e) => DeleteStudent();
        private async void ListBoxStudentsRefresh()
        {
            try
            {
                ListBoxStudents.Items.Clear();
                idStudents.Clear();
                ProgressBarListBox.Visibility = Visibility.Visible;

                using zhirovContext db = new();

                if (String.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    switch (ComboBoxSorting.SelectedIndex)
                    {
                        case 0:

                            if (ComboBoxSortingGroups.SelectedIndex == 0)
                            {
                                await db.Students.OrderBy(t => t.StudentsSurname).ForEachAsync(t =>
                                {
                                    ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                                    idStudents.Add((int)t.Idstudents);
                                });
                            }
                            else if (ComboBoxSortingGroups.SelectedIndex != -1 && ComboBoxSortingGroups.SelectedIndex != 0)
                            {
                                await db.Students.OrderBy(t => t.StudentsSurname).Where(t => t.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ComboBoxSortingGroups.SelectedItem.ToString()).ForEachAsync(t =>
                                {
                                    ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                                    idStudents.Add((int)t.Idstudents);
                                });
                            }

                            break;
                        case 1:

                            if (ComboBoxSortingGroups.SelectedIndex == 0)
                            {
                                await db.Students.OrderByDescending(t => t.StudentsSurname).ForEachAsync(t =>
                                {
                                    ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                                    idStudents.Add((int)t.Idstudents);
                                });
                            }
                            else if (ComboBoxSortingGroups.SelectedIndex != -1 && ComboBoxSortingGroups.SelectedIndex != 0)
                            {
                                await db.Students.OrderByDescending(t => t.StudentsSurname).Where(t => t.GroupsIdgroupsNavigation.GroupsNameAbbreviated == ComboBoxSortingGroups.SelectedItem.ToString()).ForEachAsync(t =>
                                {
                                    ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                                    idStudents.Add((int)t.Idstudents);
                                });
                            }
                            break;
                    }
                }
                else
                {
                    await db.Students
                        .OrderBy(t => t.StudentsSurname)
                        .Where(t => EF.Functions.Like(t.StudentsSurname, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.StudentsName, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.StudentsPatronymic, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.GroupsIdgroupsNavigation.GroupsName, $"%{SearchBox.Text}%") ||
                    EF.Functions.Like(t.GroupsIdgroupsNavigation.GroupsNameAbbreviated, $"%{SearchBox.Text}%"))
                        .ForEachAsync(t =>
                        {
                            ListBoxStudents.Items.Add($"{t.StudentsSurname} {t.StudentsName} {t.StudentsPatronymic}");
                            idStudents.Add((int)t.Idstudents);
                        });
                }

                ProgressBarListBox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxStudentsRefresh | {ex.Message}");
            }
        }
        private void TextBoxStudentsPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void ListBoxStudents_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteStudent();
            }
        }
        private async void ListBoxStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ButtonDelete.IsEnabled = true;

                if (ListBoxStudents.SelectedItem != null)
                {
                    using zhirovContext db = new();

                    var t = await db.Students.Where(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]).Include(p => p.GroupsIdgroupsNavigation.CourseIdcourseNavigation).FirstOrDefaultAsync();
                    var s = await db.Smartcards.FirstOrDefaultAsync(s => s.StudentId == idStudents[ListBoxStudents.SelectedIndex]);

                    if (t != null)
                    {
                        TextBoxStudentsFIO.Text = t.StudentsSurname + " " + t.StudentsName + " " + t.StudentsPatronymic;
                        DatePickerDateBirthday.SelectedDate = t.StudentsBirthday;
                        TextBoxStudentsResidence.Text = t.StudentsResidence;
                        TextBoxParentFIO.Text = t.StudentsParent;
                        TextBoxStudentsPhone.Text = t.StudentsPhone;
                        TextBoxParentPhone.Text = t.StudentsParentPhone;
                        ComboBoxGroups.SelectedItem = t.GroupsIdgroups == null ? null : t.GroupsIdgroupsNavigation.CourseIdcourseNavigation.CourseName;
                        ListBoxGroups.SelectedItem = t.GroupsIdgroups == null ? null : t.GroupsIdgroupsNavigation.GroupsNameAbbreviated;
                        LabelCardId.Content = s == null ? "Отсутствует" : s.SmartcardIdentifier;
                        ButtonDeleteNumber.Visibility = s == null ? Visibility.Collapsed : Visibility.Visible;
                        ButtonChangeNumber.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"ListBoxStudents_SelectionChanged | {ex.Message}");
            }
        }
        private void ComboBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e) => ListBoxStudentsRefresh();
        private async void DeleteStudent()
        {
            try
            {
                if (ListBoxStudents.Items.Count == 0)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Notifications("Сообщение", "Произошла ошибка");
                }
                else if (ListBoxStudents.SelectedItem != null)
                {
                    using zhirovContext db = new();

                    Student? student = await db.Students.FirstOrDefaultAsync(p => p.Idstudents == idStudents[ListBoxStudents.SelectedIndex]);
                    Smartcard? smart = await db.Smartcards.FirstOrDefaultAsync(s => s.StudentId == idStudents[ListBoxStudents.SelectedIndex]);

                    if (student != null)
                    {
                        if (smart != null) db.Smartcards.Remove(smart);
                        db.Students.Remove(student);
                        await db.SaveChangesAsync();
                        ClearValue();
                    }
                }
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"DeleteStudent | {ex.Message}");
            }
        }
        private async void FillComboBoxCourse()
        {
            try
            {
                ComboBoxGroups.Items.Clear();
                using zhirovContext db = new();
                await db.Courses.OrderBy(t => t.CourseName).ForEachAsync(t => ComboBoxGroups.Items.Add(t.CourseName));
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillComboBoxCourse | {ex.Message}");
            }
        }
        private async void FillListBox()
        {
            try
            {
                ListBoxGroups.Items.Clear();
                idGroups.Clear();

                using zhirovContext db = new();
                await db.Groups.Where(p => p.CourseIdcourseNavigation.CourseName == ComboBoxGroups.SelectedItem.ToString()).ForEachAsync(p =>
                {
                    ListBoxGroups.Items.Add(p.GroupsNameAbbreviated);
                    idGroups.Add((int)p.Idgroups);
                });
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"FillListBox | {ex.Message}");
            }
        }
        private void ComboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListBox();
            ListBoxGroups.Visibility = Visibility.Visible;
        }
        private void SearchBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e) => ListBoxStudentsRefresh();
        private void ButtonChangeNumber_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["AnimChangeTextNumber"]).Begin();
            StartScan();
        }
        private void CardReader_CardRead(string id)
        {
            LabelCardId.Content = id;
            ButtonChangeNumber.Visibility = Visibility.Visible;
        }
        public async new Task Show()
        {
            try
            {
                _cardContext = ContextFactory.Instance.Establish(SCardScope.System);
            }
            catch (NoServiceException)
            {
                LabelCardId.Content = "Не установлен драйвер PS/CS";
                Console.WriteLine("CardReader.xaml.cs : не установлен драйвер PS/CS");
                return;
            }

            _readerName = _cardContext.GetReaders().FirstOrDefault();
            if (string.IsNullOrEmpty(_readerName))
            {
                LabelCardId.Content = "Не найден считыватель карт";
                Console.WriteLine("CardReader.xaml.cs : не найден считыватель карт");
                return;
            }

            var factory = MonitorFactory.Instance;
            _monitor = factory.Create(SCardScope.System);

            _monitor.CardInserted += CardInserted;
            _monitor.MonitorException += MonitorException;

            _monitor.Start(_readerName);
        }
        private void MonitorException(object sender, PCSCException exception) => Console.WriteLine($"CardReader.xaml.cs : monitor exception: {exception.Message}");
        private void CardInserted(object sender, CardStatusEventArgs e)
        {
            IsoReader cardReader;
            try
            {
                cardReader = new IsoReader(_cardContext, _readerName, SCardShareMode.Shared, SCardProtocol.Any, false);
            }
            catch (Exception ex)
            {
                SettingsControl.InputLog($"CardReader.xaml.cs : card inserted exception: {ex.Message}");
                return;
            }

            var apdu = new CommandApdu(IsoCase.Case2Short, cardReader.ActiveProtocol)
            {
                CLA = 0xFF, // System class
                Instruction = InstructionCode.GetData, // CA
                P1 = 0x00, // Parameter 1
                P2 = 0x00, // Parameter 2
                Le = 0x00 // Expected length of the returned data
            };

            Response response = cardReader.Transmit(apdu);

            cardReader.Dispose();
            if (response is null) return;

            Console.WriteLine($"CardReader.xaml.cs : SW1 SW2 = {response?.SW1} {response?.SW2}");
            Console.WriteLine($"CardReader.xaml.cs : DATA = {Convert.ToHexString(response?.GetData() ?? Array.Empty<byte>())}");

            var monitor = sender as ISCardMonitor;
            monitor?.Cancel();

            Dispatcher.Invoke(() =>
            {
                CardRead(Convert.ToHexString(response.GetData()));
            });
        }
        private void Page_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D)
            {
                ClearValue();
                TextBoxStudentsFIO.Focus();
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SearchBox.Clear();
                SearchBox.Focus();
            }
        }
        private void ClearValue()
        {
            ListBoxStudentsRefresh();
            LabelCardId.Content = "Отсутствует";
            ButtonChangeNumber.Visibility = Visibility.Collapsed;
            ButtonDeleteNumber.Visibility = Visibility.Collapsed;
            TextBoxParentFIO.Clear();
            TextBoxParentPhone.Clear();
            TextBoxStudentsFIO.Clear();
            TextBoxStudentsPhone.Clear();
            TextBoxStudentsResidence.Clear();
            DatePickerDateBirthday.Text = null;
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _cardContext?.Dispose();
            _monitor?.Dispose();
        }
        private async void StartScan()
        {
            CardRead += CardReader_CardRead;
            await Show();
        }
        private async void FillComboBoxGroups()
        {
            try
            {
                ComboBoxSortingGroups.Items.Clear();
                ComboBoxSortingGroups.Items.Add("Все группы");
                using zhirovContext db = new();
                await db.Groups.OrderBy(g => g.CourseIdcourseNavigation.CourseName).ForEachAsync(g => ComboBoxSortingGroups.Items.Add(g.GroupsNameAbbreviated));
                ComboBoxSortingGroups.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }
        private async void ButtonDeleteNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListBoxStudents.SelectedIndex != -1)
                {
                    using zhirovContext db = new();

                    Smartcard? smart = await db.Smartcards.FirstOrDefaultAsync(s => s.StudentId == idStudents[ListBoxStudents.SelectedIndex]);

                    if (smart != null)
                    {
                        db.Smartcards.Remove(smart);
                        await db.SaveChangesAsync();
                        ((Storyboard)Resources["AnimChangeTextNumber"]).Begin();
                        LabelCardId.Content = "Отсутствует";
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}