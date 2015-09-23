namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Client.Metadata;

    public sealed class ResourceDescriptor : Descriptor
    {
        private Uri identity;

        private object resolvedObject;

        ///// <summary>
        ///// Contains the LinkInfo (navigation and relationship links) for navigation properties
        ///// </summary>
        private Dictionary<string, LinkInfo> relatedEntityLinks;
    }
}