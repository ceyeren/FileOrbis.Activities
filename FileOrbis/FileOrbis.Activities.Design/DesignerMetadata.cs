using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using FileOrbis.Activities.Design.Designers;
using FileOrbis.Activities.Design.Properties;

namespace FileOrbis.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(FileOrbisLogin), categoryAttribute);
            builder.AddCustomAttributes(typeof(FileOrbisLogin), new DesignerAttribute(typeof(FileOrbisLoginDesigner)));
            builder.AddCustomAttributes(typeof(FileOrbisLogin), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(FileOrbisLink), categoryAttribute);
            builder.AddCustomAttributes(typeof(FileOrbisLink), new DesignerAttribute(typeof(FileOrbisLinkDesigner)));
            builder.AddCustomAttributes(typeof(FileOrbisLink), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(FileOrbisFileUpload), categoryAttribute);
            builder.AddCustomAttributes(typeof(FileOrbisFileUpload), new DesignerAttribute(typeof(FileOrbisFileUploadDesigner)));
            builder.AddCustomAttributes(typeof(FileOrbisFileUpload), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
