using System;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FTDISample.Converters
{
    public class ReadBufferToStringConverter : DependencyObject, IValueConverter
    {
        public bool ShowInASCII // TODO: make this trigger the Converter Binding update 
        {
            get { return (bool)GetValue(ShowInASCIIDependencyProperty); }
            set { SetValue(ShowInASCIIDependencyProperty, value); }
        }

        public static readonly DependencyProperty ShowInASCIIDependencyProperty = 
            DependencyProperty.Register("ShowInASCII",
                                        typeof(bool),
                                        typeof(ReadBufferToStringConverter),
                                        new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is byte[]))
                return DependencyProperty.UnsetValue;

            if (ShowInASCII)
            {
                var asciiMessage = Encoding.ASCII.GetString((byte[]) value);
                return ReplaceNulControlCharacter(asciiMessage);
            }

            return BitConverter.ToString((byte[])value);
        }

        private static string ReplaceNulControlCharacter(string asciiMessage)
        {
            return asciiMessage.Replace("\0", "(NUL)");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}