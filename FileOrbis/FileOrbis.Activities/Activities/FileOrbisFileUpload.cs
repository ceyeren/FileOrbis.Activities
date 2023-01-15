using System;
using System.Activities;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using FileOrbis.Activities.Properties;
using Newtonsoft.Json;
using RestSharp;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace FileOrbis.Activities
{
    [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_DisplayName))]
    [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Description))]
    public class FileOrbisFileUpload : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_Url_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Url_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Url { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_DirectoryPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_DirectoryPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> DirectoryPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_Token_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Token_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Token { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_RequestBody_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_RequestBody_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> RequestBody { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_FolderId_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_FolderId_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<string> FolderId { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_ItemRelativePath_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_ItemRelativePath_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<string> ItemRelativePath { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_ItemName_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_ItemName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> ItemName { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_FileFullPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_FileFullPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FileFullPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_ItemId_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_ItemId_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<string> ItemId { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<bool> Overwrite { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_Classification_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Classification_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<string> Classification { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_DType_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_DType_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<string> DType { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_Success_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Success_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Success { get; set; }

        [LocalizedDisplayName(nameof(Resources.FileOrbisFileUpload_Response_DisplayName))]
        [LocalizedDescription(nameof(Resources.FileOrbisFileUpload_Response_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Response { get; set; }

        #endregion


        #region Constructors

        public FileOrbisFileUpload()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Url == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Url)));
            if (DirectoryPath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DirectoryPath)));
            if (Token == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Token)));
            if (RequestBody == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(RequestBody)));
            if (ItemName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ItemName)));
            if (FileFullPath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FileFullPath)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var url = Url.Get(context);
            var directorypath = DirectoryPath.Get(context);
            var token = Token.Get(context);
            var requestbody = RequestBody.Get(context);
            var folderid = FolderId.Get(context);
            var itemrelativepath = ItemRelativePath.Get(context);
            var itemname = ItemName.Get(context);
            var filefullpath = FileFullPath.Get(context);
            var itemid = ItemId.Get(context);
            var overwrite = Overwrite.Get(context);
            var classification = Classification.Get(context);
            var dtype = DType.Get(context);

            var client = new RestClient(url + "/api/v2/filesystem/upload");

            var request = new RestRequest();
            request.Method = Method.Post;
            request.RequestFormat = DataFormat.Json;
            RestResponse response = new RestResponse();
            var content = "";
            var success = "";

            try
            {
                request.AddHeader("Authorization", token.ToString());

                request.AddParameter("folderId", folderid == null ? "00000000-0000-0000-0000-000000000000" : folderid.ToString());

                request.AddParameter("directoryPath", directorypath.ToString());

                request.AddParameter("itemRelativePath", itemrelativepath == null ? "" : itemrelativepath.ToString());

                request.AddParameter("itemName", itemname.ToString());

                request.AddParameter("itemSize", new System.IO.FileInfo(filefullpath.ToString()).Length);

                request.AddParameter("itemId", itemid == null ? "" : itemid.ToString());

                request.AddParameter("overWrite", overwrite ? "true" : overwrite.ToString());

                request.AddParameter("classification", classification == null ? "null" : classification.ToString());

                request.AddFile("file", filefullpath.ToString());

                request.AddParameter("dType", dtype == null ? "0" : dtype.ToString());

                response = client.Execute(request);
                content = response.Content;
                var obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                success = bool.Parse(obj.Success);
                content = response.Content;
                
            }
            catch (Exception ex1)
            {
                Console.WriteLine("File uploading is failed \n" + response.Content);
            }

            // Outputs
            return (ctx) => {
                Success.Set(ctx, success);
                Response.Set(ctx, content);
            };
        }

        #endregion
    }
}

