using SSTService;
using SSTDataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

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

            var JsonResponse = await CallApiEndpoint(url, "get", string.Empty);

            allQnAs = JsonConvert.DeserializeObject<QnARecords>(JsonResponse);

            return allQnAs;
        }

        /// <summary>
        /// Wrapper method to call QueryKB API endpoint
        /// </summary>
        [HttpPost]
        [Route("api/QueryKB")]
        public async Task<QnARecords> QueryKB()
        {
            QnARecords qnARecord = new QnARecords();
            SuggestedQuestions question = new SuggestedQuestions();
            question.Question = "location";
            var content = JsonConvert.SerializeObject(question);

            var url = $"{configuration["QKBEndpoint"]}/language/:query-knowledgebases?projectName=SCAL&api-version=2021-10-01&deploymentName=test";

            var JsonResponse = await CallApiEndpoint(url, "post", content);

            qnARecord = JsonConvert.DeserializeObject<QnARecords>(JsonResponse);

            return qnARecord;
        }

        /// <summary>
        /// Wrapper method to call UpdateQnAs API endpoint
        /// </summary>
        [HttpPatch]
        [Route("api/UpdateQnAs")]
        public async Task<string> UpdateQnAs()
        {
            QnARecord[] qnARecords = new QnARecord[1];
            QnARecord qnARecord = new QnARecord();
            qnARecord.Operation = OperationKind.Add;
            QnA qnA = new QnA();
            qnA.QnAId = 0;
            qnA.Answer = "FirstUpdateQnAs";
            qnA.SourceName = "Editorial";
            qnA.Question = new string[] { "FirstUpdateQnAs", "FirstUpdateQnAs" };
            qnARecord.QnA = qnA;
            qnARecords[0] = qnARecord;

            var content = JsonConvert.SerializeObject(qnARecords);

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/SCAL/qnas?api-version=2021-10-01";

            var JsonResponse = await CallApiEndpoint(url, "patch", content);

            return JsonResponse;
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

            var JsonResponse = await CallApiEndpoint(url, "get", string.Empty);

            allProjects = JsonConvert.DeserializeObject<ProjectRecords>(JsonResponse);

            return allProjects;
        }

        /// <summary>
        /// Wrapper method to call UpdateSourcesFileUpload API endpoint
        /// </summary>
        [HttpPatch]
        [Route("api/UpdateSourcesFileUpload")]
        public async Task<string> UpdateSourcesFileUpload()
        {
            SourceRecord[] newSources = new SourceRecord[1];
            SourceRecord newSource = new SourceRecord();
            newSource.Operation = OperationKind.Add;
            Source source = new Source();
            source.SourceName = "SCALqnas.xlsx";
            source.DisplayName = "FirstSSTSource";
            source.SourceUri = "https://testpdconnect.blob.core.windows.net/users/SCALqnas.xlsx";
            source.SourceKind = SourceKind.File;
            newSource.Source = source;
            newSources[0] = newSource;

            var content = JsonConvert.SerializeObject(newSources);

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/test2/sources?api-version=2021-10-01";
            var JsonResponse = await CallApiEndpoint(url, "patch", content);

            return JsonResponse;
        }

        /// <summary>
        /// Wrapper method to call GetSources API endpoint
        /// </summary>
        [HttpGet]
        [Route("api/GetSources")]
        public async Task<SourceRecords> GetSources()
        {
            SourceRecords allSources = new SourceRecords();

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/test2/sources?api-version=2021-10-01";

            var JsonResponse = await CallApiEndpoint(url, "get", string.Empty);

            allSources = JsonConvert.DeserializeObject<SourceRecords>(JsonResponse);

            return allSources;
        }

        /// <summary>
        /// Common method to call API endpoint
        /// </summary>
        private async Task<string> CallApiEndpoint(string url, string operationType, string content)
        {
            var Client = new HttpClient();
            // add QnAAuthKey to Authorization header
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["QKBApikey"]);
            if (operationType == "get")
            {
                return await Client.GetAsync(url).Result.Content.ReadAsStringAsync();
            }
            else if (operationType == "patch")
            {
                return await Client.PatchAsync(url, new StringContent(content, Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync();
            }
            else if (operationType == "post")
            {
                return await Client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync();
            }
            else {
                return string.Empty;
            }
        }
    }
}
