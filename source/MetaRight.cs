using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Право
    public class MetaRight : MetaBase
    {
        public Guid fuid;
        public Version1C fver1C;

        public static SortedDictionary<Guid, MetaRight> map = new SortedDictionary<Guid, MetaRight>();
        public static SortedDictionary<String, MetaRight> smap = new SortedDictionary<string, MetaRight>();

        public MetaRight(Tree tr)
        {
            Tree tt = tr.Get_First();
            Name = tt.Get_Value();
            tt = tt.Get_Next();
            EName = tt.Get_Value();
            tt = tt.Get_Next();
            string_to_GUID(tt.Get_Value(), ref fuid);
            tt = tt.Get_Next();
            fver1C = stringtover1C(tt.Get_Value());
            if (fver1C == Version1C.v1C_min)
            {
                // error(L"Ошибка загрузки статических типов. Некорректное значение версии 1C в описании права"
                //     , L"Право", fname
                //     , L"Значение", tt->get_value());
            }
            else
            {
                map[fuid] = this;
                smap[Name] = this;
                smap[EName] = this;
            }
        }

        public static MetaRight GetRight(Guid _uid)
        {
            return (map.TryGetValue(_uid, out MetaRight val)) ? val : null;
        }

        public static MetaRight GetRight(String _name)
        {
            return (smap.TryGetValue(_name, out MetaRight val)) ? val : null;
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
