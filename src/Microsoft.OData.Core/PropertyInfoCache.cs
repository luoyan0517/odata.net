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
        private Dictionary<string, PropertySerializationInfo> propertyInfoDictionary = new Dictionary<string, PropertySerializationInfo>();

        private Dictionary<string, PropertyValueTypeInfo> typeInfoDictionary =
            new Dictionary<string, PropertyValueTypeInfo>();

        public PropertySerializationInfo GetPropertyInfo(string name, IEdmStructuredType owningType)
        {
            PropertySerializationInfo propertyInfo;
            if (!propertyInfoDictionary.TryGetValue(name, out propertyInfo))
            {
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
