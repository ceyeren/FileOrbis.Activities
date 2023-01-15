using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using FileOrbis.Activities.Properties;
using Newtonsoft.Json;
using RestSharp;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace FileOrbis.Activities
{
    [LocalizedDisplayName(nameof(Resources.FileOrbisLogin_DisplayName))]
    [LocalizedDescription(nameof(Resources.FileOrbisLogin_Description))]
    public class FileOrbisLogin : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLogin_Username_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLogin_Username_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Username { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLogin_Password_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLogin_Password_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Password { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLogin_Url_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLogin_Url_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Url { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLogin_ClientId_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLogin_ClientId_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ClientId { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisLogin_Token_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisLogin_Token_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Token { get; set; }

        #endregion


        #region Constructors

        public FileOrbisLogin()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Username == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Username)));
            if (Password == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Password)));
            if (Url == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Url)));
            if (ClientId == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ClientId)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var username = Username.Get(context);
            var password = Password.Get(context);
            var url = Url.Get(context);
            var clientid = ClientId.Get(context);

            var client = new RestClient(url + "/api/v2/account/login");

            var request = new RestRequest();
            var token = "";

            RestResponse response = new RestResponse();
            try
            {
                request.Method = Method.Post;
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { username = username, password = password });
                request.AddHeader("x-fo-client-key", clientid);
                response = client.Execute(request);
                var obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                token = obj.Data.Token;
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Login is failed \n" + response.Content);
            }

            // Outputs
            return (ctx) => {
                Token.Set(ctx, token);
            };
        }

        #endregion
    }
}

