using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    public class GeneratedType
    {
        public Guid typeuid;  // УИД типа
        public Guid valueuid; // УИД значения

        //public GeneratedType() { }
        
        

        public GeneratedType(Tree ptr, string path)
        {
            typeuid = new Guid(Constants.EMPTY_GUID);
            valueuid = new Guid(Constants.EMPTY_GUID);
            if (ptr is null)
            {
                // error(L"Ошибка формата потока 44. Ожидается значение UID генерируемого типа", L"Путь", path);
                return;
            }
            if (ptr.Get_Type() == Node_Type.nd_guid)
            {
                if (!string_to_GUID(ptr.Get_Value(), ref typeuid))
                {
                    // error(L"Ошибка формата потока 45. Ошибка преобразования UID генерируемого типа"
                    //     , L"UID", (*ptr)->get_value()
                    //     , L"Путь", path + (*ptr)->path());

                }
            }
            else
            {
                // error(L"Ошибка формата потока 46. Тип значения не UID"
                //     , L"Значение", (*ptr)->get_value()
                //     , L"Путь", path + (*ptr)->path());
            }
            ptr = ptr.Get_Next();
            if (ptr is null)
            {
                // error(L"Ошибка формата потока 47. Ожидается значение UID генерируемого типа", L"Путь", path);
                return;
            }
            if (ptr.Get_Type() == Node_Type.nd_guid)
            {
                if (!string_to_GUID(ptr.Get_Value(), ref typeuid))
                {
                    // error(L"Ошибка формата потока 48. Ошибка преобразования UID генерируемого типа"
                    //     , L"UID", (*ptr)->get_value()
                    //     , L"Путь", path + (*ptr)->path());

                }
            }
            else
            {
                // error(L"Ошибка формата потока 49. Тип значения не UID"
                //     , L"Значение", (*ptr)->get_value()
                //     , L"Путь", path + (*ptr)->path());
            }
            ptr = ptr.Get_Next();
        }

    }

    public class PredefinedValue
    {
        public string name; // Имя
        public Guid _ref;   // Ссылка
        public Value1C_metaobj owner; // Владелец предопределенного значения

        public PredefinedValue(string n, Guid r, Value1C_metaobj o)
        {
            name = n;
            _ref = r;
            owner = o;
        }

        public string getfullname(bool english = false)
        {
            string s = "";
            SortedDictionary<MetaGeneratedType, GeneratedType> ii;
            MetaType t;

            s = "";
            if (!(owner is null))
            {
                if (owner.type != null)
                {
                    if (owner.type.gentypeRef != null)
                    {

                        if (owner.v_objgentypes.TryGetValue(owner.type.gentypeRef, out GeneratedType val))
                        {
                            // TODO : Нужна реализация MetaContainer !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                            
                        }
                    }
                }
            }
            return "";
        }
    }

    /// <summary>
    /// Структура неинициализированного значения
    /// </summary>
    public class UninitValue1C
    {
        public Value1C value; // Неинициализированное значение
        public Guid uid;      // УИД значения
        public string path;   // Путь
        public Guid sauid;    // УИД стандартного реквизита
        public Value1C_stdtabsec metats; // Значение стандартной табличной части для стандартного реквизита

        public UninitValue1C(Value1C v, string p, Guid u)
        {
            value = v;
            path = p;
            uid = u;
        }

        public UninitValue1C(Value1C v, string p, Guid u, Guid su, Value1C_stdtabsec mts)
        {
            value = v;
            path = p;
            uid = u;
            sauid = su;
            metats = mts;
        }
    }

    /// <summary>
    /// Значение переменной дерева сериализации
    /// </summary>
    public class VarValue
    {
        public int value;
        public bool isset;

        public VarValue()
        {
            value = 0;
            isset = false;
        }
    }

    #region Соответствия
    //std::map<Key, Value> → SortedDictionary<TKey, TValue>
    //std::unordered_map<Key, Value> → Dictionary<TKey, TValue>
    #endregion

    public class MetaContainer
    {
        public Value1C_obj froot; // корневой объект контейнера
        public MetaTypeSet ftypes; // набор генерируемых типов

        public SortedDictionary<Guid, MetaObject> fmetamap;          // Соответствие УИД объектам метаданных
        public SortedDictionary<string, MetaObject> fsmetamap;         // Соответствие полного имени объектам метаданных (на двух языках)
        public SortedDictionary<Guid, PredefinedValue> fpredefinedvalues; // Соответствие УИД предопределенным элементам

        public ContainerVer containerver;
        public Version1C ver1C;

        public bool useExternal; // Использовать отложенную загрузку внешних файлов

        public string metaprefix;

        public List<UninitValue1C> uninitvalues;

        public uint export_thread_count; // количество потоков экспорта

        // TODO: Надо додумывать....!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //Value1C_obj_ExportThread** export_threads; // массив потоков экспорта

        public static Guid sig_standart_attribute; // сигнатура стандартного реквизита
        public static Guid sig_standart_table_sec; // сигнатура стандартной табличной части
        public static Guid sig_ext_dimension;      // сигнатура стандартного реквизита Субконто
        public static Guid sig_ext_dimension_type; // сигнатура стандартного реквизита ВидСубконто

        public Value1C readValue1C(Tree ptr, MetaType t, Value1C_obj valparent, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path, bool checkend = false)
        {
            return null;
        }

        public Value1C readValue1C(MetaType nt, SerializationTreeNode tn, Tree tr, Value1C_obj valparent, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path)
        {
            return null;
        }

        public void recursiveLoadValue1C(Value1C_obj v, VarValue varvalues, Tree ptr, SerializationTreeNode tn, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path, bool checkend = false)
        {
        }

        public int getVarValue(string vname, MetaType t, VarValue varvalues, ClassItem clitem, string path)
        {
            return 0;
        }
        public void setValue(string vname, MetaType t, Value1C_obj v, VarValue varvalues, ClassItem clitem, int value, string path)
        {

        }

        public void readPredefinedValues(Value1C_metaobj v, int ni, int ui, Value1C_obj vStrings, string spath)
        {

        }

        public MetaContainer()
        { }
    }
}
