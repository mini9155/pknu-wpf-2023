using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wp09_callburnApp.ViewModels;

namespace wp09_callburnApp
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() 
        {
            Initialize();
        }

        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            // base.OnStartup(sender, e); 부모클래스 실행은 주작처리
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
