using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.Document;
using Microsoft.Html.Editor.Tree;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Rosieks.VisualStudio.Angular.Extensions;
using Rosieks.VisualStudio.Angular.Services;

namespace Rosieks.VisualStudio.Angular.Commands
{
    internal class ComponentGoToDefinition : CommandTargetBase<VSConstants.VSStd97CmdID>
    {
        private readonly HtmlEditorTree tree;
        private readonly DTE dte;
        private readonly INgHierarchyProvider ngHierarchyProvider;

        private NgComponent component;

        public ComponentGoToDefinition(IVsTextView adapter, IWpfTextView textView, DTE dte, INgHierarchyProvider ngHierarchyProvider)
            : base(adapter, textView, VSConstants.VSStd97CmdID.GotoDefn)
        {
            HtmlEditorDocument document = HtmlEditorDocument.TryFromTextView(textView);

            this.tree = document == null ? null : document.HtmlEditorTree;
            this.dte = dte;
            this.ngHierarchyProvider = ngHierarchyProvider;
        }

        protected override bool Execute(VSConstants.VSStd97CmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (this.component != null)
            {
                if (File.Exists(component.ViewFileName))
                {
                    this.dte.OpenFileInPreviewTab(component.ViewFileName);

                    return true;
                }
            }

            return false;
        }

        protected override bool IsEnabled()
        {
            return this.dte.ActiveDocument.Name.EndsWith(".html") && IsComponent(out this.component);
        }

        private bool IsComponent(out NgComponent component)
        {
            component = null;
            int position = this.TextView.Caret.Position.BufferPosition.Position;

            ElementNode element = null;
            AttributeNode attr = null;

            this.tree.GetPositionElement(position, out element, out attr);

            if (element == null)
            {
                return false;
            }

            var currentDocumentPath = ServiceProvider.GlobalProvider.GetCurrentDocumentPath();
            var ngHierarchy = this.ngHierarchyProvider.Get(currentDocumentPath);
            component = ngHierarchy.Components.Value.FirstOrDefault(x => x.DashedName == element.Name || (attr != null && x.DashedName == attr.Name));
            return component != null;
        }
    }
}
