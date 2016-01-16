using System;

namespace YALV.Core.Domain
{
    [Serializable]
    public class PathItem : BindableObject
    {
        /// <summary>
        /// Name Property
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(PROP_Name);
            }
        }
        private string _name;
        public static string PROP_Name = "Name";

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

        public PathItem()
        {
            _name = string.Empty;
            _path = string.Empty;
        }

        public PathItem(string name, string path)
        {
            _name = name;
            _path = path;
        }
    }
}
