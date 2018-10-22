using MyA.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
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
        public static SplitView mySplit;
        public SplitView()
        {
            this.InitializeComponent();            
            mySplit = this;
            Services.GlobalHandle.changeAccountName();
            Services.GlobalHandle.ShowSignOutButton();
            PageContent.Navigate(typeof(Views.HomePage));
        }
        private void OnSuspening(object sender,SuspendingEventArgs e)
        {

        }

        private void TogglePanelEvent(object sender, TappedRoutedEventArgs e)
        {
            PaneSplitView.IsPaneOpen = !this.PaneSplitView.IsPaneOpen;            
        }

        private async void ShowLoginFormHandle(object sender, TappedRoutedEventArgs e)
        {
            

            ClearValue(SymbolIcon.ForegroundProperty);
            if (await GlobalHandle.checkToken() != null)
            {
                PageContent.Navigate(typeof(Views.UserInfomation));
            }
            else
            {                
                PageContent.Navigate(typeof(Views.LoginForm));
            }
            
        }
        private void ShowHomePageForm(object sender, TappedRoutedEventArgs e)
        {            
            PageContent.Navigate(typeof(Views.HomePage));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void ShowListSonngHandle(object sender, TappedRoutedEventArgs e)
        {

            if (await GlobalHandle.checkToken() != null)
            {
                PageContent.Navigate(typeof(Views.ListSong));
            }
            else
            {
                
                var messageDialog = new MessageDialog("Vui lòng đăng nhập để sử dụng tính năng");
                await messageDialog.ShowAsync();
                PageContent.Navigate(typeof(Views.LoginForm));
            }
            
        }

        private void LogoutHandle(object sender, TappedRoutedEventArgs e)
        {
            Services.GlobalHandle.SignOutHandle(PageContent);
            
        }
        
    }
}
