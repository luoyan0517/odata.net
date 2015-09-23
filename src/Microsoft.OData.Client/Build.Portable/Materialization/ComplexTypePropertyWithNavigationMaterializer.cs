using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Client;
using Microsoft.OData.Client.Metadata;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.Materialization
{
    internal sealed class ComplexPropertyWithNavigationPropertyMaterializer : ODataMessageReaderMaterializer
    {
        /// <summary>Current value being materialized; possibly null.</summary>
        private object currentValue;

        private EntryValueMaterializationPolicy entryValueMaterializationPolicy;  // For materializing the navigation property

        ComplexTypeDescriptor descriptor = null;  // The descriptor for current complex property descriptor, create only when complex type is single-value

        bool? isSingle;   // If complex type is single-value

        Uri identity;   // The identity of complex type

        /// <summary>The materializer plan.</summary>
        private readonly ProjectionPlan materializeEntryPlan;       // To support $select on complex type

        private EntityTrackingAdapter entityTracker;     // Used to determines if there is an existing complex value or whether a new one is created

        private FeedAndEntryMaterializerAdapter feedEntryAdapter; // Handle the navigation property

        public ComplexPropertyWithNavigationPropertyMaterializer(
            ODataMessageReader odataMessageReader,
            ODataReaderWrapper reader,
            IODataMaterializerContext materializerContext,
            EntityTrackingAdapter entityTrackingAdapter,
            Type expectedType,
            QueryComponents queryComponents)
            : base(odataMessageReader, materializerContext, expectedType, queryComponents.SingleResult)
        {
            this.isSingle = queryComponents.SingleResult;
            this.identity = new Uri(queryComponents.Uri.ToString().Split('?')[0]);
            this.feedEntryAdapter = new FeedAndEntryMaterializerAdapter(odataMessageReader, reader, materializerContext.Model, entityTrackingAdapter.MergeOption);
        }

        protected sealed override bool ReadImplementation()
        {
            // Read property and navigation property and materialize them

            if (this.isSingle.Value)
            {
                // Create complexDescriptor
            }
        }

        internal sealed override void ApplyLogToContext()
        {
            if (descriptor != null)
            {
                // Add or merge complexDescriptor in entityTracker
            }
        }
    }
}
