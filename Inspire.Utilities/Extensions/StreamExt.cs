namespace Inspire.Utilities.Extensions
{
    using System.IO;

    public static class StreamExt
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream == null || stream == Stream.Null)
            {
                return null;
            }

            using (stream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}