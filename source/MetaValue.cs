using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    // Предопределенное значение метаданных
    // 
    public class MetaValue : MetaBase
    {
        public MetaType owner;
        private int fvalue;
        private Guid fvalueUID;

        public MetaValue()
        {

        }

        public MetaValue(MetaType _owner, string _name, string _ename, int _value) : base(_name, _ename)
        {
            fvalue = _value;
            owner = _owner;
        }

        public MetaValue(MetaType _owner, Tree tr)
        {
            owner = _owner;
        }

        public int Value
        {
            get
            {
                return fvalue;
            }
        }

        public Guid ValueUID
        {
            get
            {
                return fvalueUID;
            }
        }

        public MetaType GetOwner()
        {
            return owner;
        }

    }
}
