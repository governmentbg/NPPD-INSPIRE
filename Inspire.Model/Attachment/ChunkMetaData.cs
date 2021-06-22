namespace Inspire.Model.Attachment
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ChunkMetaData
    {
        [DataMember(Name = "uploadUid")]
        public string UploadUid { get; set; }

        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

        [DataMember(Name = "relativePath")]
        public string RelativePath { get; set; }

        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        [DataMember(Name = "chunkIndex")]
        public long ChunkIndex { get; set; }

        [DataMember(Name = "totalChunks")]
        public long TotalChunks { get; set; }

        [DataMember(Name = "totalFileSize")]
        public long TotalFileSize { get; set; }
    }
}