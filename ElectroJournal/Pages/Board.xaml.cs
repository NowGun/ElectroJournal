using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using WPFUI;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;

namespace ElectroJournal.Pages
{
    /// <summary>
    /// Логика взаимодействия для Board.xaml
    /// </summary>
    public partial class Board : Page
    {
        public Board()
        {
            InitializeComponent();

        }

        DrawingAttributes inkAttributes = new();

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.InkCanvas.Strokes.Clear();
        }
        private void ImageEraser_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }
        private void ImagePencil_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
        private void InkCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            //InkCanvas.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(ColorPicker.SelectedColor.ToString());
            InkCanvas.UseCustomCursor = true;

            inkAttributes.Width = SliderBoardThikness.Value;
            inkAttributes.Height = SliderBoardThikness.Value;
            inkAttributes.Color = (Color)ColorConverter.ConvertFromString(ColorPicker.SelectedColor.ToString());

            InkCanvas.DefaultDrawingAttributes = inkAttributes;

            InkCanvas.Cursor = Cursors.Cross;
        }
        private void InkCanvas_StylusMove(object sender, StylusEventArgs e)
        {
            StylusPointCollection originalPoints = e.GetStylusPoints(InkCanvas);
            _ = originalPoints[0].PressureFactor;
        }
        private void ImagePrint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PrintDialog printDialog = new();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(InkCanvas, "Печать изображения");
            }
        }
        private void ThemeCheck()
        {
            if (Properties.Settings.Default.Theme == 1)
            {
                InkCanvas.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#222222");
                ColorPicker.SelectedColor = Colors.White;
            }
            else
            {
                InkCanvas.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#ffffff");
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeCheck();
        }
    }
}
