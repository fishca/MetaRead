using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Право
    public class MetaRight : MetaBase
    {
        public Guid fuid;
        public Version1C fver1C;

        public static SortedDictionary<Guid, MetaRight> map;
        public static SortedDictionary<String, MetaRight> smap;

        public MetaRight(Tree tr)
        { }

        public static MetaRight GetRight(Guid _uid)
        {
            return null;
        }

        public static MetaRight GetRight(String _name)
        {
            return null;
        }

        public Guid UID
        {
            get
            {
                return fuid;
            }
        }

        public Version1C Ver1C
        {
            get
            {
                return fver1C;
            }
        }

    }
}
