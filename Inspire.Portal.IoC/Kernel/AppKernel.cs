namespace Inspire.Portal.IoC.Kernel
{
    using Ninject;

    public class AppKernel
    {
        private static IKernel container;

        public static IKernel Container => container ?? (container = new StandardKernel());

        public static T Resolve<T>()
        {
            return Container.Get<T>();
        }
    }
}