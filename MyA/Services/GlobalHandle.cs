using MyA.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyA.Services
{
    class GlobalHandle
    {
        public static async Task<string> checkToken()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("token.txt") != null)
            {
                Debug.WriteLine(await folder.TryGetItemAsync("token.txt"));
                StorageFile file = await folder.GetFileAsync("token.txt");
                Debug.WriteLine("GetIn");
                var a = JObject.Parse(await FileIO.ReadTextAsync(file));
                Debug.WriteLine(a["token"]);
                string b = (string)a["token"];
                return b;
            }
            else
            {
                
                return null;

            }
        }
        public async static void saveLastUser(string email)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("lastUser.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, email);
        }
        public static async Task<string>  checkLastuser()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("lastUser.txt") != null)
            {
                StorageFile file = await folder.GetFileAsync("lastUser.txt");
                string a = await FileIO.ReadTextAsync(file);
                return a;
                
            }
            else
            {
                return null;

            }
        }
        public static async void deleteToken()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("token.txt");
            await file.DeleteAsync();
        }
        public async static void savePassWord(string password)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("passworSave.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, password);
        }
        public async static void saveToken(string responseContent)
        {
            // Doc token
            Token token = JsonConvert.DeserializeObject<Token>(responseContent);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, responseContent);
        }
        public static async Task<string> checkPassword()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("passworSave.txt") != null)
            {
                StorageFile file = await folder.GetFileAsync("passworSave.txt");
                string a = await FileIO.ReadTextAsync(file);
                return a;

            }
            else
            {
                return null;

            }
        }
        public static async void deletePassword()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("passworSave.txt");
            await file.DeleteAsync();
        }
        public async static void changeAccountName()
        {
            if (await checkToken() is string token) 
            {
                Debug.WriteLine(token);
                string tokenString = token;
                Member currentMember = new Member();
                var infoUserDataContent = await Services.APIHandle.UserInfomationHandle(tokenString).Result.Content.ReadAsStringAsync();
                currentMember = JsonConvert.DeserializeObject<Member>(infoUserDataContent);
                (MyA.Views.SplitView.mySplit.FindName("AccountName") as TextBlock).Text = currentMember.firstName + " " + currentMember.lastName;
            }
            else
            {
                (MyA.Views.SplitView.mySplit.FindName("AccountName") as TextBlock).Text = "Account";
            }
        }
        public async static void SignOutHandle(Frame frame)
        {
            
            if (await Services.GlobalHandle.checkToken() != null)
            {

                Services.GlobalHandle.deleteToken();
                //Services.GlobalHandle.changeAccountName();   
                Services.GlobalHandle.ShowSignOutButton();
                frame.Navigate(typeof(Views.LoginForm));
                
            }
            
        }
        public async static void ShowSignOutButton()
        {
            //Show SignOut Button
            if (await Services.GlobalHandle.checkToken() == null)
            {

                (MyA.Views.SplitView.mySplit.FindName("signOutLabel") as RadioButton).Visibility = Visibility.Collapsed;

            }
            else
            {
                Debug.WriteLine("Quy` Xuong");
                (MyA.Views.SplitView.mySplit.FindName("signOutLabel") as RadioButton).Visibility = Visibility.Visible;
            }
        }
        public async static void checkSongValid(string link)
        {
            HttpWebRequest req = WebRequest.CreateHttp(link);
            req.Method = "GET";
            req.ContinueTimeout = 5000;
            

            string content = null;
            HttpStatusCode code = HttpStatusCode.OK;
            HttpWebResponse response = (HttpWebResponse)await req.GetResponseAsync();
            code = response.StatusCode;
            Debug.WriteLine(code);
            return;
        }

    }
}
