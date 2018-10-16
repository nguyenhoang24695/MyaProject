using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MyA.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplitView : Page
    {
        public SplitView()
        {
            this.InitializeComponent();
            PageContent.Navigate(typeof(Views.HomePage));
        }

        private void TogglePanelEvent(object sender, TappedRoutedEventArgs e)
        {
            PaneSplitView.IsPaneOpen = !this.PaneSplitView.IsPaneOpen;            
        }

        private void ShowLoginFormHandle(object sender, TappedRoutedEventArgs e)
        {
            PageContent.Navigate(typeof(Views.LoginForm));
        }
        private void ShowHomePageForm(object sender, TappedRoutedEventArgs e)
        {
            PageContent.Navigate(typeof(Views.HomePage));
        }
    }
}
