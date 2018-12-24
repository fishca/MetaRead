using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Классы
    public class Class : MetaBase
    {
        public Guid fuid;
        public List<VarValidValue> fvervalidvalues;
        public SortedDictionary<ClassParameter, int> fparamvalues;
        public static SortedDictionary<Guid, Class> map;
        public List<MetaStandartAttribute> fstandartattributes; // Стандартные реквизиты
        public List<MetaStandartTabularSection> fstandarttabularsections; // Стандартные табличные части

        public Class(Tree tr)
        { }

        public Guid UID
        {
            get
            {
                return fuid;
            }
        }

        public List<VarValidValue> vervalidvalues
        {
            get
            {
                return fvervalidvalues;
            }
        }

        public SortedDictionary<ClassParameter, int> paramvalues
        {
            get
            {
                return fparamvalues;
            }
        }

        public List<MetaStandartAttribute> standartattributes
        {
            get
            {
                return fstandartattributes;
            }
        }

        public List<MetaStandartTabularSection> standarttabularsections
        {
            get
            {
                return fstandarttabularsections;
            }
        }

        public int GetParamValue(ClassParameter p)
        {
            return 0;
        }

        public static Class GetClass(Guid id)
        {
            return null;
        }


    }

    //---------------------------------------------------------------------------
    // Экземпляр класса
    public class ClassItem
    {
        public Class fcl;
        public bool fversionisset;
        public int fversion;

        public ClassItem(Class _cl, bool fverset = false)
        {
            fcl = _cl;
            fversionisset = fverset;

        }

        public Class Cl
        {
            get { return fcl; }
        }

        public int Version
        {
            get { return fversion; }
            set { fversion = value; }
        }

    }

}
