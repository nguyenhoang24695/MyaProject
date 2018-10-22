using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyA.Views;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyA.Services
{
    class SymbolColorChange
    {
        public static string LabelName = null;
        public static void changeColorSymbol(string LabelName)
        {
            if(LabelName != null)
            {                   
                SymbolIcon tb = (Views.SplitView.mySplit.FindName(LabelName)) as SymbolIcon;
                ((Views.SplitView.mySplit.FindName("AccountSymbol")) as SymbolIcon).ClearValue(SymbolIcon.ForegroundProperty);
                ((Views.SplitView.mySplit.FindName("HomeSymbol")) as SymbolIcon).ClearValue(SymbolIcon.ForegroundProperty);
                ((Views.SplitView.mySplit.FindName("MusicSymbol")) as SymbolIcon).ClearValue(SymbolIcon.ForegroundProperty);
                tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
                Debug.WriteLine(tb.Name);
                
            }
        }
    }
}
