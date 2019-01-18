using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaRead.APIcfBase;
using static MetaRead.Constants;
using static MetaRead.Structures;


namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Стандартный реквизит
    public class MetaStandartAttribute : MetaBase
    {
        public int fvalue;
        public bool fcount;
        public int fvaluemax;
        public Guid fuid;

        public MetaStandartAttribute(String _name, String _ename, bool _count = false) : base(_name, _ename)
        {
            fcount = _count;
        }
        public MetaStandartAttribute(Tree tr)
        {
            Tree tt = tr.Get_First();
            Name = tt.Get_Value();
            tt = tt.Get_Next();
            EName = tt.Get_Value();
            tt = tt.Get_Next();
            fvalue = Convert.ToInt32(tt.Get_Value());
            tt = tt.Get_Next();
            fcount = tt.Get_Value().CompareTo("1") == 0 ? true : false;
            tt = tt.Get_Next();
            fvaluemax = Convert.ToInt32(tt.Get_Value());
            tt = tt.Get_Next();
            string_to_GUID(tt.Get_Value(), ref fuid);
        }

        public int Value
        {
            get
            {
                return fvalue;
            }
        }
        public bool Count
        {
            get
            {
                return fcount;
            }
        }
        public int ValueMax
        {
            get
            {
                return fvaluemax;
            }
        }
        public Guid UID
        {
            get
            {
                return fuid;
            }
        }

    }
}
