using System.Collections.Generic;

namespace Crumbs.Core.Configuration
{
    public interface IFrameworkConfiguration
    {
        IReadOnlyDictionary<string, object> Values { get; }
    }
}