using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    public class MetaBase
    {
        public MetaBase() { }
        public MetaBase(String _name, String _ename)
        {
            Name = _name;
            Ename = _ename;
        }

        public String Name { get; set; }
        public String Ename { get; set; }

        public String GetName(bool english = false)
        {
            return english ? Ename : Name;
        }
    }
}
