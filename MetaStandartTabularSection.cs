using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Стандартная табличная часть
    public class MetaStandartTabularSection : MetaBase
    {
        public int fvalue;
        public Class f_class;

        public Guid class_uid;

        public static List<MetaStandartTabularSection> list;

        public MetaStandartTabularSection(String _name, String _ename) : base(_name, _ename)
        {
            f_class = null;
            class_uid = new Guid(); /* Пустой УИД */
        }
        public MetaStandartTabularSection(Tree tr)
        { }

        public int Value
        {
            get
            {
                return fvalue;
            }
        }
        public Class _class
        {
            get
            {
                return f_class;
            }
            set
            {
                f_class = value;
            }
        }

    }
}
