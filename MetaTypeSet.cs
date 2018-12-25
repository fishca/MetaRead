using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    //---------------------------------------------------------------------------
    // Набор типов метаданных (статических или генерируемых)
    public class MetaTypeSet
    {
        public SortedDictionary<String, MetaType> mapname; // соответствие имен (русских и английских) типам
        public SortedDictionary<Guid, MetaType> mapuid;    // соответствие идентификаторов типам
        public List<MetaType> alltype;                     // массив всех типов


        public static MetaTypeSet staticTypes; // Cтатические типы
        // Пустой тип
        public static MetaType mt_empty;
        // Примитивные типы
        public static MetaType mt_string;
        public static MetaType mt_number;
        public static MetaType mt_bool;
        public static MetaType mt_date;
        public static MetaType mt_undef;
        public static MetaType mt_null;
        public static MetaType mt_type;
        // УникальныйИдентификатор
        public static MetaType mt_uid;
        // ОписаниеТипаВнутр
        public static MetaType mt_typedescrinternal;
        // Двоичные данные
        public static MetaType mt_binarydata;
        // Произвольный тип
        public static MetaType mt_arbitrary;
        // Корневой тип
        public static MetaType mt_container;
        public static MetaType mt_config;
        // Псевдо-тип Стандартный атрибут
        public static MetaType mt_standart_attribute;
        // Псевдо-тип Стандартная табличная часть
        public static MetaType mt_standart_tabular_section;
        // Значения частей даты для квалификатора даты
        public static MetaValue mv_datefractionsdate;
        public static MetaValue mv_datefractionstime;
        // Тип ЧастиДаты
        public static MetaType mt_datefractions;
        // Свойство ЧастиДаты типа КвалификаторыДаты
        public static MetaProperty mp_datefractions;
        // ОбъектМетаданных: ТабличнаяЧасть
        public static MetaType mt_tabularsection;
        // ОбъектМетаданных: Реквизит
        public static MetaType mt_attribute;
        // ОбъектМетаданныхСсылка
        public static MetaType mt_metaobjref;
        // ОбъектМетаданныхСсылкаВнутр
        public static MetaType mt_metarefint; // специальный тип для свойств с галочкой Ссылка в деревьях сериализации
                                              // ТабличнаяЧасть
        public static MetaType mt_tabsection;
        // МетаСсылка
        public static MetaType mt_metaref;

        public MetaTypeSet() { }

        // Получить тип по имени
        public MetaType GetTypeByName(string n)
        {
            if (String.IsNullOrEmpty(n))
                return null;
            //TryGetValue("tif", out value)
            if (!mapname.TryGetValue(n, out MetaType val))
            {
                if ((staticTypes != null) && (staticTypes != this))
                {
                    return staticTypes.GetTypeByName(n);
                }
                else
                    return null;
            }
            else
                return val;
        }

        // Получить тип по УИД
        public MetaType GetTypeByUID(Guid u)
        {
            if (!mapuid.TryGetValue(u, out MetaType val))
            {
                if ((staticTypes != null) && (staticTypes != this))
                {
                    return staticTypes.GetTypeByUID(u);
                }
                else
                    return null;
            }
            else
                return val;
        }

        public void FillAll()
        {
            // TODO: Надо дорабатывать
            for (int i = 0; i < alltype.Count; ++i)
            {
                ;
            }
        }

        public void Add(MetaType t)
        {
            alltype.Add(t);
            mapname[t.Name.ToUpper()]  = t;
            mapname[t.EName.ToUpper()] = t;
            if (t.HasUid)
                mapuid[t.uid] = t;

        }

        public static void StaticTypesLoad(Stream str)
        {
            int number, i;
            uint j;
            MetaType mtype;
            MetaType vtype = null;
            MetaProperty prop = null;
            MetaValue value;
            Tree tr;
            Tree tt;
            Tree t;
            Guid uid = new Guid();
            string sn, sen;
            MetaObject metaobj;

            if (staticTypes != null)
                staticTypes = null;

            staticTypes = new MetaTypeSet();
            tr = Tree.Parse_1Cstream(str, "", "static types");
            tt = tr.Get_First().Get_First();
            // Параметры классов
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                new ClassParameter(tt);
            }

            // Классы
            tt = tt.Get_Next();
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                new Class(tt);
            }

            // Типы
            tt = tt.Get_Next();
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                mtype = new MetaType(staticTypes, tt);
            }
            staticTypes.FillAll();

            // Значения по умолчанию
            tt = tt.Get_Next();
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                t = tt.Get_First();
                mtype = staticTypes.GetTypeByName(t.Get_Value());
                t = t.Get_Next();
                prop = mtype.GetProperty(t.Get_Value());
                t = t.Get_Next();
                prop.defaultvaluetype = (DefaultValueType)Convert.ToInt32(t.Get_Value());
                t = t.Get_Next();
                switch (prop.defaultvaluetype)
                {
                    case DefaultValueType.dvt_novalue:
                        break;
                    case DefaultValueType.dvt_bool:
                        // TODO: Вернуться сюда после доработки MetaProperty
                        prop.dv_union_type.dv_bool = t.Get_Value().CompareTo("1") == 0 ? true : false;
                        break;
                    case DefaultValueType.dvt_number:
                        prop.dv_union_type.dv_number = Convert.ToInt32(t.Get_Value());
                        break;
                    case DefaultValueType.dvt_string:
                        prop.dv_union_type.dv_string = t.Get_Value();
                        break;
                    case DefaultValueType.dvt_date:
                        break;
                    case DefaultValueType.dvt_undef:
                        break;
                    case DefaultValueType.dvt_null:
                        break;
                    case DefaultValueType.dvt_type:
                        prop.dv_union_type.dv_type = staticTypes.GetTypeByName(t.Get_Value());
                        break;
                    case DefaultValueType.dvt_enum:
                        vtype = staticTypes.GetTypeByName(t.Get_Value());
                        break;
                    default:
                        break;
                }
                t = t.Get_Next();
                if (prop.defaultvaluetype == DefaultValueType.dvt_enum)
                    prop.dv_union_type.dv_enum = vtype.GetValue(t.Get_Value());
            }

            // Предопределенные объекты метаданных
            tt = tt.Get_Next();
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                t = tt.Get_First();
                sn = t.Get_Value();
                t = t.Get_Next();
                sen = t.Get_Value();
                t = t.Get_Next();
                if (!string_to_GUID(t.Get_Value(), ref uid))
                {
                    // error(L"Ошибка загрузки статических типов. Ошибка преобразования УИД в предопределенном объекте метаданных"
                    //         , L"Имя", sn
                    //         , L"УИД", t->get_value());
                    continue;
                }
                metaobj = new MetaObject(uid, null, sn, sen);
                metaobj.SetFullName(sn);
                metaobj.SetEfullName(sen);
                MetaObject.map[uid] = metaobj;
                MetaObject.smap[sn.ToUpper()] = metaobj;
                MetaObject.smap[sen.ToUpper()] = metaobj;
            }

            // Права
            tt = tt.Get_Next();
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                new MetaRight(tt);
            }

            // Деревья сериализации
            tt = tt.Get_Next();
            number = Convert.ToInt32(tt.Get_Value());
            for (i = 0; i < number; ++i)
            {
                tt = tt.Get_Next();
                t = tt.Get_First();
                mtype = staticTypes.GetTypeByName(t.Get_Value());
                // TODO: Не забыть доделать вернуться сюда обязательно!!!!!!!!!!!!!!!!!!!
                //mtype.set

                //mtype.setSerializationTree(t->get_next());
            }

        }

        public int Number()
        {
            return alltype.Count;
        }

        public MetaType GetType(int index)
        {
            return alltype[index];
        }


    }
}
