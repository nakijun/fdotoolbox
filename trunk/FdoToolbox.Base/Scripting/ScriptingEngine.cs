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
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.Core;
using IronPython.Runtime.Types;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Scripting
{
    public class ScriptGlobals
    {
        public const string WORKBENCH = "Workbench";
        public const string CONN_MANAGER = "ConnManager";
        public const string FILESERVICE = "FileService";
        public const string MSGSERVICE = "MsgService";
    }

    public delegate void ScriptLoadedEventHandler(CompiledScript script);

    public class ScriptingEngine
    {
        private ScriptEngine _engine;

        public event ScriptLoadedEventHandler ScriptLoaded;

        private static ScriptingEngine _instance;

        public static ScriptingEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScriptingEngine();
                }
                return _instance;
            }
        }

        private ScriptingEngine()
        {
            _engine = Python.CreateEngine();
            
            //mscorlib
            _engine.Runtime.LoadAssembly(typeof(string).Assembly);
            //System
            _engine.Runtime.LoadAssembly(typeof(Uri).Assembly);
            //System.Windows.Forms
            _engine.Runtime.LoadAssembly(typeof(Form).Assembly);
        }

        private ScriptScope CreateScope()
        {
            ScriptScope scope = _engine.CreateScope();
            //Setup common globals
            scope.SetVariable(ScriptGlobals.WORKBENCH, Workbench.Instance);
            scope.SetVariable(ScriptGlobals.FILESERVICE, DynamicHelpers.GetPythonTypeFromType(typeof(FileService)));
            scope.SetVariable(ScriptGlobals.MSGSERVICE, DynamicHelpers.GetPythonTypeFromType(typeof(MessageService)));
            scope.SetVariable(ScriptGlobals.CONN_MANAGER, ServiceManager.Instance.GetService<FdoConnectionManager>());
            
            return scope;
        }

        public void ExecuteStatements(string script)
        {
            ScriptSource src = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
            ScriptScope scope = CreateScope();
            src.Execute(scope);
        }

        private Dictionary<string, CompiledScript> loadedScripts = new Dictionary<string, CompiledScript>();

        public ICollection<string> LoadedScripts
        {
            get
            {
                return loadedScripts.Keys;
            }
        }

        public void InvokeLoadedScript(string key)
        {
            if (loadedScripts.ContainsKey(key))
            {
                loadedScripts[key].Run();
            }
        }

        public void LoadScript(string scriptPath)
        {
            if (!loadedScripts.ContainsKey(scriptPath))
            {
                try
                {
                    ScriptSource src = _engine.CreateScriptSourceFromFile(scriptPath);
                    CompiledCode code = src.Compile();
                    ScriptScope scope = CreateScope();
                    CompiledScript cpy = new CompiledScript(scriptPath, code, scope);
                    loadedScripts.Add(scriptPath, cpy);

                    ScriptLoadedEventHandler handler = this.ScriptLoaded;
                    if (handler != null)
                        handler(cpy);
                }
                catch (SyntaxErrorException ex)
                {
                    ExceptionOperations eo = _engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    string msg = "Syntax error in \"{0}\"";
                    msg = string.Format(msg, Path.GetFileName(scriptPath));
                    MessageService.ShowError(msg);
                }
            }
        }
    }

    public class CompiledScript
    {
        private string _Path;

        public string Path
        {
            get { return _Path; }
        }

        private CompiledCode _Code;

        public CompiledCode Code
        {
            get { return _Code; }
        }

        private ScriptScope _Scope;

        public ScriptScope Scope
        {
            get { return _Scope; }
        }

        public CompiledScript(string path, CompiledCode code, ScriptScope scope)
        {
            _Path = path;
            _Code = code;
            _Scope = scope;
        }

        public void Run()
        {
            _Code.Execute(_Scope);
        }
    }
}
