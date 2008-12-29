using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;

namespace FdoToolbox.Base.Controls
{
    public class TextEditor : UserControl, IViewContent
    {
        ToolStrip toolstrip;
        TextEditorControl editor;

        public TextEditor()
        {
            InitializeComponent();

            toolstrip = ToolbarService.CreateToolStrip(this, "/FdoToolbox/TextEditor/Toolbar");
            editor = new TextEditorControl();
            editor.Dock = DockStyle.Fill;

            this.Controls.Add(editor);
            this.Controls.Add(toolstrip);
        }

        private void InitializeComponent()
        {
            
        }

        public Control ContentControl
        {
            get { return this; }
        }

        public string Title
        {
            get { return ResourceService.GetString("TITLE_TEXT_EDITOR"); }
        }

        public event EventHandler TitleChanged = delegate { };

        public bool CanClose
        {
            get { return true; }
        }

        public bool Close()
        {
            return true;
        }

        public bool Save()
        {
            return true;
        }

        public bool SaveAs()
        {
            return true;
        }

        public event EventHandler ViewContentClosing = delegate { };
    }
}
