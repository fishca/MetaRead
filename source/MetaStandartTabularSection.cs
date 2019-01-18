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
    // Стандартная табличная часть
    public class MetaStandartTabularSection : MetaBase
    {
        public int fvalue;
        public Class f_class;

        public Guid class_uid;

        public static List<MetaStandartTabularSection> list = new List<MetaStandartTabularSection>();

        public MetaStandartTabularSection(String _name, String _ename) : base(_name, _ename)
        {
            f_class = null;
            class_uid = EmptyUID; /* Пустой УИД */
        }
        public MetaStandartTabularSection(Tree tr)
        {
            Tree tt = tr.Get_First();
            Name = tt.Get_Value();
            tt = tt.Get_Next();
            EName = tt.Get_Value();
            tt = tt.Get_Next();
            fvalue = Convert.ToInt32(tt.Get_Value());
            tt = tt.Get_Next();
            string_to_GUID(tt.Get_Value(), ref class_uid);

            if (list != null)
                list.Add(this);

        }

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
