namespace Inspire.Portal.Models.Provider
{
    using Inspire.Model.Attachment;
    using Inspire.Model.Base;

    public class ProviderPublicViewModel : BaseDbModel
    {
        public string Link { get; set; }

        public Attachment MainPicture { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}