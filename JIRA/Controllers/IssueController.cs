using JIRA.Bl.Dto;
using JIRA.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JIRA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class IssueController : ControllerBase
    {
        private readonly IHttpService _httpService;
        public IssueController(IHttpService httpService)
        {
            _httpService = httpService;
        }
       
        [HttpGet]
        public async Task<List<IssueDto>> GetIssues()
        {
            return await _httpService.GetIssues();
        }

        [HttpGet("{id}")]
        public async Task<IssueDto> GetIssueById(string id)
        {
            return await _httpService.GetIssueById(id);
        }

        [HttpPost]
        public async Task<string> CreateIssue([FromBody] IssueDto dto)
        {
            return await _httpService.CreateIssue(dto);
        }

        [HttpPut("{id}")]
        public async Task<string> EditIssue(string id, [FromBody] IssueDto dto)
        {
            return await _httpService.EditIssue(id, dto);
        }

        [HttpPut("assignIssue/{id}")]
        public async Task<string> AssignIssueToUser(string id, [FromBody] string name)
        {
            return await _httpService.AssignIssueToUser(id, name.ToString());
        }


        [HttpDelete]
        public async Task<string> DeleteIssue(string id)
        {
            return await _httpService.DeleteIssue(id); 
        }
    }
}
