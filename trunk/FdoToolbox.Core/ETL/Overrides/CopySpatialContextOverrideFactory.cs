using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Core.ETL.Overrides
{
    /// <summary>
    /// Factory class to create <see cref="ICopySpatialContext"/> commands
    /// </summary>
    public static class CopySpatialContextOverrideFactory
    {
        private static Dictionary<string, Type> _CopySpatialContextOverrides = new Dictionary<string, Type>();

        /// <summary>
        /// Registers an override class to copy spatial contexts
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="overrideType"></param>
        public static void RegisterCopySpatialContextOverride(string providerName, Type overrideType)
        {
            if (!Array.Exists<Type>(overrideType.GetInterfaces(), delegate(Type t) { return t == typeof(ICopySpatialContext); }))
                throw new ArgumentException("The given type does not implement ICopySpatialContextOverride");

            _CopySpatialContextOverrides[providerName] = overrideType;
        }

        /// <summary>
        /// Gets the registered override object
        /// </summary>
        /// <param name="targetConn"></param>
        /// <returns></returns>
        public static ICopySpatialContext GetCopySpatialContextOverride(FdoConnection targetConn)
        {
            string providerName = targetConn.Provider;
            if (_CopySpatialContextOverrides.ContainsKey(providerName))
            {
                return (ICopySpatialContext)Activator.CreateInstance(_CopySpatialContextOverrides[providerName]);
            }
            return new CopySpatialContext();
        }

        /// <summary>
        /// Initialize with the default overrides
        /// </summary>
        static CopySpatialContextOverrideFactory()
        {
            RegisterCopySpatialContextOverride("OSGeo.MySQL", typeof(MySqlCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SHP", typeof(ShpCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SQLServerSpatial", typeof(MsSqlCopySpatialContextOverride));

            RegisterCopySpatialContextOverride("OSGeo.MySQL.3.4", typeof(MySqlCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SHP.3.4", typeof(ShpCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SQLServerSpatial.3.4", typeof(MsSqlCopySpatialContextOverride));
        }
    }
}
