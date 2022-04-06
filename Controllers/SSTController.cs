using SSTService;
using SSTDataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CrudCoreDatabaseFirst.Controllers
{
    [ApiController]
    public class SSTController : ControllerBase
    {
        private readonly IUserService sslService;
        private readonly IConfiguration configuration;

        public SSTController(IUserService sslService, IConfiguration configuration)
        {
            this.sslService = sslService;
            this.configuration = configuration;
        }

        /// <summary>
        /// Wrapper method to call GetQnAs API endpoint
        /// </summary>
        [HttpGet]
        [Route("api/GetQnAs")]
        public async Task<QnARecords> GetQnAs()
        {
            //UserRole userRole = sslService.GetUserRoleMapping(1);

            ////Assuming RoleId 1 has access to view all QnAs
            //if (userRole.RoleId == 1)
            //{
            //    //To do - Call to API Endpoint here
            //    return new QnARecords();
            //}
            //else
            //{
            //    return Unauthorized();

            //}

            QnARecords allQnAs = new QnARecords();

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/SCAL/qnas?api-version=2021-10-01";

            var JsonResponse = await CallApiEndpoint(url);

            allQnAs = JsonConvert.DeserializeObject<QnARecords>(JsonResponse);

            for (var i = 0; i < allQnAs.QnAs.Length; i++)
            {
                allQnAs.QnAs[i].Metadata = JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(JsonResponse);
            }

            return allQnAs;
        }

        /// <summary>
        /// Wrapper method to call GetAllProjects API endpoint
        /// </summary>
        [HttpGet]
        [Route("api/GetAllProjects")]
        public async Task<ProjectRecords> GetAllProjects()
        {
            ProjectRecords allProjects = new ProjectRecords();

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects?api-version=2021-10-01";

            var JsonResponse = await CallApiEndpoint(url);

            allProjects = JsonConvert.DeserializeObject<ProjectRecords>(JsonResponse);

            return allProjects;
        }

        private async Task<string> CallApiEndpoint(string url)
        {
            var Client = new HttpClient();
            // add QnAAuthKey to Authorization header
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["QKBApikey"]);
            var JsonResponse = await Client.GetAsync(url).Result.Content.ReadAsStringAsync();

            return JsonResponse;
        }
    }
}
