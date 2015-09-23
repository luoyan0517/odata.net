using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Client;
using Microsoft.OData.Client.Metadata;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.Materialization
{
    internal sealed class ODataExpandablePropertyMaterializer : ODataMessageReaderMaterializer
    {
        private ExpandablePropertyMaterializationPolicy expandablePropertyMaterializationPolicy;

        ICollection<ODataNavigationLink> navigationLinks;

        String propertyName;

        public ODataExpandablePropertyMaterializer(ODataMessageReader reader, IODataMaterializerContext materializerContext, Type expectedType, bool? singleResult)
            : base(reader, materializerContext, expectedType, singleResult)
        {
        }

        protected sealed override bool ReadImplementation()
        { 
        }

        internal sealed override void ApplyLogToContext()
        {
           
        }
    }
}
