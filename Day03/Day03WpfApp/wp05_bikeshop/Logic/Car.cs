using System.Drawing.Printing;
using System.Media;
using System.Windows.Media;

namespace wp05_bikeshop.Logic
{
    internal class Car : Notifier // 값이 바뀌는 걸 인지하여서 처리하겠다
    {
        private string names;
        public string Name { 
            get => names;
            set
            {
                names = value;
                OnPropertyChanged("Names");
            }
                }
        private double speed;
        public double Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
        public Color Colorz { get; set; }
        public Human Driver { get; set; }
    }
}
