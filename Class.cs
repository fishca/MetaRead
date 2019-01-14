using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaRead.Constants;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    /// <summary>
    /// Класс Class
    /// </summary>
    public class Class : MetaBase
    {
        public Guid fuid = EmptyUID;
        public List<VarValidValue> fvervalidvalues = new List<VarValidValue>();
        public SortedDictionary<ClassParameter, int> fparamvalues = new SortedDictionary<ClassParameter, int>();
        public static SortedDictionary<Guid, Class> map = new SortedDictionary<Guid, Class>();
        public List<MetaStandartAttribute> fstandartattributes = new List<MetaStandartAttribute>(); // Стандартные реквизиты
        public List<MetaStandartTabularSection> fstandarttabularsections = new List<MetaStandartTabularSection>(); // Стандартные табличные части

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="tr"></param>
        public Class(Tree tr)
        {
            Tree tt;
            Tree t;
            int i, count, j;
            string s;
            ClassParameter p;

            tt = tr.Get_First();

            string_to_GUID(tt.Get_Value(), ref fuid);

            tt = tt.Get_Next();

            LoadValidValues(tt, fvervalidvalues);

            tt = tt.Get_Next();

            t = tt.Get_First();

            count = Convert.ToInt32(t.Get_Value());

            for (i = 0; i < count; i++)
            {
                t = t.Get_Next();
                s = t.Get_Value();
                t = t.Get_Next();
                p = ClassParameter.GetParam(s);
                if (p is null)
                {
                    // error(L"Ошибка загрузки статических типов. Некорректное имя параметра класса"
                    //     , L"Параметр", s);
                }
                else
                {
                    j = Convert.ToInt32(t.Get_Value());
                    fparamvalues[p] = j;
                }
            }

            // Стандартные реквизиты
            tt = tt.Get_Next();
            t = tt.Get_First();
            count = Convert.ToInt32(t.Get_Value());
            for (i = 0; i < count; ++i)
            {
                t = t.Get_Next();
                fstandartattributes.Add(new MetaStandartAttribute(t));
            }

            // Стандартные табличные части
            tt = tt.Get_Next();
            t = tt.Get_First();
            count = Convert.ToInt32(t.Get_Value());
            for (i = 0; i < count; ++i)
            {
                t = t.Get_Next();
                fstandarttabularsections.Add(new MetaStandartTabularSection(t));
            }
            map[fuid] = this;
        }

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
            return (fparamvalues.TryGetValue(p, out int val)) ? val : -1;
        }

        public static Class GetClass(Guid id)
        {
            return (map.TryGetValue(id, out Class val)) ? val : null;
        }


    }

    /// <summary>
    /// Экземпляр класса
    /// </summary>
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

        public int setversion(int v)
        {
            fversion = v;
            fversionisset = true;
            return v;
        }

        public int getversion()
        {
            if (fversionisset)
                return fversion;
            //error(L"Ошибка формата потока 117. Ошибка получения значения переменной ВерсияКласса. Значение не установлено.");
            return -1;
        }

        public Class Cl
        {
            get { return fcl; }
        }

        public int Version
        {
            get { return getversion(); }
            set { fversion = setversion(value); }
        }

    }

}
