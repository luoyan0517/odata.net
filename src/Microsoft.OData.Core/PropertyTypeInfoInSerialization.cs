using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyValueTypeInfo
    {
        private string typeName;

        private readonly IEdmTypeReference typeReference;

        private readonly string fullName;

        private readonly bool isPrimitive;

        private readonly bool isSpatial;

        private readonly bool isComplex;

        private readonly EdmPrimitiveTypeKind primitiveTypeKind;

        public PropertyValueTypeInfo(string typeName, IEdmTypeReference typeReference)
        {
            this.typeName = typeName;
            this.typeReference = typeReference;
            if (this.typeReference != null)
            {
                this.fullName = typeReference.FullName();
                this.isPrimitive = typeReference.IsPrimitive();
                this.isSpatial = typeReference.IsSpatial();
                this.isComplex = typeReference.IsComplex();
                this.primitiveTypeKind = this.IsPrimitive ? this.typeReference.AsPrimitive().PrimitiveKind() : EdmPrimitiveTypeKind.None;
            }
        }

        public IEdmTypeReference TypeReference
        {
            get { return typeReference; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public bool IsPrimitive
        {
            get { return isPrimitive; }
        }

        public bool IsSpatial
        {
            get { return isSpatial; }
        }

        public bool IsComplex
        {
            get { return isComplex; }
        }

        public string TypeName
        {
            get { return typeName; }
        }

        public EdmPrimitiveTypeKind PrimitiveTypeKind
        {
            get { return primitiveTypeKind; }
        }
    }
}
