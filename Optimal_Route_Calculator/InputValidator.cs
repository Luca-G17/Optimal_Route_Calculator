using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Diagnostics;


namespace Optimal_Route_Calculator
{
    abstract class InputValidator
    {
        public static bool TextInputCheck(TextBox textBox, double existing_value)
        {
            if (textBox.Text != "" && textBox.Text != existing_value.ToString())
            {
                return true;
            }
            return false;
        }

        public static bool NumValidate(string inp)
        {
            foreach (char character in inp)
            {
                // ASCII: 48 = '0', 57 = '9', 46 = '.'
                if ((character < 48 || character > 57) && character != 46)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
