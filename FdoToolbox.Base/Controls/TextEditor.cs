#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A text editor component
    /// </summary>
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

        public void Close()
        {
            ViewContentClosing(this, EventArgs.Empty);
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
