//---------------------------------------------------------------------
// <copyright file="PropertyInfoCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyInfoCache
    {
        private Dictionary<string, PropertySerializationInfo> propertyInfoDictionary = new Dictionary<string, PropertySerializationInfo>();

        public PropertySerializationInfo GetPropertyInfo(string name, string identicalName, IEdmStructuredType owningType)
        {
            PropertySerializationInfo propertyInfo;
            if (!propertyInfoDictionary.TryGetValue(identicalName, out propertyInfo))
            {
                WriterValidationUtils.ValidatePropertyName(name);

                propertyInfo = new PropertySerializationInfo(name, owningType);
                propertyInfoDictionary[identicalName] = propertyInfo;
            }

            return propertyInfo;
        }
    }
}
