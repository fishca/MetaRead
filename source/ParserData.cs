using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    public struct FileInfo
    {
        public string GUID;
        public string Path;
        public string Dir;
        public string CodePage;
        public string Version;
        public bool IsFolder;
    }

    

    public enum SortMode
    {
        smNone,
        smByGUID,
        smByPath,
        smByDir
    }

    // public class FileInfo
    // {
    // }


    public class ParserData
    {
    }
}
