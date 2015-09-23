//---------------------------------------------------------------------
// <copyright file="LinkDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using Microsoft.OData.Client.Metadata;

    public sealed class ComplexTypeDescriptor : ResourceDescriptor
    {
        private string propertyName;

        ///// <summary>
        ///// Contains the LinkInfo (navigation and relationship links) for navigation properties
        ///// </summary>
        private Dictionary<string, LinkInfo> relatedEntityLinks;

        /// <summary>this is a link</summary>
        internal override DescriptorKind DescriptorKind
        {
            get { return DescriptorKind.ExpandableProperty; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal ComplexTypeDescriptor()
            : base(EntityStates.Unchanged)
        {
         //   this.entity = entityDescriptor;
        }

        /// <summary>
        /// Clear all the changes associated with this descriptor
        /// This method is called when the client is done with sending all the pending requests.
        /// </summary>
        internal override void ClearChanges()
        {
            // Do nothing
        }

        public Uri Identity
        {
            get { return this.identity; }
            set { this.identity = value; }
        }

        public String PropertyName 
        {
            get { return this.propertyName; }
            set { this.propertyName = value; }
        }

        private LinkInfo GetLinkInfo(String propertyName)
        {
            if (this.relatedEntityLinks == null)
            {
                this.relatedEntityLinks = new Dictionary<string, LinkInfo>(StringComparer.Ordinal);
            }

            LinkInfo linkInfo = null;
            if (!this.relatedEntityLinks.TryGetValue(propertyName, out linkInfo))
            {
                linkInfo = new LinkInfo(propertyName);
                this.relatedEntityLinks[propertyName] = linkInfo;
            }

            return linkInfo;
        }

        /// <summary>
        /// Add the given navigation link to the entity descriptor
        /// </summary>
        /// <param name="propertyName">name of the navigation property via which this entity is related to the other end.</param>
        /// <param name="navigationUri">uri that can be used to navigate from this entity to the other end.</param>
        internal void AddNavigationLink(string propertyName, Uri navigationUri)
        {
            LinkInfo linkInfo = this.GetLinkInfo(propertyName);

            // There are scenarios where we need to overwrite an existing link (when someone tries to refresh the object)
            linkInfo.NavigationLink = navigationUri;
        }

        /// <summary>
        /// Add the given association link to the entity descriptor
        /// </summary>
        /// <param name="propertyName">name of the navigation property via which this entity is related to the other end.</param>
        /// <param name="associationUri">uri that can be used to navigate associations for this property.</param>
        internal void AddAssociationLink(string propertyName, Uri associationUri)
        {
            LinkInfo linkInfo = this.GetLinkInfo(propertyName);

            // There are scenarios where we need to overwrite an existing link (when someone tries to refresh the object)
            linkInfo.AssociationLink = associationUri;
        }

        //public EntityDescriptor Entity
        //{
        //    get
        //    {
        //        return this.entity;
        //    }
        //    set
        //    {
        //        this.entity = value;
        //    }
        //}

        ///// <summary>
        ///// Returns the LinkInfo for the given navigation property. 
        ///// </summary>
        ///// <param name="propertyName">name of the navigation property </param>
        ///// <param name="linkInfo"> LinkInfo for the navigation propery</param>
        ///// <returns>true if LinkInfo is found for the navigation property, false if not found</returns>
        internal bool TryGetLinkInfo(string propertyName, out LinkInfo linkInfo)
        {
            Util.CheckArgumentNullAndEmpty(propertyName, "propertyName");
            Debug.Assert(propertyName.IndexOf('/') == -1, "propertyName.IndexOf('/') == -1");

            linkInfo = null;
            if (this.relatedEntityLinks != null)
            {
                return this.relatedEntityLinks.TryGetValue(propertyName, out linkInfo);
            }

            return false;               

        }

        internal Uri GetNavigationLink(UriResolver baseUriResolver, ClientTypeAnnotation type, ClientPropertyAnnotation property)
        {
            LinkInfo linkInfo = null;
            Uri uri = null;
            if (this.TryGetLinkInfo(property.PropertyName, out linkInfo))
            {
                uri = linkInfo.NavigationLink;
            }

            if (uri == null)
            {
                Uri relativeUri = UriUtil.CreateUri(property.PropertyName, UriKind.Relative);
                uri = UriUtil.CreateUri(this.Entity.GetResourceUri(baseUriResolver, true /*queryLink*/), relativeUri);
            }

            return uri;
        }
    }

}
