using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertySerializationInfo
    {
        private readonly string propertyName;

        private readonly IEdmStructuredType owningType;

        private readonly IEdmTypeReference propertyTypeReference;

        private readonly IEdmProperty edmProperty;

        private readonly string fullName;

        private readonly bool isUndeclaredProperty;

        private bool isOpenPropertyInModel;

        private PropertyValueTypeInfo valueTypeInfo;

        private string typeNameToWrite;

        public PropertySerializationInfo(string name, IEdmStructuredType owningType)
        {
            this.propertyName = name;
            this.owningType = owningType;
            this.edmProperty = owningType == null? null : owningType.FindProperty(propertyName);
            this.isUndeclaredProperty = edmProperty == null;
            this.isOpenPropertyInModel = (this.owningType != null && this.owningType.IsOpen && this.isUndeclaredProperty);
            this.propertyTypeReference = this.isUndeclaredProperty ? null : edmProperty.Type;
            this.fullName = this.propertyTypeReference ==null? null : this.propertyTypeReference.Definition.AsActualType().FullTypeName();
        }

        public IEdmProperty EdmProperty
        {
            get { return this.edmProperty; }
        }

        public bool IsOpenPropertyInModel
        {
            get { return isOpenPropertyInModel; }
        }

        public bool IsUndeclaredProperty
        {
            get { return isUndeclaredProperty; }
        }

        public string PropertyName
        {
            get { return propertyName; }
        }

        public IEdmStructuredType OwningType
        {
            get { return owningType; }
        }

        public IEdmTypeReference PropertyTypeReference
        {
            get { return propertyTypeReference; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public PropertyValueTypeInfo ValueTypeInfo
        {
            get { return valueTypeInfo; }
            set { valueTypeInfo = value; }
        }

        public string TypeNameToWrite
        {
            get { return typeNameToWrite; }
            set { typeNameToWrite = value; }
        }
    }
}
