using JIRA.Bl.Dto;
using JIRA.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JIRA.Services
{
    public interface IHttpService
    {
        Task<string> CreateIssue(IssueDto dto);
        Task<string> GetIssues();
        Task<string> GetIssueById(string id);
        Task<string> EditIssue(string id, IssueDto dto);
        Task<string> AssignIssueToUser(string id, string name);
        Task<string> DeleteIssue(string id);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        private readonly JiraSettings _jiraSettings;

        public static string GetEncodedCredentials(string UserName, string Password)
        {
            string mergedCredentials = String.Format("{0}:{1}", UserName, Password);
            byte[] byteCredentials = Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        public HttpService(HttpClient httpClient, IOptions<JiraSettings> jiraSettings)
        {
            _jiraSettings = jiraSettings.Value;
            
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(_jiraSettings.Url);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", GetEncodedCredentials(_jiraSettings.User, _jiraSettings.Password));

            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetIssues()
        {
            var response = await _httpClient.GetAsync($"rest/api/2/search");

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }

        public async Task<string> GetIssueById(string id)
        {

            var response = await _httpClient.GetAsync($"rest/api/2/issue/{id}");

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }


        public async Task<string> CreateIssue(IssueDto dto)
        {

            string issue = JsonConvert.SerializeObject(
                    new
                    {
                        fields = new
                        {
                            project = new
                            {
                                key = dto.Key
                            },
                            summary = dto.Summary,
                            description = dto.Description,
                            issuetype = new { name = "Task" }
                        }
                    }
                );

            var content = new StringContent(issue, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("rest/api/2/issue", content);

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }


        public async Task<string> EditIssue(string id, IssueDto dto)
        {
            var issue = await GetIssueById(id);

            if (issue is null) return null;

            string update = JsonConvert.SerializeObject(
                    new
                    {
                        fields = new
                        {
                            summary = dto.Summary,
                            description = dto.Description,
                        }
                    }
                );

            var content = new StringContent(update, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"rest/api/2/issue/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                return response.StatusCode.ToString();
            }

        }

        public async Task<string> AssignIssueToUser(string id, string accountId)
        {
            var issue = await GetIssueById(id);

            if (issue is null) return null;

            string update = JsonConvert.SerializeObject(
                    new
                    {
                        accountId
                    }
                );

            var content = new StringContent(update, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"rest/api/2/issue/{id}/assignee", content);

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                return response.StatusCode.ToString();
            }

        }

        public async Task<string> DeleteIssue(string id)
        {

            var response = await _httpClient.DeleteAsync($"rest/api/2/issue/{id}");

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }
    }
}
