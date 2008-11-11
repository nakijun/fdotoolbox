using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using System.IO;

namespace FdoToolbox.Base.Services
{
    public sealed class FileService
    {
        static OpenFileDialog _openDialog = new OpenFileDialog();
        static SaveFileDialog _saveDialog = new SaveFileDialog();
        static FolderBrowserDialog _folderDialog = new FolderBrowserDialog();

        static FileService()
        {
            string path = Preferences.WorkingDirectory;
            _openDialog.InitialDirectory = path;
            _saveDialog.InitialDirectory = path;
            _folderDialog.SelectedPath = path; 
            _folderDialog.ShowNewFolderButton = true;
        }

        public static string OpenFile(string title, string filter)
        {
            _openDialog.FileName = string.Empty;
            _openDialog.Title = title;
            _openDialog.Multiselect = false;
            _openDialog.Filter = filter;
            if (_openDialog.ShowDialog() == DialogResult.OK)
            {
                return _openDialog.FileName;
            }
            return string.Empty;
        }

        public static string[] OpenFiles(string title, string filter)
        {
            _openDialog.Title = title;
            _openDialog.Multiselect = true;
            _openDialog.Filter = filter;
            if (_openDialog.ShowDialog() == DialogResult.OK)
            {
                return _openDialog.FileNames;
            }
            return new string[0];
        }

        public static string SaveFile(string title, string filter)
        {
            _openDialog.FileName = string.Empty;
            _saveDialog.Title = title;
            _saveDialog.Filter = filter;
            if (_saveDialog.ShowDialog() == DialogResult.OK)
            {
                return _saveDialog.FileName;
            }
            return string.Empty;
        }

        public static string GetDirectory(string title)
        {
            _folderDialog.Description = title;
            if (_folderDialog.ShowDialog() == DialogResult.OK)
            {
                return _folderDialog.SelectedPath;
            }
            return string.Empty;
        }

        public static bool FileExists(string file)
        {
            return !string.IsNullOrEmpty(file) && File.Exists(file);
        }

        public static bool DirectoryExists(string dir)
        {
            return !string.IsNullOrEmpty(dir) && Directory.Exists(dir);
        }
    }
}
