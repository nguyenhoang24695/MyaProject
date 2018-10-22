using MyA.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyA.Services
{
    class APIHandle
    {
        private static string DOMAIN = "https://2-dot-backup-server-002.appspot.com";
        private static string REGISTER_API = DOMAIN + "/_api/v2/members";
        private static string LOGIN_API = DOMAIN + "/_api/v2/members/authentication";
        private static string USERINFOMATION_API = DOMAIN + "/_api/v2/members/information";
        private static string GETSONG_API = DOMAIN + "/_api/v2/songs";
        public  static Task<HttpResponseMessage> RegisterHandle(Member member)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(REGISTER_API, content);
            //var contents = await response.Result.Content.ReadAsStringAsync();
            //var ct = await response.Result.Content.ReadAsStringAsync();
            //Debug.WriteLine(ct);
            return response;            
        }
        public static Task<HttpResponseMessage> LoginHandle(Member member)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(LOGIN_API, content);
            //var contents = await response.Result.Content.ReadAsStringAsync();
            //var ct = await response.Result.Content.ReadAsStringAsync();
            //Debug.WriteLine(ct);
            return response;
        }
        public static Task<HttpResponseMessage> UserInfomationHandle(string token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
            //var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.GetAsync(USERINFOMATION_API);
            //var contents = await response.Result.Content.ReadAsStringAsync();
            //var ct = await response.Result.Content.ReadAsStringAsync();
            
            return response;
        }
        public static Task<HttpResponseMessage> SongDataHandle(Token token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
            //var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.GetAsync(GETSONG_API);
            //var contents = await response.Result.Content.ReadAsStringAsync();
            //var ct = await response.Result.Content.ReadAsStringAsync();
            Debug.WriteLine(response.Result.StatusCode);
            return response;
        }

    }
}
