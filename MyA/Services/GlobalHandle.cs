using MyA.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyA.Services
{
    class GlobalHandle
    {
        public static async Task<string> checkToken()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("token.txt") != null)
            {
                StorageFile file = await folder.GetFileAsync("token.txt");
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
            file.DeleteAsync();
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
            file.DeleteAsync();
        }
    }
}
