using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Word.xaml
    /// </summary>
    public partial class Word : Page
    {

        internal class EditorDataStack : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            private int
                _line = 1,
                _character = 0,
                _progress = 80;

            private string _file = "Документ";

            public int Line
            {
                get => _line;
                set
                {
                    if (value != _line)
                    {
                        _line = value;
                        OnPropertyChanged(nameof(Line));
                    }
                }
            }

            public int Character
            {
                get => _character;
                set
                {
                    if (value != _character)
                    {
                        _character = value;
                        OnPropertyChanged(nameof(Character));
                    }
                }
            }

            public int Progress
            {
                get => _progress;
                set
                {
                    if (value != _progress)
                    {
                        _progress = value;
                        OnPropertyChanged(nameof(Progress));
                    }
                }
            }

            public string File
            {
                get => _file;
                set
                {
                    if (value != _file)
                    {
                        _file = value;
                        OnPropertyChanged(nameof(File));
                    }
                }
            }
        }


        public Word()
        {
            InitializeComponent();
            DataContext = DataStack;
            FillComboBoxFonts();
        }

        private EditorDataStack DataStack = new();

        public string Line { get; set; } = "0";

        private void RootTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateLine();
        }

        private void UpdateLine()
        {
            TextPointer caretPosition = RootTextBox.CaretPosition;
            TextPointer p = RootTextBox.Document.ContentStart.GetLineStartPosition(0);

            RootTextBox.CaretPosition.GetLineStartPosition(-Int32.MaxValue, out int lineMoved);

            DataStack.Line = -lineMoved;
            DataStack.Character = Math.Max(p.GetOffsetToPosition(caretPosition) - 1, 0);
        }

        private void RootTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            UpdateLine();
        }

        private void RootTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateLine();
        }

        private void FillComboBoxFonts()
        {
            ComboBoxFont.Items.Clear();
            foreach (System.Windows.Media.FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                ComboBoxFont.Items.Add(fontFamily.Source);
            }

            ComboBoxFont.SelectedIndex = 0;
        }
    }
}
