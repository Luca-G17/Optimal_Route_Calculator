using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Optimal_Route_Calculator
{
    class TextBlockObject : MainObject
    {
        private string message;
        public TextBlockObject(int x, int y, string msg, FrameworkElement element, int font_size, int type_code)
        {
            shape = new TextBlock { Text = msg, FontSize = font_size, Style = (Style)Application.Current.Resources["CustomFont"], Margin = new Thickness(5, 5, 5, 5), FontWeight = FontWeights.Thin };

            message = msg;
            GetLeft = x;
            GetTop = y;

            // TODO: Find a way to detect weather the element is the stack panel or the canvas
            // Type_code == 0: Text block is added to a stack panel, Type_code == 1: Text block is added to a defined location on the canvas
            if (type_code == 0)
            {
                DrawObject(element);
            }
            else
            {
                base.DrawObject(element);
            }
        }

        public string SetMessage
        {
            get { return message; }
            set { ((TextBlock)shape).Text = value; }
        }
        public void SetBackground(SolidColorBrush colour)
        {
            ((TextBlock)shape).Background = colour;
        }

        public override void DrawObject(FrameworkElement stack_panel)
        {
            ((StackPanel)stack_panel).Children.Add(shape);
        }
    }
}
