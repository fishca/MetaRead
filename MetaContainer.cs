using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.APIcfBase;
using static MetaRead.Constants;

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

    /// <summary>
    /// Контейнер метаданных (конфигурация, внешняя обработка, внешний отчет)
    /// </summary>
    public class MetaContainer
    {
        public Value1C_obj froot; // корневой объект контейнера
        public MetaTypeSet ftypes; // набор генерируемых типов

        // Бывшая глобальная переменная, пока сюда
        //__declspec(thread) std::vector<UninitValue1C>* puninitvalues = NULL;
        // 
        public static List<UninitValue1C> puninitvalues;

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
            Tree tr;
            Tree tt;
            Tree ttt;
            Tree tx;
            Value1C v;
            Value1C vv;
            Value1C_bool vb;
            Value1C_string vs;
            Value1C_number vn;
            Value1C_number_exp vne;
            Value1C_date vd;
            Value1C_type vt;
            Value1C_uid vu;
            Value1C_binary vbn;
            Value1C_enum ve;
            Value1C_stdattr vsa;
            Value1C_stdtabsec vst;
            Value1C_obj vo;
            Value1C_refobj vro;
            String s;
            Guid uid, ouid;
            int i, k, n, j;
            string spath;
            StreamWriter sw;
            MetaProperty prop;
            bool b;
            Value1C_stdtabsec _metats;
            //HANDLE handle;
            string sn;

            spath = storpresent + path;
            v = null;
            vo = null;

            uid = new Guid();

            if (t == MetaTypeSet.mt_arbitrary) // Если тип Произвольный, то тип не задан!
                t = null;

            if (ptr is null)
            {
                if (t != null)
                {
                    v = new Value1C_obj(valparent, this);
                    v.type = t;
                    if (t.Serialization_Ver == 0)
                    {
                        if (t.SerializationTree != null)
                        {
                            // TODO : Необходима реализация
                            // error(L"Ошибка формата потока 73. Ожидается значение."
                            //     , L"Загружаемый тип", t->name
                            //     , L"Путь", spath);

                        }
                        else
                        {
                            // TODO : Необходима реализация
                            // error(L"Ошибка формата потока 17. Не определен алгоритм загрузки типа."
                            //     , L"Загружаемый тип", t->name
                            //     , L"Путь", spath);
                        }
                    }
                    else if (t.Serialization_Ver > 1)
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 18. Ожидается значение."
                        //     , L"Загружаемый тип", t->name
                        //     , L"Путь", spath);
                    }
                }
                else
                {
                    // TODO : Необходима реализация
                    // error(L"Ошибка формата потока 1. Ожидается значение.", L"Путь", spath);
                }
            }
            else if (t is null) // Тип не задан
            {
                tr = ptr;
                ptr = tr.Get_Next();
                if (tr.Get_Type() == Node_Type.nd_list)
                {
                    tr = tr.Get_First();
                    if (tr.Get_Type() == Node_Type.nd_string)
                    {
                        s = tr.Get_Value();
                        if (s.CompareTo("S") == 0)
                        {
                            tr = tr.Get_Next();
                            v = readValue1C(tr, MetaTypeSet.mt_string, valparent, metauid, metats, clitem, path);
                        }
                        else if (s.CompareTo("N") == 0)
                        {
                            tr = tr.Get_Next();
                            v = readValue1C(tr, MetaTypeSet.mt_number, valparent, metauid, metats, clitem, path);
                        }
                        else if (s.CompareTo("B") == 0)
                        {
                            tr = tr.Get_Next();
                            v = readValue1C(tr, MetaTypeSet.mt_bool, valparent, metauid, metats, clitem, path);
                        }
                        else if (s.CompareTo("D") == 0)
                        {
                            tr = tr.Get_Next();
                            v = readValue1C(tr, MetaTypeSet.mt_date, valparent, metauid, metats, clitem, path);
                        }
                        else if (s.CompareTo("U") == 0)
                        {
                            tr = tr.Get_Next();
                            v = new Value1C_undef(valparent);
                        }
                        else if (s.CompareTo("L") == 0)
                        {
                            tr = tr.Get_Next();
                            v = new Value1C_null(valparent);
                        }
                        else if (s.CompareTo("T") == 0)
                        {
                            tr = tr.Get_Next();
                            v = readValue1C(tr, MetaTypeSet.mt_type, valparent, metauid, metats, clitem, path);
                        }
                        else if (s.CompareTo("#") == 0)
                        {
                            tr = tr.Get_Next();
                            if (tr != null)
                            {
                                if (tr.Get_Type() == Node_Type.nd_guid)
                                {
                                    if (string_to_GUID(tr.Get_Value(), ref uid))
                                    {
                                        t = MetaTypeSet.staticTypes.GetTypeByUID(uid);
                                        if (t != null)
                                        {
                                            tr = tr.Get_Next();
                                            v = readValue1C(tr, t, valparent, metauid, metats, clitem, path);
                                        }
                                        else
                                        {
                                            // TODO : Необходима реализация
                                            // error(L"Ошибка формата потока 2. Не найден тип по UID"
                                            //     , L"UID", tr->get_value()
                                            //     , L"Путь", spath + tr->path()
                                            //     , L"Мета", valparent->fullpath(this, false));
                                        }
                                    }
                                    else
                                    {
                                        // TODO : Необходима реализация
                                        // error(L"Ошибка формата потока 4. Ожидается значение UID"
                                        //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                                        //     , L"Значение", tr->get_value()
                                        //     , L"Путь", spath + tr->path());

                                    }
                                }
                            }
                            else
                            {
                                // TODO : Необходима реализация
                                // error(L"Ошибка формата потока 5. Отсутствует UID типа значения"
                                //     , L"Путь", spath);
                            }

                        }
                        else
                        {
                            // TODO : Необходима реализация
                            // error(L"Ошибка формата потока 6. Неизвестный символ типа значения"
                            //     , L"Символ значения", s
                            //     , L"Путь", spath + tr->path());
                        }
                    }
                    else
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 7. Ожидается строковое представление типа значения"
                        //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                        //     , L"Значение", tr->get_value()
                        //     , L"Путь", spath + tr->path());
                    }
                }
                else
                {
                    // TODO : Необходима реализация
                    // error(L"Ошибка формата потока 8. Ожидается значение типа список"
                    //     , "Тип значения", get_node_type_presentation(tr->get_type())
                    //     , L"Путь", spath + tr->path());
                }
            }
            else // Тип задан
            {
                tr = ptr;
                if (t == MetaTypeSet.mt_string)
                {
                    if (tr.Get_Type() == Node_Type.nd_string)
                    {
                        s = tr.Get_Value();
                        if (s.Length > maxStringLength)
                        {
                            vbn = new Value1C_binary(valparent);
                            v = vbn;
                            vbn.v_binformat = ExternalFileFormat.eff_string;
                            vbn.v_binary = new MemoryStream();
                            vbn.v_binary.Write(Encoding.UTF8.GetPreamble(), 0, Encoding.UTF8.GetPreamble().Length);
                            sw = new StreamWriter(vbn.v_binary, Encoding.UTF8, 1024);
                            sw.Write(s);
                            sw.Dispose();
                        }
                        else
                        {
                            vs = new Value1C_string(valparent);
                            v = vs;
                            vs.v_string = s;
                        }
                    }
                    else
                    {
                        vs = new Value1C_string(valparent);
                        vs.v_string = "";
                        v = vs;
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 113. Ошибка получения строки. Тип значения не Строка"
                        //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                        //     , L"Значение", tr->get_value()
                        //     , L"Путь", spath + tr->path());
                    }
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_number)
                {
                    if (tr.Get_Type() == Node_Type.nd_number)
                    {
                        vn = new Value1C_number(valparent);
                        v = vn;
                        vn.v_string = tr.Get_Value();
                        vn.v_number = Convert.ToInt32(vn.v_string);
                    }
                    else if (tr.Get_Type() == Node_Type.nd_number_exp)
                    {
                        vne = new Value1C_number_exp(valparent);
                        v = vne;
                        vne.v_string = tr.Get_Value();
                        try
                        {
                            vne.v_number = Convert.ToDecimal(vne.v_string);
                        }
                        catch (Exception)
                        {
                            // TODO : Необходима реализация
                            // error(L"Ошибка формата потока 234. Ошибка получения числа с плавающей запятой."
                            //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                            //     , L"Значение", tr->get_value()
                            //     , L"Путь", spath + tr->path());

                            //throw;
                        }

                    }
                    else
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 114. Ошибка получения числа. Тип значения не Число"
                        //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                        //     , L"Значение", tr->get_value()
                        //     , L"Путь", spath + tr->path());
                    }
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_bool)
                {
                    vb = new Value1C_bool(valparent);
                    v = vb;
                    s = tr.Get_Value();
                    if (tr.Get_Type() != Node_Type.nd_number)
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 115. Ошибка получения значения Булево. Тип значения не Число"
                        //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                        //     , L"Значение", s
                        //     , L"Путь", spath + tr->path());
                    }
                    else if (s.CompareTo("0") != 0 && s.CompareTo("1") != 0)
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 204. Ошибка получения значения типа Булево."
                        //     , L"Значение", s
                        //     , L"Путь", spath + tr->path());
                    }
                    vb.v_bool = s.CompareTo("0") != 0;
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_date)
                {
                    vd = new Value1C_date(valparent);
                    v = vd;
                    if (!string1C_to_date(tr.Get_Value(), ref vd.v_date))
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 9. Ошибка разбора значения даты"
                        //     , L"Значение", tr->get_value()
                        //     , L"Путь", spath + tr->path());
                    }
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_undef)
                {
                    v = new Value1C_undef(valparent);
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_null)
                {
                    v = new Value1C_null(valparent);
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_type)
                {
                    vt = new Value1C_type(valparent);
                    v = vt;
                    if (tr.Get_Type() == Node_Type.nd_guid)
                    {
                        if (string_to_GUID(tr.Get_Value(), ref uid))
                        {
                            puninitvalues.Add(new UninitValue1C(v, spath + tr.Path(), uid));
                            vt.v_uid = uid;
                        }
                        else
                        {
                            // TODO : Необходима реализация
                            // error(L"Ошибка формата потока 185. Ошибка преобразования UID"
                            //     , L"UID", tr->get_value()
                            //     , L"Путь", spath + tr->path());
                        }
                    }
                    else
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 186. Ожидается значение UID"
                        //     , L"Путь", spath + tr->path());
                    }
                    ptr = tr.Get_Next();
                }
                else if (t == MetaTypeSet.mt_typedescrinternal)
                {
                    vt = new Value1C_type(valparent);
                    v = vt;
                    uid = EmptyUID;
                    if (tr.Get_Type() == Node_Type.nd_string)
                    {
                        s = tr.Get_Value();
                        if (s.CompareTo("S") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_string;
                            vt.v_uid = MetaTypeSet.mt_string.uid;
                        }
                        else if (s.CompareTo("N") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_number;
                            vt.v_uid = MetaTypeSet.mt_number.uid;
                        }
                        else if (s.CompareTo("B") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_bool;
                            vt.v_uid = MetaTypeSet.mt_bool.uid;
                        }
                        else if (s.CompareTo("D") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_date;
                            vt.v_uid = MetaTypeSet.mt_date.uid;
                        }
                        else if (s.CompareTo("U") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_undef;
                            vt.v_uid = MetaTypeSet.mt_undef.uid;
                        }
                        else if (s.CompareTo("L") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_null;
                            vt.v_uid = MetaTypeSet.mt_null.uid;
                        }
                        else if (s.CompareTo("T") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_type;
                            vt.v_uid = MetaTypeSet.mt_type.uid;
                        }
                        else if (s.CompareTo("R") == 0)
                        {
                            vt.v_type = MetaTypeSet.mt_binarydata;
                            vt.v_uid = MetaTypeSet.mt_binarydata.uid;
                        }
                        else if (s.CompareTo("#") == 0)
                        {
                            tr = tr.Get_Next();
                            if (tr != null)
                            {
                                if (tr.Get_Type() == Node_Type.nd_guid)
                                {
                                    if (!string_to_GUID(tr.Get_Value(), ref uid))
                                    {
                                        uid = EmptyUID;
                                        // TODO : Необходима реализация
                                        // error(L"Ошибка формата потока 11. Ошибка преобразования UID"
                                        //     , L"UID", tr->get_value()
                                        //     , L"Путь", spath + tr->path());
                                    }
                                }
                                else
                                {
                                    // TODO : Необходима реализация
                                    // error(L"Ошибка формата потока 12. Ожидается значение UID"
                                    //     , L"Путь", spath + tr->path());
                                }
                            }
                            else
                            {
                                // TODO : Необходима реализация
                                // error(L"Ошибка формата потока 10. Отсутствует значение"
                                // , L"Загружаемый тип", t->name
                                // , L"Путь", spath);
                            }
                        }
                        else
                        {
                            // TODO : Необходима реализация
                            // error(L"Ошибка формата потока 110. Неизвестный символ типа значения"
                            //     , L"Загружаемый тип", t->name
                            //     , L"Символ значения", s
                            //     , L"Путь", spath + tr->path());

                        }
                        if (uid != EmptyUID)
                        {
                            puninitvalues.Add(new UninitValue1C(v, spath + tr.Path(), uid));
                            vt.v_uid = uid;
                        }
                    }
                    else
                    {
                        // TODO : Необходима реализация
                        // error(L"Ошибка формата потока 109. Ожидается значение типа Строка"
                        //     , L"Загружаемый тип", t->name
                        //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                        //     , L"Значение", tr->get_value()
                        //     , L"Путь", spath + tr->path());
                    }
                    ptr = tr.Get_Next();
                }
                else
                {
                    // TODO : Необходима реализация
                    switch (t.Serialization_Ver)
                    {
                        case 0:
                            vo = t.Meta ? new Value1C_metaobj(valparent, this) : new Value1C_obj(valparent, this);
                            vo.type = t;
                            v = vo;
                            if (t.SerializationTree is null)
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 13. Не определен алгоритм загрузки типа."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Путь", spath + tr->path());
                                ptr = tr.Get_Next();
                                break;
                            }
                            loadValue1C(vo, tr, t.SerializationTree, metauid, metats, clitem, path, checkend);
                            ptr = tr;
                            break;
                        case 1: // Без значения. В реальности, должно было бы обработаться выше в блоке if(*ptr == NULL)
                                //v->kind = kv_obj;
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 19. Не ожидаемое значение."
                                //      , L"Загружаемый тип", t->name
                                //      , L"Тип значения", get_node_type_presentation(tr->get_type())
                                //      , L"Значение", tr->get_value()
                                //      , L"Путь", spath + tr->path());
                            ptr = tr;
                            break;
                        case 2:
                            if (tr.Get_Type() == Node_Type.nd_number)
                            {
                                i = Convert.ToInt32(tr.Get_Value());
                                for (j = 0; j < t.Values.Count; ++j)
                                {
                                    if (t.Values[j].Value == i)
                                    {
                                        ve = new Value1C_enum(valparent);
                                        ve.type = t;
                                        v = ve;
                                        ve.v_enum = t.Values[j];
                                        break;
                                    }
                                }
                                if (v is null)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 15. Не найдено значение системного перечисления по числовому значению."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", i
                                    //     , L"Путь", spath + tr->path());
                                }
                            }
                            else if (tr.Get_Type() == Node_Type.nd_guid)
                            {
                                if (!string_to_GUID(tr.Get_Value(), ref uid))
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 20. Ошибка преобразвания UID."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", tr->get_value()
                                    //     , L"Путь", spath + tr->path());
                                }
                                else
                                {
                                    for (j = 0; j < t.Values.Count; ++j)
                                    {
                                        if (t.Values[j].ValueUID == uid)
                                        {
                                            ve = new Value1C_enum(valparent);
                                            ve.type = t;
                                            v = ve;
                                            ve.v_enum = t.Values[j];
                                            break;
                                        }
                                    }
                                    if (v is null)
                                    {
                                        // TODO : Надо реализовывать
                                        // error(L"Ошибка формата потока 21. Не найдено значение системного перечисления по UID."
                                        //     , L"Загружаемый тип", t->name
                                        //     , L"Значение", tr->get_value()
                                        //     , L"Путь", spath + tr->path());
                                    }

                                }
                            }
                            else
                            {
                                // error(L"Ошибка формата потока 14. Ожидается значение типа Число или UID."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                                //     , L"Путь", spath + tr->path());
                            }
                            ptr = tr.Get_Next();
                            break;
                        case 3:
                            if (tr.Get_Type() == Node_Type.nd_number)
                            {
                                i = Convert.ToInt32(tr.Get_Value());
                                for (j = 0; j < t.Values.Count; ++j)
                                {
                                    if (t.Values[j].Value == i)
                                    {
                                        ve = new Value1C_enum(valparent);
                                        ve.type = t;
                                        v = ve;
                                        ve.v_enum = t.Values[j];
                                        break;
                                    }
                                }
                                if (v is null)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 65. Не найдено значение системного перечисления по числовому значению."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", i
                                    //     , L"Путь", spath + tr->path());
                                }
                            }
                            else if (tr.Get_Type() == Node_Type.nd_list)
                            {
                                tt = tr.Get_First();
                                ptr = tr.Get_Next();
                                if (tt is null)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 66. Ожидается UID типа системного перечисления."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Путь", spath + tr->path());
                                    break;
                                }
                                if (tt.Get_Type() != Node_Type.nd_guid)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 67. Тип значения не UID, ожидается UID типа системного перечисления."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Тип значения", tt->get_type()
                                    //     , L"Путь", spath + tt->path());
                                    break;
                                }
                                if (!string_to_GUID(tt.Get_Value(), ref uid))
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 68. Ошибка преобразвания UID."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", tt->get_value()
                                    //     , L"Путь", spath + tt->path());
                                    break;
                                }
                                if (uid != t.uid)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 69. UID типа не совпадает с UID загружаемого системного перечисления."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"UID типа", GUID_to_string(t->uid)
                                    //     , L"Загружаемый UID", tt->get_value()
                                    //     , L"Путь", spath + tt->path());
                                    break;
                                }
                                tt = tt.Get_Next();
                                if (tt is null)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 70. Ожидается значение системного перечисления."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Путь", spath + tr->path());
                                    break;
                                }
                                if (tt.Get_Type() != Node_Type.nd_number)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 71. Тип значения не Число, ожидается значение системного перечисления."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Тип значения", tt->get_type()
                                    //     , L"Путь", spath + tt->path());
                                    break;
                                }
                                i = Convert.ToInt32(tt.Get_Value());
                                for (j = 0; j < t.Values.Count; ++j)
                                {
                                    if (t.Values[j].Value == i)
                                    {
                                        ve = new Value1C_enum(valparent);
                                        ve.type = t;
                                        v = ve;
                                        ve.v_enum = t.Values[j];
                                        break;
                                    }
                                }
                                if (v is null)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 72. Не найдено значение системного перечисления по числовому значению."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", i
                                    //     , L"Путь", spath + tt->path());
                                }
                                break;
                            }
                            else
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 14. Ожидается значение типа Число или UID."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", get_node_type_presentation(tr->get_type())
                                //     , L"Путь", spath + tr->path());
                            }
                            ptr = tr.Get_Next();
                            break;
                        case 4:
                            vu = new Value1C_uid(valparent);
                            v = vu;
                            if (tr.Get_Type() != Node_Type.nd_guid)
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 100. Тип значения не UID."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", tr->get_type()
                                //     , L"Путь", spath + tr->path());
                            }
                            else
                            {
                                if (!string_to_GUID(tr.Get_Value(), ref uid))
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 101. Ошибка преобразвания UID."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", tr->get_value()
                                    //     , L"Путь", spath + tr->path());
                                }
                                else
                                {
                                    vu.v_uid = uid;
                                }
                            }
                            ptr = tr.Get_Next();
                            break;
                        case 5: // Стандартный реквизит
                            vsa = new Value1C_stdattr(valparent);
                            v = vsa;
                            vsa.v_stdattr = null;
                            if (tr.Get_Type() != Node_Type.nd_number)
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 104. Тип значения не Число при загрузке стандартного реквизита."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", tr->get_type()
                                //     , L"Значение", tr->get_value()
                                //     , L"Путь", spath + tr->path());

                            }
                            break;
                        case 6:
                            break;
                        case 7:
                            break;
                        default:
                            break;
                    }
                }
            }
            if (vo != null)
                vo.fillpropv();

            return v;
        }

        public Value1C readValue1C(MetaType nt, SerializationTreeNode tn, Tree tr, Value1C_obj valparent, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path)
        {
            return null;
        }

        public void recursiveLoadValue1C(Value1C_obj v, VarValue[] varvalues, Tree ptr, SerializationTreeNode tn, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path, bool checkend = false)
        {
        }

        public int getVarValue(string vname, MetaType t, VarValue[] varvalues, ClassItem clitem, string path)
        {
            if (vname.CompareTo("ВерсияКонтейнера") == 0)
                return (int)containerver;
            if (vname.CompareTo("Версия1С") == 0)
                return (int)ver1C;
            if (vname.CompareTo("ВерсияКласса") == 0)
            {
                if (clitem != null)
                    return clitem.Version;
                // TODO : Надо реализовать логирование
                // error(L"Ошибка формата потока 119. Ошибка получения значения переменной. Класс не определён."
                //     , L"Загружаемый тип", t->name
                //     , L"Имя переменной", vname
                //     , L"Путь", path);
                return -1;

            }
            if (varvalues != null)
            {
                for (int i = 0; i < t.SerializationVars.Count; ++i)
                {
                    if (t.SerializationVars[i].Name.CompareTo(vname) == 0)
                    {
                        if (varvalues[i].isset)
                        {
                            return varvalues[i].value;
                        }
                        else
                        {
                            // TODO : Надо реализовать логирование
                            // error(L"Ошибка формата потока 43. Ошибка получения значения переменной. Значение переменной не установлено."
                            //     , L"Загружаемый тип", t->name
                            //     , L"Имя переменной", vname
                            //     , L"Путь", path);
                            return 0;
                        }
                    }
                }
            }
            // TODO : Надо реализовать логирование
            // error(L"Ошибка формата потока 34. Ошибка получения значения переменной. Недопустимое имя переменной."
            //     , L"Загружаемый тип", t->name
            //     , L"Имя переменной", vname
            //     , L"Путь", path);
            return 0;

        }

        public void setVarValue(string vname, MetaType t, Value1C_obj v, VarValue[] varvalues, ClassItem clitem, int value, string path)
        {
            SerializationTreeVar var;
            int i, j;
            VarValidValue vvv;

            if (vname.CompareTo("ВерсияКласса") == 0)
            {
                if (clitem != null)
                {
                    clitem.Version = value;
                    for (j = 0; j < clitem.Cl.vervalidvalues.Count; ++j)
                    {
                        if (clitem.Cl.vervalidvalues[j].value == value)
                            return;
                    }
                    // TODO : Надо реализовать логирование
                    // error(L"Ошибка формата потока 122. Недопустимое значение переменной."
                    //     , L"Загружаемый тип", t->name
                    //     , L"Имя переменной", vname
                    //     , L"Значение", value
                    //     , L"Класс", GUID_to_string(clitem->cl->uid)
                    //     , L"Путь", path);

                }
                else
                {
                    // TODO : Надо реализовать логирование
                    // error(L"Ошибка формата потока 118. Ошибка установки переменной. Класс не определён."
                    //     , L"Загружаемый тип", t->name
                    //     , L"Имя переменной", vname
                    //     , L"Значение", value
                    //     , L"Путь", path);
                }
                return;
            }
            if (varvalues != null)
            {
                for (i = 0; i < t.SerializationVars.Count; ++i)
                {
                    var = t.SerializationVars[i];
                    if (var.Name.CompareTo(vname) == 0)
                    {
                        varvalues[i].value = value;
                        varvalues[i].isset = true;
                        if (var.validvalues.Count == 0)
                            return;
                        for (j = 0; j < var.validvalues.Count; ++j)
                        {
                            vvv = var.validvalues[j];
                            if (vvv.value == value)
                            {
                                if (var.IsGlobal)
                                    v.globalvars[vname.ToUpper()] = vvv.globalvalue;
                                return;
                            }
                        }
                        // TODO : Надо реализовать логирование
                        // error(L"Ошибка формата потока 64. Недопустимое значение переменной."
                        //     , L"Загружаемый тип", t->name
                        //     , L"Имя переменной", vname
                        //     , L"Значение", value
                        //     , L"Путь", path);
                        return;
                    }
                }
            }
            // TODO : Надо реализовать логирование
            // error(L"Ошибка формата потока 37. Ошибка установки значения переменной. Недопустимое имя переменной."
            //     , L"Загружаемый тип", t->name
            //     , L"Имя переменной", vname
            //     , L"Путь", path);
        }

        public void readPredefinedValues(Value1C_metaobj v, int ni, int ui, Value1C_obj vStrings, string spath)
        {
            Value1C_obj vo;
            Value1C_obj vvo;
            Value1C_uid vu;
            Value1C_string vs;
            Value1C vv;
            int i;
            Guid u;
            String n;
            PredefinedValue pre;

            for (i = 0; i < vStrings.v_objcol.Count; ++i)
            {
                vo = (Value1C_obj)vStrings.v_objcol[i];
                if (vo is null)
                {
                    // TODO : Надо реализовать логирование
                    // error(L"Ошибка формата потока 179. Ошибка получения значения предопределенного элемента"
                    //     , L"Путь", spath);
                    continue;
                }

                vvo = (Value1C_obj)vo.v_objcol[ui];
                if (vvo is null)
                {
                    // TODO : Надо реализовать логирование
                    // error(L"Ошибка формата потока 180. Ошибка получения идентификатора предопределенного элемента"
                    //     , L"Путь", spath);
                    continue;
                }
                vu = (Value1C_uid)vvo.getproperty("Ссылка");
                if (vvo is null)
                {
                    // TODO : Надо реализовать логирование
                    // error(L"Ошибка формата потока 181. Ошибка получения значения идентификатора предопределенного элемента"
                    //     , L"Путь", spath);
                    continue;
                }
                if (vu.kind != KindOfValue1C.kv_uid)
                {
                    // TODO : Надо реализовать логирование
                    // error(L"Ошибка формата потока 182. Ошибка получения значения идентификатора предопределенного элемента. Тип значения не UID"
                    //     , L"Путь", spath
                    //     , L"Тип значения", KindOfValue1C_presantation(vu->kind));
                    continue;
                }
                u = vu.v_uid;
                if (u != EmptyUID)
                {
                    vv = vo.v_objcol[ni];
                    if (vv is null)
                    {
                        // TODO : Надо реализовать логирование
                        // error(L"Ошибка формата потока 183. Ошибка получения имени предопределенного элемента"
                        //     , L"Путь", spath);
                        continue;
                    }
                    if (vv.kind != KindOfValue1C.kv_string && vv.kind != KindOfValue1C.kv_binary)
                    {
                        // TODO : Надо реализовать логирование
                        // error(L"Ошибка формата потока 184. Ошибка получения имени предопределенного элемента. Тип значения не Строка"
                        //     , L"Путь", spath
                        //     , L"Тип значения", KindOfValue1C_presantation(vs->kind));
                        continue;
                    }
                    n = vv.presentation();
                    pre = new PredefinedValue(n, u, v);
                    v.v_prevalues.Add(pre);
                    fpredefinedvalues[u] = pre;
                }
                vvo = (Value1C_obj)vo.getproperty("Строки");
                if (vvo != null)
                    readPredefinedValues(v, ni, ui, vvo, spath);
            }

        }

        public ConfigStorage stor;

        public string storpresent;

        public static Tree gettree(ConfigStorage stor, string path, bool reporterror = true)
        {
            string fullpath = stor.presentation() + "\\" + path;
            ConfigFile cf = stor.readfile(path);

            if (cf is null)
            {
                if (reporterror)
                {
                    // TODO : Надо реализовать логирование
                    //error(L"Ошибка чтения контейнера. Не найден файл." , L"Путь", fullpath);
                }
                return null;
            }

            Tree t = Tree.Parse_1Cstream(cf.str, "", fullpath);
            stor.close(cf);

            if (t is null)
            {
                // TODO : Надо реализовать логирование
                //error(L"Ошибка формата потока 140. Ошибка разбора файла.", L"Путь", fullpath);
                return null;
            }
            return t;
        }

        public void loadValue1C(Value1C_obj v, Tree ptr, SerializationTreeNode tn, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path, bool checkend = false)
        {
        }

        public void inituninitvalues()
        {
            int j;
            uint _in;
            Value1C v;
            Value1C_obj vo;
            Value1C_metaobj vvo;
            Value1C_refpre vrp;
            Value1C_type vt;
            Value1C_stdattr vsa;
            Value1C_stdtabsec vst;
            Value1C_refobj vro;
            MetaObject mo;
            MetaType typ;
            //TGUID uid;
            Guid uid;
            MetaStandartAttribute sa;
            MetaStandartTabularSection st;
            Class cl;
            bool flag;
            //std::vector<UninitValue1C>* _puninitvalues;
            List<UninitValue1C> _puninitvalues = null; // этот список должен где-то заполняться........
            
            // TODO : Разобраться с заполнением данного списка "uninitvalues"
            //_puninitvalues = puninitvalues;

            foreach (var item_puninitvalues in _puninitvalues)
            {
                v = item_puninitvalues.value;
                uid = item_puninitvalues.uid;
                if (v.kind == KindOfValue1C.kv_refobj)
                {
                    vro = (Value1C_refobj)v;
                    uid = vro.v_uid;
                    vro.v_metaobj = getMetaObject(uid);
                    if (vro.v_metaobj is null)
                    {
                        // TODO: с этим надо что-то делать....
                        // error(L"Ошибка формата потока 74. Не определен объект метаданных по UID"
                        //     , L"UID", GUID_to_string(uid)
                        //     , L"Путь", ui->path
                        //     , L"Мета", v->fullpath(this, false));
                    }
                }
                else if (v.kind == KindOfValue1C.kv_refpre)
                {
                    vrp = (Value1C_refpre)v;
                    vrp.v_prevalue = getPreValue(uid);
                    if (vrp.v_prevalue is null)
                    {
                        // TODO: с этим надо что-то делать....
                        // error(L"Ошибка формата потока 162. Не определен предопределенный элемент по UID"
                        //     , L"UID", GUID_to_string(uid)
                        //     , L"Путь", ui->path
                        //     , L"Мета", v->fullpath(this, false));
                    }
                }
                else if (v.kind == KindOfValue1C.kv_type)
                {
                    vt = (Value1C_type)v;
                    uid = vt.v_uid;
                    vt.v_type = (uid == EmptyUID) ? MetaTypeSet.mt_empty : ftypes.GetTypeByUID(uid);

                    if (vt.v_type is null)
                    {
                        // TODO: с этим надо что-то делать....
                        // error(L"Ошибка формата потока 75. Не определен тип по UID"
                        //     , L"UID", GUID_to_string(uid)
                        //     , L"Путь", ui->path
                        //     , L"Мета", v->fullpath(this, false));
                    }
                }
                else if (v.kind == KindOfValue1C.kv_stdattr)
                {
                    vsa = (Value1C_stdattr)v;
                    mo = getMetaObject(uid);
                    vsa.v_stdattr = null;
                    if (mo is null)
                    {
                        // TODO: с этим надо что-то делать....
                        // error(L"Ошибка формата потока 106. Ошибка загрузки стандартного реквизита. Не определен объект метаданных по UID"
                        //     , L"UID", GUID_to_string(uid)
                        //     , L"Путь", ui->path
                        //     , L"Мета", v->fullpath(this, false));
                    }
                    else
                    {
                        j = vsa.v_value;
                        if (mo.Value.type == MetaTypeSet.mt_attribute)
                        {
                            vt = null;
                            typ = null;
                            vo = mo.Value;
                            vo = (Value1C_obj)vo.getproperty("Тип"); // Описание типов
                            if (vo != null)
                            {
                                if (vo.v_objcol.Count > 0)
                                {
                                    vo = (Value1C_obj)vo.v_objcol[0]; // Описание типа
                                }
                            }
                            if (vo != null)
                            {
                                vt = (Value1C_type)vo.getproperty("Тип"); // Описание Типа Внутр
                            }
                            if (vt != null)
                            {
                                typ = vt.v_type;
                                if (typ is null)
                                    typ = ftypes.GetTypeByUID(uid);
                            }
                            if (typ != null)
                            {
                                if (typ.DefaultClass != null)
                                {
                                    cl = typ.DefaultClass;
                                    for (_in = 0; _in < cl.standartattributes.Count; ++_in)
                                    {
                                        sa = cl.standartattributes[(int)_in];
                                        if (j == sa.Value)
                                        {
                                            vsa.v_stdattr = sa;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (vsa.v_stdattr is null)
                        {
                            if (item_puninitvalues.metats != null)
                            {
                                cl = item_puninitvalues.metats.v_stdtabsec._class;
                                for (_in = 0; _in < cl.standartattributes.Count; ++_in)
                                {
                                    sa = cl.standartattributes[(int)_in];
                                    if (sa.Count)
                                    {
                                        if (sa.UID == item_puninitvalues.sauid)
                                        {
                                            vsa.v_stdattr = sa;
                                            if (j < sa.Value || j > sa.ValueMax)
                                            {
                                                // TODO: с этим надо что-то делать....
                                                // error(L"Ошибка формата потока 212. Ошибка загрузки стандартного реквизита. Значение за пределами допустимого."
                                                //     , L"Класс", GUID_to_string(cl->uid)
                                                //     , L"Значение", j
                                                //     , L"Минимальное значение", sa->value
                                                //     , L"Максимальное значение", sa->valuemax
                                                //     , L"Путь", ui->path
                                                //     , L"Мета", v->fullpath(this, false));
                                            }
                                            break;
                                        }
                                    }
                                    else if (j == sa.Value)
                                    {
                                        vsa.v_stdattr = sa;
                                        break;
                                    }
                                }
                            }
                        }
                        if (vsa.v_stdattr is null)
                        {
                            cl = mo.Value.getclass(true);
                            flag = false;
                            if (cl is null)
                            {
                                flag = true;
                            }
                            else
                            {
                                if (cl.standartattributes.Count == 0)
                                    flag = true;
                            }
                            if (flag)
                            {
                                for (vo = mo.Value.parent, mo = null; vo != null; vo = vo.parent)
                                {
                                    if (vo.kind != KindOfValue1C.kv_metaobj)
                                        continue;
                                    vvo = (Value1C_metaobj)vo;
                                    if (vvo.type is null)
                                        continue;
                                    if (vvo.type == MetaTypeSet.mt_tabularsection)
                                        continue;
                                    cl = vvo.getclass(true);
                                    if (cl != null)
                                    {
                                        if (cl.standartattributes.Count > 0)
                                        {
                                            mo = vvo.v_metaobj;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (mo is null)
                            {
                                // TODO: с этим надо что-то делать....
                                // error(L"Ошибка формата потока 116. Не определен объект метаданных стандартного реквизита по UID"
                                //     , L"UID", GUID_to_string(uid)
                                //     , L"Путь", ui->path
                                //     , L"Мета", v->fullpath(this, false));

                            }
                            else
                            {
                                cl = mo.Value.getclass();
                                if (cl is null)
                                {
                                    // TODO: с этим надо что-то делать....
                                    // error(L"Ошибка формата потока 107. Не определен класс объекта метаданных"
                                    //     , L"Объект метаданных", mo->fullname
                                    //     , L"Путь", ui->path
                                    //     , L"Мета", v->fullpath(this, false));
                                }
                                else
                                {
                                    for (_in = 0; _in < cl.standartattributes.Count; ++_in)
                                    {
                                        sa = cl.standartattributes[(int)_in];
                                        if (sa.Count)
                                        {
                                            if (sa.UID == item_puninitvalues.sauid)
                                            {
                                                vsa.v_stdattr = sa;
                                                if (j < sa.Value || j > sa.ValueMax)
                                                {
                                                    // TODO: с этим надо что-то делать....
                                                    // error(L"Ошибка формата потока 206. Ошибка загрузки стандартного реквизита. Значение за пределами допустимого."
                                                    //     , L"Класс", GUID_to_string(cl->uid)
                                                    //     , L"Значение", j
                                                    //     , L"Минимальное значение", sa->value
                                                    //     , L"Максимальное значение", sa->valuemax
                                                    //     , L"Путь", ui->path
                                                    //     , L"Мета", v->fullpath(this, false));
                                                }
                                                break;
                                            }
                                        }
                                        else if (j == sa.Value)
                                        {
                                            vsa.v_stdattr = sa;
                                            break;
                                        }
                                    }
                                    if (vsa.v_stdattr is null)
                                    {
                                        // TODO: с этим надо что-то делать....
                                        // error(L"Ошибка формата потока 108. Не найден стандартный реквизит по значению"
                                        //     , L"Класс", GUID_to_string(cl->uid)
                                        //     , L"Значение", j
                                        //     , L"Путь", ui->path
                                        //     , L"Мета", v->fullpath(this, false));
                                    }
                                }
                            }
                        }
                    }
                    vsa.v_metaobj = mo;
                }
                else if (v.kind == KindOfValue1C.kv_stdtabsec)
                {
                    vst = (Value1C_stdtabsec)v;
                    vst.v_stdtabsec = null;
                    mo = getMetaObject(uid);
                    vst.v_metaobj = mo;
                    if (mo is null)
                    {
                        // TODO: с этим надо что-то делать....
                        // error(L"Ошибка формата потока 209. Ошибка загрузки стандартной табличной части.  Не определен объект метаданных по metauid."
                        //     , L"UID", GUID_to_string(uid)
                        //     , L"Путь", ui->path
                        //     , L"Мета", v->fullpath(this, false));
                    }
                    else
                    {
                        cl = mo.Value.getclass();
                        if (cl is null)
                        {
                            // TODO: с этим надо что-то делать....
                            // error(L"Ошибка формата потока 210. Не определен класс объекта метаданных"
                            //     , L"Объект метаданных", mo->fullname
                            //     , L"Путь", ui->path
                            //     , L"Мета", v->fullpath(this, false));
                        }
                        else
                        {
                            j = vst.v_value;
                            for (_in = 0; _in < cl.standartattributes.Count; ++_in)
                            {
                                st = cl.standarttabularsections[(int)_in];
                                if (j == st.Value)
                                {
                                    vst.v_stdtabsec = st;
                                    break;
                                }
                            }
                            if (vst.v_stdtabsec is null)
                            {
                                // TODO: с этим надо что-то делать....
                                // error(L"Ошибка формата потока 211. Не найдена стандартная табличная часть по значению"
                                //     , L"Класс", GUID_to_string(cl->uid)
                                //     , L"Значение", j
                                //     , L"Путь", ui->path
                                //     , L"Мета", v->fullpath(this, false));
                            }
                        }
                    }

                }

            } // конец цикла foreach (var item_puninitvalues in _puninitvalues)

            // TODO: с этим надо что-то делать....
            // _puninitvalues->clear();

        }

        // Если _useExternal истина, _stor принадлежит MetaContainer и удаляется в деструкторе. 
        // Иначе _stor принадлежит вызывающей стороне, и может быть удален сразу после выполнения конструктора
        public MetaContainer(ConfigStorage _stor, bool _useExternal = false)
        {
            String version_version;
            String s;
            String metaname;
            String emetaname;
            String typepref;
            String etypepref;
            String mtypename;

            int i, j;

            Tree tr = null;
            Tree t = null;

            MetaGeneratedType gt;
            Value1C_metaobj vo;
            Value1C_metaobj vvo;
            MetaObject mo;
            Value1C_stdtabsec metats;


            sig_standart_attribute = new Guid("03f171e8-326f-41c6-9fa5-932a0b12cddf");
            sig_standart_table_sec = new Guid("28db313d-dbc2-4b83-8c4a-d2aeee708062");
            sig_ext_dimension      = new Guid("91162600-3161-4326-89a0-4a7cecd5092a");
            sig_ext_dimension_type = new Guid("b3b48b29-d652-47ab-9d21-7e06768c31b5");

            ftypes = new MetaTypeSet();
            froot = null;

            useExternal = _useExternal;
            stor = _stor;
            storpresent = stor.presentation() + "\\";

            // ниже глобальная переменная ....
            // puninitvalues = &uninitvalues; 

            tr = gettree(stor, "version");
            if (tr == null)
            {
                stor = null;
                return;
            }

            try
            {
                t = tr.Get_First().Get_First().Get_First();
                i = Convert.ToInt32(t.Get_Value());
                t = t.Get_Next();
                j = Convert.ToInt32(t.Get_Value());
            }
            catch (Exception)
            {
                //error(L"Ошибка формата потока 82. Ошибка разбора файла version", L"Путь", storpresent + L"version");
                stor = null;
                return;
                //throw;
            }

            version_version = i.ToString() + "." + j.ToString();
            if (i == 2 && j == 0)
            {
                containerver = ContainerVer.cv_2_0;
                ver1C = Version1C.v1C_8_0;
            }
            else if (i == 5 && j == 0)
            {
                containerver = ContainerVer.cv_5_0;
                ver1C = Version1C.v1C_8_0;
            }
            else if (i == 6 && j == 0)
            {
                containerver = ContainerVer.cv_6_0;
                ver1C = Version1C.v1C_8_0;
            }
            else if (i == 106 && j == 0)
            {
                containerver = ContainerVer.cv_106_0;
                ver1C = Version1C.v1C_8_1;
            }
            else if (i == 200 && j == 0)
            {
                containerver = ContainerVer.cv_200_0;
                ver1C = Version1C.v1C_8_2;
            }
            else if (i == 202 && j == 2)
            {
                containerver = ContainerVer.cv_202_2;
                ver1C = Version1C.v1C_8_2;
            }
            else if (i == 216 && j == 0)
            {
                containerver = ContainerVer.cv_216_0;
                ver1C = Version1C.v1C_8_2_14;
            }
            else
            {
                // TODO : Необходимо реализовать вывод ошибки....
                // error(L"Ошибка формата потока 81. Неизвестная версия в файле version"
                //     , L"Путь", storpresent + L"version"
                //     , L"Версия", version_version);
                stor = null;
                return;
            }

            metaprefix = containerver < ContainerVer.cv_106_0 ? "metadata\\" : "";

            // Файл versions
            // пока не разбираем!

            // Файл root
            tr = gettree(stor, metaprefix + "root");
            if (tr is null)
            {
                stor = null;
                return;
            }

            try
            {
                t = tr.Get_First().Get_First();
                i = Convert.ToInt32(t.Get_Value());
                t = t.Get_Next();
                s = t.Get_Value();
            }
            catch (Exception)
            {
                //error(L"Ошибка формата потока 80. Ошибка разбора файла root", L"Путь", storpresent + metaprefix + L"version");
                stor = null;
                return;
                //throw;
            }

            if (i == 1)
            {
                if (containerver >= ContainerVer.cv_106_0)
                {
                    // TODO : Надо реализовывать
                    // error(L"Ошибка формата потока 79. Версия root не соответствует версии version"
                    //     , L"Версия root", i
                    //     , L"Версия version", version_version
                    //     , L"Путь", storpresent);
                    stor = null;
                    return;
                }
            }
            else if (i == 2)
            {
                if (containerver < ContainerVer.cv_106_0)
                {
                    // TODO : Надо реализовывать
                    // error(L"Ошибка формата потока 78. Версия root не соответствует версии version"
                    //     , L"Версия root", i
                    //     , L"Версия version", version_version
                    //     , L"Путь", storpresent);
                    stor = null;
                    return;
                }
            }
            else
            {
                // TODO : Надо реализовывать
                // error(L"Ошибка формата потока 77. Неизвестная версия root"
                //     , L"Версия root", i
                //     , L"Путь", storpresent);
                stor = null;
                return;
            }
            tr = gettree(stor, metaprefix + s);
            if (tr is null) return;
            t = tr.Get_First();
            metats = null;
            froot = (Value1C_obj)readValue1C(t, MetaTypeSet.mt_container, null, Constants.EmptyUID, metats, null, metaprefix + s, true);

            // Заполнение имен объектов метаданных
            foreach (var item_fmetamap in fmetamap)
            {
                mo = item_fmetamap.Value;
                vo = mo.Value;
                if (vo.type == MetaTypeSet.mt_container)
                {
                    mo.SetName("<Контейнер>");
                    mo.SetName("<Container>", true);
                }
                else
                {
                    s = vo.getproperty("Имя").presentation();
                    mo.SetName(s);
                    mo.SetName(s, true);
                }
            }

            foreach (var item_fmetamap in fmetamap)
            {
                mo = item_fmetamap.Value;
                vvo = mo.Value;
                metaname = mo.Name;
                emetaname = mo.EName;
                typepref = "";
                etypepref = "";
                mtypename = mo.Name;
                if (vvo.type != MetaTypeSet.mt_config)
                {
                    for (vo = (Value1C_metaobj)vvo.parent ; vo != null; vo = (Value1C_metaobj)vo.parent)
                    {
                        if (vo.type == MetaTypeSet.mt_config)
                            break;
                        if (vo.kind == KindOfValue1C.kv_metaobj)
                        {
                            metaname = vo.v_metaobj.Name + "." + metaname; //-V525
                            emetaname = vo.v_metaobj.EName + "." + emetaname;
                            mtypename = vo.v_metaobj.EName + "." + mtypename;
                            typepref = vo.type.GenTypePrefix + typepref;
                            etypepref = vo.type.EgenTypePrefix + etypepref;
                        }
                        else if (vo.type.Metaname.Length > 0)
                        {
                            metaname = vo.type.Metaname + "." + metaname;
                            emetaname = vo.type.Emetaname + "." + emetaname;
                        }
                    }
                }

                mo.SetFullName(metaname);
                mo.SetEfullName(emetaname);
                fsmetamap[metaname] = mo;
                fsmetamap[emetaname] = mo;

                foreach (var item_v_objgentypes in vvo.v_objgentypes)
                {
                    gt = item_v_objgentypes.Key;
                    if (gt.Woprefix)
                    {
                        metaname = typepref + gt.Name + "." + mtypename;
                        emetaname = etypepref + gt.EName + "." + mtypename;
                    }
                    else
                    {
                        metaname = typepref + vvo.type.GenTypePrefix + gt.Name + "." + mtypename;
                        emetaname = etypepref + vvo.type.EgenTypePrefix + gt.EName + "." + mtypename;
                    }
                    ftypes.Add(new MetaType(ftypes, metaname, emetaname, "", "", item_v_objgentypes.Value.typeuid));
                }
            }

            inituninitvalues();

            if (!useExternal)
            {
                stor = null;
            }
        }

        /// <summary>
        /// // Получить объект метаданных по имени
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public MetaObject getMetaObject(string n) 
        {
            if (fsmetamap.TryGetValue(n.ToUpper(), out MetaObject val1))
            {
                return val1;
            }
            else
            {
                return (MetaObject.smap.TryGetValue(n.ToUpper(), out MetaObject val2)) ? val2 : null;
            }
        }

        /// <summary>
        /// // Получить объект метаданных по УИД
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public MetaObject getMetaObject(Guid u) 
        {
            if (fmetamap.TryGetValue(u, out MetaObject val1))
            {
                return val1;
            }
            else
            {
                return (MetaObject.map.TryGetValue(u, out MetaObject val2)) ? val2 : null;
            }
        }

        public PredefinedValue getPreValue(Guid u)  // Получить предопределенный элемент по УИД
        {
            return fpredefinedvalues.TryGetValue(u, out PredefinedValue val) ? val : null;
        }

        public bool Export(string path, bool english = false, uint thread_count = 0)
        {
            #region cpp realization
            // String npath;
            // Value1C_obj_ExportThread* thr;
            // unsigned int i;
            // 
            // npath = String(L"\\\\?\\") + path;
            // if (!DirectoryExists(npath)) if (!CreateDir(npath)) return false;
            // if (!froot) return false;
            // 
            // export_thread_count = thread_count;
            // export_work_count = 0;
            // if (export_thread_count)
            // {
            //     export_threads = new Value1C_obj_ExportThread*[export_thread_count];
            //     for (i = 0; i < export_thread_count; ++i)
            //     {
            //         thr = new Value1C_obj_ExportThread(this);
            //         export_threads[i] = thr;
            //     }
            // }
            // 
            // ExportThread(froot, npath, english);
            // 
            // if (export_thread_count)
            // {
            //     while (export_work_count)
            //     {
            //         CheckSynchronize(10);
            //         //Sleep(0);
            //     }
            //     for (i = 0; i < export_thread_count; ++i)
            //     {
            //         thr = export_threads[i];
            //         thr->finish = true;
            //         thr->work->SetEvent();
            //     }
            //     for (i = 0; i < export_thread_count; ++i)
            //     {
            //         thr = export_threads[i];
            //         thr->WaitFor();
            //         delete thr;
            //     }
            //     delete[] export_threads;
            // }
            #endregion
            // TODO: после понимания реализации ExportThread
            return true;
        }

        public bool ExportThread(Value1C_obj v, string path, bool english)
        {
            #region cpp realization
            // unsigned int i;
            // Value1C_obj_ExportThread* thr;
            // bool multithread;
            // 
            // if (export_thread_count)
            // {
            //     if (cur_export_thread) multithread = !cur_export_thread->singlethread;
            //     else multithread = true;
            // }
            // else multithread = false;
            // 
            // 
            // if (multithread)
            // {
            //     for (i = 0; i < export_thread_count; ++i)
            //     {
            //         thr = export_threads[i];
            //         if (!InterlockedExchange(&thr->busy, 1))
            //         {
            //             thr->v = v;
            //             thr->path = path;
            //             thr->english = english;
            //             InterlockedIncrement(&export_work_count);
            //             thr->work->SetEvent();
            //             return true;
            //         }
            //     }
            // }
            // return v->Export(path, NULL, 0, english);
            #endregion
            return true;
        }

        public Value1C_obj root
        {
            get
            {
                return froot;
            }
              
        }

        public MetaTypeSet types
        {
            get
            {
                return ftypes;
            }

        }

        public SortedDictionary<Guid, MetaObject> metamap
        {
            get { return fmetamap; }
        }

        public SortedDictionary<string, MetaObject> smetamap
        {
            get { return fsmetamap; }
        }

        public SortedDictionary<Guid, PredefinedValue> predefinedvalues
        {
            get { return fpredefinedvalues; }
        }

        public long export_work_count; // количество заданий выгрузки

    }
}
