using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode1
    {
        [FieldOffset(0)]
        public int num1;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(0)]
        public Guid uid1;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(0)]
        public MetaValue val1;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(0)]
        public MetaProperty prop1;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(0)]
        public MetaGeneratedType gentype;                // генерируемый тип (type == stt_gentype)
        [FieldOffset(0)]
        public ContainerVer vercon1;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(0)]
        public Version1C ver1C1;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(0)]
        public SerializationTreeClassType classtype;     // вид коллекции классов ((type == stt_classcol) 
        [FieldOffset(0)]
        public ClassParameter classpar1;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UTreeNode2
    {
        [FieldOffset(0)]
        public int num2;                                 // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_number)   
        [FieldOffset(0)]
        public Guid uid2;                                // (((type == stt_const ИЛИ type == stt_cond) И typeval1 = stv_uid) ИЛИ type == stt_class ИЛИ type == stt_idel) 
        [FieldOffset(0)]
        public MetaValue val2;                           // значение (type == stt_cond И typeval1 = stv_value)
        [FieldOffset(0)]
        public MetaProperty prop2;                       // свойство (type == stt_prop ИЛИ ((type == stt_cond ИЛИ type == stt_elcol) И typeval1 = stv_prop))
        [FieldOffset(0)]
        public ContainerVer vercon2;                     // версия контейнера (type == stt_cond И typeval1 = stv_vercon)
        [FieldOffset(0)]
        public Version1C ver1C2;                         // версия 1С (type == stt_cond И typeval1 = stv_ver1С)
        [FieldOffset(0)]
        public ClassParameter classpar2;                 // параметр класса (type == stt_cond И typeval1 = stv_classpar)
    }


    //---------------------------------------------------------------------------
    // Переменная дерева сериализации
    public class SerializationTreeVar
    {
        public String fname;
        public bool fcolcount;
        public bool fisglobal;
        public bool fisfix;
        public int ffixvalue;
        public List<VarValidValue> fvalidvalues;

        public SerializationTreeVar(Tree tr)
        {

        }

        public String Name
        {
            get { return fname; }
        }

        public bool ColCount
        {
            get { return fcolcount; }
        }

        public bool IsGlobal
        {
            get { return fisglobal; }
        }

        public bool IsFix
        {
            get { return fisfix; }
        }

        public int FixValue
        {
            get { return ffixvalue; }
        }

        public List<VarValidValue> validvalues
        {
            get { return fvalidvalues; }
        }

    }

    //---------------------------------------------------------------------------
    // Узел дерева сериализации
    public class SerializationTreeNode
    {
        public SerializationTreeNode next; // следующий на этом уровне
        public SerializationTreeNode first; // первый на подчиненном уровне
        public SerializationTreeNode parent; // родитель
        public uint index; // индекс на текущем уровне
        public MetaType owner;

        public SerializationTreeNodeType type;
        public SerializationTreeCondition condition; // (type == stt_cond)

        public SerializationTreeValueType typeval1; // Тип значения 1 (type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_elcol)
        public string str1; // ((type == stt_const ИЛИ type == stt_cond ИЛИ type == stt_var ИЛИ type == stt_elcol) И typeval1 = stv_string ИЛИ typeval1 = stv_var ИЛИ typeval1 = stv_globalvar)

        public UTreeNode1 uTreeNode1;

        public MetaType typeprop; // тип свойства (type == stt_prop ИЛИ type == stt_elcol ИЛИ type == stt_idel)
        public SerializationTreeValueType typeval2; // Тип значения 2 (type == stt_cond)
        public string str2; // (type == stt_cond И (typeval2 = stv_string ИЛИ typeval2 = stv_var ИЛИ typeval2 = stv_globalvar))

        public UTreeNode2 uTreeNode2;

        public bool nomove; //
        public bool isref; // Признак ссылки на объект метаданных (т.е. тут есть только UID, самого определения объекта метаданных тут нет)
        public bool isrefpre; // Признак ссылки на предопределенный элемент (т.е. тут есть только UID, самого предопределенного элемента тут нет)
        public bool isright; // Это право
        public bool exnernal; // Объект находится во внешнем файле, тут только UID, который является именем внешнего файла
        public BinarySerializationType binsertype;
        public ExternalFileFormat binformat;

        public SerializationTreeNode()
        {
            next = null;
            first = null;
            nomove = false;
        }
        public SerializationTreeNode(MetaType _owner, Tree tr, SerializationTreeNode _parent, uint _index)
        {
            uint i;
            string str;
            string stval;
            string sval1;
            string sval2;
            MetaType typ;

            owner = _owner;
            parent = _parent;
            index = _index;

            type = (SerializationTreeNodeType)(Convert.ToInt32(tr.Get_Value()));
            tr = tr.Get_Next();
            stval = tr.Get_Value();
            tr = tr.Get_Next();
            sval1 = tr.Get_Value();
            tr = tr.Get_Next();
            sval2 = tr.Get_Value();

            if (type == SerializationTreeNodeType.stt_class)
            {
                if (parent == null)
                {
                    // error(L"Ошибка загрузки статических типов. Узел типа Класс находится на верхнем уровне"
                    //     , L"Тип", owner->name
                    //     , L"Путь", path());
                }   
                else if (parent.type != SerializationTreeNodeType.stt_classcol)
                {
                    // error(L"Ошибка загрузки статических типов. Узел типа Класс находится не в узле типа Коллекция классов"
                    //     , L"Тип", owner->name
                    //     , L"Путь", path());
                }
            }
            else
            {
                if (parent != null)
                    if (parent.type == SerializationTreeNodeType.stt_classcol)
                    {
                        // error(L"Ошибка загрузки статических типов. Узел не типа Класс находится в узле типа Коллекция классов"
                        //     , L"Тип", owner->name
                        //     , L"Путь", path());

                    }
            }

            if (type == SerializationTreeNodeType.stt_idel)
            {
                if (parent == null)
                {
                    // error(L"Ошибка загрузки статических типов. Узел типа ИД-элемент находится на верхнем уровне"
                    //     , L"Тип", owner->name
                    //     , L"Путь", path());
                }
                else
                {
                    if (parent.type != SerializationTreeNodeType.stt_idcol)
                    {
                        // error(L"Ошибка загрузки статических типов. Узел типа ИД-элемент находится не в узле типа Коллекция ИД-элементов"
                        //     , L"Тип", owner->name
                        //     , L"Путь", path());

                    }
                }

            }
            else
            {
                if ((parent != null) && (parent.type == SerializationTreeNodeType.stt_idcol))
                {
                    // error(L"Ошибка загрузки статических типов. Узел не типа ИД-элемент находится в узле типа Коллекция ИД-элементов"
                    //     , L"Тип", owner->name
                    //     , L"Путь", path());

                }
            }

            switch (type)
            {
                case SerializationTreeNodeType.stt_min:
                    break;
                case SerializationTreeNodeType.stt_const:
                    typeval1 = (SerializationTreeValueType)(Convert.ToInt32(stval));
                    switch (typeval1)
                    {
                        case SerializationTreeValueType.stv_string:
                            str1 = sval1;
                            break;
                        case SerializationTreeValueType.stv_number:
                            uTreeNode1.num1 = Convert.ToInt32(sval1);
                            break;
                        case SerializationTreeValueType.stv_uid:
                            if (!string_to_GUID(sval1, ref uTreeNode1.uid1))
                            {
                                // error(L"Ошибка загрузки статических типов. Ошибка преобразования УИД в константе дерева сериализации"
                                //     , L"Тип", owner->name
                                //     , L"Путь", path()
                                //     , L"УИД", sval1);
                            }
                            break;
                        default:
                            // error(L"Ошибка загрузки статических типов. Некорректный тип значения константы"
                            //     , L"Тип", owner->name
                            //     , L"Путь", path()
                            //     , L"Тип значения", (int)typeval1);

                            break;
                    }
                    break;
                case SerializationTreeNodeType.stt_var:
                    str1 = sval1;
                    break;
                case SerializationTreeNodeType.stt_list:
                    break;
                case SerializationTreeNodeType.stt_prop:
                    uTreeNode1.prop1 = owner.GetProperty(sval1);
                    if (uTreeNode1.prop1 ==  null)
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное имя свойства дерева сериализации"
                        //     , L"Тип", owner->name
                        //     , L"Путь", path()
                        //     , L"Имя свойства", sval1);
                    }
                    typeprop = null;
                    if (!String.IsNullOrEmpty(stval))
                    {
                        typeprop = MetaTypeSet.staticTypes.GetTypeByName(stval);
                        if (typeprop == null)
                        {
                            // error(L"Ошибка загрузки статических типов. Некорректное имя типа свойства"
                            //  , L"Тип", owner->name
                            //  , L"Путь", path()
                            //  , L"Имя свойства", sval1
                            //  , L"Имя типа", stval);
                        }
                    }
                    uTreeNode2.prop2 = null;
                    if (sval2.Length > 0)
                    {
                        uTreeNode2.prop2 = owner.GetProperty(sval2);
                        if (uTreeNode2.prop2 == null)
                        {
                            // error(L"Ошибка загрузки статических типов. Некорректное имя свойства дерева сериализации"
                            //     , L"Тип", owner->name
                            //     , L"Путь", path()
                            //     , L"Имя свойства", sval2);
                        }
                    }
                    break;
                case SerializationTreeNodeType.stt_elcol:
                    str = sval1.Substring(1, 1);
                    if (string.IsNullOrEmpty(str))
                    {
                        typeval1 = SerializationTreeValueType.stv_none;
                    }
                    else if (str.CompareTo("%") == 0)
                    {
                        typeval1 = SerializationTreeValueType.stv_var;
                        str1 = sval1.Substring(2, sval1.Length - 1);
                    }
                    else if (str.CompareTo(".") == 0)
                    {
                        sval1 = sval1.Substring(2, sval1.Length - 1);
                        typeval1 = SerializationTreeValueType.stv_prop;
                        uTreeNode1.prop1 = owner.GetProperty(sval1);
                        if (uTreeNode1.prop1 == null)
                        {
                            // error(L"Ошибка загрузки статических типов. Некорректное имя свойства дерева сериализации"
                            //     , L"Тип", owner->name
                            //     , L"Путь", path()
                            //     , L"Имя свойства", sval1);
                        }
                    }
                    else
                    {
                        str = sval1.Substring(1, 2);
                        if (str.CompareTo("N'") == 0)
                        {
                            typeval1 = SerializationTreeValueType.stv_number;
                            uTreeNode1.num1 = Convert.ToInt32(sval1.Substring(3, sval1.Length - 2));
                        }
                        else
                        {
                            // error(L"Ошибка загрузки статических типов. Ошибка разбора значения в элементе коллекции дерева сериализации"
                            //     , L"Тип", owner->name
                            //     , L"Путь", path()
                            //     , L"Значение", sval1);
                        }

                    }
                    typeprop = null;
                    if (!string.IsNullOrEmpty(stval))
                    {
                        typeprop = MetaTypeSet.staticTypes.GetTypeByName(stval);
                        if (typeprop == null)
                        {
                            // error(L"Ошибка загрузки статических типов. Некорректное имя типа элемента коллекции"
                            //  , L"Тип", owner->name
                            //  , L"Путь", path()
                            //  , L"Имя типа", stval);
                        }
                    }

                    break;
                case SerializationTreeNodeType.stt_gentype:
                    uTreeNode1.gentype = null;
                    for (i = 0; i < owner.generatedtypes.size(); ++i) if (sval1.CompareIC(owner->generatedtypes[i]->name) == 0)
                        {
                            gentype = owner->generatedtypes[i];
                        }
                    if (!gentype)
                    {
                        error(L"Ошибка загрузки статических типов. Некорректное имя генерируемого типа"
                            , L"Тип", owner->name
                            , L"Путь", path()
                            , L"Имя генерируемого типа", sval1);
                    }

                    break;
                case SerializationTreeNodeType.stt_cond:
                    break;
                case SerializationTreeNodeType.stt_metaid:
                    break;
                case SerializationTreeNodeType.stt_classcol:
                    break;
                case SerializationTreeNodeType.stt_class:
                    break;
                case SerializationTreeNodeType.stt_idcol:
                    break;
                case SerializationTreeNodeType.stt_idel:
                    break;
                case SerializationTreeNodeType.stt_max:
                    break;
                default:
                    break;
            }


        }

        public string path()
        {
            SerializationTreeNode stn;
            string path;

            for (stn = parent, path = Convert.ToString(index); stn != null; stn = stn.parent)
            {
                path = Convert.ToString((stn.index) + ":" + path;
            }
            return path;
        }

        public static SerializationTreeNode SerializationTree(MetaType _owner, Tree tr, SerializationTreeNode parent)
        {

            int count, i;
            SerializationTreeNode st = null;
            SerializationTreeNode cst = null;
            SerializationTreeNode nst;

            count = Convert.ToInt32(tr.Get_Value());

            if (count == 0)
                return null;
            for (i = 0; i < count; ++i)
            {
                tr = tr.Get_Next();
                nst = new SerializationTreeNode(_owner, tr.Get_First(), parent, (uint)i);
                if (i !=0 )
                    cst.next = nst;
                else
                    st = nst;
                cst = nst;
            }
            return st;
        }

        public static string typevalpresentation(SerializationTreeValueType typeval)
        {
            switch (typeval)
            {
                case SerializationTreeValueType.stv_string:    return "Строка";
                case SerializationTreeValueType.stv_number:    return "Число";
                case SerializationTreeValueType.stv_uid:       return "УникальныйИдентификатор";
                case SerializationTreeValueType.stv_value:     return "Значение";
                case SerializationTreeValueType.stv_var:       return "Переменная";
                case SerializationTreeValueType.stv_prop:      return "Свойство";
                case SerializationTreeValueType.stv_vercon:    return "Версия контейнера";
                case SerializationTreeValueType.stv_ver1C:     return "Версия 1C";
                case SerializationTreeValueType.stv_classpar:  return "Параметр класса";
                case SerializationTreeValueType.stv_globalvar: return "Глобальная переменная";
                case SerializationTreeValueType.stv_none:      return "<Нет значения>";
            }
            return "<Неизвестный тип значения дерева сериализации>";
        }

        public string typeval1presentation()
        {
            return typevalpresentation(typeval1);
        }

        public string typeval2presentation()
        {
            return typevalpresentation(typeval2);
        }

    }

}
