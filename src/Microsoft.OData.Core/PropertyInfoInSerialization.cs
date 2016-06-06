﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class PropertyInfoInSerialization
    {
        private readonly string propertyName;

        private readonly IEdmStructuredType owningType;

        private readonly IEdmTypeReference propertyTypeReference;

        // FindProperty()
        private readonly IEdmProperty edmProperty;

        // FullName()
        private readonly string fullName;

      //  private readonly string escapedName;

        private readonly bool isUndeclaredProperty;

        private bool isOpenProperty;

        private PropertyTypeInfoInSerialization valueTypeInfo;

        private string typeNameToWrite;

        public PropertyInfoInSerialization(string name, IEdmStructuredType owningType)
        {
            this.propertyName = name;
            this.owningType = owningType;
         //   this.escapedName = EscapeString(propertyName);
            this.edmProperty = owningType == null? null : owningType.FindProperty(propertyName);
            this.isUndeclaredProperty = edmProperty == null;
            this.isOpenProperty = (this.owningType != null && this.owningType.IsOpen && this.isUndeclaredProperty);
            this.propertyTypeReference = this.isUndeclaredProperty ? null : edmProperty.Type;
            this.fullName = this.propertyTypeReference ==null? null : this.propertyTypeReference.Definition.AsActualType().FullTypeName();
        }

        public IEdmProperty EdmProperty
        {
            get { return this.edmProperty; }
        }

        //public string EscapedName
        //{
        //    get { return escapedName; }
        //}

        public bool IsOpenProperty
        {
            get { return isOpenProperty; }
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

        public PropertyTypeInfoInSerialization ValueTypeInfo
        {
            get { return valueTypeInfo; }
            set { valueTypeInfo = value; }
        }

        public string TypeNameToWrite
        {
            get { return typeNameToWrite; }
            set { typeNameToWrite = value; }
        }

        //private static string EscapeString(string inputString)
        //{
        //    StringBuilder outputString = new StringBuilder();

        //    int startIndex = 0;
        //    int inputStringLength = inputString.Length;
        //    int subStrLength;
        //    for (int currentIndex = 0; currentIndex < inputStringLength; currentIndex++)
        //    {
        //        char c = inputString[currentIndex];

        //        // Append the unhandled characters (that do not require special treatment)
        //        // to the string builder when special characters are detected.
        //        if (SpecialCharToEscapedStringMap[c] == null)
        //        {
        //            continue;
        //        }

        //        // Flush out the unescaped characters we've built so far.
        //        subStrLength = currentIndex - startIndex;
        //        if (subStrLength > 0)
        //        {
        //            outputString.Append(inputString.Substring(startIndex, subStrLength));
        //        }

        //        outputString.Append(SpecialCharToEscapedStringMap[c]);
        //        startIndex = currentIndex + 1;
        //    }

        //    subStrLength = inputStringLength - startIndex;
        //    if (subStrLength > 0)
        //    {
        //        outputString.Append(inputString.Substring(startIndex, subStrLength));
        //    }

        //    return outputString.ToString();
        //}
    }
}