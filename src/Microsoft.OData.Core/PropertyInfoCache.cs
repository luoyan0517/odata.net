using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyInfoCache
    {
        private Dictionary<string, PropertySerializationInfo> propertyInfoDictionary = new Dictionary<string, PropertySerializationInfo>();

        private Dictionary<string, PropertyValueTypeInfo> typeInfoDictionary =
            new Dictionary<string, PropertyValueTypeInfo>();

        public PropertyInfoCache()
        {
        }

        public PropertySerializationInfo GetPropertyInfo(string name, IEdmStructuredType owningType)
        {
            PropertySerializationInfo propertyInfo;
            if (!propertyInfoDictionary.TryGetValue(name, out propertyInfo))
            {
                WriterValidationUtils.ValidatePropertyName(name);
                propertyInfo = new PropertySerializationInfo(name, owningType);
                propertyInfoDictionary[name] = propertyInfo;
            }
            return propertyInfo;
        }

        public bool TryGetTypeInfo(string typeName, out PropertyValueTypeInfo typeInfo)
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

        public PropertyValueTypeInfo SetTypeInfo(string typeName, IEdmTypeReference typeReference)
        {
            PropertyValueTypeInfo typeInfo = new PropertyValueTypeInfo(typeName, typeReference);
            typeInfoDictionary[typeName] = typeInfo;
            return typeInfo;
        }
    }
}
