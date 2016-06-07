//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPropertySerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Globalization;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for properties.
    /// </summary>
    internal class ODataJsonLightPropertySerializer : ODataJsonLightSerializer
    {
        /// <summary>
        /// Serializer to use to write property values.
        /// </summary>
        private readonly ODataJsonLightValueSerializer jsonLightValueSerializer;

        /// <summary>
        /// Serialization info for current property.
        /// </summary>
        private PropertySerializationInfo currentProperty;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="initContextUriBuilder">Whether contextUriBuilder should be initialized.</param>
        internal ODataJsonLightPropertySerializer(ODataJsonLightOutputContext jsonLightOutputContext, bool initContextUriBuilder = false)
            : base(jsonLightOutputContext, initContextUriBuilder)
        {
            this.jsonLightValueSerializer = new ODataJsonLightValueSerializer(this, initContextUriBuilder);
        }

        /// <summary>
        /// Gets the json light value writer.
        /// </summary>
        internal ODataJsonLightValueSerializer JsonLightValueSerializer
        {
            get
            {
                return this.jsonLightValueSerializer;
            }
        }

        /// <summary>
        /// Write an <see cref="ODataProperty" /> to the given stream. This method creates an
        /// async buffered stream and writes the property to it.
        /// </summary>
        /// <param name="property">The property to write.</param>
        internal void WriteTopLevelProperty(ODataProperty property)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(!(property.Value is ODataStreamReferenceValue), "!(property.Value is ODataStreamReferenceValue)");

            this.WriteTopLevelPayload(
                () =>
                {
                    this.JsonWriter.StartObjectScope();
                    ODataPayloadKind kind = this.JsonLightOutputContext.MessageWriterSettings.IsIndividualProperty ? ODataPayloadKind.IndividualProperty : ODataPayloadKind.Property;

                    ODataContextUrlInfo contextInfo = ODataContextUrlInfo.Create(property.ODataValue, this.JsonLightOutputContext.MessageWriterSettings.ODataUri, this.Model);
                    this.WriteContextUriProperty(kind, () => contextInfo);

                    // Note we do not allow named stream properties to be written as top level property.
                    this.JsonLightValueSerializer.AssertRecursionDepthIsZero();
                    this.WriteProperty(
                        property,
                        null /*owningType*/,
                        true /* isTopLevel */,
                        false /* allowStreamProperty */,
                        this.CreateDuplicatePropertyNamesChecker(),
                        null /* projectedProperties */);
                    this.JsonLightValueSerializer.AssertRecursionDepthIsZero();

                    this.JsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes property names and value pairs.
        /// </summary>
        /// <param name="owningType">The <see cref="IEdmStructuredType"/> of the resource (or null if not metadata is available).</param>
        /// <param name="properties">The enumeration of properties to write out.</param>
        /// <param name="isComplexValue">
        /// Whether the properties are being written for complex value. Also used for detecting whether stream properties
        /// are allowed as named stream properties should only be defined on ODataResource instances
        /// </param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        internal void WriteProperties(
            IEdmStructuredType owningType,
            IEnumerable<ODataProperty> properties,
            bool isComplexValue,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            if (properties == null)
            {
                return;
            }

            foreach (ODataProperty property in properties)
            {
                this.WriteProperty(
                    property,
                    owningType,
                    false /* isTopLevel */,
                    !isComplexValue,
                    duplicatePropertyNamesChecker,
                    projectedProperties);
            }
        }

        /// <summary>
        /// Test to see if <paramref name="property"/> is an open property or not.
        /// </summary>
        /// <param name="property">The property in question.</param>
        /// <returns>true if the property is an open property; false if it is not, or if openness cannot be determined</returns>
        private bool IsOpenProperty(ODataProperty property)
        {
            Debug.Assert(property != null, "property != null");

            bool isOpenProperty;

            if (property.SerializationInfo != null)
            {
                isOpenProperty = property.SerializationInfo.PropertyKind == ODataPropertyKind.Open;
            }
            else
            {
                isOpenProperty = (!this.WritingResponse && this.currentProperty.OwningType == null) // Treat property as dynamic property when writing request and owning type is null
                || this.currentProperty.IsOpenPropertyInModel;
            }

            if (isOpenProperty)
            {
                this.WriterValidator.ValidateOpenPropertyValue(property.Name, property.ODataValue);
            }

            return isOpenProperty;
        }

        /// <summary>
        /// Writes a name/value pair for a property.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        /// <param name="owningType">The owning type for the <paramref name="property"/> or null if no metadata is available.</param>
        /// <param name="isTopLevel">true when writing a top-level property; false for nested properties.</param>
        /// <param name="allowStreamProperty">Should pass in true if we are writing a property of an ODataResource instance, false otherwise.
        /// Named stream properties should only be defined on ODataResource instances.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Splitting the code would make the logic harder to understand; class coupling is only slightly above threshold.")]
        private void WriteProperty(
            ODataProperty property,
            IEdmStructuredType owningType,
            bool isTopLevel,
            bool allowStreamProperty,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            WriterValidationUtils.ValidatePropertyNotNull(property);

            string propertyName = property.Name;

            if (!this.JsonLightOutputContext.PropertyCacheHandler.InResourceSetScope())
            {
                WriterValidationUtils.ValidatePropertyName(propertyName);
                this.currentProperty = new PropertySerializationInfo(propertyName, owningType) { IsTopLevel = isTopLevel };
            }
            else
            {
                this.currentProperty = this.JsonLightOutputContext.PropertyCacheHandler.GetProperty(propertyName, owningType);
            }

            WriterValidationUtils.ValidatePropertyDefined(this.currentProperty, this.MessageWriterSettings.ThrowOnUndeclaredProperty);

            if (projectedProperties.ShouldSkipProperty(propertyName))
            {
                return;
            }

            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);

            WriteInstanceAnnotation(property, isTopLevel, currentProperty.IsUndeclaredProperty);

            ODataValue value = property.ODataValue;

            // handle ODataUntypedValue
            ODataUntypedValue untypedValue = value as ODataUntypedValue;
            if (untypedValue != null)
            {
                WriteUntypedValue(untypedValue);
                return;
            }

            ODataStreamReferenceValue streamReferenceValue = value as ODataStreamReferenceValue;
            if (streamReferenceValue != null)
            {
                if (!allowStreamProperty)
                {
                    throw new ODataException(ODataErrorStrings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(propertyName));
                }

                Debug.Assert(owningType == null || owningType.IsODataEntityTypeKind(), "The metadata should not allow named stream properties to be defined on a non-entity type.");
                Debug.Assert(!isTopLevel, "Stream properties are not allowed at the top level.");
                WriterValidationUtils.ValidateStreamReferenceProperty(property, currentProperty.EdmProperty, this.WritingResponse);
                this.WriteStreamReferenceProperty(propertyName, streamReferenceValue);
                return;
            }

            if (value is ODataNullValue || value == null)
            {
                this.WriteNullProperty(property);
                return;
            }

            bool isOpenPropertyType = this.IsOpenProperty(property);

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                this.WritePrimitiveProperty(primitiveValue, isOpenPropertyType);
                return;
            }

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                this.WriteComplexProperty(complexValue, isOpenPropertyType);
                return;
            }

            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                this.WriteEnumProperty(enumValue, isOpenPropertyType);
                return;
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                this.WriteCollectionProperty(collectionValue, isOpenPropertyType);
                return;
            }
        }

        private void WriteUntypedValue(ODataUntypedValue untypedValue)
        {
            if (!this.MessageWriterSettings.ThrowOnUndeclaredProperty)
            {
                this.JsonWriter.WriteName(this.currentProperty.WireName);
                this.jsonLightValueSerializer.WriteUntypedValue(untypedValue);
                return;
            }

            Debug.Assert(
                this.MessageWriterSettings.ThrowOnUndeclaredProperty,
                "this.MessageWriterSettings.ThrowOnUndeclaredProperty");
            throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(this.currentProperty.PropertyName, this.currentProperty.OwningType.FullTypeName()));
        }

        /// <summary>
        /// Writes instance annotation for property
        /// </summary>
        /// <param name="property">The property to handle.</param>
        /// <param name="isTopLevel">If writing top level property.</param>
        /// <param name="isUndeclaredProperty">If writing an undeclared property.</param>
        private void WriteInstanceAnnotation(ODataProperty property, bool isTopLevel, bool isUndeclaredProperty)
        {
            if (property.InstanceAnnotations.Count != 0)
            {
                if (isTopLevel)
                {
                    this.InstanceAnnotationWriter.WriteInstanceAnnotations(property.InstanceAnnotations);
                }
                else
                {
                    this.InstanceAnnotationWriter.WriteInstanceAnnotations(property.InstanceAnnotations, property.Name, isUndeclaredProperty);
                }
            }
        }

        /// <summary>
        /// Writes a stream property.
        /// </summary>
        /// <param name="propertyName">The name of the property to write.</param>
        /// <param name="streamReferenceValue">The stream reference value to be written</param>
        private void WriteStreamReferenceProperty(string propertyName, ODataStreamReferenceValue streamReferenceValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            Uri mediaEditLink = streamReferenceValue.EditLink;
            if (mediaEditLink != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaEditLink);
                this.JsonWriter.WriteValue(this.UriToString(mediaEditLink));
            }

            Uri mediaReadLink = streamReferenceValue.ReadLink;
            if (mediaReadLink != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaReadLink);
                this.JsonWriter.WriteValue(this.UriToString(mediaReadLink));
            }

            string mediaContentType = streamReferenceValue.ContentType;
            if (mediaContentType != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaContentType);
                this.JsonWriter.WriteValue(mediaContentType);
            }

            string mediaETag = streamReferenceValue.ETag;
            if (mediaETag != null)
            {
                this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataMediaETag);
                this.JsonWriter.WriteValue(mediaETag);
            }
        }

        /// <summary>
        /// Writes a Null property.
        /// </summary>
        /// <param name="property">The property to write out.</param>
        private void WriteNullProperty(
            ODataProperty property)
        {
            this.WriterValidator.ValidateNullPropertyValue(
                this.currentProperty.TypeReference, property.Name, this.Model);

            if (this.currentProperty.IsTopLevel)
            {
                // Write the special null marker for top-level null properties.
                this.ODataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNull);
                this.JsonWriter.WriteValue(true);
            }
            else
            {
                this.JsonWriter.WriteName(property.Name);
                this.JsonLightValueSerializer.WriteNullValue();
            }
        }

        /// <summary>
        /// Writes a complex property.
        /// </summary>
        /// <param name="complexValue">The complex value to be written</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WriteComplexProperty(
            ODataComplexValue complexValue,
            bool isOpenPropertyType)
        {
            if (!this.currentProperty.IsTopLevel)
            {
                this.JsonWriter.WriteName(this.currentProperty.PropertyName);
            }

            this.JsonLightValueSerializer.WriteComplexValue(complexValue, this.currentProperty.TypeReference, this.currentProperty.IsTopLevel, isOpenPropertyType, this.CreateDuplicatePropertyNamesChecker());
        }

        /// <summary>
        /// Writes a enum property.
        /// </summary>
        /// <param name="enumValue">The enum value to be written.</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WriteEnumProperty(
            ODataEnumValue enumValue,
            bool isOpenPropertyType)
        {
            ResolveEnumValueTypeName(enumValue, isOpenPropertyType);

            this.WritePropertyTypeName();
            this.JsonWriter.WriteName(this.currentProperty.WireName);
            this.JsonLightValueSerializer.WriteEnumValue(enumValue, this.currentProperty.TypeReference);
        }

        private void ResolveEnumValueTypeName(ODataEnumValue enumValue, bool isOpenPropertyType)
        {
            if (this.currentProperty.ValueType == null || this.currentProperty.ValueType.TypeName != enumValue.TypeName)
            {
                IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeForEnumValue(
                    this.Model,
                    enumValue,
                    isOpenPropertyType);

                // This is a work around, needTypeOnWire always = true for client side:
                // ClientEdmModel's reflection can't know a property is open type even if it is, so here
                // make client side always write 'odata.type' for enum.
                bool needTypeOnWire = string.Equals(this.JsonLightOutputContext.Model.GetType().Name, "ClientEdmModel",
                    StringComparison.OrdinalIgnoreCase);
                string typeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(
                    enumValue, this.currentProperty.TypeReference, typeFromValue, needTypeOnWire || isOpenPropertyType);

                this.currentProperty.ValueType = new PropertyValueType(enumValue.TypeName, typeFromValue);
                this.currentProperty.TypeNameToWrite = typeNameToWrite;
            }
            else
            {
                string typeNameToWrite;
                if (TypeNameOracle.TryGetTypeNameFromAnnotation(enumValue, out typeNameToWrite))
                {
                    this.currentProperty.TypeNameToWrite = typeNameToWrite;
                }
            }
        }

        /// <summary>
        /// Writes a collection property.
        /// </summary>
        /// <param name="collectionValue">The collection value to be written</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WriteCollectionProperty(
            ODataCollectionValue collectionValue,
            bool isOpenPropertyType)
        {
            ResolveCollectionValueTypeName(collectionValue, isOpenPropertyType);

            this.WritePropertyTypeName();
            this.JsonWriter.WriteName(this.currentProperty.WireName);

            // passing false for 'isTopLevel' because the outer wrapping object has already been written.
            this.JsonLightValueSerializer.WriteCollectionValue(
                collectionValue,
                this.currentProperty.TypeReference,
                this.currentProperty.ValueType.TypeReference,
                this.currentProperty.IsTopLevel,
                false /*isInUri*/,
                isOpenPropertyType);
        }

        private void ResolveCollectionValueTypeName(ODataCollectionValue collectionValue, bool isOpenPropertyType)
        {
            if (this.currentProperty.ValueType == null || this.currentProperty.ValueType.TypeName != collectionValue.TypeName)
            {
                IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeForCollectionValue(
                    this.Model,
                    this.currentProperty.TypeReference,
                    collectionValue,
                    isOpenPropertyType,
                    this.WriterValidator);

                this.currentProperty.ValueType = new PropertyValueType(collectionValue.TypeName, typeFromValue);
                this.currentProperty.TypeNameToWrite =
                    this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(collectionValue,
                        this.currentProperty, isOpenPropertyType);
            }
            else
            {
                string typeNameToWrite;
                if (TypeNameOracle.TryGetTypeNameFromAnnotation(collectionValue, out typeNameToWrite))
                {
                    this.currentProperty.TypeNameToWrite = typeNameToWrite;
                }
            }
        }

        /// <summary>
        /// Writes a primitive property.
        /// </summary>
        /// <param name="primitiveValue">The primitive value to be written</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private void WritePrimitiveProperty(
            ODataPrimitiveValue primitiveValue,
            bool isOpenPropertyType)
        {
            ResolvePrimitiveValueTypeName(primitiveValue, isOpenPropertyType);

            this.WritePropertyTypeName();
            this.JsonWriter.WriteName(this.currentProperty.WireName);
            this.JsonLightValueSerializer.WritePrimitiveValue(primitiveValue.Value, this.currentProperty.ValueType.TypeReference, this.currentProperty.TypeReference);
        }

        private void ResolvePrimitiveValueTypeName(
            ODataPrimitiveValue primitiveValue,
            bool isOpenPropertyType)
        {
            string typeName = primitiveValue.Value.GetType().Name;
            if (this.currentProperty.ValueType == null || this.currentProperty.ValueType.TypeName != typeName)
            {
                IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeForPrimitiveValue(primitiveValue);

                this.currentProperty.ValueType = new PropertyValueType(typeName, typeFromValue);
                this.currentProperty.TypeNameToWrite = this.JsonLightOutputContext.TypeNameOracle.GetValueTypeNameForWriting(primitiveValue,
                        this.currentProperty, isOpenPropertyType);
            }
            else
            {
                string typeNameToWrite;
                if (TypeNameOracle.TryGetTypeNameFromAnnotation(primitiveValue, out typeNameToWrite))
                {
                    this.currentProperty.TypeNameToWrite = typeNameToWrite;
                }
            }
        }

        /// <summary>
        /// Writes the type name on the wire.
        /// </summary>
        private void WritePropertyTypeName()
        {
            string typeNameToWrite = this.currentProperty.TypeNameToWrite;
            if (typeNameToWrite != null)
            {
                // We write the type name as an instance annotation (named "odata.type") for top-level properties, but as a property annotation (e.g., "...@odata.type") if not top level.
                if (this.currentProperty.IsTopLevel)
                {
                    this.ODataAnnotationWriter.WriteODataTypeInstanceAnnotation(typeNameToWrite);
                }
                else
                {
                    this.ODataAnnotationWriter.WriteODataTypePropertyAnnotation(this.currentProperty.PropertyName, typeNameToWrite);
                }
            }
        }
    }
}
