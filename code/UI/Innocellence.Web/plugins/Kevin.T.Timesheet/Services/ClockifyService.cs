using Infrastructure.Core.Logging;
using Kevin.T.Clockify.Data.Models;
using Kevin.T.Timesheet.Entities;
using Kevin.T.Timesheet.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;

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


        public async Task<List<TimeEntryModelV2>> GetTimeEntriesV2(string userid, string token, DateTime startDate, DateTime endDate)
        {
            List<TimeEntryModelV2> timeEntryModelV2s = new List<TimeEntryModelV2>();

            var page = 0;
            var totalRecord = 20000;

            do
            {
                page++;
                var r = await GetTimeEntriesByPageV2(userid, token, page, startDate, endDate);

                timeEntryModelV2s.AddRange(r.timeEntries);
                totalRecord = r.totals[0].entriesCount;
            } while (page * 200 < totalRecord);

            return timeEntryModelV2s;
        }
        public async Task<List<TimeEntry>> GetTimeEntriesV3(string userid, string token, DateTime startDate, DateTime endDate)
        {
            var url = "https://reports.api.clockify.me/workspaces/5d5f3b1bbf6ed132e4c82eb8/reports/summary";
            List<TimeEntry> timeEnteries = new List<TimeEntry>();

            var date = startDate;
            while (date <= endDate)
            {
                var dateString = date.ToString("yyyy-MM-dd");

                string bodyFormat = "{{\"dateRangeStart\":\"{0}T00:00:00.000Z\",\"dateRangeEnd\":\"{0}T23:59:59.999Z\",\"sortOrder\":\"ASCENDING\",\"description\":\"\",\"rounding\":false,\"withoutDescription\":false,\"amountShown\":\"HIDE_AMOUNT\",\"zoomLevel\":\"WEEK\",\"userLocale\":\"zh_CN\",\"customFields\":null,\"summaryFilter\":{{\"sortColumn\":\"GROUP\",\"groups\":[\"USER\",\"PROJECT\",\"TASK\"]}}}}";
                string body = string.Format(bodyFormat, dateString);
                var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
                httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
                httpClient.DefaultRequestHeaders.Referrer = new System.Uri(url);

                httpClient.Timeout = new TimeSpan(0, 10, 0);

                var response = await httpClient.PostAsync(url, content);

                var res = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new System.Exception(res);
                }

                var resultModel = JsonConvert.DeserializeObject<TimeEntryResponseModelV3>(res);

                var now = DateTime.Now;

                foreach (var u in resultModel.groupOne)
                {
                    foreach (var p in u.children)
                    {
                        foreach (var t in p.children)
                        {

                            timeEnteries.Add(new TimeEntry()
                            {
                                Date = date,
                                Gid = "",
                                IsBillable = true,
                                IsDeleted = false,
                                IsLocked = false,
                                ProjectId = p._id,
                                TaskId = t._id,
                                TotalHours = (decimal)(t.duration / 3600f),
                                UserId = u._id,
                                CreatedDate = now,
                                CreatedUserID = "",
                                CreatedUserName = "",
                                UpdatedDate = now,
                                UpdatedUserID = "",
                                UpdatedUserName = ""
                            });
                        }
                    }
                }
                _logger.Debug(JsonConvert.SerializeObject(timeEnteries));
                date = date.AddDays(1);

            }

            return timeEnteries;
        }


        public async Task<TimeEntryResponseModelV2> GetTimeEntriesByPageV2(string userid, string token, int pageId, DateTime startDate, DateTime endDate)
        {
            string urlTemplate = "https://reports.api.clockify.me/workspaces/{0}/reports/detailed?start={1}&end={2}&page={3}&pageSize=200";

            string bodyFormat = "{{\"dateRangeStart\":\"{0}\",\"dateRangeEnd\":\"{1}\",\"sortOrder\":\"DESCENDING\",\"description\":\"\",\"rounding\":false,\"withoutDescription\":false,\"amountShown\":\"HIDE_AMOUNT\",\"zoomLevel\":\"WEEK\",\"userLocale\":\"zh_CN\",\"customFields\":null,\"detailedFilter\":{{\"sortColumn\":\"ID\",\"page\":{2},\"pageSize\":200,\"auditFilter\":null}}}}";
            string startString = startDate.ToString("s") + "Z";
            string endString = endDate.AddDays(1).ToString("s") + "Z";
            string body = string.Format(bodyFormat, startString, endString, pageId);
            _logger.Debug(body);

            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var url = string.Format(urlTemplate, userid, startString, endString, pageId);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
            httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri(url);

            httpClient.Timeout = new TimeSpan(0, 10, 0);

            var response = await httpClient.PostAsync(url, content);

            var res = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new System.Exception(res);
            }

            _logger.Debug(res);

            var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeEntryResponseModelV2>(res);

            return resultModel;
        }

        //public async Task<TimeEntryResponseModelV2> GetTimeEntriesByPageV3(string userid, string token, DateTime startDate, DateTime endDate)
        //{
        //    string urlTemplate = "https://reports.api.clockify.me/workspaces/{0}/reports/detailed?start={1}&end={2}&page={3}&pageSize=200";

        //    string bodyFormat = "{{\"dateRangeStart\":\"{0}\",\"dateRangeEnd\":\"{1}\",\"sortOrder\":\"DESCENDING\",\"description\":\"\",\"rounding\":false,\"withoutDescription\":false,\"amountShown\":\"HIDE_AMOUNT\",\"zoomLevel\":\"WEEK\",\"userLocale\":\"zh_CN\",\"customFields\":null,\"detailedFilter\":{{\"sortColumn\":\"ID\",\"page\":{2},\"pageSize\":200,\"auditFilter\":null}}}}";
        //    string startString = startDate.ToString("s") + "Z";
        //    string endString = endDate.AddDays(1).ToString("s") + "Z";
        //    string body = string.Format(bodyFormat, startString, endString, pageId);
        //    _logger.Debug(body);

        //    var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        //    var url = string.Format(urlTemplate, userid, startString, endString, pageId);
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("Origin", "https://clockify.me");
        //    httpClient.DefaultRequestHeaders.Add("x-auth-token", token);
        //    httpClient.DefaultRequestHeaders.Referrer = new System.Uri(url);

        //    httpClient.Timeout = new TimeSpan(0, 10, 0);

        //    var response = await httpClient.PostAsync(url, content);

        //    var res = await response.Content.ReadAsStringAsync();
        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        throw new System.Exception(res);
        //    }

        //    _logger.Debug(res);

        //    var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeEntryResponseModelV2>(res);

        //    return resultModel;
        //}

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
