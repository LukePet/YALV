using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using YALV.Common;
using YALV.Common.Interfaces;
using YALV.Core;
using YALV.Core.Domain;
using YALV.Properties;

namespace YALV.ViewModel
{
    public class AddFolderPathVM
        : BindableObject
    {
        public AddFolderPathVM(IWinSimple win)
        {
            _callingWin = win;

            CommandExit = new CommandRelay(commandExitExecute, p => true);
            CommandSave = new CommandRelay(commandSaveExecute, commandSaveCanExecute);
            CommandAdd = new CommandRelay(commandAddExecute, commandAddCanExecute);
            CommandRemove = new CommandRelay(commandRemoveExecute, commandRemoveCanExecute);
            CommandSelectFolder = new CommandRelay(commandSelectFolderExecute, p => true);

            string path = Constants.FOLDERS_FILE_PATH;
            IList<PathItem> folders = null;
            ;
            try
            {
                folders = DataService.ParseFolderFile(path);
            }
            catch (Exception ex)
            {
                string message = String.Format((string) Resources.GlobalHelper_ParseFolderFile_Error_Text, path, ex.Message);
                MessageBox.Show(message, Resources.GlobalHelper_ParseFolderFile_Error_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                
            }
           
            
            _pathList = folders != null ? new ObservableCollection<PathItem>(folders) : new ObservableCollection<PathItem>();

            ListChanged = false;
        }

        #region Commands

        /// <summary>
        /// Exit Command
        /// </summary>
        public ICommandAncestor CommandExit { get; protected set; }

        /// <summary>
        /// Save Command
        /// </summary>
        public ICommandAncestor CommandSave { get; protected set; }

        /// <summary>
        /// Add Command
        /// </summary>
        public ICommandAncestor CommandAdd { get; protected set; }

        /// <summary>
        /// Remove Command
        /// </summary>
        public ICommandAncestor CommandRemove { get; protected set; }

        /// <summary>
        /// SelectFolder Command
        /// </summary>
        public ICommandAncestor CommandSelectFolder { get; protected set; }

        protected virtual object commandExitExecute(object parameter)
        {
            _callingWin.Close();
            return null;
        }

        protected virtual object commandSaveExecute(object parameter)
        {
            if (PathList != null)
            {
                //Clear item with empty information
                for (int i = PathList.Count - 1; i >= 0; i--)
                {
                    PathItem item = PathList[i];
                    if (String.IsNullOrWhiteSpace(item.Name) || String.IsNullOrWhiteSpace(item.Path))
                    {
                        PathList.RemoveAt(i);
                        continue;
                    }
                    item.Path = item.Path.TrimEnd('\\');
                }

                //Order list to save
                IList<PathItem> orderList = (from p in PathList
                                             orderby p.Name
                                             select p).ToList<PathItem>();
                PathList.Clear();
                foreach (var item in orderList)
                    PathList.Add(item);

                //Save XML File
                string path = Constants.FOLDERS_FILE_PATH;
                
                try
                {
                    DataService.SaveFolderFile(orderList, path);
                    MessageBox.Show(Resources.AddFolderPathVM_commandSaveExecute_SuccessMessage, Resources.AddFolderPathVM_commandSaveExecute_SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    ListChanged = true;
                }
                catch (Exception ex)
                {
                    string message = String.Format((string)Resources.GlobalHelper_SaveFolderFile_Error_Text, path, ex.Message);
                    MessageBox.Show(message, Resources.GlobalHelper_SaveFolderFile_Error_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);        
                }

            }
            return null;
        }

        protected virtual bool commandSaveCanExecute(object parameter)
        {
            return true;
        }

        protected virtual object commandAddExecute(object parameter)
        {
            var newItem = new PathItem();
            if (PathList != null)
                PathList.Add(newItem);
            return null;
        }

        protected virtual bool commandAddCanExecute(object parameter)
        {
            return true;
        }

        protected virtual object commandRemoveExecute(object parameter)
        {
            if (PathList != null)
                PathList.Remove(SelectedPath);
            return null;
        }

        protected virtual bool commandRemoveCanExecute(object parameter)
        {
            return SelectedPath != null;
        }

        protected virtual object commandSelectFolderExecute(object parameter)
        {
            using (System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                dlg.Description = "Select Log Folder";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK && SelectedPath != null)
                    SelectedPath.Path = dlg.SelectedPath;
            }
            return null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Notify saved list
        /// </summary>
        public bool ListChanged;

        /// <summary>
        /// PathList Property
        /// </summary>
        public ObservableCollection<PathItem> PathList
        {
            get { return _pathList; }
            set
            {
                _pathList = value;
                RaisePropertyChanged(PROP_PathList);
            }
        }
        private ObservableCollection<PathItem> _pathList;
        public static string PROP_PathList = "PathList";

        /// <summary>
        /// SelectedPath Property
        /// </summary>
        public PathItem SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                _selectedPath = value;
                RaisePropertyChanged(PROP_SelectedPath);
                CommandRemove.OnCanExecuteChanged();
            }
        }
        private PathItem _selectedPath;
        public static string PROP_SelectedPath = "SelectedPath";

        #endregion

        #region Privates

        private IWinSimple _callingWin;

        #endregion
    }
}
