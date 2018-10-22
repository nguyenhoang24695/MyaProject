using MyA.Entity;
using MyA.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
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
    public sealed partial class RegisterForm : Page
    {        
        
        private Member registerMember;
        private static StorageFile file;
        private static string UploadUrl;
        public RegisterForm()
        {
            Services.SymbolColorChange.changeColorSymbol("AccountSymbol");
            GetUploadUrl();
            this.registerMember = new Member();
            this.InitializeComponent();
            
        }

        private async void RegisterHandle(object sender, RoutedEventArgs e)
        {
            registerMember.address = address.Text;            
            
            registerMember.email = email.Text;
            registerMember.firstName = firstName.Text;
            
            registerMember.lastName = lastName.Text;
            registerMember.password = password.Password;
            registerMember.phone = phone.Text;
            registerMember.avatar = ImageUrl.Text;
            var data = Services.APIHandle.RegisterHandle(registerMember);
            var responseContent = data.Result.Content.ReadAsStringAsync().Result;
            if (data.Result.StatusCode == HttpStatusCode.Created )
            {
                Debug.WriteLine(data.Result.Content.ReadAsStringAsync().Result);
                var messageDialog = new MessageDialog("Đăng ký thành công");
                await messageDialog.ShowAsync();
                //Auto Login
                var loginData = Services.APIHandle.LoginHandle(registerMember).Result;
                var loginResponseContent = Services.APIHandle.LoginHandle(registerMember).Result.Content.ReadAsStringAsync().Result;
                // Luu token
                GlobalHandle.saveToken(loginResponseContent);
                //save email last user
                GlobalHandle.saveLastUser(registerMember.email);
                //Change account Name
                Services.GlobalHandle.changeAccountName();
                //Show Login Button
                Services.GlobalHandle.ShowSignOutButton();
                //Navigate Frame
                this.Frame.Navigate(typeof(Views.HomePage));
                
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
                        content += "\n";


                        //TextBlock textBlock = textMessage as TextBlock;
                        //textBlock.Text = error.error[key];
                        //textBlock.Visibility = Visibility.Visible;
                    }
                    var messageDialog = new MessageDialog(content);
                    await messageDialog.ShowAsync();
                }
            }
            
            
        }
        private async void Capture_Photo(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file == null)
            {
                // User cancelled photo capture
                return;
            }
            HttpUploadFile(UploadUrl, "myFile", "image/png");
        }
        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            Debug.WriteLine(url);
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";

            Stream rs = await wr.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            // write file.
            Stream fileStream = await file.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
                //string imgUrl = reader2.ReadToEnd();
                Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
                Debug.WriteLine(u.AbsoluteUri);
                ImageUrl.Text = u.AbsoluteUri;                
                MyAvatar.Source = new BitmapImage(u);                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

        }
        private static async void GetUploadUrl()
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri requestUri = new Uri("https://2-dot-backup-server-002.appspot.com/get-upload-token");
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            //Debug.WriteLine(httpResponseBody);
            UploadUrl = httpResponseBody;
        }

        private void Change_Birthday(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.registerMember.birthday = sender.Date.Value.ToString("yyyy-MM-dd");
            Debug.WriteLine(this.registerMember.birthday);
        }
        private void Select_Gender(object sender, RoutedEventArgs e)
        {
            RadioButton radioGender = sender as RadioButton;
            this.registerMember.gender = Int32.Parse(radioGender.Tag.ToString());
            Debug.WriteLine(this.registerMember.gender);
        }

        private void ReturnLoginForm(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.LoginForm));
        }
    }
}
