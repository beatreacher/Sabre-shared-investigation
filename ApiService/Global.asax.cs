using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.WebApi;
using SabreApiClient;
using SabreApiClient.Interfaces;
using System.Reflection;
using System.Web.Http;

namespace ApiService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;

            //var config = new HttpSelfHostConfiguration("http://localhost:8080");//For selfhosting

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //builder.RegisterType<ValuesController>().InstancePerRequest();

            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();

            builder.RegisterModule<NLogModule>();
            builder.RegisterType<SabreApi>().As<ISabreApi>().SingleInstance();
            builder.RegisterType<SessionManager>().As<ISessionManager>().SingleInstance();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
