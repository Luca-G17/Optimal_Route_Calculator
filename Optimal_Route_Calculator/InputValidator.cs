using System.Windows.Controls;


namespace Optimal_Route_Calculator
{
    static class InputValidator
    {
        /// <summary>
        /// Verifies that a text input will actually change the exsting value and that it contains a value
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="existing_value"></param>
        /// <returns></returns>
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
            // Verifies that the input only consists of numbers
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
