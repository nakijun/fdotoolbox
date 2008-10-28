using System;
using System.Collections.Generic;

using OSGeo.FDO.Expression;
using OSGeo.FDO.Commands;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Represents a single feature
    /// </summary>
    public class FdoFeature : ICloneable, IDisposable
    {
        private Dictionary<string, LiteralValue> _Values;

        public FdoFeature() { _Values = new Dictionary<string, LiteralValue>(); }

        public void Clear() { _Values.Clear(); }

        public ICollection<string> PropertyNames
        {
            get { return _Values.Keys; }
        }

        public LiteralValue this[string name]
        {
            get
            {
                LiteralValue expr;
                if (_Values.TryGetValue(name, out expr))
                    return expr;
                return null;
            }
            set
            {
                _Values[name] = value;
            }
        }

        public void Rename(string propertyName, string newPropertyName)
        {
            LiteralValue expr = _Values[propertyName];
            _Values.Remove(propertyName);
            _Values[newPropertyName] = expr;
        }

        /// <summary>
        /// Removes a property value
        /// </summary>
        /// <param name="name">The name of the property</param>
        public void Remove(string name)
        {
            _Values.Remove(name);
        }

        /// <summary>
        /// Gets this feature as a collection of property values
        /// </summary>
        /// <returns></returns>
        public PropertyValueCollection ToValueCollection()
        {
            PropertyValueCollection values = new PropertyValueCollection();
            foreach (string name in _Values.Keys)
            {
                if (_Values[name] != null)
                {
                    values.Add(new PropertyValue(name, _Values[name]));
                }
            }
            return values;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (string key in _Values.Keys)
            {
                _Values[key].Dispose();
                _Values.Remove(key);
            }
        }
    }
}
