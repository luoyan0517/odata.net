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

        private int previousResourceSetScopeLevel;

        private int resourceSetScopeLevel;

        private int currentResourceScopeLevel;


        public PropertyInfoCache InfoCache
        {
            get { return propertyInfoCache; }
            set { propertyInfoCache = value; }
        }

        public int ResourceSetScopeLevel
        {
            get { return this.resourceSetScopeLevel; }
            set
            {
                previousResourceSetScopeLevel = this.resourceSetScopeLevel;
                this.resourceSetScopeLevel = value;
            }
        }

        public int CurrentResourceScopeLevel
        {
            set { currentResourceScopeLevel = value; }
        }

        public PropertyInfoInSerialization GetCurrentProperty(string name, IEdmStructuredType owningType)
        {
            string identicalName;
            if (this.currentResourceScopeLevel == this.resourceSetScopeLevel + 1)
            {
                identicalName = name;
            }
            else
            {
                identicalName = name + (this.currentResourceScopeLevel - this.resourceSetScopeLevel);
            }
            this.currentProperty = this.propertyInfoCache.GetPropertyInfo(identicalName, owningType);
            return this.currentProperty;
        }

        public PropertyInfoInSerialization GetCurrentProperty()
        {
            return this.currentProperty;
        }

        public void LeaveResourceSetScope()
        {
            this.resourceSetScopeLevel = previousResourceSetScopeLevel;
        }
    }
}
