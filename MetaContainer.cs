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

        public GeneratedType(Tree ptr, string path)
        {
            typeuid = new Guid(Constants.EMPTY_GUID);
            valueuid = new Guid(Constants.EMPTY_GUID);
            if (ptr is null)
            {
                Form1.log.Error($"Ошибка формата потока 44. Ожидается значение UID генерируемого типа. Путь {path}");
                return;
            }
            if (ptr.Get_Type() == Node_Type.nd_guid)
            {
                if (!string_to_GUID(ptr.Get_Value(), ref typeuid))
                {
                    Form1.log.Error($"Ошибка формата потока 45. Ошибка преобразования UID генерируемого типа. " +
                                    $"UID {ptr.Get_Value()}, " +
                                    $"Путь {ptr.Path()}");
                }
            }
            else
            {
                Form1.log.Error($"Ошибка формата потока 46. Тип значения не UID. " +
                                $"Значение {ptr.Get_Value()}, " +
                                $"Путь {ptr.Path()}");
            }
            ptr = ptr.Get_Next();
            if (ptr is null)
            {
                Form1.log.Error($"Ошибка формата потока 47. Ожидается значение UID генерируемого типа. " +
                                $"Путь {path}");
                return;
            }
            if (ptr.Get_Type() == Node_Type.nd_guid)
            {
                if (!string_to_GUID(ptr.Get_Value(), ref typeuid))
                {
                    Form1.log.Error($"Ошибка формата потока 48. Ошибка преобразования UID генерируемого типа. " +
                                    $"UID {ptr.Get_Value()}, " +
                                    $"Путь {path + ptr.Path()}");
                }
            }
            else
            {
                Form1.log.Error($"Ошибка формата потока 49. Тип значения не UID. " +
                                $"Значение {ptr.Get_Value()}, " +
                                $"Путь {path + ptr.Path()}");
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

            if (!(owner is null))
            {
                if (owner.type != null)
                {
                    if (owner.type.gentypeRef != null)
                    {

                        if (owner.v_objgentypes.TryGetValue(owner.type.gentypeRef, out GeneratedType val))
                        {
                            MetaType t = owner.owner.types.GetTypeByUID(val.typeuid);
                            s = english ? t.EName : t.Name;
                            s += ".";
                        }
                    }
                }
            }
            s += name;

            return s;
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


    public static class PValues
    {
        public static List<UninitValue1C> puninitvalues;
    }

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
        public static List<UninitValue1C> puninitvalues = new List<UninitValue1C>();

        public SortedDictionary<Guid, MetaObject> fmetamap = new SortedDictionary<Guid, MetaObject>();          // Соответствие УИД объектам метаданных
        public SortedDictionary<string, MetaObject> fsmetamap = new SortedDictionary<string, MetaObject>();         // Соответствие полного имени объектам метаданных (на двух языках)
        public SortedDictionary<Guid, PredefinedValue> fpredefinedvalues = new SortedDictionary<Guid, PredefinedValue>(); // Соответствие УИД предопределенным элементам

        public ContainerVer containerver;
        public Version1C ver1C;

        public bool useExternal; // Использовать отложенную загрузку внешних файлов

        public string metaprefix;

        //public List<UninitValue1C> uninitvalues;

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
                            //puninitvalues.Add(new UninitValue1C(v, spath + tr.Path(), uid));
                            PValues.puninitvalues.Add(new UninitValue1C(v, spath + tr.Path(), uid));
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
                            PValues.puninitvalues.Add(new UninitValue1C(v, spath + tr.Path(), uid));
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
                            else
                            {
                                i = Convert.ToInt32(tr.Get_Value());
                                vsa.v_value = i;
                                if (metauid == EmptyUID)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 105. Ошибка загрузки стандартного реквизита. Не определён metauid."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", tr->get_value()
                                    //     , L"Путь", spath + tr->path());
                                }
                                else
                                {
                                    tt = tr;
                                    tr = tr.Get_Next();
                                    if (tr != null && i >= 0)
                                    {
                                        if (tr.Get_Type() == Node_Type.nd_guid)
                                        {
                                            uid = EmptyUID;
                                            string_to_GUID(tr.Get_Value(), ref uid);
                                            PValues.puninitvalues.Add(new UninitValue1C(v, spath + tt.Path(), metauid, uid, metats));
                                            tr = tr.Get_Next();
                                        }
                                        else
                                        {
                                            // TODO : Надо реализовывать
                                            // error(L"Ошибка формата потока 205. Ошибка загрузки стандартного реквизита. Тип значения не UID."
                                            //     , L"Загружаемый тип", t->name
                                            //     , L"Тип значения", tr->get_type()
                                            //     , L"Значение", tr->get_value()
                                            //     , L"Путь", spath + tr->path());
                                            PValues.puninitvalues.Add(new UninitValue1C(v, spath + tt.Path(), metauid, EmptyUID, metats));
                                        }
                                    }
                                    else
                                    {
                                        PValues.puninitvalues.Add(new UninitValue1C(v, spath + tt.Path(), metauid, EmptyUID, metats));
                                    }
                                }
                            }
                            ptr = tr;
                            break;
                        case 6:
                            vo = new Value1C_obj(valparent, this);
                            v = vo;
                            ve = new Value1C_enum(vo);
                            ve.type = MetaTypeSet.mt_datefractions;
                            ve.v_enum = null;
                            vo.v_objprop[MetaTypeSet.mp_datefractions] = ve;
                            if (tr.Get_Type() != Node_Type.nd_string)
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 111. Тип значения не Строка."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", tr->get_type()
                                //     , L"Значение", tr->get_value()
                                //     , L"Путь", spath + tr->path());
                            }
                            else
                            {
                                s = tr.Get_Value();
                                if (s.CompareTo("D") == 0)
                                {
                                    ve.v_enum = MetaTypeSet.mv_datefractionsdate;
                                }
                                else if (s.CompareTo("T") == 0)
                                {
                                    ve.v_enum = MetaTypeSet.mv_datefractionstime;
                                }
                                else
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 112. Неизвестное значение частей даты."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", s
                                    //     , L"Путь", spath + tr->path());
                                }
                            }
                            ptr = tr.Get_Next();
                            break;
                        case 7: // Стандартная табличная часть
                            vst = new Value1C_stdtabsec(valparent);
                            v = vst;
                            metats = vst;
                            vst.v_stdtabsec = null;
                            if (tr.Get_Type() == Node_Type.nd_number)
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 207. Тип значения не Число при загрузке стандартной табличной части."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", tr->get_type()
                                //     , L"Значение", tr->get_value()
                                //     , L"Путь", spath + tr->path());
                            }
                            else
                            {
                                i = Convert.ToInt32(tr.Get_Value());
                                vst.v_value = i;
                                if (metauid == EmptyUID)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 208. Ошибка загрузки стандартной табличной части. Не определён metauid."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Значение", tr->get_value()
                                    //     , L"Путь", spath + tr->path());
                                }
                                else
                                {
                                    PValues.puninitvalues.Add(new UninitValue1C(v, spath + tr.Path(), metauid));
                                }
                            }
                            ptr = tr.Get_Next();
                            break;
                        case 8:
                            vo = new Value1C_obj(valparent, this);
                            v = vo;
                            _metats = null;
                            vo.type = t;
                            if (tr.Get_Type() != Node_Type.nd_list)
                            {
                                // TODO : Надо реализовывать
                                // error(L"Ошибка формата потока 216. Ошибка загрузки типа МетаСсылка. Узел не является списком."
                                //     , L"Загружаемый тип", t->name
                                //     , L"Тип значения", tr->get_type()
                                //     , L"Значение", tr->get_value()
                                //     , L"Путь", spath + tr->path());

                            }
                            else
                            {
                                tt = tr.Get_First();
                                if (tt is null)
                                {
                                    // TODO : Надо реализовывать
                                    // error(L"Ошибка формата потока 217. Ошибка загрузки типа МетаСсылка. Нет значений."
                                    //     , L"Загружаемый тип", t->name
                                    //     , L"Путь", spath + tr->path());
                                }
                                else
                                {
                                    if (tt.Get_Type() != Node_Type.nd_number)
                                    {
                                        // TODO : Надо реализовывать
                                        // error(L"Ошибка формата потока 218. Ошибка загрузки типа МетаСсылка. Узел не является числом."
                                        //     , L"Загружаемый тип", t->name
                                        //     , L"Тип значения", tt->get_type()
                                        //     , L"Значение", tt->get_value()
                                        //     , L"Путь", spath + tt->path());
                                    }
                                    else
                                    {
                                        if (tt.Get_Value().CompareTo("1") != 0)
                                        {
                                            // TODO : Надо реализовывать
                                            // error(L"Ошибка формата потока 219 Ошибка загрузки типа МетаСсылка. Значение узла не равно 1."
                                            //     , L"Загружаемый тип", t->name
                                            //     , L"Значение", tt->get_value()
                                            //     , L"Путь", spath + tt->path());
                                        }
                                        tt = tt.Get_Next();
                                        if (tt.Get_Type() != Node_Type.nd_guid)
                                        {
                                            // TODO : Надо реализовывать
                                            // error(L"Ошибка формата потока 220. Ошибка загрузки типа МетаСсылка. Узел не UID."
                                            //     , L"Загружаемый тип", t->name
                                            //     , L"Тип значения", tt->get_type()
                                            //     , L"Значение", tt->get_value()
                                            //     , L"Путь", spath + tt->path());

                                        }
                                        else
                                        {
                                            ouid = EmptyUID;
                                            vro = new Value1C_refobj(valparent);
                                            vro.type = MetaTypeSet.mt_metarefint;
                                            if (!string_to_GUID(tt.Get_Value(), ref ouid))
                                            {
                                                // TODO : Надо реализовывать
                                                // error(L"Ошибка формата потока 221. Ошибка преобразования UID при загрузке ссылки на объект метаданных."
                                                //     , L"Загружаемый тип", t->name
                                                //     , L"Значение", tt->get_value()
                                                //     , L"Путь", spath + tt->path());
                                            }
                                            else if (ouid != EmptyUID)
                                            {
                                                PValues.puninitvalues.Add(new UninitValue1C(vro, spath + tr.Path(), ouid));
                                                vro.v_uid = ouid;
                                            }

                                            prop = t.GetProperty("Объект");
                                            vo.v_objprop[prop] = vro;

                                            tt = tt.Get_Next();
                                            if (tt.Get_Type() != Node_Type.nd_number)
                                            {
                                                // TODO : Надо реализовывать
                                                // error(L"Ошибка формата потока 222. Ошибка загрузки типа МетаСсылка. Узел не является числом."
                                                //     , L"Загружаемый тип", t->name
                                                //     , L"Тип значения", tt->get_type()
                                                //     , L"Значение", tt->get_value()
                                                //     , L"Путь", spath + tt->path());
                                            }
                                            else
                                            {
                                                vst = null;
                                                vsa = null;
                                                k = Convert.ToInt32(tt.Get_Value());
                                                for (i = 0; i < k; ++i)
                                                {
                                                    tt = tt.Get_Next();
                                                    if (tt.Get_Type() != Node_Type.nd_list)
                                                    {
                                                        // TODO : Надо реализовывать
                                                        // error(L"Ошибка формата потока 225. Ошибка загрузки типа МетаСсылка. Узел не является списком."
                                                        //     , L"Загружаемый тип", t->name
                                                        //     , L"Тип значения", tt->get_type()
                                                        //     , L"Значение", tt->get_value()
                                                        //     , L"Путь", spath + tt->path());
                                                    }
                                                    else
                                                    {
                                                        ttt = tt.Get_First();
                                                        if (ttt is null)
                                                        {
                                                            // TODO : Надо реализовывать
                                                            // error(L"Ошибка формата потока 226. Ошибка загрузки типа МетаСсылка. Нет значений."
                                                            //     , L"Загружаемый тип", t->name
                                                            //     , L"Путь", spath + tt->path());
                                                        }
                                                        else
                                                        {
                                                            tx = ttt;
                                                            ttt = ttt.Get_Next();
                                                            if (ttt is null)
                                                            {
                                                                // TODO : Надо реализовывать
                                                                // error(L"Ошибка формата потока 227. Ошибка загрузки типа МетаСсылка. Нет значений."
                                                                //     , L"Загружаемый тип", t->name
                                                                //     , L"Путь", spath + tx->path());
                                                            }
                                                            else
                                                            {
                                                                if (ttt.Get_Type() != Node_Type.nd_guid)
                                                                {
                                                                    // TODO : Надо реализовывать
                                                                    // error(L"Ошибка формата потока 228. Ошибка загрузки типа МетаСсылка. Узел не UID."
                                                                    //     , L"Загружаемый тип", t->name
                                                                    //     , L"Тип значения", ttt->get_type()
                                                                    //     , L"Значение", ttt->get_value()
                                                                    //     , L"Путь", spath + ttt->path());
                                                                }
                                                                else
                                                                {
                                                                    if (!string_to_GUID(ttt.Get_Value(), ref uid))
                                                                    {
                                                                        // TODO : Надо реализовывать
                                                                        // error(L"Ошибка формата потока 229. Ошибка преобразования UID при загрузке типа МетаСсылка."
                                                                        //     , L"Загружаемый тип", t->name
                                                                        //     , L"Значение", ttt->get_value()
                                                                        //     , L"Путь", spath + ttt->path());
                                                                    }
                                                                    else
                                                                    {
                                                                        if (uid == sig_standart_attribute)
                                                                        {
                                                                            if (i == k - 1) // стандартный реквизит читаем только если он последний
                                                                            {
                                                                                vv = readValue1C(tx, MetaTypeSet.mt_standart_attribute, vo, ouid, _metats, clitem, path, true);
                                                                                prop = t.GetProperty("СтандартныйРеквизит");
                                                                                vo.v_objprop[prop] = vv;
                                                                            }
                                                                        }
                                                                        else if (uid == sig_standart_table_sec)
                                                                        {
                                                                            vv = readValue1C(tx, MetaTypeSet.mt_standart_tabular_section, vo, ouid, _metats, clitem, path, true);
                                                                            prop = t.GetProperty("СтандартнаяТабличнаяЧасть");
                                                                            vo.v_objprop[prop] = vv;
                                                                        }
                                                                        else if (uid == sig_ext_dimension || uid == sig_ext_dimension_type)
                                                                        {
                                                                            vv = readValue1C(tx, MetaTypeSet.mt_standart_attribute, vo, ouid, _metats, clitem, path, true);
                                                                            prop = t.GetProperty("СтандартныйРеквизит");
                                                                            vo.v_objprop[prop] = vv;
                                                                        }
                                                                        else
                                                                        {
                                                                            // TODO : Надо реализовывать
                                                                            // error(L"Ошибка формата потока 230. Неизвестная сигнатура при загрузке типа МетаСсылка."
                                                                            //     , L"Загружаемый тип", t->name
                                                                            //     , L"Значение", ttt->get_value()
                                                                            //     , L"Путь", spath + ttt->path());
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                }

                                                tt = tt.Get_Next();
                                                if (tt.Get_Type() != Node_Type.nd_number)
                                                {
                                                    // TODO : Надо реализовывать
                                                    // error(L"Ошибка формата потока 223. Ошибка загрузки типа МетаСсылка. Узел не является числом."
                                                    //     , L"Загружаемый тип", t->name
                                                    //     , L"Тип значения", tt->get_type()
                                                    //     , L"Значение", tt->get_value()
                                                    //     , L"Путь", spath + tt->path());
                                                }
                                                else
                                                {
                                                    vb = new Value1C_bool(vo);
                                                    if (tt.Get_Value().CompareTo("0") == 0)
                                                        b = false;
                                                    else if (tt.Get_Value().CompareTo("1") == 0)
                                                        b = true;
                                                    else
                                                    {
                                                        // TODO : Надо реализовывать
                                                        // error(L"Ошибка формата потока 224. Ошибка загрузки типа Булево."
                                                        //     , L"Загружаемый тип", t->name
                                                        //     , L"Значение", tt->get_value()
                                                        //     , L"Путь", spath + tt->path());
                                                        b = false;
                                                    }
                                                    vb.v_bool = b;
                                                    prop = t.GetProperty("Подчиненный");
                                                    vo.v_objprop[prop] = vb;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ptr = tr.Get_Next();
                            break;
                        default:
                            // TODO : Надо реализовывать
                            // error(L"Ошибка формата потока 16. Неизвестный вариант сериализации."
                            //     , L"Загружаемый тип", t->name
                            //     , L"Вариант сериализации", t->serialization_ver
                            //     , L"Путь", spath + tr->path());
                            ptr = tr.Get_Next();
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
            Value1C nv;
            Value1C_refobj vro;
            Value1C_refpre vrp;
            Value1C_binary vb;
            Value1C_right vr;
            Guid uid = EmptyUID;
            MetaType t;
            Tree tt;
            Tree tte;
            string npath = "";
            string spath = "";
            string sn = "";

            spath = storpresent + path;
            nv = null;
            t = tn.owner;

            if (tn.isref)
            {
                vro = new Value1C_refobj(valparent);
                nv = vro;
                vro.type = nt;
                if (tr.Get_Type() == Node_Type.nd_guid)
                {
                    if (!string_to_GUID(tr.Get_Value(), ref uid))
                    {
                        Form1.log.Error("Ошибка формата потока 83. Ошибка преобразования UID при загрузке ссылки на объект метаданных. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Значение {2}, " +
                                        "Путь {3}",
                                        t.Name,
                                        tn.path(),
                                        tr.Get_Value(),
                                        spath + tr.Path());
                    }
                    else if (uid != EmptyUID)
                    {
                        PValues.puninitvalues.Add(new UninitValue1C(nv, spath + tr.Path(), uid));
                        vro.v_uid = uid;
                    }
                }
                else
                {
                    Form1.log.Error("Ошибка формата потока 84. Тип значения не UID при загрузке ссылки на объект метаданных. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь ДС {1}, " +
                                    "Значение {2}, " +
                                    "Тип значения {3}, " +
                                    "Путь {4}",
                                    t.Name,
                                    tn.path(),
                                    tr.Get_Value(),
                                    get_node_type_presentation(tr.Get_Type()),
                                    spath + tr.Path());
                }
                //if(!tn->nomove) tr = tr->get_next();
                tr = tr.Get_Next();
            }
            else if (tn.isrefpre)
            {
                vrp = new Value1C_refpre(valparent);
                nv = vrp;
                vrp.type = nt;
                if (tr.Get_Type() == Node_Type.nd_guid)
                {
                    if (!string_to_GUID(tr.Get_Value(), ref uid))
                    {
                        Form1.log.Error("Ошибка формата потока 163. Ошибка преобразования UID при загрузке ссылки на предопределенный элемент. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Значение {2}, " +
                                        "Путь {3}",
                                        t.Name,
                                        tn.path(),
                                        tr.Get_Value(),
                                        spath + tr.Path());
                    }
                    else if (uid != EmptyUID)
                        PValues.puninitvalues.Add(new UninitValue1C(nv, spath + tr.Path(), uid));
                }
                else
                {
                    Form1.log.Error("Ошибка формата потока 164. Тип значения не UID при загрузке ссылки на предопределенный элемент. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь ДС {1}, " +
                                    "Значение {2}, " +
                                    "Тип значения {3}, " +
                                    "Путь {4}",
                                    t.Name,
                                    tn.path(),
                                    tr.Get_Value(),
                                    get_node_type_presentation(tr.Get_Type()),
                                    spath + tr.Path());
                }
                //if(!tn->nomove) tr = tr->get_next();
                tr = tr.Get_Next();
            }
            else if (tn.isright)
            {
                vr = new Value1C_right(valparent);
                nv = vr;
                vr.type = nt;
                if (tr.Get_Type() == Node_Type.nd_guid)
                {
                    if (!string_to_GUID(tr.Get_Value(), ref uid))
                    {
                        Form1.log.Error("Ошибка формата потока 213. Ошибка преобразования UID при загрузке права. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Значение {2}, " +
                                        "Путь {3}",
                                        t.Name,
                                        tn.path(),
                                        tr.Get_Value(),
                                        spath + tr.Path());
                    }
                    else
                    {
                        vr.v_right = MetaRight.GetRight(uid);
                        if (vr.v_right is null)
                        {
                            Form1.log.Error("Ошибка формата потока 215. Неизвестное право. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "UID права {2}, " +
                                            "Путь {3}",
                                            t.Name,
                                            tn.path(),
                                            tr.Get_Value(),
                                            spath + tr.Path());
                        }
                    }
                }
                else
                {
                    Form1.log.Error("Ошибка формата потока 214. Тип значения не UID при загрузке права. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь ДС {1}, " +
                                    "Значение {2}, " +
                                    "Тип значения {3}, " +
                                    "Путь {4}",
                                    t.Name,
                                    tn.path(),
                                    tr.Get_Value(),
                                    get_node_type_presentation(tr.Get_Type()),
                                    spath + tr.Path());
                }
                //if(!tn->nomove) tr = tr->get_next();
                tr = tr.Get_Next();
            }
            else if (tn.exnernal)
            {
                if (tr.Get_Type() == Node_Type.nd_guid)
                {
                    if (!string_to_GUID(tr.Get_Value(), ref uid))
                    {
                        Form1.log.Error("Ошибка формата потока 85. Ошибка преобразования UID при загрузке имени файла. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Значение {2}, " +
                                        "Путь {3}",
                                        t.Name,
                                        tn.path(),
                                        tr.Get_Value(),
                                        spath + tr.Path());
                        /////////////////////////////////////////nv = new Value1C(valparent);
                        /////////////////////////////////////////nv->type = nt;
                    }
                    else
                    {
                        npath = metaprefix + tr.Get_Value();
                        tt = gettree(stor, npath);
                        if (tt is null)
                        {
                            Form1.log.Error("Ошибка формата потока 76. Пустой файл. " +
                                            "Путь {0}",
                                            storpresent + npath);
                            /////////////////////////////////////////////////////nv = new Value1C(valparent);
                            /////////////////////////////////////////////////////nv->type = nt;
                        }
                        else
                        {
                            tte = tt.Get_First();
                            nv = readValue1C(tte, nt, valparent, metauid, metats, clitem, npath);
                        }
                    }
                }
                else
                {
                    Form1.log.Error("Ошибка формата потока 86. Тип значения не UID при загрузке имени файла. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь ДС {1}, " +
                                    "Значение {2}, " +
                                    "Тип значения {3}, " +
                                    "Путь {4}",
                                    t.Name,
                                    tn.path(),
                                    tr.Get_Value(),
                                    get_node_type_presentation(tr.Get_Type()),
                                    spath + tr.Path());
                    ///////////////////////////////////////////nv = new Value1C(valparent);
                    ///////////////////////////////////////////nv->type = nt;
                }

                tr = tr.Get_Next();
            }
            else if (nt == MetaTypeSet.mt_binarydata)
            {
                vb = new Value1C_binary(valparent);
                vb.type = nt;
                nv = vb;
                switch (tn.binsertype)
                {
                    case BinarySerializationType.bst_empty:
                        if (tr.Get_Type() == Node_Type.nd_binary2)
                        {
                            vb.v_binary = new MemoryStream();
                            //base64_decode(tr->get_value(), vb->v_binary, 0);
                            // TODO : Реализация под большим вопросом, надо сильно проверять и тестировать....
                            byte[] data = Convert.FromBase64String(tr.Get_Value());
                            string decodedString = Encoding.UTF8.GetString(data);
                            vb.v_binary.Write(data, 0, data.Length);
                        }
                        else
                        {
                            Form1.log.Error("Ошибка формата потока 146. Несовпадение вида сериализации двоичных данных. Ожидается строка base64 без префикса. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Значение {2}, " +
                                            "Тип значения {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            tr.Get_Value(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            spath + tr.Path());
                        }
                        break;
                    case BinarySerializationType.bst_base64:
                        if (tr.Get_Type() == Node_Type.nd_binary)
                        {
                            //vb->v_binary = new TTempStream();
                            vb.v_binary = new MemoryStream();
                            //base64_decode(tr->get_value(), vb->v_binary, 8);
                            // TODO : Реализация под большим вопросом, надо сильно проверять и тестировать....
                            byte[] data = Convert.FromBase64String(tr.Get_Value());
                            string decodedString = Encoding.UTF8.GetString(data);
                            vb.v_binary.Write(data, 8, data.Length);
                        }
                        else
                        {
                            Form1.log.Error("Ошибка формата потока 147. Несовпадение вида сериализации двоичных данных. Ожидается строка base64 c префиксом #base64:. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Значение {2}, " +
                                            "Тип значения {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            tr.Get_Value(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            spath + tr.Path());
                        }
                        break;
                    case BinarySerializationType.bst_data:
                        if (tr.Get_Type() == Node_Type.nd_binary_d)
                        {
                            vb.v_binary = new MemoryStream();
                            // TODO : Реализация под большим вопросом, надо сильно проверять и тестировать....
                            byte[] data = Convert.FromBase64String(tr.Get_Value());
                            string decodedString = Encoding.UTF8.GetString(data);
                            vb.v_binary.Write(data, 6, data.Length);
                        }
                        else
                        {
                            Form1.log.Error("Ошибка формата потока 148. Несовпадение вида сериализации двоичных данных. Ожидается строка base64 c префиксом #data:. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Значение {2}, " +
                                            "Тип значения {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            tr.Get_Value(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            spath + tr.Path());
                        }
                        break;
                    case BinarySerializationType.bst_base64_or_data:
                        if (tr.Get_Type() == Node_Type.nd_binary || tr.Get_Type() == Node_Type.nd_binary_d)
                        {
                            vb.v_binary = new MemoryStream();
                            //base64_decode(tr->get_value(), vb->v_binary, tr->get_type() == nd_binary ? 8 : 6);
                            // TODO : Реализация под большим вопросом, надо сильно проверять и тестировать....
                            byte[] data = Convert.FromBase64String(tr.Get_Value());
                            string decodedString = Encoding.UTF8.GetString(data);
                            vb.v_binary.Write(data, tr.Get_Type() == Node_Type.nd_binary ? 8 : 6, data.Length);
                        }
                        else
                        {
                            Form1.log.Error("Ошибка формата потока 153. Несовпадение вида сериализации двоичных данных. Ожидается строка base64 c префиксом #base64: или #data:. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Значение {2}, " +
                                            "Тип значения {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            tr.Get_Value(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            spath + tr.Path());
                        }
                        break;
                    case BinarySerializationType.bst_min:
                        Form1.log.Error("Ошибка формата потока 149. Не указан вид сериализации двоичных данных. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Значение {2}, " +
                                        "Тип значения {3}, " +
                                        "Путь {4}",
                                        t.Name,
                                        tn.path(),
                                        tr.Get_Value(),
                                        get_node_type_presentation(tr.Get_Type()),
                                        spath + tr.Path());
                        break;
                    default:
                        Form1.log.Error("Ошибка формата потока 150. Неизвестный вид сериализации двоичных данных. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Вид сериализации {2}, " +
                                        "Тип значения {3}, " +
                                        "Путь {4}",
                                        t.Name,
                                        tn.path(),
                                        tn.binsertype,
                                        get_node_type_presentation(tr.Get_Type()),
                                        spath + tr.Path());
                        break;
                }
                vb.v_binformat = tn.binformat;
                if (tn.binformat == ExternalFileFormat.eff_min)
                {
                    Form1.log.Error("Ошибка формата потока 151. Не указан формат двоичных данных. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь ДС {1}, " +
                                    "Путь {2}",
                                    t.Name,
                                    tn.path(),
                                    spath + tr.Path());
                }
                else if (tn.binformat >= ExternalFileFormat.eff_max)
                {
                    Form1.log.Error("Ошибка формата потока 152. Неизвестный формат двоичных данных. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь ДС {1}, " +
                                    "Формат {2}, " +
                                    "Путь {3}",
                                    t.Name,
                                    tn.path(),
                                    tn.binformat,
                                    spath + tr.Path());
                    vb.v_binformat = ExternalFileFormat.eff_min;
                }

                tr = tr.Get_Next();
            }
            else
                nv = readValue1C(tr, nt, valparent, metauid, metats, clitem, path);

            return nv;
        }

        public void recursiveLoadValue1C(Value1C_obj v, VarValue[] varvalues, Tree ptr, SerializationTreeNode tn, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path, bool checkend = false)
        {
            MetaType t;
            Guid uid = EmptyUID;
            Tree tr;
            Tree tt;
            Value1C nv;
            Value1C_metaobj vm;
            Value1C_metaobj vvm;
            Value1C_obj vo;
            Value1C_string vs;
            Value1C_number vn;
            Value1C vv = null;
            Value1C vvv;
            MetaType nt;
            int i, j;
            GeneratedType gt;
            SerializationTreeNode tnn;

            SerializationTreeValueType vt1, vt2;
            string sv1 = "";
            string sv2 = "";
            int nv1 = 0;
            int nv2 = 0;
            Guid uv1 = EmptyUID;
            Guid uv2 = EmptyUID;
            bool cv, ok;
            
            MetaProperty prop;
            Class cl;
            ClassItem cli = null;
            
            string spath;
            MetaValue mv;

            int ii;
            Guid u = EmptyUID;
            string n = "";
            PredefinedValue pre;
            MetaProperty p;
            DefaultValueType dvt;

            spath = storpresent + path;
            tr = ptr;
            if (tn is null)
            {
                if (checkend)
                {
                    if (tr != null)
                    {
                        Form1.log.Error("Ошибка формата потока 63. Остались необработанные значения. " +
                                        "Путь {0}", spath + tr.Path());
                    }
                }
                return;
            }

            t = tn.owner;

            for (; tn != null; tn = tn.next)
            {
                switch (tn.type)
                {
                    case SerializationTreeNodeType.stt_const:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 27. Отсутствует ожидаемое значение константы. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}", t.Name, tn.path(), spath);
                            break;
                        }

                        switch (tn.typeval1)
                        {
                            case SerializationTreeValueType.stv_string:
                                if (tr.Get_Type() == Node_Type.nd_string)
                                {
                                    if (tr.Get_Value().CompareTo(tn.str1) != 0)
                                    {
                                        Form1.log.Error("Ошибка формата потока 26. Значение не совпадает с константой дерева сериализации. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Значение {2}, " +
                                                        "Значение константы {3}, " +
                                                        "Путь {4}", 
                                                        t.Name,
                                                        tn.path(),
                                                        tr.Get_Value(),
                                                        tn.str1,
                                                        spath + tr.Path());
                                    }
                                }
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 25. Тип значения не совпадает с типом значения константы дерева сериализации. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Тип значения {2}, " +
                                                    "Тип значения константы {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    get_node_type_presentation(tr.Get_Type()),
                                                    tn.typeval1presentation(),
                                                    spath + tr.Path() );
                                }
                                break;
                            case SerializationTreeValueType.stv_number:
                                if (tr.Get_Type() == Node_Type.nd_number)
                                {
                                    if (Convert.ToInt32(tr.Get_Value()) != tn.uTreeNode1.num1)
                                    {
                                        Form1.log.Error("Ошибка формата потока 28. Значение не совпадает с константой дерева сериализации. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Значение {2}, " +
                                                        "Значение константы {3}, " +
                                                        "Путь {4}",
                                                        t.Name,
                                                        tn.path(),
                                                        tr.Get_Value(),
                                                        tn.uTreeNode1.num1,
                                                        spath + tr.Path());
                                    }

                                }
                                else if (tr.Get_Type() == Node_Type.nd_number_exp)
                                {
                                    if (Convert.ToDouble(tr.Get_Value()) != (double)tn.uTreeNode1.num1)
                                    {
                                        Form1.log.Error("Ошибка формата потока 235. Значение не совпадает с константой дерева сериализации. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Значение {2}, " +
                                                        "Значение константы {3}, " +
                                                        "Путь {4}",
                                                        t.Name,
                                                        tn.path(),
                                                        tr.Get_Value(),
                                                        tn.uTreeNode1.num1,
                                                        spath + tr.Path());
                                    }

                                }
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 29. Тип значения не совпадает с типом значения константы дерева сериализации. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Тип значения {2}, " +
                                                    "Тип значения константы {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    get_node_type_presentation(tr.Get_Type()),
                                                    tn.typeval1presentation(),
                                                    spath + tr.Path());
                                }
                                break;
                            case SerializationTreeValueType.stv_uid:
                                if (tr.Get_Type() == Node_Type.nd_guid)
                                {
                                    if (!string_to_GUID(tr.Get_Value(), ref uid))
                                    {
                                        Form1.log.Error("Ошибка формата потока 30. Ошибка преобразования UID при загрузке константы. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Значение {2}, " +
                                                        "Значение константы {3}, " +
                                                        "Путь {4}",
                                                        t.Name,
                                                        tn.path(),
                                                        tr.Get_Value(),
                                                        tn.uTreeNode1.uid1.ToString(),
                                                        spath + tr.Path());
                                    }
                                    else if (uid != tn.uTreeNode1.uid1)
                                    {
                                        Form1.log.Error("Ошибка формата потока 31. Значение не совпадает с константой дерева сериализации. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Значение {2}, " +
                                                        "Значение константы {3}, " +
                                                        "Путь {4}",
                                                        t.Name,
                                                        tn.path(),
                                                        tr.Get_Value(),
                                                        tn.uTreeNode1.uid1.ToString(),
                                                        spath + tr.Path());
                                    }
                                }
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 32. Тип значения не совпадает с типом значения константы дерева сериализации. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Тип значения {2}, " +
                                                    "Тип значения константы {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    get_node_type_presentation(tr.Get_Type()),
                                                    tn.typeval1presentation(),
                                                    spath + tr.Path());
                                }
                                break;
                            default:
                                Form1.log.Error("Ошибка формата потока 24. Недопустимыйый тип значения константы дерева сериализации. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Тип значения {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tn.typeval1presentation(),
                                                spath + tr.Path());
                                break;
                        }
                        if (!tn.nomove)
                            tr = tr.Get_Next();
                        break;
                    case SerializationTreeNodeType.stt_var:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 94. Отсутствует ожидаемое значение переменной. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath + tr.Path());
                            break;
                        }
                        if (tr.Get_Type() == Node_Type.nd_number)
                            setVarValue(tn.str1, t, v, varvalues, clitem, Convert.ToInt32(tr.Get_Value()), spath);
                        else
                        {
                            Form1.log.Error("Ошибка формата потока 33. Тип переменной не Число. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Тип переменной {2}, " +
                                            "Имя переменной {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            tn.str1,
                                            spath + tr.Path());
                        }
                        if (!tn.nomove)
                            tr = tr.Get_Next();
                        break;
                    case SerializationTreeNodeType.stt_list:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 95. Отсутствует ожидаемый список. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath);
                            break;
                        }
                        if (tr.Get_Type() != Node_Type.nd_list)
                        {
                            Form1.log.Error("Ошибка формата потока 35. Значение не является списком. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath + tr.Path());
                        }
                        else
                        {
                            tt = tr.Get_First();
                            recursiveLoadValue1C(v, varvalues, tt, tn.first, metauid, metats, clitem, path, true);
                        }
                        if (!tn.nomove)
                            tr = tr.Get_Next();
                        break;
                    case SerializationTreeNodeType.stt_prop:
                        if (tr is null)
                        {
                            if (!tn.nomove)
                                Form1.log.Error("Ошибка формата потока 96. Отсутствует ожидаемое значение свойства. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Путь {2}",
                                                t.Name,
                                                tn.path(),
                                                spath);
                            break;
                        }
                        if (tn.isref)
                            nt = MetaTypeSet.mt_metarefint; // Для ссылки на объект метаданных жестко задаем тип ОбъектМетаданныхСсылкаВнутр
                        else
                        {
                            nt = tn.typeprop;
                            if (nt is null)
                            {
                                if (tn.uTreeNode1.prop1.Types.Count == 1)
                                    nt = tn.uTreeNode1.prop1.Types[0];
                            }
                        }

                        //_metats = metats;
                        prop = tn.uTreeNode2.prop2;
                        if (prop != null)
                        {
                            vv = v.getproperty(prop);
                            if (vv != null)
                            {
                                if (vv.kind == KindOfValue1C.kv_obj && vv.type == MetaTypeSet.mt_metaobjref) // ОбъектМетаданныхСсылка
                                {
                                    vv = ((Value1C_obj)vv).getproperty("ОбъектМетаданных");
                                }
                            }
                            if (vv != null)
                            {
                                if (vv.kind == KindOfValue1C.kv_metaobj)
                                {
                                    if (((Value1C_metaobj)vv).v_metaobj != null)
                                        uv1 = ((Value1C_metaobj)vv).v_metaobj.UID;
                                    else
                                    {
                                        Form1.log.Error("Ошибка формата потока 199. Ошибка получения metauid из свойства. metauid еще не загружен. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        prop.Name,
                                                        spath);
                                        uv1 = EmptyUID;
                                    }
                                }
                                else if (vv.kind == KindOfValue1C.kv_refobj) uv1 = ((Value1C_refobj)vv).v_uid;
                                else if (vv.kind == KindOfValue1C.kv_obj && vv.type == MetaTypeSet.mt_tabsection)
                                {
                                    vvv = ((Value1C_obj)vv).getproperty("ТабличнаяЧасть");
                                    if (vvv != null)
                                    {
                                        if (vvv.kind == KindOfValue1C.kv_refobj)
                                            uv1 = ((Value1C_refobj)vvv).v_uid;
                                        else if (vvv.kind == KindOfValue1C.kv_stdtabsec)
                                            metats = (Value1C_stdtabsec)vvv;
                                    }
                                }
                                else if (vv.kind == KindOfValue1C.kv_obj && vv.type == MetaTypeSet.mt_metaref)
                                {
                                    vvv = ((Value1C_obj)vv).getproperty("Объект");
                                    if (vvv != null)
                                        uv1 = ((Value1C_refobj)vvv).v_uid;
                                    else
                                    {
                                        Form1.log.Error("Ошибка формата потока 231. Ошибка получения свойства Объект из значения типа МетаСсылка. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        prop.Name,
                                                        spath);
                                        uv1 = EmptyUID;
                                    }
                                }
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 103. Ошибка получения metauid из свойства. Свойство имеет недопустимый тип. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Свойство {2}, " +
                                                    "Тип значения {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    prop.Name,
                                                    KindOfValue1C_presantation(vv.kind),
                                                    spath);
                                    uv1 = EmptyUID;
                                }
                            }
                            else
                            {
                                Form1.log.Error("Ошибка формата потока 102. Ошибка получения metauid из свойства. Свойство не определено. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Свойство {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                prop.Name,
                                                spath);
                                uv1 = EmptyUID;
                            }
                        }
                        else
                            uv1 = metauid;
                        nv = readValue1C(nt, tn, tr, v, uv1, metats, clitem, path);
                        prop = tn.uTreeNode1.prop1;
                        v.v_objprop[prop] = nv;

                        //==> Обработка предопределенных значений
                        if (prop.Predefined)
                        {
                            if (nv.kind != KindOfValue1C.kv_obj)
                            {
                                Form1.log.Error("Ошибка формата потока 195. Ошибка получения значения предопределенного элемента. Значение свойства не является объектом. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Свойство {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                prop.Name,
                                                spath);
                            }
                            else if (v.kind != KindOfValue1C.kv_metaobj)
                            {
                                Form1.log.Error("Ошибка формата потока 196. Ошибка получения значения предопределенного элемента. Значение, которому принадлежит свойство с признаком \"Предопределенные\" не является объектом метаданных. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Свойство {2}, " +
                                                "Путь {3}, " +
                                                "Тип владельца {4}",
                                                t.Name,
                                                tn.path(),
                                                prop.Name,
                                                spath,
                                                KindOfValue1C_presantation(v.kind));
                            }
                            else
                            {
                                vm = (Value1C_metaobj)v;
                                vo = (Value1C_obj)nv;
                                for (ii = 0; ii < vo.v_objcol.Count; ++ii)
                                {
                                    vvm = (Value1C_metaobj)vo.v_objcol[ii];
                                    if (vvm is null)
                                    {
                                        Form1.log.Error("Ошибка формата потока 187. Ошибка получения значения предопределенного элемента. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        prop.Name,
                                                        spath);
                                        continue;
                                    }
                                    if (vvm.kind != KindOfValue1C.kv_metaobj)
                                    {
                                        Form1.log.Error("Ошибка формата потока 188. Ошибка получения значения предопределенного элемента. Значение не является объектом метаданных. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}, " +
                                                        "Тип значения {4}",
                                                        t.Name,
                                                        tn.path(),
                                                        prop.Name,
                                                        spath, 
                                                        KindOfValue1C_presantation(v.kind));
                                        continue;
                                    }
                                    u = vvm.v_metaobj.UID;
                                    if (u != EmptyUID)
                                    {
                                        nv = vvm.getproperty("Имя");
                                        if (nv is null)
                                        {
                                            Form1.log.Error("Ошибка формата потока 189. Ошибка получения имени предопределенного элемента. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Свойство {2}, " +
                                                            "Путь {3}",
                                                            t.Name,
                                                            tn.path(),
                                                            prop.Name,
                                                            spath);
                                            continue;
                                        }
                                        if (nv.kind != KindOfValue1C.kv_string && nv.kind != KindOfValue1C.kv_binary)
                                        {
                                            Form1.log.Error("Ошибка формата потока 190. Ошибка получения имени предопределенного элемента. Тип значения не Строка. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Свойство {2}, " +
                                                            "Путь {3}," +
                                                            "Тип значения {4}",
                                                            t.Name,
                                                            tn.path(),
                                                            prop.Name,
                                                            spath,
                                                            KindOfValue1C_presantation(nv.kind));
                                            continue;
                                        }
                                        n = nv.presentation();
                                        pre = new PredefinedValue(n, u, vm);
                                        vm.v_prevalues.Add(pre);
                                        fpredefinedvalues[u] = pre;
                                    }
                                }
                            }
                        }
                        //<== Обработка предопределенных значений
                        break;
                    case SerializationTreeNodeType.stt_elcol:
                        nt = tn.typeprop;
                        if (nt is null)
                        {
                            if (t.CollectionTypes.Count == 1)
                                nt = t.CollectionTypes[0];
                        }

                        if (tn.typeval1 == SerializationTreeValueType.stv_none)
                        {
                            for (tt = tr; tt != null;)
                            {
                                nv = readValue1C(nt, tn, tt, v, metauid, metats, clitem, path);
                                v.v_objcol.Add(nv);
                            }
                        }
                        else
                        {
                            j = 0;
                            switch (tn.typeval1)
                            {
                                case SerializationTreeValueType.stv_number:
                                    j = tn.uTreeNode1.num1;
                                    break;
                                case SerializationTreeValueType.stv_prop:
                                    vn = (Value1C_number)v.getproperty(tn.uTreeNode1.prop1);
                                    if (vn is null)
                                    {
                                        Form1.log.Error("Ошибка формата потока 40. Не установлено значение свойства, являющегося счетчиком коллекции. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        tn.uTreeNode1.prop1.Name,
                                                        spath + tr.Path());
                                    }
                                    else if (vn.kind != KindOfValue1C.kv_number)
                                    {
                                        Form1.log.Error("Ошибка формата потока 41. Cвойство, являющееся счетчиком коллекции, имеет значение типа не Число. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        tn.uTreeNode1.prop1.Name,
                                                        spath + tr.Path());
                                    }
                                    else
                                        j = vn.v_number;
                                    break;
                                case SerializationTreeValueType.stv_var:
                                    j = getVarValue(tn.str1, t, varvalues, clitem, spath);
                                    break;
                                default:
                                    Form1.log.Error("Ошибка формата потока 42. Недопусимый тип значения счетчика коллекции дерева сериализации. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Тип значения {2}, " +
                                                    "Путь {3}",
                                                    t.Name,
                                                    tn.path(),
                                                    tn.typeval1presentation(),
                                                    spath + tr.Path());
                                    break;
                            }
                            for (tt = tr, i = 0; i < j; ++i)
                            {
                                nv = readValue1C(nt, tn, tt, v, metauid, metats, clitem, path);
                                v.v_objcol.Add(nv);
                            }
                        }
                        if (!tn.nomove)
                            tr = tt;
                        break;
                    case SerializationTreeNodeType.stt_gentype:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 97. Отсутствует ожидаемое значение генерируемого типа. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath);
                            break;
                        }
                        if (v.kind != KindOfValue1C.kv_metaobj)
                        {
                            Form1.log.Error("Ошибка формата потока 197. Попытка загрузки генерируемого типа для значения, не являющегося объектом метаданных. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath + tr.Path());
                            break;
                        }
                        gt = new GeneratedType(tr, spath);
                        ((Value1C_metaobj)v).v_objgentypes[tn.uTreeNode1.gentype] = gt;
                        break;
                    case SerializationTreeNodeType.stt_cond:
                        cv = false;

                        // Значение 1
                        vt1 = SerializationTreeValueType.stv_none;
                        switch (tn.typeval1)
                        {
                            case SerializationTreeValueType.stv_string:
                                vt1 = SerializationTreeValueType.stv_string;
                                sv1 = tn.str1;
                                break;
                            case SerializationTreeValueType.stv_number:
                                vt1 = SerializationTreeValueType.stv_number;
                                nv1 = tn.uTreeNode1.num1;
                                break;
                            case SerializationTreeValueType.stv_uid:
                                vt1 = SerializationTreeValueType.stv_uid;
                                uv1 = tn.uTreeNode1.uid1;
                                break;
                            case SerializationTreeValueType.stv_value:
                                mv = tn.uTreeNode1.val1;
                                if (mv.ValueUID == EmptyUID)
                                {
                                    vt1 = SerializationTreeValueType.stv_number;
                                    nv1 = mv.Value;
                                }
                                else
                                {
                                    vt1 = SerializationTreeValueType.stv_uid;
                                    uv1 = mv.ValueUID;
                                }
                                break;
                            case SerializationTreeValueType.stv_var:
                                vt1 = SerializationTreeValueType.stv_number;
                                nv1 = getVarValue(tn.str1, t, varvalues, clitem, spath);
                                break;
                            case SerializationTreeValueType.stv_prop:
                                p = tn.uTreeNode1.prop1;

                                if (!v.v_objprop.TryGetValue(p, out Value1C val))
                                {
                                    dvt = p.defaultvaluetype;
                                    switch (dvt)
                                    {
                                        case DefaultValueType.dvt_bool:
                                            vt1 = SerializationTreeValueType.stv_number;
                                            nv1 = p.dv_union_type.dv_bool ? 1 : 0;
                                            break;
                                        case DefaultValueType.dvt_number:
                                            vt1 = SerializationTreeValueType.stv_number;
                                            nv1 = p.dv_union_type.dv_number;
                                            break;
                                        case DefaultValueType.dvt_string:
                                            vt1 = SerializationTreeValueType.stv_string;
                                            sv1 = p.dv_union_type.dv_string;
                                            break;
                                        case DefaultValueType.dvt_type:
                                            vt1 = SerializationTreeValueType.stv_uid;
                                            uv1 = p.dv_union_type.dv_type.uid;
                                            break;
                                        case DefaultValueType.dvt_enum:
                                            mv = p.dv_union_type.dv_enum;
                                            if (mv.ValueUID == EmptyUID)
                                            {
                                                vt1 = SerializationTreeValueType.stv_number;
                                                nv1 = mv.Value;
                                            }
                                            else
                                            {
                                                vt1 = SerializationTreeValueType.stv_uid;
                                                uv1 = mv.ValueUID;
                                            }
                                            break;
                                        default:
                                            Form1.log.Error("Ошибка формата потока 53. Ошибка вычисления условия. Не найдено значение свойства. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Свойство {2}, " +
                                                            "Путь {3}",
                                                            t.Name,
                                                            tn.path(),
                                                            p.Name,
                                                            spath + tr.Path());
                                            break;
                                    }
                                }
                                else
                                {
                                    //vv = ip->second;
                                    vv = val;
                                    if (vv is null)
                                    {
                                        Form1.log.Error("Ошибка формата потока 232. Ошибка вычисления условия. Значение свойства не определено. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        p.Name,
                                                        spath + tr.Path());
                                    }
                                    else
                                    {
                                        if (vv.kind == KindOfValue1C.kv_string)
                                        {
                                            vt1 = SerializationTreeValueType.stv_string;
                                            sv1 = ((Value1C_string)vv).v_string;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_number)
                                        {
                                            vt1 = SerializationTreeValueType.stv_number;
                                            nv1 = ((Value1C_number)vv).v_number;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_number_exp)
                                        {
                                            vt1 = SerializationTreeValueType.stv_number;
                                            nv1 = (int)((Value1C_number_exp)vv).v_number;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_uid)
                                        {
                                            vt1 = SerializationTreeValueType.stv_uid;
                                            uv1 = ((Value1C_uid)vv).v_uid;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_enum)
                                        {
                                            mv = ((Value1C_enum)vv).v_enum;
                                            if (mv.ValueUID == EmptyUID)
                                            {
                                                vt1 = SerializationTreeValueType.stv_number;
                                                nv1 = mv.Value;
                                            }
                                            else
                                            {
                                                vt1 = SerializationTreeValueType.stv_uid;
                                                uv1 = mv.ValueUID;
                                            }
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_bool)
                                        {
                                            vt1 = SerializationTreeValueType.stv_number;
                                            nv1 = ((Value1C_bool)vv).v_bool ? 1 : 0;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_type)
                                        {
                                            vt1 = SerializationTreeValueType.stv_uid;
                                            uv1 = ((Value1C_type)vv).v_uid;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_binary)
                                        {
                                            vt1 = SerializationTreeValueType.stv_string;
                                            sv1 = vv.presentation();
                                        }
                                        else
                                        {
                                            Form1.log.Error("Ошибка формата потока 54. Ошибка вычисления условия. Недопустимый тип значения свойства. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Свойство {2}, " +
                                                            "Тип значения {3}, " +
                                                            "Путь {4}",
                                                            t.Name,
                                                            tn.path(),
                                                            p.Name,
                                                            vv.kind,
                                                            spath + tr.Path());
                                        }
                                    }

                                }
                                break;
                            case SerializationTreeValueType.stv_vercon:
                                vt1 = SerializationTreeValueType.stv_number;
                                nv1 = (int)tn.uTreeNode1.vercon1;
                                break;
                            case SerializationTreeValueType.stv_ver1C:
                                vt1 = SerializationTreeValueType.stv_number;
                                nv1 = (int)tn.uTreeNode1.ver1C1;
                                break;
                            case SerializationTreeValueType.stv_classpar:
                                vt1 = SerializationTreeValueType.stv_number;
                                if (clitem != null)
                                    nv1 = clitem.Cl.GetParamValue(tn.uTreeNode1.classpar1);
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 120. Ошибка вычисления условия. Класс не определён. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Параметр класса {2}, " +
                                                    "Тип значения {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    tn.uTreeNode1.classpar1.Name,
                                                    vv.kind,
                                                    spath + tr.Path());
                                    nv1 = -1;
                                }
                                break;
                            case SerializationTreeValueType.stv_globalvar:
                                vt1 = SerializationTreeValueType.stv_number;
                                nv1 = 0;
                                ok = false;
                                for (vo = v.parent; vo != null; vo = vo.parent)
                                {
                                    if (vo.globalvars.TryGetValue(tn.str1.ToUpper(), out int val2))
                                    {
                                        nv1 = val2;
                                        ok = true;
                                        break;
                                    }
                                }
                                if (!ok)
                                {
                                    Form1.log.Error("Ошибка формата потока 123. Ошибка получения первого значения условия. Не удалось получить значение глобальной переменной. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Глобальная переменная {2}, " +
                                                    "Путь {3}",
                                                    t.Name,
                                                    tn.path(),
                                                    tn.str1,
                                                    spath + tr.Path());
                                }
                                break;
                            default:
                                Form1.log.Error("Ошибка формата потока 55. Ошибка вычисления условия. Недопустимый тип значения 1. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Тип значения {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tn.typeval1presentation(),
                                                spath + tr.Path());
                                break;

                        }

                        // Значение 2
                        vt2 = SerializationTreeValueType.stv_none;
                        switch (tn.typeval2)
                        {
                            case SerializationTreeValueType.stv_string:
                                vt2 = SerializationTreeValueType.stv_string;
                                sv2 = tn.str2;
                                break;
                            case SerializationTreeValueType.stv_number:
                                vt2 = SerializationTreeValueType.stv_number;
                                nv2 = tn.uTreeNode2.num2;
                                break;
                            case SerializationTreeValueType.stv_uid:
                                vt2 = SerializationTreeValueType.stv_uid;
                                uv2 = tn.uTreeNode2.uid2;
                                break;
                            case SerializationTreeValueType.stv_value:
                                mv = tn.uTreeNode2.val2;
                                if (mv.ValueUID == EmptyUID)
                                {
                                    vt2 = SerializationTreeValueType.stv_number;
                                    nv2 = mv.Value;
                                }
                                else
                                {
                                    vt2 = SerializationTreeValueType.stv_uid;
                                    uv2 = mv.ValueUID;
                                }
                                break;
                            case SerializationTreeValueType.stv_var:
                                vt2 = SerializationTreeValueType.stv_number;
                                nv2 = getVarValue(tn.str2, t, varvalues, clitem, spath);
                                break;
                            case SerializationTreeValueType.stv_prop:
                                p = tn.uTreeNode2.prop2;

                                if (!v.v_objprop.TryGetValue(p, out Value1C val))
                                {
                                    dvt = p.defaultvaluetype;
                                    switch (dvt)
                                    {
                                        case DefaultValueType.dvt_bool:
                                            vt2 = SerializationTreeValueType.stv_number;
                                            nv2 = p.dv_union_type.dv_bool ? 1 : 0;
                                            break;
                                        case DefaultValueType.dvt_number:
                                            vt2 = SerializationTreeValueType.stv_number;
                                            nv2 = p.dv_union_type.dv_number;
                                            break;
                                        case DefaultValueType.dvt_string:
                                            vt2 = SerializationTreeValueType.stv_string;
                                            sv2 = p.dv_union_type.dv_string;
                                            break;
                                        case DefaultValueType.dvt_type:
                                            vt2 = SerializationTreeValueType.stv_uid;
                                            uv2 = p.dv_union_type.dv_type.uid;
                                            break;
                                        case DefaultValueType.dvt_enum:
                                            mv = p.dv_union_type.dv_enum;
                                            if (mv.ValueUID == EmptyUID)
                                            {
                                                vt2 = SerializationTreeValueType.stv_number;
                                                nv2 = mv.Value;
                                            }
                                            else
                                            {
                                                vt2 = SerializationTreeValueType.stv_uid;
                                                uv2 = mv.ValueUID;
                                            }
                                            break;
                                        default:
                                            Form1.log.Error("Ошибка формата потока 53. Ошибка вычисления условия. Не найдено значение свойства. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Свойство {2}, " +
                                                            "Путь {3}",
                                                            t.Name,
                                                            tn.path(),
                                                            p.Name,
                                                            spath + tr.Path());
                                            break;
                                    }
                                }
                                else
                                {
                                    //vv = ip->second;
                                    vv = val;
                                    if (vv is null)
                                    {
                                        Form1.log.Error("Ошибка формата потока 232. Ошибка вычисления условия. Значение свойства не определено. " +
                                                        "Загружаемый тип {0}, " +
                                                        "Путь ДС {1}, " +
                                                        "Свойство {2}, " +
                                                        "Путь {3}",
                                                        t.Name,
                                                        tn.path(),
                                                        p.Name,
                                                        spath + tr.Path());
                                    }
                                    else
                                    {
                                        if (vv.kind == KindOfValue1C.kv_string)
                                        {
                                            vt2 = SerializationTreeValueType.stv_string;
                                            sv2 = ((Value1C_string)vv).v_string;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_number)
                                        {
                                            vt2 = SerializationTreeValueType.stv_number;
                                            nv2 = ((Value1C_number)vv).v_number;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_number_exp)
                                        {
                                            vt2 = SerializationTreeValueType.stv_number;
                                            nv2 = (int)((Value1C_number_exp)vv).v_number;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_uid)
                                        {
                                            vt2 = SerializationTreeValueType.stv_uid;
                                            uv2 = ((Value1C_uid)vv).v_uid;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_enum)
                                        {
                                            mv = ((Value1C_enum)vv).v_enum;
                                            if (mv.ValueUID == EmptyUID)
                                            {
                                                vt2 = SerializationTreeValueType.stv_number;
                                                nv2 = mv.Value;
                                            }
                                            else
                                            {
                                                vt2 = SerializationTreeValueType.stv_uid;
                                                uv2 = mv.ValueUID;
                                            }
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_bool)
                                        {
                                            vt2 = SerializationTreeValueType.stv_number;
                                            nv2 = ((Value1C_bool)vv).v_bool ? 1 : 0;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_type)
                                        {
                                            vt2 = SerializationTreeValueType.stv_uid;
                                            uv2 = ((Value1C_type)vv).v_uid;
                                        }
                                        else if (vv.kind == KindOfValue1C.kv_binary)
                                        {
                                            vt2 = SerializationTreeValueType.stv_string;
                                            sv2 = vv.presentation();
                                        }
                                        else
                                        {
                                            Form1.log.Error("Ошибка формата потока 54. Ошибка вычисления условия. Недопустимый тип значения свойства. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Свойство {2}, " +
                                                            "Тип значения {3}, " +
                                                            "Путь {4}",
                                                            t.Name,
                                                            tn.path(),
                                                            p.Name,
                                                            vv.kind,
                                                            spath + tr.Path());
                                        }
                                    }

                                }
                                break;
                            case SerializationTreeValueType.stv_vercon:
                                vt2 = SerializationTreeValueType.stv_number;
                                nv2 = (int)tn.uTreeNode2.vercon2;
                                break;
                            case SerializationTreeValueType.stv_ver1C:
                                vt2 = SerializationTreeValueType.stv_number;
                                nv2 = (int)tn.uTreeNode2.ver1C2;
                                break;
                            case SerializationTreeValueType.stv_classpar:
                                vt2 = SerializationTreeValueType.stv_number;
                                if (clitem != null)
                                    nv2 = clitem.Cl.GetParamValue(tn.uTreeNode2.classpar2);
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 120. Ошибка вычисления условия. Класс не определён. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Параметр класса {2}, " +
                                                    "Тип значения {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    tn.uTreeNode2.classpar2.Name,
                                                    vv.kind,
                                                    spath + tr.Path());
                                    nv2 = -1;
                                }
                                break;
                            case SerializationTreeValueType.stv_globalvar:
                                vt2 = SerializationTreeValueType.stv_number;
                                nv2 = 0;
                                ok = false;
                                for (vo = v.parent; vo != null; vo = vo.parent)
                                {
                                    if (vo.globalvars.TryGetValue(tn.str1.ToUpper(), out int val2))
                                    {
                                        nv2 = val2;
                                        ok = true;
                                        break;
                                    }
                                }
                                if (!ok)
                                {
                                    Form1.log.Error("Ошибка формата потока 123. Ошибка получения первого значения условия. Не удалось получить значение глобальной переменной. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Глобальная переменная {2}, " +
                                                    "Путь {3}",
                                                    t.Name,
                                                    tn.path(),
                                                    tn.str2,
                                                    spath + tr.Path());
                                }
                                break;
                            default:
                                Form1.log.Error("Ошибка формата потока 55. Ошибка вычисления условия. Недопустимый тип значения 1. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Тип значения {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tn.typeval2presentation(),
                                                spath + tr.Path());
                                break;

                        }

                        if (vt1 != SerializationTreeValueType.stv_none && vt2 != SerializationTreeValueType.stv_none)
                        {
                            if (vt1 != vt2)
                            {
                                Form1.log.Error("Ошибка формата потока 59. Ошибка вычисления условия. Несовпадающие типы сравниваемых значений. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Тип значения 1 {2}, " +
                                                "Тип значения 2 {3}, " +
                                                "Путь {4}",
                                                t.Name,
                                                tn.path(),
                                                vt1,
                                                vt2,
                                                spath + tr.Path());
                            }
                            else
                            {
                                if (vt1 == SerializationTreeValueType.stv_string)
                                {
                                    switch (tn.condition)
                                    {
                                        case SerializationTreeCondition.stc_e:
                                            cv = sv1.CompareTo(sv2) == 0;
                                            break;
                                        case SerializationTreeCondition.stc_ne:
                                            cv = sv1.CompareTo(sv2) != 0;
                                            break;
                                        case SerializationTreeCondition.stc_l:
                                            cv = sv1.CompareTo(sv2) < 0;
                                            break;
                                        case SerializationTreeCondition.stc_g:
                                            cv = sv1.CompareTo(sv2) > 0;
                                            break;
                                        case SerializationTreeCondition.stc_le:
                                            cv = sv1.CompareTo(sv2) <= 0;
                                            break;
                                        case SerializationTreeCondition.stc_ge:
                                            cv = sv1.CompareTo(sv2) >= 0;
                                            break;
                                        default:
                                            Form1.log.Error("Ошибка формата потока 60. Ошибка вычисления условия. Некорректное условие при сравнении строк. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Условие {2}, " +
                                                            "Путь {3}",
                                                            t.Name,
                                                            tn.path(),
                                                            tn.condition,
                                                            spath + tr.Path());
                                            break;
                                    }
                                }
                                else if (vt1 == SerializationTreeValueType.stv_number)
                                {
                                    switch (tn.condition)
                                    {
                                        case SerializationTreeCondition.stc_e:
                                            cv = nv1 == nv2;
                                            break;
                                        case SerializationTreeCondition.stc_ne:
                                            cv = nv1 != nv2;
                                            break;
                                        case SerializationTreeCondition.stc_l:
                                            cv = nv1 < nv2;
                                            break;
                                        case SerializationTreeCondition.stc_g:
                                            cv = nv1 > nv2;
                                            break;
                                        case SerializationTreeCondition.stc_le:
                                            cv = nv1 <= nv2;
                                            break;
                                        case SerializationTreeCondition.stc_ge:
                                            cv = nv1 >= nv2;
                                            break;
                                        case SerializationTreeCondition.stc_bs:
                                            if (nv2 < 0 || nv2 > 31)
                                            {
                                                Form1.log.Error("Ошибка формата потока 193. Ошибка вычисления условия. Номер бита за пределами 0-31. " +
                                                                "Загружаемый тип {0}, " +
                                                                "Номер бита {1}",
                                                                t.Name,
                                                                nv2);
                                                break;
                                            }
                                            cv = (nv1 & (1 << nv2)) != 0;
                                            break;
                                        case SerializationTreeCondition.stc_bn:
                                            if (nv2 < 0 || nv2 > 31)
                                            {
                                                Form1.log.Error("Ошибка формата потока 194. Ошибка вычисления условия. Номер бита за пределами 0-31. " +
                                                                "Загружаемый тип {0}, " +
                                                                "Номер бита {1}",
                                                                t.Name,
                                                                nv2);
                                                break;
                                            }
                                            cv = (nv1 & (1 << nv2)) == 0;
                                            break;
                                        default:
                                            Form1.log.Error("Ошибка формата потока 61. Ошибка вычисления условия. Некорректное условие при сравнении чисел. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Условие {2}, " +
                                                            "Путь {3}",
                                                            t.Name,
                                                            tn.path(),
                                                            tn.condition,
                                                            spath + tr.Path());
                                            break;
                                    }
                                }
                                else if (vt1 == SerializationTreeValueType.stv_uid)
                                {
                                    switch (tn.condition)
                                    {
                                        case SerializationTreeCondition.stc_e:
                                            cv = uv1 == uv2;
                                            break;
                                        case SerializationTreeCondition.stc_ne:
                                            cv = uv1 != uv2;
                                            break;
                                        case SerializationTreeCondition.stc_l:
                                            cv = (uv1.CompareTo(uv2) < 0);
                                            break;
                                        case SerializationTreeCondition.stc_g:
                                            cv = (uv1.CompareTo(uv2) > 0);
                                            break;
                                        case SerializationTreeCondition.stc_le:
                                            cv = (uv1.CompareTo(uv2) < 0) || uv1 == uv2;
                                            break;
                                        case SerializationTreeCondition.stc_ge:
                                            cv = (uv1.CompareTo(uv2) > 0) || uv1 == uv2;
                                            break;
                                        default:
                                            Form1.log.Error("Ошибка формата потока 62. Ошибка вычисления условия. Некорректное условие при сравнении UID. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Условие {2}, " +
                                                            "Путь {3}",
                                                            t.Name,
                                                            tn.path(),
                                                            tn.condition,
                                                            spath + tr.Path());
                                            break;
                                    }
                                }
                            }
                        }
                        if (cv)
                            recursiveLoadValue1C(v, varvalues, tr, tn.first, metauid, metats, clitem, path);
                        break;
                    case SerializationTreeNodeType.stt_metaid:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 98. Отсутствует ожидаемое значение идентификатора объекта метаданных. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath);
                            break;
                        }
                        if (tr.Get_Type() == Node_Type.nd_guid)
                        {
                            if (!string_to_GUID(tr.Get_Value(), ref uid))
                            {
                                Form1.log.Error("Ошибка формата потока 38. Ошибка преобразования UID при загрузке ИД метаданных. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Значение {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tr.Get_Value(),
                                                spath);
                            }
                            else
                            {
                                if (v.kind == KindOfValue1C.kv_metaobj)
                                {
                                    vm = (Value1C_metaobj)v;
                                    if (vm.v_metaobj != null)
                                    {
                                        if (vm.v_metaobj.UID != uid)
                                        {
                                            Form1.log.Error("Ошибка формата потока 125. Повторное определение ИД метаданных с отличающимся UID. " +
                                                            "Загружаемый тип {0}, " +
                                                            "Путь ДС {1}, " +
                                                            "Существующий ИД {2}, " +
                                                            "Загружаемый ИД {3}, " +
                                                            "Путь {4}",
                                                            t.Name,
                                                            tn.path(),
                                                            vm.v_metaobj.UID.ToString(),
                                                            uid.ToString(),
                                                            spath);
                                        }
                                    }
                                    else
                                    {
                                        vm.v_metaobj = new MetaObject(uid, vm);
                                        fmetamap[uid] = vm.v_metaobj;
                                        metauid = uid;
                                    }
                                }
                                else
                                {
                                    Form1.log.Error("Ошибка формата потока 198. Попытка загрузки ИД метаданных для значения, не являющегося объектом метаданных. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Загружаемый ИД {2}, " +
                                                    "Путь {3}",
                                                    t.Name,
                                                    tn.path(),
                                                    uid.ToString(),
                                                    spath);
                                }
                            }
                        }
                        else
                        {
                            Form1.log.Error("Ошибка формата потока 39. Тип значения не UID при загрузке ИД метаданных. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Тип значения {2}, " +
                                            "Путь {3}",
                                            t.Name,
                                            tn.path(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            spath);
                        }
                        if (!tn.nomove)
                            tr = tr.Get_Next();
                        break;
                    case SerializationTreeNodeType.stt_classcol:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 99. Отсутствует ожидаемое значение счетчика коллекции классов. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath);
                            break;
                        }
                        if (tr.Get_Type() != Node_Type.nd_number)
                        {
                            Form1.log.Error("Ошибка формата потока 88. Ожидается счетчик коллекции классов. Тип значения не число. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Тип значения {2}, " +
                                            "Значение {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            tr.Get_Value(),
                                            spath + tr.Path());
                            if (!tn.nomove)
                                tr = tr.Get_Next();
                            break;
                        }
                        j = Convert.ToInt32(tr.Get_Value());
                        for (i = 0, tr = tr.Get_Next(); i < j; ++i)
                        {
                            if (tn.uTreeNode1.classtype == SerializationTreeClassType.stct_inlist)
                            {
                                if (tr.Get_Type() != Node_Type.nd_list)
                                {
                                    Form1.log.Error("Ошибка формата потока 89. Ожидается список очередного класса коллекции классов. Тип значения не Список. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Тип значения {2}, " +
                                                    "Значение {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    get_node_type_presentation(tr.Get_Type()),
                                                    tr.Get_Value(),
                                                    spath + tr.Path());
                                    continue;
                                }
                                tt = tr.Get_First();
                            }
                            else tt = tr;
                            if (tt is null)
                            {
                                Form1.log.Error("Ошибка формата потока 90. Ожидается идентификатор очередного класса коллекции классов. Значение отсутствует. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Путь {2}",
                                                t.Name,
                                                tn.path(),
                                                spath + tr.Path());
                                continue;
                            }
                            if (tt.Get_Type() != Node_Type.nd_guid)
                            {
                                Form1.log.Error("Ошибка формата потока 91. Ожидается идентификатор очередного класса коллекции классов. Тип значения не UID. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Тип значения {2}, " +
                                                "Значение {3}, " +
                                                "Путь {4}",
                                                t.Name,
                                                tn.path(),
                                                get_node_type_presentation(tr.Get_Type()),
                                                tr.Get_Value(),
                                                spath + tr.Path());
                                continue;
                            }
                            if (!string_to_GUID(tt.Get_Value(), ref uid))
                            {
                                Form1.log.Error("Ошибка формата потока 92. Ожидается идентификатор очередного класса коллекции классов. Ошибка преобразования UID. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Значение {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tr.Get_Value(),
                                                spath + tr.Path());
                                continue;
                            }
                            for (tnn = tn.first; tnn != null; tnn = tnn.next)
                                if (tnn.uTreeNode1.uid1 == uid)
                                    break;
                            if (tnn is null)
                            {
                                Form1.log.Error("Ошибка формата потока 93. Неизвестный идентификатор класса коллекции классов. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Значение {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tr.Get_Value(),
                                                spath + tr.Path());
                                continue;
                            }
                            cl = Class.GetClass(uid);

                            cli = (cl != null) ? new ClassItem(cl) : null;

                            tt = tt.Get_Next();

                            recursiveLoadValue1C(v, varvalues, tt, tnn.first, metauid, metats, cli, path, tn.uTreeNode1.classtype == SerializationTreeClassType.stct_inlist);

                            tr = (tn.uTreeNode1.classtype == SerializationTreeClassType.stct_inlist) ? tr.Get_Next() : tt;

                        }
                        break;
                    case SerializationTreeNodeType.stt_class:
                        Form1.log.Error("Ошибка формата потока 87. Неожидаемый тип \"Класс\" узла дерева сериализации. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Путь {2}",
                                        t.Name,
                                        tn.path(),
                                        spath + tr.Path());
                        break;
                    case SerializationTreeNodeType.stt_idcol:
                        if (tr is null)
                        {
                            Form1.log.Error("Ошибка формата потока 155. Отсутствует ожидаемое значение счетчика коллекции ИД-элементов. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Путь {2}",
                                            t.Name,
                                            tn.path(),
                                            spath + tr.Path());
                            break;
                        }
                        if (tr.Get_Type() != Node_Type.nd_number)
                        {
                            Form1.log.Error("Ошибка формата потока 156. Ожидается счетчик коллекции ИД-элементов. Тип значения не число. " +
                                            "Загружаемый тип {0}, " +
                                            "Путь ДС {1}, " +
                                            "Тип значения {2}, " +
                                            "Значение {3}, " +
                                            "Путь {4}",
                                            t.Name,
                                            tn.path(),
                                            get_node_type_presentation(tr.Get_Type()),
                                            tr.Get_Value(),
                                            spath + tr.Path());
                            if (!tn.nomove)
                                tr = tr.Get_Next();
                            break;
                        }
                        //j = tr->get_value().ToIntDef(0);
                        j = Convert.ToInt32(tr.Get_Value());
                        for (i = 0, tr = tr.Get_Next(); i < j; ++i)
                        {
                            if (tn.uTreeNode1.classtype == SerializationTreeClassType.stct_inlist)
                            {
                                if (tr.Get_Type() != Node_Type.nd_list)
                                {
                                    Form1.log.Error("Ошибка формата потока 157. Ожидается список очередного ИД-элемента. Тип значения не Список. " +
                                                    "Загружаемый тип {0}, " +
                                                    "Путь ДС {1}, " +
                                                    "Тип значения {2}, " +
                                                    "Значение {3}, " +
                                                    "Путь {4}",
                                                    t.Name,
                                                    tn.path(),
                                                    get_node_type_presentation(tr.Get_Type()),
                                                    tr.Get_Value(),
                                                    spath + tr.Path());
                                    continue;
                                }
                                tt = tr.Get_First();
                            }
                            else
                                tt = tr;
                            if (tt is null)
                            {
                                Form1.log.Error("Ошибка формата потока 158. Ожидается идентификатор очередного ИД-элемента. Значение отсутствует. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Путь {2}",
                                                t.Name,
                                                tn.path(),
                                                spath + tr.Path());
                                continue;
                            }
                            if (tt.Get_Type() != Node_Type.nd_guid)
                            {
                                Form1.log.Error("Ошибка формата потока 159. Ожидается идентификатор очередного ИД-элемента. Тип значения не UID. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Тип значения {2}, " +
                                                "Значение {3}, " +
                                                "Путь {4}",
                                                t.Name,
                                                tn.path(),
                                                get_node_type_presentation(tr.Get_Type()),
                                                tr.Get_Value(),
                                                spath + tr.Path());
                                continue;
                            }
                            if (!string_to_GUID(tt.Get_Value(), ref uid))
                            {
                                Form1.log.Error("Ошибка формата потока 160. Ожидается идентификатор очередного ИД-элемента. Ошибка преобразования UID. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Значение {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tr.Get_Value(),
                                                spath + tr.Path());
                                continue;
                            }
                            for (tnn = tn.first; tnn != null; tnn = tnn.next)
                                if (tnn.uTreeNode1.uid1 == uid)
                                    break;
                            if (tnn is null)
                            {
                                Form1.log.Error("Ошибка формата потока 161. Неизвестный идентификатор ИД-элемента. " +
                                                "Загружаемый тип {0}, " +
                                                "Путь ДС {1}, " +
                                                "Значение {2}, " +
                                                "Путь {3}",
                                                t.Name,
                                                tn.path(),
                                                tr.Get_Value(),
                                                spath + tr.Path());
                                continue;
                            }

                            nt = tnn.typeprop;

                            if (nt is null)
                                if (t.CollectionTypes.Count == 1)
                                    nt = t.CollectionTypes[0];

                            tt = tt.Get_Next();

                            nv = readValue1C(tt, nt, v, metauid, metats, clitem, path);

                            v.v_objcol.Add(nv);

                            tr = (tn.uTreeNode1.classtype == SerializationTreeClassType.stct_inlist) ? tr.Get_Next() : tt;
                        }
                        break;
                    case SerializationTreeNodeType.stt_idel:
                        Form1.log.Error("Ошибка формата потока 154. Неожидаемый тип \"ИД-элемент\" узла дерева сериализации. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Путь {2}",
                                        t.Name,
                                        tn.path(),
                                        spath + tr.Path());
                        break;
                    default:
                        Form1.log.Error("Ошибка формата потока 23. Неизвестный тип узла дерева сериализации. " +
                                        "Загружаемый тип {0}, " +
                                        "Путь ДС {1}, " +
                                        "Тип узна {2}, " +
                                        "Путь {3}",
                                        t.Name,
                                        tn.path(),
                                        spath + tr.Path());
                        break;
                }
            }
            if (checkend) 
                if (tr != null)
                {
                    Form1.log.Error("Ошибка формата потока 36. Остались необработанные значения. " +
                                    "Загружаемый тип {0}, " +
                                    "Путь {1}",
                                    t.Name,
                                    spath + tr.Path());
                }
            ptr = tr;
        }

        public int getVarValue(string vname, MetaType t, VarValue[] varvalues, ClassItem clitem, string path)
        {
            if (vname.CompareTo("ВерсияКонтейнера") == 0)
            {
                return (int)containerver;
            }

            if (vname.CompareTo("Версия1С") == 0)
            {
                return (int)ver1C;
            }

            if (vname.CompareTo("ВерсияКласса") == 0)
            {
                if (clitem != null)
                {
                    return clitem.Version;
                }

                Form1.log.Error("Ошибка формата потока 119. Ошибка получения значения переменной. Класс не определён. " +
                                "Загружаемый тип {0}, " +
                                "Имя переменной {1}, " +
                                "Путь {2}", t.Name, vname, path);
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
                            Form1.log.Error("Ошибка формата потока 43. Ошибка получения значения переменной. Значение переменной не установлено. " +
                                            "Загружаемый тип {0}, " +
                                            "Имя переменной {1}, " +
                                            "Путь {2}", t.Name, vname, path);
                            return 0;
                        }
                    }
                }
            }

            Form1.log.Error("Ошибка формата потока 34. Ошибка получения значения переменной. Недопустимое имя переменной. " +
                            "Загружаемый тип {0}, " +
                            "Имя переменной {1}, " +
                            "Путь {2}", t.Name, vname, path);
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
                    Form1.log.Error("Ошибка формата потока 122. Недопустимое значение переменной. " +
                                    "Загружаемый тип {0}, " +
                                    "Имя переменной {1}, " +
                                    "Значение {2}, " +
                                    "Класс {3}, " +
                                    "Путь {4}", t.Name, vname, value, clitem.Cl.UID.ToString(), path);
                }
                else
                {
                    Form1.log.Error("Ошибка формата потока 118. Ошибка установки переменной. Класс не определён. " +
                                    "Загружаемый тип {0}, " +
                                    "Имя переменной {1}, " +
                                    "Значение {2}, " +
                                    "Путь {3}", t.Name, vname, value, path);
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
                        Form1.log.Error("Ошибка формата потока 64. Недопустимое значение переменной. " +
                                        "Загружаемый тип {0}, " +
                                        "Имя переменной {1}, " +
                                        "Значение {2}, " +
                                        "Путь {3}", t.Name, vname, value, path);
                        return;
                    }
                }
            }
            Form1.log.Error("Ошибка формата потока 37. Ошибка установки значения переменной. Недопустимое имя переменной. " +
                            "Загружаемый тип {0}, " +
                            "Имя переменной {1}, " +
                            "Путь {2}", t.Name, vname, path);
        }

        public void readPredefinedValues(Value1C_metaobj v, int ni, int ui, Value1C_obj vStrings, string spath)
        {
            Value1C_obj vo;
            Value1C_obj vvo;
            Value1C_uid vu;
            Value1C_string vs = null;
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
                    Form1.log.Error("Ошибка формата потока 179. Ошибка получения значения предопределенного элемента. " +
                                    "Путь {0}", spath);
                    continue;
                }

                vvo = (Value1C_obj)vo.v_objcol[ui];
                if (vvo is null)
                {
                    Form1.log.Error("Ошибка формата потока 180. Ошибка получения значения идентификатора предопределенного элемента. " +
                                    "Путь {0}", spath);
                    continue;
                }
                vu = (Value1C_uid)vvo.getproperty("Ссылка");
                if (vvo is null)
                {
                    Form1.log.Error("Ошибка формата потока 181. Ошибка получения значения идентификатора предопределенного элемента. " +
                                    "Путь {0}", spath);
                    continue;
                }
                if (vu.kind != KindOfValue1C.kv_uid)
                {
                    Form1.log.Error("Ошибка формата потока 182. Ошибка получения значения идентификатора предопределенного элемента. Тип значения не UID. " +
                                    "Путь {0}, " +
                                    "Тип значения {1}", spath, KindOfValue1C_presantation(vu.kind));
                    continue;
                }
                u = vu.v_uid;
                if (u != EmptyUID)
                {
                    vv = vo.v_objcol[ni];
                    if (vv is null)
                    {
                        Form1.log.Error("Ошибка формата потока 183. Ошибка получения имени предопределенного элемента. " +
                                        "Путь {0}", spath);
                        continue;
                    }
                    if (vv.kind != KindOfValue1C.kv_string && vv.kind != KindOfValue1C.kv_binary)
                    {
                        Form1.log.Error("Ошибка формата потока 184. Ошибка получения имени предопределенного элемента. Тип значения не Строка. " +
                                        "Путь {0}, " +
                                        "Тип значения {1}", spath, KindOfValue1C_presantation(vv.kind));
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
                    Form1.log.Error("Ошибка чтения контейнера. Не найден файл. Путь {0}", fullpath);
                }
                return null;
            }

            Tree t = Tree.Parse_1Cstream(cf.str, "", fullpath);
            string s_tree = "";

            t.OutText(ref s_tree);

            stor.close(cf);

            if (t is null)
            {
                Form1.log.Error("Ошибка формата потока 140. Ошибка разбора файла. Путь {0}", fullpath);
                return null;
            }
            return t;
        }

        public void loadValue1C(Value1C_obj v, Tree ptr, SerializationTreeNode tn, Guid metauid, Value1C_stdtabsec metats, ClassItem clitem, string path, bool checkend = false)
        {
            MetaType t;
            MetaType nt;
            
            int i, j;
            int k, l;
            
            MetaProperty p;
            MetaGeneratedType gt;
            SerializationTreeVar var;
            ExternalFile ext;

            bool cv, ok;
            SerializationTreeValueType vt1, vt2;
            String sv1 = "";
            String sv2 = "";
            int nv1 = 0;
            int nv2 = 0;
            Guid uv1 = EmptyUID;
            Guid uv2 = EmptyUID;
            Value1C vv = null;
            Value1C_obj vo;
            Value1C_number vn = null;
            
            String spath, npath;
            Tree tt;
            Tree tte;
            //TStreamReader* reader;
            Stream str;
            MetaValue mv;
            DefaultValueType dvt;

            MetaProperty prop;
            int ni, ui;
            int ii;
            Value1C_obj vValues;
            Value1C_obj vColumns;
            Value1C_obj vColumnsAndValuesMap = null;
            Value1C_obj vStrings = null;
            Value1C_binary vb;
            Value1C_extobj veo;
            bool nok, uok;

            ConfigFile cf;
            ConfigFile cfc;
            String sn;


            spath = storpresent + path;
            t = tn.owner;
            //varvalues = null;
            VarValue[] varvalues = null;
            i = t.SerializationVars.Count;
            if (i != 0)
            {
                varvalues = new VarValue[i];
                for (j = 0; j < i; ++j)
                {
                    var = t.SerializationVars[j];
                    if (var.IsGlobal)
                    {
                        if (var.IsFix)
                        {
                            v.globalvars[var.Name.ToUpper()] = var.FixValue;
                        }
                    }
                }
            }

            recursiveLoadValue1C(v, varvalues, ptr, tn, metauid, metats, clitem, path, checkend);

            if (v.kind == KindOfValue1C.kv_metaobj)
            {
                if (((Value1C_metaobj)v).v_metaobj is null)
                {
                    Form1.log.Info("Ошибка формата потока 203. Не загружен UID объекта метаданных. " +
                                   "Загружаемый тип {0}, " +
                                   "Путь {1}, ", t.Name, spath);
                }
            }

            #region Чтение внешних файлов

            for (i = 0; i < t.ExternalFiles.Count; ++i)
            {
                ext = t.ExternalFiles[i];
                if (ext.havecondition)
                {
                    cv = false;

                    //Значение 1
                    vt1 = SerializationTreeValueType.stv_none;
                    switch (ext.typeval1)
                    {
                        case SerializationTreeValueType.stv_string:
                            vt1 = SerializationTreeValueType.stv_string;
                            sv1 = ext.str1;
                            break;
                        case SerializationTreeValueType.stv_number:
                            vt1 = SerializationTreeValueType.stv_number;
                            nv1 = ext.uTreeNode3.num1;
                            break;
                        case SerializationTreeValueType.stv_uid:
                            vt1 = SerializationTreeValueType.stv_uid;
                            uv1 = ext.uTreeNode3.uid1;
                            break;
                        case SerializationTreeValueType.stv_value:
                            mv = ext.uTreeNode3.val1;
                            if (mv.ValueUID == EmptyUID)
                            {
                                vt1 = SerializationTreeValueType.stv_number;
                                nv1 = mv.Value;
                            }
                            else
                            {
                                vt1 = SerializationTreeValueType.stv_uid;
                                uv1 = mv.ValueUID;
                            }
                            break;
                        case SerializationTreeValueType.stv_var:
                            vt1 = SerializationTreeValueType.stv_number;
                            nv1 = getVarValue(ext.str1, t, varvalues, clitem, spath);
                            break;
                        case SerializationTreeValueType.stv_prop:

                            if (!v.v_objprop.TryGetValue(ext.uTreeNode3.prop1, out Value1C val))
                            {
                                p = ext.uTreeNode3.prop1;
                                dvt = p.defaultvaluetype;
                                switch (dvt)
                                {
                                    case DefaultValueType.dvt_bool:
                                        vt1 = SerializationTreeValueType.stv_number;
                                        nv1 = p.dv_union_type.dv_bool ? 1 : 0;
                                        break;
                                    case DefaultValueType.dvt_number:
                                        vt1 = SerializationTreeValueType.stv_number;
                                        nv1 = p.dv_union_type.dv_number;
                                        break;
                                    case DefaultValueType.dvt_string:
                                        vt1 = SerializationTreeValueType.stv_string;
                                        sv1 = p.dv_union_type.dv_string;
                                        break;
                                    case DefaultValueType.dvt_type:
                                        vt1 = SerializationTreeValueType.stv_uid;
                                        uv1 = p.dv_union_type.dv_type.uid;
                                        break;
                                    case DefaultValueType.dvt_enum:
                                        mv = p.dv_union_type.dv_enum;
                                        if (mv.ValueUID == EmptyUID)
                                        {
                                            vt1 = SerializationTreeValueType.stv_number;
                                            nv1 = mv.Value;
                                        }
                                        else
                                        {
                                            vt1 = SerializationTreeValueType.stv_uid;
                                            uv1 = mv.ValueUID;
                                        }
                                        break;
                                    default:
                                        Form1.log.Info("Ошибка формата потока 126. Ошибка вычисления условия внешнего файла. Не найдено значение свойства. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Свойство {1}, " +
                                                       "Путь {2}, ", t.Name, p.Name, spath);
                                        break;
                                }
                            }
                            else
                            {
                                vv = val;
                                if (vv.kind == KindOfValue1C.kv_string)
                                {
                                    vt1 = SerializationTreeValueType.stv_string;
                                    sv1 = ((Value1C_string)vv).v_string;
                                }
                                else if (vv.kind == KindOfValue1C.kv_number)
                                {
                                    vt1 = SerializationTreeValueType.stv_number;
                                    nv1 = ((Value1C_number)vv).v_number;
                                }
                                else if (vv.kind == KindOfValue1C.kv_number_exp)
                                {
                                    vt1 = SerializationTreeValueType.stv_number;
                                    nv1 = (int)((Value1C_number_exp)vv).v_number;
                                }
                                else if (vv.kind == KindOfValue1C.kv_uid)
                                {
                                    vt1 = SerializationTreeValueType.stv_uid;
                                    uv1 = ((Value1C_uid)vv).v_uid;
                                }
                                else if (vv.kind == KindOfValue1C.kv_enum)
                                {
                                    mv = ((Value1C_enum)vv).v_enum;
                                    if (mv.ValueUID == EmptyUID)
                                    {
                                        vt1 = SerializationTreeValueType.stv_number;
                                        nv1 = mv.Value;
                                    }
                                    else
                                    {
                                        vt1 = SerializationTreeValueType.stv_uid;
                                        uv1 = mv.ValueUID;
                                    }
                                }
                                else if (vv.kind == KindOfValue1C.kv_bool)
                                {
                                    vt1 = SerializationTreeValueType.stv_number;
                                    nv1 = ((Value1C_bool)vv).v_bool ? 1 : 0;
                                }
                                else if (vv.kind == KindOfValue1C.kv_binary)
                                {
                                    vt1 = SerializationTreeValueType.stv_string;
                                    sv1 = vv.presentation();
                                }
                                else
                                {
                                    Form1.log.Info("Ошибка формата потока 127. Ошибка вычисления условия внешнего файла. Недопустимый тип значения свойства. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Свойство {1}, " +
                                                   "Тип значения {2}, " +
                                                   "Путь {3}", t.Name, ext.uTreeNode3.prop1.Name, vv.kind, spath);
                                }

                            }
                            break;
                        case SerializationTreeValueType.stv_vercon:
                            vt1 = SerializationTreeValueType.stv_number;
                            nv1 = (int)ext.uTreeNode3.vercon1;
                            break;
                        case SerializationTreeValueType.stv_ver1C:
                            vt1 = SerializationTreeValueType.stv_number;
                            nv1 = (int)ext.uTreeNode3.ver1C1;
                            break;
                        case SerializationTreeValueType.stv_classpar:
                            vt1 = SerializationTreeValueType.stv_number;
                            if (clitem != null)
                            {
                                nv1 = clitem.Cl.GetParamValue(ext.uTreeNode3.classpar1);
                            }
                            else
                            {
                                Form1.log.Info("Ошибка формата потока 128. Ошибка вычисления условия внешнего файла. Класс не определён. " +
                                               "Загружаемый тип {0}, " +
                                               "Параметр класса {1}, " +
                                               "Тип значения {2}, " +
                                               "Путь {3}", t.Name, ext.uTreeNode3.classpar1.Name, vv.kind, spath);
                                nv1 = -1;
                            }
                            break;
                        case SerializationTreeValueType.stv_globalvar:
                            vt1 = SerializationTreeValueType.stv_number;
                            nv1 = 0;
                            ok = false;
                            vo = null;
                            for (vo = vo.parent; vo != null; vo = vo.parent)
                            {
                                if (vo.globalvars.TryGetValue(ext.str1.ToUpper(), out int val2))
                                {
                                    nv1 = val2;
                                    ok = true;
                                    break;
                                }
                            }
                            if (!ok)
                            {
                                Form1.log.Info("Ошибка формата потока 129. Ошибка получения первого значения условия внешнего файла. Не удалось получить значение глобальной переменной. " +
                                               "Загружаемый тип {0}, " +
                                               "Глобальная переменная {1}, " +
                                               "Путь {2}", t.Name, ext.str1, spath);
                            }
                            break;
                        default:
                            Form1.log.Info("Ошибка формата потока 130. Ошибка вычисления условия внешнего файла. Недопустимый тип значения 1. " +
                                           "Загружаемый тип {0}, " +
                                           "Тип значения {1}, " +
                                           "Путь {2}", t.Name, ext.typeval1presentation(), spath);
                            break;
                    }

                    // Значение 2
                    vt2 = SerializationTreeValueType.stv_none;
                    switch (ext.typeval2)
                    {
                        case SerializationTreeValueType.stv_string:
                            vt2 = SerializationTreeValueType.stv_string;
                            sv2 = ext.str2;
                            break;
                        case SerializationTreeValueType.stv_number:
                            vt2 = SerializationTreeValueType.stv_number;
                            nv2 = ext.uTreeNode4.num2;
                            break;
                        case SerializationTreeValueType.stv_uid:
                            vt2 = SerializationTreeValueType.stv_uid;
                            uv2 = ext.uTreeNode4.uid2;
                            break;
                        case SerializationTreeValueType.stv_value:
                            mv = ext.uTreeNode4.val2;
                            if (mv.ValueUID == EmptyUID)
                            {
                                vt2 = SerializationTreeValueType.stv_number;
                                nv2 = mv.Value;
                            }
                            else
                            {
                                vt2 = SerializationTreeValueType.stv_uid;
                                uv2 = mv.ValueUID;
                            }
                            break;
                        case SerializationTreeValueType.stv_var:
                            vt2 = SerializationTreeValueType.stv_number;
                            nv2 = getVarValue(ext.str2, t, varvalues, clitem, spath);
                            break;
                        case SerializationTreeValueType.stv_prop:

                            if (!v.v_objprop.TryGetValue(ext.uTreeNode4.prop2, out Value1C val))
                            {
                                p = ext.uTreeNode4.prop2;
                                dvt = p.defaultvaluetype;
                                switch (dvt)
                                {
                                    case DefaultValueType.dvt_bool:
                                        vt2 = SerializationTreeValueType.stv_number;
                                        nv2 = p.dv_union_type.dv_bool ? 1 : 0;
                                        break;
                                    case DefaultValueType.dvt_number:
                                        vt2 = SerializationTreeValueType.stv_number;
                                        nv2 = p.dv_union_type.dv_number;
                                        break;
                                    case DefaultValueType.dvt_string:
                                        vt2 = SerializationTreeValueType.stv_string;
                                        sv2 = p.dv_union_type.dv_string;
                                        break;
                                    case DefaultValueType.dvt_type:
                                        vt2 = SerializationTreeValueType.stv_uid;
                                        uv2 = p.dv_union_type.dv_type.uid;
                                        break;
                                    case DefaultValueType.dvt_enum:
                                        mv = p.dv_union_type.dv_enum;
                                        if (mv.ValueUID == EmptyUID)
                                        {
                                            vt2 = SerializationTreeValueType.stv_number;
                                            nv2 = mv.Value;
                                        }
                                        else
                                        {
                                            vt2 = SerializationTreeValueType.stv_uid;
                                            uv2 = mv.ValueUID;
                                        }
                                        break;
                                    default:
                                        Form1.log.Info("Ошибка формата потока 131. Ошибка вычисления условия внешнего файла. Не найдено значение свойства. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Свойство {1}, " +
                                                       "Путь {2}", t.Name, p.Name, spath);
                                        break;
                                }
                            }
                            else
                            {
                                vv = val;
                                if (vv.kind == KindOfValue1C.kv_string)
                                {
                                    vt2 = SerializationTreeValueType.stv_string;
                                    sv2 = ((Value1C_string)vv).v_string;
                                }
                                else if (vv.kind == KindOfValue1C.kv_number)
                                {
                                    vt2 = SerializationTreeValueType.stv_number;
                                    nv2 = ((Value1C_number)vv).v_number;
                                }
                                else if (vv.kind == KindOfValue1C.kv_number_exp)
                                {
                                    vt2 = SerializationTreeValueType.stv_number;
                                    nv2 = (int)((Value1C_number_exp)vv).v_number;
                                }
                                else if (vv.kind == KindOfValue1C.kv_uid)
                                {
                                    vt2 = SerializationTreeValueType.stv_uid;
                                    uv2 = ((Value1C_uid)vv).v_uid;
                                }
                                else if (vv.kind == KindOfValue1C.kv_enum)
                                {
                                    mv = ((Value1C_enum)vv).v_enum;
                                    if (mv.ValueUID == EmptyUID)
                                    {
                                        vt2 = SerializationTreeValueType.stv_number;
                                        nv2 = mv.Value;
                                    }
                                    else
                                    {
                                        vt2 = SerializationTreeValueType.stv_uid;
                                        uv2 = mv.ValueUID;
                                    }
                                }
                                else if (vv.kind == KindOfValue1C.kv_bool)
                                {
                                    vt2 = SerializationTreeValueType.stv_number;
                                    nv2 = ((Value1C_bool)vv).v_bool ? 1 : 0;
                                }
                                else if (vv.kind == KindOfValue1C.kv_binary)
                                {
                                    vt2 = SerializationTreeValueType.stv_string;
                                    sv2 = vv.presentation();
                                }
                                else
                                {
                                    Form1.log.Info("Ошибка формата потока 131. Ошибка вычисления условия внешнего файла. Не найдено значение свойства. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Свойство {1}, " +
                                                   "Тип значения {2}" +
                                                   "Путь {3}", t.Name, ext.uTreeNode4.prop2.Name, vv.kind, spath);
                                }

                            }
                            break;
                        case SerializationTreeValueType.stv_vercon:
                            vt2 = SerializationTreeValueType.stv_number;
                            nv2 = (int)ext.uTreeNode4.vercon2;
                            break;
                        case SerializationTreeValueType.stv_ver1C:
                            vt2 = SerializationTreeValueType.stv_number;
                            nv2 = (int)ext.uTreeNode4.ver1C2;
                            break;
                        case SerializationTreeValueType.stv_classpar:
                            vt2 = SerializationTreeValueType.stv_number;
                            if (clitem != null)
                            {
                                nv2 = clitem.Cl.GetParamValue(ext.uTreeNode4.classpar2);
                            }
                            else
                            {
                                Form1.log.Info("Ошибка формата потока 133. Ошибка вычисления условия внешнего файла. Класс не определён. " +
                                               "Загружаемый тип {0}, " +
                                               "Параметр класса {1}, " +
                                               "Тип значения {2}" +
                                               "Путь {3}", t.Name, ext.uTreeNode4.classpar2.Name, vv.kind, spath);
                                nv1 = -1;
                            }
                            break;
                        case SerializationTreeValueType.stv_globalvar:
                            vt2 = SerializationTreeValueType.stv_number;
                            nv2 = 0;
                            ok = false;
                            vo = null;
                            for (vo = vo.parent; vo != null; vo = vo.parent)
                            {
                                if (vo.globalvars.TryGetValue(ext.str2.ToUpper(), out int val2))
                                {
                                    nv2 = val2;
                                    ok = true;
                                    break;
                                }
                            }
                            if (!ok)
                            {
                                Form1.log.Info("Ошибка формата потока 134. Ошибка получения второго значения условия внешнего файла. Не удалось получить значение глобальной переменной. " +
                                               "Загружаемый тип {0}, " +
                                               "Глобальная переменная {1}, " +
                                               "Путь {2}", t.Name, ext.str2, spath);
                            }
                            break;
                        default:
                            Form1.log.Info("Ошибка формата потока 135. Ошибка вычисления условия внешнего файла. Недопустимый тип значения 2. " +
                                           "Загружаемый тип {0}, " +
                                           "Тип значения {1}, " +
                                           "Путь {2}", t.Name, ext.typeval2presentation(), spath);
                            break;
                    }

                    ///
                    if (vt1 != SerializationTreeValueType.stv_none && vt2 != SerializationTreeValueType.stv_none)
                    {
                        if (vt1 != vt2)
                        {
                            Form1.log.Info("Ошибка формата потока 136. Ошибка вычисления условия внешнего файла. Несовпадающие типы сравниваемых значений. " +
                                           "Загружаемый тип {0}, " +
                                           "Тип значения 1 {1}, " +
                                           "Тип значения 2 {2}, " +
                                           "Путь {3}", t.Name, vt1, vt2, spath);
                        }
                        else
                        {
                            if (vt1 == SerializationTreeValueType.stv_string)
                            {
                                switch (ext.condition)
                                {
                                    case SerializationTreeCondition.stc_e:
                                        cv = sv1.CompareTo("sv2") == 0;
                                        break;
                                    case SerializationTreeCondition.stc_ne:
                                        cv = sv1.CompareTo("sv2") != 0;
                                        break;
                                    case SerializationTreeCondition.stc_l:
                                        cv = sv1.CompareTo("sv2") < 0;
                                        break;
                                    case SerializationTreeCondition.stc_g:
                                        cv = sv1.CompareTo("sv2") > 0;
                                        break;
                                    case SerializationTreeCondition.stc_le:
                                        cv = sv1.CompareTo("sv2") <= 0;
                                        break;
                                    case SerializationTreeCondition.stc_ge:
                                        cv = sv1.CompareTo("sv2") >= 0;
                                        break;
                                    default:
                                        Form1.log.Info("Ошибка формата потока 137. Ошибка вычисления условия внешнего файла. Некорректное условие при сравнении строк. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Условие {1}, " +
                                                       "Путь {2}", t.Name, ext.condition, spath);
                                        break;
                                }
                            }
                            else if (vt1 == SerializationTreeValueType.stv_number)
                            {
                                switch (ext.condition)
                                {
                                    case SerializationTreeCondition.stc_e:
                                        cv = nv1 == nv2;
                                        break;
                                    case SerializationTreeCondition.stc_ne:
                                        cv = nv1 != nv2;
                                        break;
                                    case SerializationTreeCondition.stc_l:
                                        cv = nv1 < nv2;
                                        break;
                                    case SerializationTreeCondition.stc_g:
                                        cv = nv1 > nv2;
                                        break;
                                    case SerializationTreeCondition.stc_le:
                                        cv = nv1 <= nv2;
                                        break;
                                    case SerializationTreeCondition.stc_ge:
                                        cv = nv1 >= nv2;
                                        break;
                                    case SerializationTreeCondition.stc_bs:
                                        if (nv2 < 0 || nv2 > 31)
                                        {
                                            Form1.log.Info("Ошибка формата потока 191. Ошибка вычисления условия внешнего файла. Номер бита за пределами 0-31. " +
                                                           "Загружаемый тип {0}, " +
                                                           "Номер бита {1}, " +
                                                           "Путь {2}", t.Name, nv2, spath);
                                            break;
                                        }
                                        cv = (nv1 & (1 << nv2)) != 0;
                                        break;
                                    case SerializationTreeCondition.stc_bn:
                                        if (nv2 < 0 || nv2 > 31)
                                        {
                                            Form1.log.Info("Ошибка формата потока 192. Ошибка вычисления условия внешнего файла. Номер бита за пределами 0-31. " +
                                                           "Загружаемый тип {0}, " +
                                                           "Номер бита {1}, " +
                                                           "Путь {2}", t.Name, nv2, spath);
                                            break;
                                        }
                                        cv = (nv1 & (1 << nv2)) == 0;
                                        break;
                                    default:
                                        Form1.log.Info("Ошибка формата потока 192. Ошибка вычисления условия внешнего файла. Номер бита за пределами 0-31. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Условие {1}, " +
                                                       "Путь {2}", t.Name, ext.condition, spath);
                                        break;
                                }
                            }
                            else if (vt1 == SerializationTreeValueType.stv_uid)
                            {
                                switch (ext.condition)
                                {
                                    case SerializationTreeCondition.stc_e:
                                        cv = uv1 == uv2;
                                        break;
                                    case SerializationTreeCondition.stc_ne:
                                        cv = uv1 != uv2;
                                        break;
                                    case SerializationTreeCondition.stc_l:
                                        cv = (uv1.CompareTo(uv2) < 0);
                                        break;
                                    case SerializationTreeCondition.stc_g:
                                        cv = (uv1.CompareTo(uv2) > 0);
                                        break;
                                    case SerializationTreeCondition.stc_le:
                                        cv = (uv1.CompareTo(uv2) < 0) || uv1 == uv2;
                                        break;
                                    case SerializationTreeCondition.stc_ge:
                                        cv = (uv1.CompareTo(uv2) > 0) || uv1 == uv2;
                                        break;
                                    default:
                                        Form1.log.Info("Ошибка формата потока 139. Ошибка вычисления условия внешнего файла. Некорректное условие при сравнении UID. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Условие {1}, " +
                                                       "Путь {2}", t.Name, ext.condition, spath);
                                        break;
                                }

                            }
                        }
                    } // if (vt1 != SerializationTreeValueType.stv_none && vt2 != SerializationTreeValueType.stv_none)
                    if (!cv)
                        continue;

                }

                npath = "";
                if (ext.relativepath)
                {
                    k = path.LastIndexOfAny("\\/".ToCharArray());
                    npath = path.Substring(1, k);
                }

                if (ext.name.CompareTo("<Мета ИД>") == 0)
                {
                    if (v.kind == KindOfValue1C.kv_metaobj)
                    {
                        if (((Value1C_metaobj)v).v_metaobj != null)
                        {
                            npath += ((Value1C_metaobj)v).v_metaobj.UID.ToString();
                        }
                        else
                        {
                            Form1.log.Info("Ошибка формата потока 144. Ошибка получения <Мета ИД> при определении имени внешнего файла. UID объекта метаданных не загружен. " +
                                           "Загружаемый тип {0}, " +
                                           "Путь {1}", t.Name, spath);
                            continue;
                        }
                    }
                    else
                    {
                        Form1.log.Info("Ошибка формата потока 141. Ошибка получения <Мета ИД> при определении имени внешнего файла. Значение 1С не является объектом метаданных. " +
                                       "Загружаемый тип {0}, " +
                                       "Вид значения 1С {1}, " +
                                       "Путь {1}", t.Name, KindOfValue1C_presantation(v.kind) , spath);
                        continue;
                    }
                }
                else
                {
                    npath += ext.name;
                }

                if (ext.ext.Length > 0)
                {
                    npath += ".";
                    npath += ext.ext;
                }
                cfc = null;
                if (ext.catalog)
                {
                    cfc = stor.readfile(npath);
                    npath += "\\";
                    npath += ext.filename;
                }

                nt = ext.type;
                if (nt is null)
                {
                    if (ext.prop.Types.Count == 1)
                    {
                        nt = ext.prop.Types[0];
                    }
                }

                switch (ext.format)
                {
                    case ExternalFileFormat.eff_servalue:
                        prop = ext.prop;
                        if (useExternal && !prop.Predefined)
                        {
                            if (stor.fileexists(npath))
                            {
                                veo = new Value1C_extobj(v, this, npath, nt, metauid);
                                vo = veo;
                            }
                            else
                            {
                                if (ext.optional)
                                    break;
                                Form1.log.Info("Ошибка формата потока 200. Не найден внешний файл. " +
                                               "Загружаемый тип {0}, " +
                                               "Свойство {1}, " +
                                               "Путь {2}", t.Name, prop.Name, storpresent + npath);
                                vo = null;
                            }
                        }
                        else
                        {
                            tt = gettree(stor, npath, !ext.optional);
                            if (tt is null)
                            {
                                if (ext.optional)
                                    break;
                                Form1.log.Info("Ошибка формата потока 142. Не найден или пустой внешний файл. " +
                                               "Загружаемый тип {0}, " +
                                               "Путь {1}", t.Name, storpresent + npath);
                                vo = null;
                            }
                            else
                            {
                                tte = tt.Get_First();
                                vo = (Value1C_obj)readValue1C(tte, nt, v, metauid, metats, clitem, npath);
                            }
                        }

                        v.v_objprop[prop] = vo;

                        #region Обработка предопределенных элементов

                        vValues = null;
                        vColumns = null;

                        if (prop.Predefined)
                        {
                            ni = (int)v.type.PreNameIndex;
                            ui = (int)v.type.PreIdIndex;
                            ok = true;
                            if (ni == ui)
                            {
                                Form1.log.Info("Ошибка формата потока 165. Не установлены индексы колонок предопределенных значений. " +
                                               "Загружаемый тип {0}, " +
                                               "Путь ДС {1}, " +
                                               "Свойство {2}, " +
                                               "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                ok = false;
                            }
                            if (ok)
                            {
                                vValues = (Value1C_obj)vo.getproperty("Значения");
                                if (vValues is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 166. Не найдено свойство \"Значения\" при разборое предопределенных значений. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                vColumns = (Value1C_obj)vValues.getproperty("Колонки");
                                if (vColumns is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 167. Не найдено свойство \"Колонки\" при разборое предопределенных значений. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                vColumnsAndValuesMap = (Value1C_obj)vValues.getproperty("СоответствиеКолонокИЗначений");
                                if (vColumnsAndValuesMap is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 168. Не найдено свойство \"СоответствиеКолонокИЗначений\" при разборое предопределенных значений. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                vStrings = (Value1C_obj)vValues.getproperty("Строки");
                                if (vStrings is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 169. Не найдено свойство \"Строки\" при разборое предопределенных значений. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                if (ni >= (int)vColumns.v_objcol.Count)
                                {
                                    Form1.log.Info("Ошибка формата потока 170. Индекс колонки имени предопределенного элемента больше количества колонок. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                if (ui >= (int)vColumns.v_objcol.Count)
                                {
                                    Form1.log.Info("Ошибка формата потока 171. Индекс колонки идентификатора предопределенного элемента больше количества колонок. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                vo = (Value1C_obj)vColumns.v_objcol[ni];
                                if (vo is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 172. Ошибка получения колонки имени предопределенного элемента. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                vn = (Value1C_number)vo.getproperty("УникальныйНомер");
                                if (vn is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 173. Ошибка получения уникального номера колонки имени предопределенного элемента. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                ni = vn.v_number;
                                vo = (Value1C_obj)vColumns.v_objcol[ui];
                                if (vo is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 174. Ошибка получения колонки идентификатора предопределенного элемента. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                vn = (Value1C_number)vo.getproperty("УникальныйНомер");
                                if (vn is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 175. Ошибка получения уникального номера колонки идентификатора предопределенного элемента. " +
                                                   "Загружаемый тип {0}, " +
                                                   "Путь ДС {1}, " +
                                                   "Свойство {2}, " +
                                                   "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                ui = vn.v_number;
                                nok = uok = false;
                                for (ii = 0; ii < vColumnsAndValuesMap.v_objcol.Count; ++ii)
                                {
                                    vo = (Value1C_obj)vColumnsAndValuesMap.v_objcol[ii];
                                    if (vo is null)
                                    {
                                        Form1.log.Info("Ошибка формата потока 176. Ошибка получения соответствия уникального номера колонки и индекса значения при разборе предопределенных элементов. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Путь ДС {1}, " +
                                                       "Свойство {2}, " +
                                                       "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                        ok = false;
                                        break;
                                    }
                                    vn = (Value1C_number)vo.getproperty("УникальныйНомерКолонки");
                                    if (vn is null)
                                    {
                                        Form1.log.Info("Ошибка формата потока 177. Ошибка получения уникального номера колонки в соответствии колонок и значений при разборе предопределенных элементов. " +
                                                       "Загружаемый тип {0}, " +
                                                       "Путь ДС {1}, " +
                                                       "Свойство {2}, " +
                                                       "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                        ok = false;
                                        break;
                                    }
                                    if (!nok)
                                    {
                                        if (ni == vn.v_number)
                                        {
                                            vn = (Value1C_number)vo.getproperty("ИндексЗначения");
                                            if (vn is null)
                                            {
                                                Form1.log.Info("Ошибка формата потока 178. Ошибка получения индекса значения в соответствии колонок и значений при разборе предопределенных элементов. " +
                                                               "Загружаемый тип {0}, " +
                                                               "Путь ДС {1}, " +
                                                               "Свойство {2}, " +
                                                               "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                                ok = false;
                                                break;
                                            }
                                            ni = vn.v_number;
                                            nok = true;
                                        }
                                    }
                                    if (!uok)
                                    {
                                        if (ui == vn.v_number)
                                        {
                                            vn = (Value1C_number)vo.getproperty("ИндексЗначения");
                                            if (vn is null)
                                            {
                                                Form1.log.Info("Ошибка формата потока 178. Ошибка получения индекса значения в соответствии колонок и значений при разборе предопределенных элементов. " +
                                                               "Загружаемый тип {0}, " +
                                                               "Путь ДС {1}, " +
                                                               "Свойство {2}, " +
                                                               "Путь {3}", t.Name, tn.path(), prop.Name, spath);
                                                ok = false;
                                                break;
                                            }
                                            ui = vn.v_number;
                                            uok = true;
                                        }
                                    }
                                    if (nok && uok) break;
                                }
                            }
                            if (ok)
                            {
                                readPredefinedValues((Value1C_metaobj)v, ni, ui, vStrings, spath);
                            }
                        }

                        #endregion

                        break;
                    case ExternalFileFormat.eff_text:
                    case ExternalFileFormat.eff_tabdoc:
                    case ExternalFileFormat.eff_binary:
                    case ExternalFileFormat.eff_activedoc:
                    case ExternalFileFormat.eff_htmldoc:
                    case ExternalFileFormat.eff_textdoc:
                    case ExternalFileFormat.eff_geo:
                    case ExternalFileFormat.eff_kd:
                    case ExternalFileFormat.eff_mkd:
                    case ExternalFileFormat.eff_graf:
                    case ExternalFileFormat.eff_xml:
                    case ExternalFileFormat.eff_wsdl:
                    case ExternalFileFormat.eff_picture:
                        cf = stor.readfile(npath);
                        if (cf != null)
                        {
                            vb = new Value1C_binary(v);
                            vb.type = nt;
                            vb.v_binformat = ext.format;
                            vb.v_binary = new MemoryStream();
                            cf.str.CopyTo(vb.v_binary);
                            stor.close(cf);
                        }
                        else
                        {
                            if (ext.optional)
                                break;
                            Form1.log.Info("Ошибка формата потока 145. Не найден внешний файл. " +
                                           "Загружаемый тип {0}, " +
                                           "Путь {2}", t.Name, storpresent + npath);
                            vb = new Value1C_binary(v);
                            vb.type = nt;
                        }
                        v.v_objprop[ext.prop] = vb;
                        break;
                    default:
                        Form1.log.Info("Ошибка формата потока 143. Неизвестное значение формата внешнего файла. " +
                                       "Загружаемый тип {0}, " +
                                       "Формат {1}, " +
                                       "Путь {2}", t.Name, ext.format, storpresent + npath);
                        break;
                }
                if (cfc != null)
                    stor.close(cfc);
            }

            #endregion

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
            List<UninitValue1C> _puninitvalues = new List<UninitValue1C>(); // этот список должен где-то заполняться........

            // TODO : Разобраться с заполнением данного списка "uninitvalues"
            //_puninitvalues = puninitvalues;

            if (PValues.puninitvalues != null)
                _puninitvalues = PValues.puninitvalues;

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
                        Form1.log.Info("Ошибка формата потока 74. Не определен объект метаданных по UID. " +
                                       "UID {0}, " +
                                       "Путь {1}" +
                                       "Мета {2}", uid.ToString(), v.path(this, false), v.fullpath(this, false));
                    }
                }
                else if (v.kind == KindOfValue1C.kv_refpre)
                {
                    vrp = (Value1C_refpre)v;
                    vrp.v_prevalue = getPreValue(uid);
                    if (vrp.v_prevalue is null)
                    {
                        Form1.log.Info("Ошибка формата потока 162. Не определен предопределенный элемент по UID. " +
                                       "UID {0}, " +
                                       "Путь {1}" +
                                       "Мета {2}", uid.ToString(), v.path(this, false), v.fullpath(this, false));
                    }
                }
                else if (v.kind == KindOfValue1C.kv_type)
                {
                    vt = (Value1C_type)v;
                    uid = vt.v_uid;
                    vt.v_type = (uid == EmptyUID) ? MetaTypeSet.mt_empty : ftypes.GetTypeByUID(uid);

                    if (vt.v_type is null)
                    {
                        Form1.log.Info("Ошибка формата потока 75. Не определен тип по UID. " +
                                       "UID {0}, " +
                                       "Путь {1}" +
                                       "Мета {2}", uid.ToString(), v.path(this, false), v.fullpath(this, false));
                    }
                }
                else if (v.kind == KindOfValue1C.kv_stdattr)
                {
                    vsa = (Value1C_stdattr)v;
                    mo = getMetaObject(uid);
                    vsa.v_stdattr = null;
                    if (mo is null)
                    {
                        Form1.log.Info("Ошибка формата потока 106. Ошибка загрузки стандартного реквизита. Не определен объект метаданных по UID. " +
                                       "UID {0}, " +
                                       "Путь {1}" +
                                       "Мета {2}", uid.ToString(), v.path(this, false), v.fullpath(this, false));
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
                                                Form1.log.Info("Ошибка формата потока 212. Ошибка загрузки стандартного реквизита. Значение за пределами допустимого. " +
                                                               "Класс {0}, " +
                                                               "Значение {1}, " +
                                                               "Минимальное значение {2}, " +
                                                               "Максимальное значение {3}, " +
                                                               "Путь {4}, " +
                                                               "Мета {5}.", cl.UID.ToString(), j, sa.Value, sa.ValueMax, v.path(this), v.fullpath(this));
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
                                Form1.log.Info("Ошибка формата потока 116. Не определен объект метаданных стандартного реквизита по UID. " +
                                               "UID {0}, " +
                                               "Путь {1}, " +
                                               "Мета {2}.", uid.ToString(), v.path(this), v.fullpath(this));
                            }
                            else
                            {
                                cl = mo.Value.getclass();
                                if (cl is null)
                                {
                                    Form1.log.Info("Ошибка формата потока 107. Не определен класс объекта метаданных. " +
                                                   "Объект метаданных {0}, " +
                                                   "Путь {1}, " +
                                                   "Мета {2}.", mo.FullName, v.path(this), v.fullpath(this));
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
                                                    Form1.log.Info("Ошибка формата потока 206. Ошибка загрузки стандартного реквизита. Значение за пределами допустимого. " +
                                                                   "Класс {0}, " +
                                                                   "Значение {1}, " +
                                                                   "Минимальное значение {2}, " +
                                                                   "Максимальное значение {3}, " +
                                                                   "Путь {4}, " +
                                                                   "Мета {5}.", cl.UID.ToString(), j, sa.Value, sa.ValueMax, v.path(this), v.fullpath(this));
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
                                        Form1.log.Info("Ошибка формата потока 108. Не найден стандартный реквизит по значению. " +
                                                       "Класс {0}, " +
                                                       "Значение {1}, " +
                                                       "Путь {2}, " +
                                                       "Мета {3}.", cl.UID.ToString(), j, v.path(this), v.fullpath(this));
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
                        Form1.log.Info("Ошибка формата потока 209. Ошибка загрузки стандартной табличной части.  Не определен объект метаданных по metauid. " +
                                       "UID {0}, " +
                                       "Путь {1}, " +
                                       "Мета {2}.", uid.ToString(), v.path(this), v.fullpath(this));
                    }
                    else
                    {
                        cl = mo.Value.getclass();
                        if (cl is null)
                        {
                            Form1.log.Info("Ошибка формата потока 210. Не определен класс объекта метаданных. " +
                                           "Объект метаданных {0}, " +
                                           "Путь {1}, " +
                                           "Мета {2}.", mo.FullName, v.path(this), v.fullpath(this));
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
                                Form1.log.Info("Ошибка формата потока 211. Не найдена стандартная табличная часть по значению. " +
                                               "Класс {0}, " +
                                               "Значение {1}, " +
                                               "Путь {2}, " +
                                               "Мета {3}.", cl.UID.ToString(), j, v.path(this), v.fullpath(this));
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
            string version_version;
            string s;
            string metaname;
            string emetaname;
            string typepref;
            string etypepref;
            string mtypename;

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
                Form1.log.Info("Ошибка формата потока 81. Неизвестная версия в файле version. Путь {0}, Версия {1}", storpresent + "version", version_version);
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
                    Form1.log.Info("Ошибка формата потока 79. Версия root не соответствует версии version. Версия root {0}, Версия version {1}, Путь {2}", i, version_version, storpresent);
                    stor = null;
                    return;
                }
            }
            else if (i == 2)
            {
                if (containerver < ContainerVer.cv_106_0)
                {
                    Form1.log.Info("Ошибка формата потока 78. Версия root не соответствует версии version. Версия root {0}, Версия version {1}, Путь {2}", i, version_version, storpresent);
                    stor = null;
                    return;
                }
            }
            else
            {
                Form1.log.Info("Ошибка формата потока 77. Неизвестная версия root. Версия root {0}, Путь {1}", i, storpresent);
                stor = null;
                return;
            }
            tr = gettree(stor, metaprefix + s);
            if (tr is null) return;
            t = tr.Get_First();
            metats = null;

            // TODO: Данный код пока не работает :( Надо исправлять.......!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
                    for (vo = (Value1C_metaobj)vvo.parent; vo != null; vo = (Value1C_metaobj)vo.parent)
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

        /// <summary>
        /// Получить предопределенный элемент по УИД
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public PredefinedValue getPreValue(Guid u)   
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
