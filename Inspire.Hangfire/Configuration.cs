namespace Inspire.Hangfire
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Configuration;

    using global::Hangfire;
    using global::Hangfire.Dashboard;
    using global::Hangfire.PostgreSql;

    using Ninject;

    using Owin;

    public class Configuration
    {
        private static readonly bool UseHangfireDashboard =
            WebConfigurationManager.AppSettings["UseHangfireDashboard"] != null
            && Convert.ToBoolean(WebConfigurationManager.AppSettings["UseHangfireDashboard"]);

        private static readonly bool UseHangfireServer =
            WebConfigurationManager.AppSettings["UseHangfireServer"] != null
            && Convert.ToBoolean(WebConfigurationManager.AppSettings["UseHangfireServer"]);

        private static readonly int WorkerCount = WebConfigurationManager.AppSettings["HangfireWorkerCount"] != null
                                                      ? Convert.ToInt32(WebConfigurationManager.AppSettings["HangfireWorkerCount"])
                                                      : 1;

        private static readonly JobStorage storage = new PostgreSqlStorage("HangfireConnection");

        public static string AddBackgroundJob(Expression<Action> action)
        {
            return BackgroundJob.Enqueue(action);
        }

        public static void ConfigureHangfireDashboard(IAppBuilder app)
        {
            if (UseHangfireDashboard)
            {
                var options = new DashboardOptions
                                  {
                                      AppPath = VirtualPathUtility.ToAbsolute("~")
                                  };

                app.UseHangfireDashboard("/hangfirejobs", options, storage);
            }
        }

        public static BackgroundJobServer CreateBackgroundJobServer(IKernel kernel = null)
        {
            GlobalConfiguration.Configuration
                               .UseStorage(storage)
                               .UseNLogLogProvider()
                               .UseNinjectActivator(kernel)
                               .UseDashboardMetric(DashboardMetrics.EnqueuedCountOrNull)
                               .UseDashboardMetric(DashboardMetrics.ScheduledCount)
                               .UseDashboardMetric(DashboardMetrics.ProcessingCount)
                               .UseDashboardMetric(DashboardMetrics.SucceededCount)
                               .UseDashboardMetric(DashboardMetrics.FailedCount)
                               .UseDashboardMetric(DashboardMetrics.DeletedCount);

            if (UseHangfireServer)
            {
                // configure options for the server
                var options = new BackgroundJobServerOptions
                                  {
                                      WorkerCount = WorkerCount
                                  };

                // setup server usage with configured options
                return new BackgroundJobServer(options, storage);
            }

            return null;
        }
    }
}