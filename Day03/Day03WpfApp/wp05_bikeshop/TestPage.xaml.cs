﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wp05_bikeshop.Logic;

namespace wp05_bikeshop
{
    /// <summary>
    /// SupportPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestPage : Page
    {
        Car myCar = null;
        public TestPage()
        {
            InitializeComponent();
            InitCar();
        }

        private void InitCar()
        {
            myCar = new Car();
            myCar.Name = "아이오닉";
            myCar.Colorz = Colors.White;
            myCar.Speed = 220;

            // 리스트박스에 바인딩하기 위한 Car 리스트
            var cars = new List<Car>();
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                cars.Add(new Car()
                {
                    Speed = i * 10,
                    Colorz = Color.FromRgb((Byte)rand.Next(255), (Byte)rand.Next(256), (Byte)rand.Next(256)),
                   
                });
            }
            // this.DataContext = cars; // 중요 코드 비하인드에서 만든 데이터(DB,excel)을 xaml에 있는 컨트롤에 
            CtlCars.DataContext = cars;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           // TxtSample.Text = myCar.Speed.ToString();
        }

        private void SldValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("안녕하세요", "안녕");
        }
    }
}
