using System;
using System.Collections.Generic;


namespace CmsForWas
{
    public abstract class FileToInstall
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public FileToInstall(string filename)
        {
            Name = filename;
        }
        public abstract bool Install(string destFolder);
    }
}
