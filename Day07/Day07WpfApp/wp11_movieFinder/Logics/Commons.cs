using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace wp11_movieFinder.Logics
{
    public class Commons
    {

        public static readonly string connString = "Data Source=localhost;" +
            "Initial Catalog=pknu;" +
            "Integrated Security=True;"+
            "User ID = sa;"+
            "Password=12345;";


        //연결 문자열 담을 변수
        // 창을 띄워줌
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
                                                      MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style,null);
        }
    }
}
