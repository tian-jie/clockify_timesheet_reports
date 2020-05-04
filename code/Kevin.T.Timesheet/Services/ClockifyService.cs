using Infrastructure.Core.Logging;
using Kevin.T.Clockify.Data.Models;
using Kevin.T.Timesheet.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kevin.T.Timesheet.Services
{
    public class ClockifyService : IClockifyService
    {
        ILogger _logger = LogManager.GetLogger("Banner Management");


        public async Task<LoginModel> Login(string username, string password)
        {
            string url = "https://global.api.clockify.me/auth/token";
            string loginBodyFormat = "{{\"email\":\"{0}\",\"password\":\"{1}\"}}";

            string loginBody = string.Format(loginBodyFormat, username, password);
            var content = new StringContent(loginBody, System.Text.Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://clockify.me/login");
            //httpClient.DefaultRequestHeaders.Add(":authority", "global.api.clockify.me");

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync(url, content);


            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginModel>(res);

            return resultModel;
        }

        public async Task<List<UserGroupModel>> GetUserGroups(string userid, string token)
        {
            string url = string.Format("https://global.api.clockify.me/workspaces/{0}/userGroups/", userid);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("global.api.clockify.me");
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://clockify.me/reports/detailed");

            var response = await httpClient.GetAsync(url);


            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserGroupModel>>(res);

            return resultModel;
        }

        public async Task<List<UserModel>> GetUsers(string userid, string token)
        {
            string url = string.Format("https://global.api.clockify.me/workspaces/{0}/users", userid);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://clockify.me/reports/detailed");

            var response = await httpClient.GetAsync(url);

            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserModel>>(res);

            return resultModel;
        }
        public async Task<List<TimeEntryModel>> GetTimeEntries(string userid, string token, DateTime startDate, DateTime endDate)
        {
            string url = string.Format("https://global.api.clockify.me/workspaces/{0}/reports/summary/entries", userid);

            string bodyFormat = "{{\"userGroupIds\":[],\"userIds\":[],\"projectIds\":[],\"clientIds\":[],\"taskIds\":[],\"tagIds\":[],\"billable\":\"BOTH\",\"description\":\"\",\"firstTime\":true,\"archived\":\"All\"," +
                "\"startDate\":\"{0}\",\"endDate\":\"{1}\"," +
                "\"me\":\"TEAM\",\"includeTimeEntries\":true,\"zoomLevel\":\"month\",\"name\":\"\",\"groupingOn\":false,\"groupedByDate\":false,\"page\":0,\"sortDetailedBy\":\"timeAsc\",\"count\":1000000}}";

            string body = string.Format(bodyFormat, startDate.ToString("s") + "Z", endDate.AddDays(1).ToString("s") + "Z");

            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://clockify.me/reports/detailed");

            httpClient.Timeout = new TimeSpan(0, 10, 0);

            var response = await httpClient.PostAsync(url, content);

            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeEntryModel>>(res);

            return resultModel;
        }

        public async Task<List<ClientModel>> GetClients(string userid, string token)
        {
            string url = string.Format("https://global.api.clockify.me/workspaces/{0}/clients", userid);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://clockify.me/reports/detailed");

            var response = await httpClient.GetAsync(url);

            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClientModel>>(res);

            return resultModel;
        }

        public async Task<List<ProjectModel>> GetProjects(string userid, string token)
        {
            string url = string.Format("https://global.api.clockify.me/workspaces/{0}/projects/reportFilter", userid);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri("https://clockify.me/reports/detailed");

            var response = await httpClient.GetAsync(url);

            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectModel>>(res);

            return resultModel;
        }

    }
}
