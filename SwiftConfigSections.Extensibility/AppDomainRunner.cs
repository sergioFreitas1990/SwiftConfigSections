using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace SwiftConfigSections.Extensibility
{
    public static class AppDomainRunner
    {
        public static TResult Run<TArgument, TResult>(string dllFileFullName,
            TArgument argument, Func<Assembly, TArgument, TResult> codeToRun)
             where TResult : class
        {
            using (var appDomainRunner = new AppDomainRunner<TArgument, TResult>(dllFileFullName))
            {
                return appDomainRunner.Run(argument, codeToRun);
            }
        }
    }

    /// <summary>
    /// A handle that loads and unloads an assembly from a directory.
    /// </summary>
    public class AppDomainRunner<TArgument, TResult> : IDisposable
         where TResult : class
    {
        private readonly string _dllFileFullName;
        private readonly AppDomain _appDomain;

        /// <summary>
        /// Loads an assembly calling the LoadFrom method.
        /// </summary>
        /// <param name="fileName">Unloads the assembly.</param>
        public AppDomainRunner(string dllFileFullName)
        {
            var domainId = Guid.NewGuid().ToString();
            var assemblyLocation = new FileInfo(GetType().Assembly.Location).Directory.FullName;

            _dllFileFullName = dllFileFullName;            
            _appDomain = AppDomain.CreateDomain(domainId,
                AppDomain.CurrentDomain.Evidence,
                new AppDomainSetup
                {
                    ApplicationBase = assemblyLocation
                },
                new PermissionSet(PermissionState.Unrestricted));
        }

        public TResult Run(TArgument argument, Func<Assembly, TArgument, TResult> codeToRun)
        {
            var assemblyContainer = new AssemblyContainer<TArgument, TResult>()
            {
                Container = _appDomain,
                DllFileFullName = _dllFileFullName,
                Arguments = argument,
                CodeToRun = codeToRun
            };

            // collect data from the other app domain
            _appDomain.DoCallBack(new CrossAppDomainDelegate(assemblyContainer.RunCode));
            var result = assemblyContainer.GetResult(_appDomain);
            
            // Close the domain            
            return (TResult) result;
        }

        public void Dispose()
        {
            AppDomain.Unload(_appDomain);
        }
    }

    [Serializable]
    public class AssemblyContainer<TArgument, TResult>
         where TResult : class
    {
        private const string DataKey = "TypeValueKey";

        public string DllFileFullName { get; set; }
        public AppDomain Container { get; set; }
        public TArgument Arguments { get; set; }
        public Func<Assembly, TArgument, TResult> CodeToRun { get; set; }

        /// <summary>
        /// Load the assembly
        /// </summary>
        /// <remarks>This will be executed</remarks>
        public void RunCode()
        {
            var assembly = Assembly.LoadFrom(DllFileFullName);
            var result = CodeToRun(assembly, Arguments);

            // set data to pick up from the main app domain
            Container.SetData(DataKey, SerializeToByteArray(result));
        }

        public object GetResult(AppDomain domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }
            var objResult = (byte[]) domain.GetData(DataKey);
            return Deserialize<TResult>(objResult);
        }

        public static byte[] SerializeToByteArray<T>(T obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }
            using (var ms = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream(byteArray))
            {
                var serializer = new XmlSerializer(typeof(T));
                var obj = (T)serializer.Deserialize(memStream);
                return obj;
            }
        }
    }
}
