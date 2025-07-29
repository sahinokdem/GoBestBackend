using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace GoBest.Util
{
    public static class ServiceRegistration
    {
        public static void AddServicesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Service") && t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                services.AddScoped(type);
            }
        }
    }
}

