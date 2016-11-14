using System;

namespace Rosieks.VisualStudio.Angular.Services
{
    [Flags]
    internal enum NgComponentRestrict
    {
        Element = 1,
        Attribute = 2,
        Class = 4,
        All = 7,
    }

    internal class NgComponent
    {
        public string Name { get; set; }

        public string DashedName { get; set; }

        public string CodeFileName { get; set; }

        public string ViewFileName { get; set; }

        public NgComponentRestrict Restrict { get; set; }

        public NgComponentAttribute[] Attributes { get; set; }
    }

    internal class NgComponentAttribute
    {
        public string DashedName { get; set; }
        public string Name { get; set; }
    }
}