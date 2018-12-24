using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        { }

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
