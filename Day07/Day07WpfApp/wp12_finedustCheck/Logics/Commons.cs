using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wp12_finedustCheck.Logics
{
    public class Commons
    {

        public static readonly string connString = "Sever=localhost; Port=3306; userid=root; password=1234; Database=dunstsensor; Uid = root; Pwd=1234;";


        //연결 문자열 담을 변수
        // 창을 띄워줌
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
                                                      MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}

