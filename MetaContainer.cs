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
                        owner.v
                    }
                }
            }
            return "";
        }
    }

    public class MetaContainer
    {
        public Value1C_obj froot; // корневой объект контейнера
        public MetaTypeSet ftypes; // набор генерируемых типов

        //std::map<Key, Value> → SortedDictionary<TKey, TValue>
        //std::unordered_map<Key, Value> → Dictionary<TKey, TValue>
        //std::map<TGUID, MetaObject*> fmetamap; // Соответствие УИД объектам метаданных
        public SortedDictionary<Guid, MetaObject> fmetamap; // Соответствие УИД объектам метаданных
        //std::map<String, MetaObject*> fsmetamap; 
        public SortedDictionary<string, MetaObject> fsmetamap; // Соответствие полного имени объектам метаданных (на двух языках)
        //std::map<TGUID, PredefinedValue*> fpredefinedvalues; // Соответствие УИД предопределенным элементам
        public SortedDictionary<Guid, PredefinedValue> fpredefinedvalues; // Соответствие УИД предопределенным элементам
        ContainerVer containerver;
        Version1C ver1C;
        bool useExternal; // Использовать отложенную загрузку внешних файлов
        public MetaContainer()
        { }
    }
}
