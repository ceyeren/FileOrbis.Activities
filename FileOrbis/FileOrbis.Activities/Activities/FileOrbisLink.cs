using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using FileOrbis.Activities.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace FileOrbis.Activities
{
    [LocalizedDisplayName(nameof(Resources.FileOrbisLink_DisplayName))]
    [LocalizedDescription(nameof(Resources.FileOrbisLink_Description))]
    public class FileOrbisLink : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedDisplayName(nameof(Resources.FileOrbisLink_Token_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLink_Token_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Token { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLink_Body_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLink_Body_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Body { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLink_Url_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLink_Url_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Url { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLink_Link_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLink_Link_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Link { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLink_Response_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLink_Response_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Response { get; set; }

        #endregion


        #region Constructors

        public FileOrbisLink()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Token == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Token)));
            if (Body == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Body)));
            if (Url == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Url)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);
            var token = Token.Get(context);
            var body = Body.Get(context);
            var url = Url.Get(context);

            var client = new RestClient(url + "/api/v2/link/create");
            var request = new RestRequest(); 
            RestResponse response = new RestResponse();
            var link = "";
            var content = "";
            try
            {
                request.Method = Method.Post;
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", token.ToString());
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                response = client.Execute(request);
                content = response.Content;
                var obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                link = obj.Data.AccessLinkCollection[0];
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Creating link is failed \n" + response.Content);
            }

            // Outputs
            return (ctx) => {
                Link.Set(ctx, link);
                Response.Set(ctx, content);
            };
        }

        private async Task ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var timeout = TimeoutMS.Get(context);
            var token = Token.Get(context);
            var body = Body.Get(context);
            var url = Url.Get(context);

            
        }

        #endregion
    }
}

