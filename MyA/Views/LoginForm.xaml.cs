using MyA.Entity;
using MyA.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            Services.SymbolColorChange.changeColorSymbol("AccountSymbol");
            this.currentMember = new Member();
            this.InitializeComponent();            
            //Debug.WriteLine()
            
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

                
                // Luu token
                GlobalHandle.saveToken(responseContent);
                //save email last user
                GlobalHandle.saveLastUser(this.currentMember.email);
                //save passWord if check savebox
                if (savePassword.IsChecked == true)
                {
                    GlobalHandle.savePassWord(this.currentMember.password);
                }
                else
                {
                    if(await GlobalHandle.checkPassword() != null)
                    {
                        GlobalHandle.deletePassword();
                    }
                    
                }
                //Change Account Name in splitView
                Services.GlobalHandle.changeAccountName();
                Services.GlobalHandle.ShowSignOutButton();

                // Next page
                this.Frame.Navigate(typeof(Views.UserInfomation));
            }
            else
            {

                ErrorList error = JsonConvert.DeserializeObject<ErrorList>(responseContent);
                Debug.WriteLine(responseContent);
                if (error != null && error.error.Count > 0)
                {
                    var content = "";
                    foreach (var key in error.error.Keys)
                    {
                        //var textMessage = this.FindName(key);
                        //if (textMessage == null)
                        //{
                        //    continue;
                        //}
                        content += error.error[key].ToString();

                        
                        //TextBlock textBlock = textMessage as TextBlock;
                        //textBlock.Text = error.error[key];
                        //textBlock.Visibility = Visibility.Visible;
                    }
                    var messageDialog = new MessageDialog(content);
                    await messageDialog.ShowAsync();
                }

            }

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(await GlobalHandle.checkLastuser() != null)
            {
                userEmail.Text = await GlobalHandle.checkLastuser();
            }
            if(await GlobalHandle.checkPassword() != null)
            {
                password.Password = await GlobalHandle.checkPassword();
            }
        }
    }
}
