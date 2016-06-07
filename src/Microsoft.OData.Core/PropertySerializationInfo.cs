//---------------------------------------------------------------------
// <copyright file="PropertySerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;

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

        private PropertyValueType valueType;

        private string typeNameToWrite;

        private bool isTopLevel = false;

        private string wireName;

        public PropertySerializationInfo(string name, IEdmStructuredType owningType)
        {
            this.propertyName = name;
            this.owningType = owningType;
            this.edmProperty = owningType == null ? null : owningType.FindProperty(propertyName);
            this.isUndeclaredProperty = edmProperty == null;
            this.isOpenPropertyInModel = (this.owningType != null && this.owningType.IsOpen && this.isUndeclaredProperty);
            this.propertyTypeReference = this.isUndeclaredProperty ? null : edmProperty.Type;
            this.fullName = this.propertyTypeReference == null ? null : this.propertyTypeReference.Definition.AsActualType().FullTypeName();
            this.wireName = isTopLevel ? JsonLightConstants.ODataValuePropertyName : propertyName;
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

        public IEdmTypeReference TypeReference
        {
            get { return propertyTypeReference; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public PropertyValueType ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        public string TypeNameToWrite
        {
            get { return typeNameToWrite; }
            set { typeNameToWrite = value; }
        }

        public bool IsTopLevel
        {
            get
            {
                return isTopLevel;
            }

            set
            {
                isTopLevel = value;
                this.wireName = isTopLevel ? JsonLightConstants.ODataValuePropertyName : propertyName;
            }
        }

        public string WireName
        {
            get { return this.wireName; }
        }
    }
}
