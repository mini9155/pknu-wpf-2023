using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace wp11_movieFinder.Logics
{
    public class Commons
    {
        // 창을 띄워줌
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
                                                      MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style,null);
        }
    }
}
