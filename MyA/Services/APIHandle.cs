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
        private static string REGISTER_API = "https://2-dot-backup-server-002.appspot.com/_api/v2/members";
        private static string LOGIN_API = "https://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
        private static string USERINFOMATION_API = "http://2-dot-backup-server-002.appspot.com/_api/v2/members/information";
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
        public static Task<HttpResponseMessage> UserInfomationHandle(Token token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic" + token.token);
            //var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.GetAsync(USERINFOMATION_API);
            //var contents = await response.Result.Content.ReadAsStringAsync();
            //var ct = await response.Result.Content.ReadAsStringAsync();
            
            return response;
        }

    }
}
