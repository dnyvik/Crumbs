using System.Collections.Generic;

namespace Crumbs.Core.Configuration
{
    public class FrameworkConfigurationValues : IFrameworkConfiguration
    {
        private Dictionary<string, object> _internalValues = new Dictionary<string, object>();

        public IReadOnlyDictionary<string, object> Values => _internalValues;

        public void AddValue(string key, object value)
        {
            _internalValues.Add(key, value);
        }
    }
}
