using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData
{
    internal class PropertyInfoCache
    {
        private Dictionary<string, PropertyInfoInSerialization> propertyInfoDictionary = new Dictionary<string, PropertyInfoInSerialization>();

        private Dictionary<string, PropertyTypeInfoInSerialization> typeInfoDictionary =
            new Dictionary<string, PropertyTypeInfoInSerialization>();

        public PropertyInfoInSerialization GetPropertyInfo(string name, IEdmStructuredType owningType)
        {
            PropertyInfoInSerialization propertyInfo;
            if (!propertyInfoDictionary.TryGetValue(name, out propertyInfo))
            {
                propertyInfo = new PropertyInfoInSerialization(name, owningType);
                propertyInfoDictionary[name] = propertyInfo;
            }
            return propertyInfo;
        }

        public bool TryGetTypeInfo(string typeName, out PropertyTypeInfoInSerialization typeInfo)
        {
            if (typeInfoDictionary.TryGetValue(typeName, out typeInfo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PropertyTypeInfoInSerialization SetTypeInfo(string typeName, IEdmTypeReference typeReference)
        {
            PropertyTypeInfoInSerialization typeInfo = new PropertyTypeInfoInSerialization(typeName, typeReference);
            typeInfoDictionary[typeName] = typeInfo;
            return typeInfo;
        }
    }
}
