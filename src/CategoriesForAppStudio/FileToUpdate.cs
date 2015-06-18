using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;


namespace CmsForWas
{
    public class FileToUpdate : FileToInstall
    {
        private NameValueCollection _searchReplace;

        public NameValueCollection SearchReplace
        {
            get { return _searchReplace; }
            set { _searchReplace = value; }
        }

        public FileToUpdate(string filename, NameValueCollection searchReplace) : base(filename)
        {
            SearchReplace = searchReplace == null ? new NameValueCollection() : searchReplace;
        }
        public override bool Install(string destFolder)
        {
            return Update(destFolder, destFolder);
        }
        protected bool Update(string sourceFolder, string destFolder)
        {
            string source_file = (string.IsNullOrWhiteSpace(sourceFolder) ? Environment.CurrentDirectory : sourceFolder) + "\\" + Name;
            string dest_file = (string.IsNullOrWhiteSpace(destFolder) ? Environment.CurrentDirectory : destFolder) + "\\" + Name;
            try
            {
                string modified_text = File.ReadAllText(source_file);
                Regex regex;
                foreach (string search_string in SearchReplace)
                {
                    regex = new Regex(search_string);
                    if (regex.IsMatch(modified_text))
                    {
                        modified_text = regex.Replace(modified_text, SearchReplace[search_string]);
                    }
                    else
                    {
                        return false;
                    }
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
