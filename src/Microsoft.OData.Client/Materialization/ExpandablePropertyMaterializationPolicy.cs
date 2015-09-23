using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSClient = Microsoft.OData.Client;

namespace Microsoft.OData.Client.Materialization
{
    public class ExpandablePropertyMaterializationPolicy : StructuralValueMaterializationPolicy
    {
        private readonly EntryValueMaterializationPolicy entryValueMaterializationPolicy;

        internal void MaterializeExpandableProperty(Type propertyType, ODataExpandableProeprty propertyValue, ICollection<ODataNavigationLink> navigationLinks)
        { 
        
        }
    }

}
