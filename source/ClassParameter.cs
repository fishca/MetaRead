using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Параметры классов
    public class ClassParameter : IComparable<ClassParameter>
    {
        public String fname;
        public static SortedDictionary<String, ClassParameter> map = new SortedDictionary<string, ClassParameter>();

        public int CompareTo(ClassParameter y)
        {
            return this.ToString().CompareTo(y.ToString());
        }

        public ClassParameter(Tree tr)
        {
            Tree tt = tr.Get_First();
            fname = tt.Get_Value();
            //map = new SortedDictionary<string, ClassParameter>();
            map[fname] = this;
            
        }

        public String Name
        {
            get
            {
                return fname;
            }
        }

        public static ClassParameter GetParam(String paramname)
        {
            return (map.TryGetValue(paramname, out ClassParameter val)) ? val : null;
        }
    }
}
