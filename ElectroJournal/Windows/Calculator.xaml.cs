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

namespace ElectroJournal.Windows
{
    /// <summary>
    /// Логика взаимодействия для Calculator.xaml
    /// </summary>
    public partial class Calculator : Window
    {
        public Calculator()
        {
            InitializeComponent();
            System.Diagnostics.Process.Start("calc");
        }


        private void SetUnicodeSymbols(object sender, EventArgs e)
        {
            ButtonPow.Content = "x\u00b2";
            ButtonSquare.Content = "\u221ax";
            ButtonDivide.Content = "\u00f7";
            ButtonMulty.Content = "\u00d7";
            ButtonMinus.Content = "\u2212";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonPercent_Click(object sender, RoutedEventArgs e)
        {
            double actual = Convert.ToDouble(Label_Calc.Content);
            double part = Convert.ToDouble(Label_Res.Content);

            Label_Res.Content = (actual * part / 100).ToString();
        }

        private void ButtonDeleteALL_Click(object sender, RoutedEventArgs e) // CE
        {
            if (Convert.ToString(Label_Res.Content) != "0" && Convert.ToString(Label_Calc.Content) != "")
            {
                Label_Res.Content = "0";
            }
            else if (Convert.ToString(Label_Res.Content) == "0" && Convert.ToString(Label_Calc.Content) != "")
            {
                Label_Operation.Content = "";
                Label_Calc.Content = "";
                Label_Res.Content = "0";
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e) // C
        {
            Label_Operation.Content = "";
            Label_Calc.Content = "";
            Label_Res.Content = "0";
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e) // DEL
        {
            string content = Convert.ToString(Label_Res.Content);
            if (content.Length > 1)
            {
                Label_Res.Content = content.Substring(0, content.Length - 1);
            }
            else
            {
                Label_Res.Content = "0";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonReverse_Click(object sender, RoutedEventArgs e)
        {
            double res = 1 / Convert.ToDouble(Label_Res.Content);
            Label_Res.Content = res.ToString();
        }

        private void ButtonPow_Click(object sender, RoutedEventArgs e)
        {
            double res = Convert.ToDouble(Label_Res.Content);
            Label_Res.Content = Math.Pow(res, 2).ToString();
        }

        private void ButtonSquare_Click(object sender, RoutedEventArgs e)
        {
            double res = Convert.ToDouble(Label_Res.Content);
            Label_Res.Content = Math.Sqrt(res).ToString();
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            Label_Operation.Content = "\u00f7";
            Label_Calc.Content = Label_Res.Content;
            Label_Res.Content = "0";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonAddSeven_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "7";
            }
            else
            {
                Label_Res.Content += "7";
            }
        }

        private void ButtonAddEight_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "8";
            }
            else
            {
                Label_Res.Content += "8";
            }
        }

        private void ButtonAddNine_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "9";
            }
            else
            {
                Label_Res.Content += "9";
            }
        }

        private void ButtonMulty_Click(object sender, RoutedEventArgs e)
        {
            Label_Operation.Content = "\u00d7";
            Label_Calc.Content = Label_Res.Content;
            Label_Res.Content = "0";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonAddFour_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "4";
            }
            else
            {
                Label_Res.Content += "4";
            }
        }

        private void ButtonAddFive_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "5";
            }
            else
            {
                Label_Res.Content += "5";
            }
        }

        private void ButtonAddSix_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "6";
            }
            else
            {
                Label_Res.Content += "6";
            }
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            Label_Operation.Content = "\u2212";
            Label_Calc.Content = Label_Res.Content;
            Label_Res.Content = "0";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonAddOne_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "1";
            }
            else
            {
                Label_Res.Content += "1";
            }
        }

        private void ButtonAddTwo_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "2";
            }
            else
            {
                Label_Res.Content += "2";
            }
        }

        private void ButtonAddThree_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "3";
            }
            else
            {
                Label_Res.Content += "3";
            }
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            Label_Operation.Content = "+";
            Label_Calc.Content = Label_Res.Content;
            Label_Res.Content = "0";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        private void ButtonNegate_Click(object sender, RoutedEventArgs e)
        {
            double res = Convert.ToDouble(Label_Res.Content) * (-1);
            Label_Res.Content = res.ToString();
        }

        private void ButtonAddZero_Click(object sender, RoutedEventArgs e)
        {
            if (Label_Res.Content.Equals("0"))
            {
                Label_Res.Content = "0";
            }
            else
            {
                Label_Res.Content += "0";
            }
        }

        private void ButtonAddDot_Click(object sender, RoutedEventArgs e)
        {
            bool hasDot = false;
            string content = Convert.ToString(Label_Res.Content);
            foreach (char c in content)
            {
                if (c.Equals(",")) hasDot = true;
            }
            if (!hasDot) Label_Res.Content += ",";
        }

        private void ButtonCalc_Click(object sender, RoutedEventArgs e)
        {
            double bottom_value = Convert.ToDouble(Label_Res.Content);
            double top_value = Convert.ToDouble(Label_Calc.Content);
            double res = 0;
            string op = string.Empty;

            if (Convert.ToString(top_value) == null)
            {
                Label_Res.Content = Convert.ToString(bottom_value);
            }
            else
            {
                switch (Convert.ToString(Label_Operation.Content))
                {
                    case "\u00d7":
                        res = top_value * bottom_value;
                        op = "\u00d7";
                        break;
                    case "\u00f7":
                        res = top_value / bottom_value;
                        op = "\u00f7";
                        break;
                    case "+":
                        res = top_value + bottom_value;
                        op = "+";
                        break;
                    case "\u2212":
                        res = top_value - bottom_value;
                        op = "\u2212";
                        break;
                }
                Label_Calc.Content = top_value + op + bottom_value;
                Label_Res.Content = Convert.ToString(res);
                Label_Operation.Content = "=";
            }
        }


    }
}
