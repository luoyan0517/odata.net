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

            this.currentProperty = this.propertyInfoCache.GetPropertyInfo(name, identicalName, owningType);
            return this.currentProperty;
        }

        public void SetCacheForCurrentResourceSet()
        {
            PropertyInfoCache newCache = new PropertyInfoCache();
            this.cacheStack.Push(this.propertyInfoCache);
            propertyInfoCache = newCache;
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
