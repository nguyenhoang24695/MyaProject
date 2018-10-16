using MyA.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
    public sealed partial class LoginForm : Page
    {
        private Member currentMember;
        public LoginForm()
        {
            this.currentMember = new Member();
            this.InitializeComponent();
        }
        private void showRegisterForm(object sender, RoutedEventArgs e)
        {
            //CoreApplicationView newView = CoreApplication.CreateNewView();
            //int newViewId = 0;
            //await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    Frame frame = new Frame();
            //    frame.Navigate(typeof(Views.RegisterForm), this.currentMember);
            //    Window.Current.Content = frame;
            //    // You have to activate the window in order to show it later.
            //    Window.Current.Activate();

            //    newViewId = ApplicationView.GetForCurrentView().Id;
            //});
            //bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            this.Frame.Navigate(typeof(Views.RegisterForm));
        }

        private async void LoginHandle(object sender, RoutedEventArgs e)
        {

            this.currentMember.email = userEmail.Text;
            this.currentMember.password = password.Password;
            var data = Services.APIHandle.LoginHandle(this.currentMember).Result;
            var responseContent = Services.APIHandle.LoginHandle(this.currentMember).Result.Content.ReadAsStringAsync().Result;
            //Debug.WriteLine(data.Result.Content.ReadAsStringAsync().Result);
            if (data.StatusCode == System.Net.HttpStatusCode.Created)
            {
                // save file...
                Debug.WriteLine(responseContent);
                // Doc token
                Token token = JsonConvert.DeserializeObject<Token>(responseContent);

                // Luu token
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, responseContent);
                this.Frame.Navigate(typeof(Views.UserInfomation));
            }
            else
            {

                ErrorList error = JsonConvert.DeserializeObject<ErrorList>(responseContent);
                Debug.WriteLine(error.error.Count);
                //if (error != null && error.error.Count > 0)
                //{
                //    var messageDialog = new MessageDialog(error.error as string);
                //    foreach (var key in error.error.Keys)
                //    {
                //        var textMessage = this.FindName(key);
                //        if (textMessage == null)
                //        {
                //            continue;
                //        }

                //        await messageDialog.ShowAsync();
                //        //TextBlock textBlock = textMessage as TextBlock;
                //        //textBlock.Text = error.error[key];
                //        //textBlock.Visibility = Visibility.Visible;
                //    }
                //}

            }

        }
    }
}
