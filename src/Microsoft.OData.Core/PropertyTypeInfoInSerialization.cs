using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyTypeInfoInSerialization
    {
        private string typeName;

        private readonly IEdmTypeReference typeReference;

        // FullName()
        private readonly string fullName;

        private readonly bool isPrimitive;

        private readonly bool isSpatial;

        private readonly bool isComplex;

        private readonly EdmPrimitiveTypeKind primitiveTypeKind;

        public PropertyTypeInfoInSerialization(string typeName, IEdmTypeReference typeReference)
        {
            this.typeName = typeName;
            this.typeReference = typeReference;
            this.fullName = typeReference.FullName();
            this.isPrimitive = typeReference.IsPrimitive();
            this.isSpatial = typeReference.IsSpatial();
            this.isComplex = typeReference.IsComplex();
            this.primitiveTypeKind = this.IsPrimitive ? this.typeReference.AsPrimitive().PrimitiveKind() : EdmPrimitiveTypeKind.None;
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
