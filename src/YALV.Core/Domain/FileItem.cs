using System;

namespace YALV.Core.Domain
{
    [Serializable]
    public class FileItem : BindableObject
    {
        /// <summary>
        /// Checked Property
        /// </summary>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    RaisePropertyChanged(PROP_Checked);
                }
            }
        }
        private bool _checked;
        public static string PROP_Checked = "Checked";

        /// <summary>
        /// FileName Property
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(PROP_FileName);
            }
        }
        private string _fileName;
        public static string PROP_FileName = "FileName";

        /// <summary>
        /// Path Property
        /// </summary>
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(PROP_Path);
            }
        }
        private string _path;
        public static string PROP_Path = "Path";

        public FileItem(string _fileName, string _path)
        {
            Checked = false;
            FileName = _fileName;
            Path = _path;
        }
    }
}
