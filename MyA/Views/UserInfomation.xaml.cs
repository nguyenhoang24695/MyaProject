using MyA.Entity;
using MyA.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class UserInfomation : Page
    {
        private Member registerMember;
        private static StorageFile file;
        private static string UploadUrl;
        private Token token;
        public UserInfomation()
        {
            Services.SymbolColorChange.changeColorSymbol("AccountSymbol");
            GetUploadUrl();
            this.registerMember = new Member();
            this.token = new Token();
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
            if (data.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine(data.Result.Content.ReadAsStringAsync().Result);
                var messageDialog = new MessageDialog("Đăng ký thành công");
                await messageDialog.ShowAsync();
                this.Frame.GoBack();
            }
            else
            {
                var messageDialog = new MessageDialog("Lỗi: " + data.Result.Content.ReadAsStringAsync().Result);
                await messageDialog.ShowAsync();
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
                this.registerMember.avatar = ImageUrl.Text;
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
        

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalHandle.checkToken() != null)
            {
                this.token.token = await GlobalHandle.checkToken();
                Debug.WriteLine(this.token.token);
                
                var data = Services.APIHandle.UserInfomationHandle(this.token.token);
                var responseContent = data.Result.Content.ReadAsStringAsync().Result;
                if ( data.Result.StatusCode == HttpStatusCode.Created)
                {
                    Member currentMember = JsonConvert.DeserializeObject<Member>(responseContent);
                    lastName.Text = currentMember.lastName;
                    firstName.Text = currentMember.firstName;
                    email.Text = currentMember.email;
                    phone.Text = currentMember.phone;
                    address.Text = currentMember.address;
                    ImageUrl.Text = currentMember.avatar;

                    //check Gender
                    switch (currentMember.gender)
                    {
                        case 1:
                            Gender_Male.IsChecked = true;
                            break;
                        case 0:
                            Gender_Female.IsChecked = true;
                            break;
                        case 2:
                            Gender_Other.IsChecked = true;
                            break;
                        default:
                            break;
                    }

                    //show pic
                    Uri u;
                    
                    try
                    {
                        u = new Uri(currentMember.avatar, UriKind.Absolute);
                        Debug.WriteLine(u.AbsoluteUri);
                        ImageUrl.Text = u.AbsoluteUri;
                        MyAvatar.Source = new BitmapImage(u);
                    } catch (Exception error)
                    {

                    }
                    
                    
                    
                    
                    //add birthday into form
                    BirthDay.Date = DateTime.ParseExact(currentMember.birthday, "yyyy-MM-ddT00:00:00.000+0000", CultureInfo.InvariantCulture);
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

        private void SignOut(object sender, RoutedEventArgs e)
        {
           Services.GlobalHandle.SignOutHandle(this.Frame);
        }
    }
}

