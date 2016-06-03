using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyCacheHelper
    {
        private PropertyInfoCache propertyInfoCache;

        private PropertyInfoInSerialization currentProperty;

        public PropertyInfoCache InfoCache
        {
            get { return propertyInfoCache; }
            set { propertyInfoCache = value; }
        }

        public PropertyInfoInSerialization GetCurrentProperty(string name, IEdmStructuredType owningType)
        {
            this.currentProperty = this.propertyInfoCache.GetPropertyInfo(name, owningType);
            return this.currentProperty;
        }

        public PropertyInfoInSerialization GetCurrentProperty()
        {
            return this.currentProperty;
        }

    }
}
