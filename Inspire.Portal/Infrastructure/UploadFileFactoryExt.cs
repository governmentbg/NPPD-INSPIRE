namespace Inspire.Portal.Infrastructure
{
    using System;
    using System.Reflection;

    using Kendo.Mvc.UI;
    using Kendo.Mvc.UI.Fluent;

    public static class UploadFileFactoryExt
    {
        public static UploadFileBuilder AddCustomFile<T>(this UploadFileFactory uploadFileFactory, T file)
            where T : UploadFile
        {
            if (!(uploadFileFactory.GetType().GetField("container", BindingFlags.Instance | BindingFlags.NonPublic)
                                   ?.GetValue(uploadFileFactory) is Upload container))
            {
                throw new ArgumentNullException();
            }

            container.Files.Add(file);
            return new UploadFileBuilder(file);
        }
    }
}