using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;

namespace SwiftConfigSections.Utils
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

            var service = serviceProvider.GetService(typeof(TServiceType));
            if (service == null)
            {
                return default(TServiceInterface);
            }
            return (TServiceInterface) service;
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

        public static ITextTemplating GetT4(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return serviceProvider.GetService<STextTemplating, ITextTemplating>();
        }
    }
}
