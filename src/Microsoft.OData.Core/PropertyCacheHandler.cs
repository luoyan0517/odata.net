using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyCacheHandler
    {
        private PropertyInfoCache propertyInfoCache;

        private PropertySerializationInfo currentProperty;

        private int resourceSetScopeLevel;

        private int currentResourceScopeLevel;

        private Stack<PropertyInfoCache> cacheStack = new Stack<PropertyInfoCache>();

        private Stack<int> scopeLevelStack = new Stack<int>();

        public PropertyInfoCache InfoCache
        {
            get { return propertyInfoCache; }
            set
            {
                this.cacheStack.Push(this.propertyInfoCache);
                propertyInfoCache = value;
            }
        }

        public int ResourceSetScopeLevel
        {
            get { return this.resourceSetScopeLevel; }
            set
            {
                this.scopeLevelStack.Push(this.resourceSetScopeLevel);
                this.resourceSetScopeLevel = value;
            }
        }

        public int CurrentResourceScopeLevel
        {
            set { currentResourceScopeLevel = value; }
        }

        public PropertySerializationInfo GetProperty(string name, IEdmStructuredType owningType)
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

        public PropertySerializationInfo GetCurrentProperty()
        {
            return this.currentProperty;
        }

        public void LeaveResourceSetScope()
        {
            this.resourceSetScopeLevel = this.scopeLevelStack.Pop();
            this.propertyInfoCache = this.cacheStack.Pop();
        }
    }
}
