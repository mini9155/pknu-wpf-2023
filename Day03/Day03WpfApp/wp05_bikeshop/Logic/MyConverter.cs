using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wp05_bikeshop.Logic
{
    internal class MyConverter : IValueConverter

    {
        // 대상에다가 표현 할 때 값을 변환 , 표현(OneWay)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) + "Km/h";
        }

        // 대상 값이 바뀌어 원본(소스)의 값을
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) * 3;
        }
    }
}
