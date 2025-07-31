using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace GoBest.Util
{
    public static class ServiceRegistration
    {
        public static void AddServicesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Repository"));

            foreach (var type in types)
            {
                services.AddScoped(type);
            }
        }
    }
}

