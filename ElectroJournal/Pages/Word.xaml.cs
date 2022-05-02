using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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
            //FillComboBoxFonts();
            string[] FontStandartSize = new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };

            foreach (string font in FontStandartSize)
            {
                ComboBoxSize.Items.Add(font);
            }

            ComboBoxFont.SelectedIndex = 4;
            ComboBoxSize.SelectedIndex = 5;
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

        private void RootTextBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            UpdateLine();

        }

        private void RootTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            UpdateLine();


            string fontName = ComboBoxFont.SelectedItem.ToString();

            if (fontName != null)
            {
                RootTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.RichTextBox.FontFamilyProperty, fontName);
                RootTextBox.Focus();
            }

            try
            {

                RootTextBox.Selection.ApplyPropertyValue(FontSizeProperty, ComboBoxSize.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ComboBoxSize.SelectedItem.ToString());
            }
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

        private void MenuItemBold_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButtonTextAlignCenter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBoxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fontName = ComboBoxFont.SelectedItem.ToString();

            if (fontName != null)
            {
                RootTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.RichTextBox.FontFamilyProperty, fontName);
                RootTextBox.Focus();
            }
        }

        private void ComboBoxSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                RootTextBox.Selection.ApplyPropertyValue(FontSizeProperty, ComboBoxSize.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ComboBoxSize.SelectedItem.ToString());
            }

        }

        private void RootTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            var range = new TextRange(RootTextBox.Document.ContentStart, RootTextBox.Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.FontSizeProperty, 13);

            TextRange tr = new TextRange(RootTextBox.Selection.Start, RootTextBox.Selection.End);
            tr.ApplyPropertyValue(TextElement.FontSizeProperty, ComboBoxSize.SelectedItem.ToString());*/
        }

        private void RootTextBox_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}

