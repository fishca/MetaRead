using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    public class MetaBase
    {
        private string fname;
        private string fename;

        public MetaBase() { }
        public MetaBase(string _name, string _ename)
        {
            fname  = _name;
            fename = _ename;
        }

        public string Name
        {
            get
            {
                return GetName();
            }
            set
            {
                SetName(value);
            }
        }

        public string EName
        {
            get
            {
                return GetName();
            }
            set
            {
                SetName(value);
            }
        }

        public String GetName(bool english = false)
        {
            return english ? fename : fname;
        }

        public void SetName(string _name, bool english = false)
        {
            if (english)
                fename = _name;
            else
                fname = _name;
        }
    }
}
