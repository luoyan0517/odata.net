//---------------------------------------------------------------------
// <copyright file="PropertyCacheHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
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

        private Dictionary<IEdmStructuredType, PropertyInfoCache> cacheDictionary = new Dictionary<IEdmStructuredType, PropertyInfoCache>();

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

            this.currentProperty = this.propertyInfoCache.GetPropertyInfo(name, identicalName, owningType);
            return this.currentProperty;
        }

        public void SetCurrentResourceScopeLevel(int level)
        {
            this.currentResourceScopeLevel = level;
        }

        public void SetCacheForCurrentResourceSet(IEdmStructuredType resourceType)
        {
            PropertyInfoCache propertyCache;
            if (resourceType != null)
            {
                if (!cacheDictionary.TryGetValue(resourceType, out propertyCache))
                {
                    propertyCache = new PropertyInfoCache();
                    cacheDictionary[resourceType] = propertyCache;
                }
            }
            else
            {
                propertyCache = new PropertyInfoCache();
            }

            this.cacheStack.Push(this.propertyInfoCache);
            propertyInfoCache = propertyCache;
        }

        public void EnterResourceSetScope(int scopeLevel)
        {
            this.scopeLevelStack.Push(this.resourceSetScopeLevel);
            this.resourceSetScopeLevel = scopeLevel;
        }

        public void LeaveResourceSetScope()
        {
            Debug.Assert(this.cacheStack.Count != 0, "this.cacheStack.Count != 0");
            Debug.Assert(this.scopeLevelStack.Count != 0, "this.scopeLevelStack.Count != 0");

            this.resourceSetScopeLevel = this.scopeLevelStack.Pop();
            this.propertyInfoCache = this.cacheStack.Pop();
        }

        public bool InResourceSetScope()
        {
            return this.resourceSetScopeLevel > 0;
        }
    }
}
