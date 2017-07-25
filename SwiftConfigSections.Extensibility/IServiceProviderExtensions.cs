using System;

namespace SwiftConfigSections.Extensibility
{
    public static class IServiceProviderExtensions
    {
        public static TServiceInterface GetService<TServiceType, TServiceInterface>(
            this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return (TServiceInterface)serviceProvider.GetService(typeof(TServiceType));
        }

        public static TServiceInterface GetService<TServiceInterface>(
            this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return serviceProvider.GetService<TServiceInterface, TServiceInterface>();
        }
    }
}
