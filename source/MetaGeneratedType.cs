using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //Генерируемый тип
    // 
    public class MetaGeneratedType : MetaBase
    {
        public bool fwoprefix; // Признак "Без префикса"

        public bool Woprefix
        {
            get { return fwoprefix; }
        }

        public MetaGeneratedType(String _name, String _ename, bool _pref = false) : base(_name, _ename)
        {
            fwoprefix = _pref;
        }

        public MetaGeneratedType(Tree tr)
        { }

    }
}
