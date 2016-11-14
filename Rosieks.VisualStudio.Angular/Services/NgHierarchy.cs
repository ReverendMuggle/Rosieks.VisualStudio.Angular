﻿namespace Rosieks.VisualStudio.Angular.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class NgHierarchy
    {
        internal static readonly NgHierarchy Null = CreateNull();

        private static NgHierarchy CreateNull()
        {
            return new NgHierarchy
            {
                Controllers = new Lazy<IReadOnlyList<NgController>>(() => new NgController[0]),
                Directives = new Lazy<IReadOnlyList<NgDirective>>(() => new NgDirective[0]),
                States = new Lazy<IReadOnlyList<NgState>>(() => new NgState[0]),
                Components = new Lazy<IReadOnlyList<NgComponent>>(() => new NgComponent[0]),
            };
        }

        public NgHierarchy()
        {
        }

        public string RootPath { get; set; }

        public Lazy<IReadOnlyList<NgController>> Controllers { get; set; }

        public Lazy<IReadOnlyList<NgDirective>> Directives { get; set; }

        public Lazy<IReadOnlyList<NgState>> States { get; set; }

        public Lazy<IReadOnlyList<NgComponent>> Components { get; set; }

    }
}
