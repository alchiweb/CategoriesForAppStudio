using System;
using System.Collections.Specialized;
using System.IO;

namespace CmsForWas
{
    public class FileToCopy : FileToUpdate
    {
        private bool _newFile;
        public bool NewFile
        {
            get { return _newFile; }
            set { _newFile = value; }
        }

        public FileToCopy(string filename, NameValueCollection searchReplace = null, bool newFile = false) : base(filename, searchReplace)
        {
            NewFile = newFile;
        }
        public override bool Install(string destFolder)
        {
            return Copy(null, destFolder);
        }
        protected bool Copy(string sourceFolder, string destFolder)
        {
            string source_file = (string.IsNullOrWhiteSpace(sourceFolder) ? Environment.CurrentDirectory : sourceFolder) + "\\" + Name;
            string dest_file = (string.IsNullOrWhiteSpace(destFolder) ? Environment.CurrentDirectory : destFolder) + "\\" + Name;
            try
            {
                string modified_text = File.ReadAllText(source_file);
                foreach (string search_string in SearchReplace)
                {
                    modified_text = modified_text.Replace(search_string, SearchReplace[search_string]);
                    dest_file = dest_file.Replace(search_string, SearchReplace[search_string]);
                }
                Directory.CreateDirectory(dest_file.Remove(dest_file.LastIndexOf("\\")));
                File.WriteAllText(dest_file, modified_text);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
