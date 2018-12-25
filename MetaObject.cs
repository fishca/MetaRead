using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Объект метаданных
    public class MetaObject : MetaBase
    {
        public String ffullname;
        public String fefullname;
        public Guid fuid;
        public Value1C_metaobj fvalue;

        public static SortedDictionary<Guid, MetaObject> map;
        public static SortedDictionary<String, MetaObject> smap;

        public MetaObject(Guid _uid, Value1C_metaobj _value)
        {
            fuid = _uid;
            fvalue = _value;
        }

        public MetaObject(Guid _uid, Value1C_metaobj _value, String _name, String _ename) : base(_name, _ename)
        {
            fuid = _uid;
            fvalue = _value;
        }

        public void SetFullName(String _fullname)
        {
            ffullname = _fullname;
        }

        public void SetEfullName(String _efullname)
        {
            fefullname = _efullname;
        }

        public String FullName
        {
            get
            {
                return ffullname;
            }
        }

        public String EfullName
        {
            get
            {
                return fefullname;
            }
        }

        public Guid UID
        {
            get
            {
                return fuid;
            }
        }

        // Value1C_metaobj* value
        public Value1C_metaobj Value
        {
            get
            {
                return fvalue;
            }
        }

        public String GetFullName(bool english = false)
        {
            return english ? fefullname : ffullname;
        }


    }
}
