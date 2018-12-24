using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Параметры классов
    public class ClassParameter
    {
        public String fname;
        public static SortedDictionary<String, ClassParameter> map;

        public ClassParameter(Tree tr)
        {

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
            return null;
        }
    }
}
