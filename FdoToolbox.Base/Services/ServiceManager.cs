using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;

namespace FdoToolbox.Base.Services
{
    public class ServiceManager
    {
        private static ServiceManager instance = null;

        private List<IService> _services = new List<IService>();
        private Dictionary<Type, IService> _serviceDict = new Dictionary<Type, IService>();

        public static ServiceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceManager();
                }
                return instance;
            }
        }

        private ServiceManager() { InitializeServicesSubsystem("/FdoToolbox/Services"); }

        internal void InitializeServicesSubsystem(string servicePath)
        {
            List<IService> services = AddInTree.BuildItems<IService>(servicePath, null, false);
            if (services != null && services.Count > 0)
                AddServices(services);

            foreach (IService service in _services)
            {
                if(!service.IsInitialized)
                    service.InitializeService();
            }
        }

        internal void UnloadAllServices()
        {
            foreach (IService service in _services)
            {
                service.UnloadService();
            }
        }

        protected void AddService(IService service)
        {
            _services.Add(service);
        }

        protected void AddServices(IEnumerable<IService> services)
        {
            _services.AddRange(services);
        }

        /// <summary>
        /// Requests a specific service. May return null if service is not found
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public T GetService<T>() where T : class, IService
        {
            Type serviceType = typeof(T);
            if (_serviceDict.ContainsKey(serviceType))
                return _serviceDict[serviceType] as T;

            foreach (IService service in _services)
            {
                if (serviceType.IsInstanceOfType(service))
                {
                    _serviceDict[serviceType] = service;
                    return service as T;
                }
            }
            return null;
        }
    }
}
