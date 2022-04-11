#region Header

#endregion

#region Using

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SSTDataAccess;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

#endregion

namespace CrudCoreDatabaseFirst.Controllers
{
    /// <summary>
    /// Controller class for QnA Self Service Tool
    /// </summary>
    [ApiController]
    public class SSTController : ControllerBase
    {
        #region Private Fields

        private readonly IConfiguration configuration;
        private readonly string get = "get";
        private readonly string post = "post";
        private readonly string patch = "patch";
        private readonly string applicationJson = "application/json";
        private readonly string ocpApimSubscriptionKey = "Ocp-Apim-Subscription-Key";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SSTController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Wrapper method to call GetQnAs API endpoint
        /// </summary>
        [HttpGet]
        [Route("api/GetQnAs")]
        public async Task<QnARecords> GetQnAs()
        {
            QnARecords allQnAs = new QnARecords();

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/SCAL/qnas?api-version=2021-10-01";

            var JsonResponse = await CallApiEndpoint(url, get, string.Empty);

            allQnAs = JsonConvert.DeserializeObject<QnARecords>(JsonResponse);

            return allQnAs;
        }

        /// <summary>
        /// Wrapper method to call QueryKB API endpoint
        /// </summary>
        [HttpPost]
        [Route("api/QueryKB")]
        public async Task<QnARecords> QueryKB(SuggestedQuestions question)
        {
            QnARecords qnARecord = new QnARecords();
            var content = JsonConvert.SerializeObject(question);

            var url = $"{configuration["QKBEndpoint"]}/language/:query-knowledgebases?projectName=SCAL&api-version=2021-10-01&deploymentName=test";

            var JsonResponse = await CallApiEndpoint(url, post, content);

            qnARecord = JsonConvert.DeserializeObject<QnARecords>(JsonResponse);

            return qnARecord;
        }

        /// <summary>
        /// Wrapper method to call UpdateQnAs API endpoint
        /// </summary>
        [HttpPatch]
        [Route("api/UpdateQnAs")]
        public async Task<string> UpdateQnAs(QnARecord[] qnARecords)
        {
            var content = JsonConvert.SerializeObject(qnARecords, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/SCAL/qnas?api-version=2021-10-01";

            var JsonResponse = await CallApiEndpoint(url, patch, content);

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

            var JsonResponse = await CallApiEndpoint(url, get, string.Empty);

            allProjects = JsonConvert.DeserializeObject<ProjectRecords>(JsonResponse);

            return allProjects;
        }

        /// <summary>
        /// Wrapper method to call UpdateSourcesFileUpload API endpoint
        /// </summary>
        [HttpPatch]
        [Route("api/UpdateSourcesFileUpload")]
        public async Task<string> UpdateSourcesFileUpload(SourceRecord[] newSources)
        {
            var content = JsonConvert.SerializeObject(newSources, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var url = $"{configuration["QKBEndpoint"]}/language/query-knowledgebases/projects/test2/sources?api-version=2021-10-01";
            var JsonResponse = await CallApiEndpoint(url, patch, content);

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

            var JsonResponse = await CallApiEndpoint(url, get, string.Empty);

            allSources = JsonConvert.DeserializeObject<SourceRecords>(JsonResponse);

            return allSources;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Common method to call API endpoint
        /// </summary>
        private async Task<string> CallApiEndpoint(string url, string operationType, string content)
        {
            var Client = new HttpClient();
            // add QnAAuthKey to Authorization header
            Client.DefaultRequestHeaders.Add(ocpApimSubscriptionKey, configuration["QKBApikey"]);
            if (operationType == get)
            {
                return await Client.GetAsync(url).Result.Content.ReadAsStringAsync();
            }
            else if (operationType == patch)
            {
                return await Client.PatchAsync(url, new StringContent(content, Encoding.UTF8, applicationJson)).Result.Content.ReadAsStringAsync();
            }
            else if (operationType == post)
            {
                return await Client.PostAsync(url, new StringContent(content, Encoding.UTF8, applicationJson)).Result.Content.ReadAsStringAsync();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
