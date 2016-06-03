using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
    internal class ResourceSharedInfo
    {
        private DuplicatePropertyNamesChecker duplicateNameChecker;

        private PropertyInfoCache propertyCache;

        public ResourceSharedInfo(DuplicatePropertyNamesChecker duplicateNameChecker, PropertyInfoCache propertyCache)
        {
            this.duplicateNameChecker = duplicateNameChecker;
            this.propertyCache = propertyCache;
        }

        public DuplicatePropertyNamesChecker DuplicateNameChecker
        {
            get { return this.duplicateNameChecker; }
        }

        public PropertyInfoCache PropertyCache
        {
            get { return this.propertyCache; }
        }
    }
}
