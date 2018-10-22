using MyA.Entity;
using MyA.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MyA.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListSong : Page
    {
        private Token token;
        //private Song listSong;
        private ObservableCollection<Song> listSong;

        internal ObservableCollection<Song> List_Song { get => listSong; set => listSong = value; }
        public ListSong()
        {
            Services.SymbolColorChange.changeColorSymbol("MusicSymbol");
            token = new Token();
            this.List_Song = new ObservableCollection<Song>();            
            this.InitializeComponent();

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalHandle.checkToken() != null)
            {
                this.token.token = await GlobalHandle.checkToken();
                Debug.WriteLine("in");

                var data = Services.APIHandle.SongDataHandle(token);
                var responseContent = await data.Result.Content.ReadAsStringAsync();
                if (data.Result.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine(responseContent);
                    var arrayData = JsonConvert.DeserializeObject<List<Song>>(responseContent);
                    Debug.WriteLine(arrayData[0].name);
                    foreach(var SongItem in arrayData)
                    {
                        
                        this.List_Song.Add(new Song()
                        {
                            name = SongItem.name,
                            singer = SongItem.singer,
                            thumbnail = SongItem.thumbnail,
                            link = SongItem.link,
                        });
                    }
                    

                }
                else
                {

                    ErrorList error = JsonConvert.DeserializeObject<ErrorList>(responseContent);
                    Debug.WriteLine(responseContent);
                    Debug.WriteLine(error.error.Count);
                    if (error != null)
                    {

                        var content = error.message;
                        content += "\n Tài khoản hết hạn, vui lòng đăng nhập lại";

                        var messageDialog = new MessageDialog(content);
                        await messageDialog.ShowAsync();
                        //Xoa file token
                        GlobalHandle.deleteToken();
                        //Chuyen huong frame
                        this.Frame.Navigate(typeof(Views.LoginForm));
                    }
                }
            }
        }

        private async void playClickedSong(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Song dataSong = (btn.Tag as Song);
            
            
            Debug.WriteLine(dataSong.link);
            //return;
            MediaPlayerElement mediaplay = (SplitView.mySplit.FindName("mediaPlayer") as MediaPlayerElement);
            
            try
            {
                mediaplay.Source = MediaSource.CreateFromUri(new Uri(dataSong.link));      
                mediaplay.AutoPlay = true;
                //picture
                var img = (SplitView.mySplit.FindName("imageSong") as ImageBrush);
                BitmapImage bitmapImage = new BitmapImage(new Uri(dataSong.thumbnail));
                img.ImageSource = bitmapImage;
                //title 
                var title = (SplitView.mySplit.FindName("titleSong") as TextBlock);
                title.Text = dataSong.name;
            }
            catch
            {
                var messageDialog = new MessageDialog("Link nhạc lỗi hoặc không tồn tại");
                await messageDialog.ShowAsync();
            }
            
            

            
        }
    }
}
