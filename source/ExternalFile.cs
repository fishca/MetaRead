using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static MetaRead.APIcfBase;


namespace MetaRead
{

    //---------------------------------------------------------------------------
    // Внешний файл типа
    public class ExternalFile
    {
        public MetaType owner;
        public MetaProperty prop; // Свойство
        public bool relativepath; // признак относительного пути файла
        public string name; // Имя
        public string ext; // Расширение (без точки впереди)
        public bool catalog; // признак каталога (если catalog == true, то это файл <name+"."+ext+"/"+filename>, иначе <name+"."+ext>)
        public string filename; // Имя файла (только если catalog == true)
        public ExternalFileFormat format; // Формат файла
        public MetaType type; // Тип свойства
        public bool optional; // признак необязательности
        public Version1C ver1C; // Минимальная версия 1С
        public bool havecondition; // признак наличия условия
        public SerializationTreeCondition condition; // (type == stt_cond)

        public SerializationTreeValueType typeval1; // Тип значения 1 (havecondition == true)
        public string str1; // typeval1 = stv_string ИЛИ typeval1 = stv_var ИЛИ typeval1 = stv_globalvar

        public UTreeNode3 uTreeNode3;
        public SerializationTreeValueType typeval2; // Тип значения 2 (havecondition == true)
        public string str2; // typeval2 = stv_string ИЛИ typeval2 = stv_var ИЛИ typeval2 = stv_globalvar

        public UTreeNode4 uTreeNode4;

        public ExternalFile(MetaType _owner, Tree tr)
        {
            Tree tt;
            string sval;
            string str;
            uint i;
            MetaType typ;

            owner = _owner;

            tt = tr.Get_First();
            prop = owner.GetProperty(tt.Get_Value());

            if (!(prop is null))
            {
                // error(L"Ошибка загрузки статических типов. Некорректное имя свойства дерева сериализации в описании внешнего файла"
                //     , L"Тип", owner->name
                //     , L"Имя свойства", tt->get_value());
            }

            tt = tt.Get_Next();
            relativepath = tt.Get_Value().CompareTo("1") == 0 ? true : false;
            tt = tt.Get_Next();
            name = tt.Get_Value();
            tt = tt.Get_Next();
            ext = tt.Get_Value();
            tt = tt.Get_Next();
            catalog = tt.Get_Value().CompareTo("1") == 0 ? true : false;
            tt = tt.Get_Next();
            filename = tt.Get_Value();
            tt = tt.Get_Next();
            format = (ExternalFileFormat)(Convert.ToInt32(tt.Get_Value()));
            if (format <= ExternalFileFormat.eff_min || format >= ExternalFileFormat.eff_max)
            {
                // error(L"Ошибка загрузки статических типов. Некорректное значение формата внешнего файла"
                //     , L"Тип", owner->name
                //     , L"Значение", tt->get_value());
            }
            tt = tt.Get_Next();
            str = tt.Get_Value();

            if (string.IsNullOrEmpty(str))
                type = null;
            else
            {
                type = MetaTypeSet.staticTypes.GetTypeByName(str);
                if (!(type is null))
                {
                    //   error(L"Ошибка загрузки статических типов. Некорректное имя типа внешнего файла"
                    //    , L"Тип", owner->name
                    //    , L"Имя типа", str);
                }
            }
            tt = tt.Get_Next();
            optional = tt.Get_Value().CompareTo("1") == 0 ? true : false;
            tt = tt.Get_Next();
            ver1C = stringtover1C(tt.Get_Value());
            if (ver1C == Version1C.v1C_min)
            {
                // error(L"Ошибка загрузки статических типов. Некорректное значение версии 1C в описании внешнего файла"
                //     , L"Тип", owner->name
                //     , L"Значение", tt->get_value());
            }
            tt = tt.Get_Next();
            havecondition = tt.Get_Value().CompareTo("1") == 0 ? true : false;
            if (havecondition)
            {
                tt = tt.Get_Next();
                condition = (SerializationTreeCondition)(Convert.ToInt32(tt.Get_Value()));
                if (condition <= SerializationTreeCondition.stc_min || condition >= SerializationTreeCondition.stc_max)
                {
                    // error(L"Ошибка загрузки статических типов. Некорректный вид условия в описании внешнего файла"
                    //     , L"Тип", owner->name
                    //     , L"Вид условия", tt->get_value());
                }

                tt = tt.Get_Next();
                sval = tt.Get_Value();


                str = sval.Substring(1, 1);

                if (str.CompareTo("%") == 0)
                {
                    typeval1 = SerializationTreeValueType.stv_var;
                    str1 = sval.Substring(2, sval.Length - 1);
                }
                else if (str.CompareTo(".") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval1 = SerializationTreeValueType.stv_prop;
                    uTreeNode3.prop1 = owner.GetProperty(sval);
                    if (!(uTreeNode3.prop1 is null))
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное имя свойства в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Имя свойства", sval);

                    }
                }
                else if (str.CompareTo("*") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval1 = SerializationTreeValueType.stv_value;
                    uTreeNode3.val1 = null;
                    //i = sval.Pos(".");
                    i = (uint)sval.IndexOf(".");
                    if (i != 0)
                    {
                        str = sval.Substring(1, (int)i - 1);
                        typ = MetaTypeSet.staticTypes.GetTypeByName(str);
                        if (typ != null)
                        {
                            str = sval.Substring((int)i + 1, (int)(sval.Length - i));
                            uTreeNode3.val1 = typ.GetValue(str);
                        }
                    }

                    if (!(uTreeNode3.val1 is null))
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное значение в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo("v") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval1 = SerializationTreeValueType.stv_vercon;

                    if (sval == "2.0")
                        uTreeNode3.vercon1 = ContainerVer.cv_2_0;
                    else if (sval == "5.0") uTreeNode3.vercon1 = ContainerVer.cv_5_0;
                    else if (sval == "6.0") uTreeNode3.vercon1 = ContainerVer.cv_6_0;
                    else if (sval == "106.0") uTreeNode3.vercon1 = ContainerVer.cv_106_0;
                    else if (sval == "200.0") uTreeNode3.vercon1 = ContainerVer.cv_200_0;
                    else if (sval == "202.2") uTreeNode3.vercon1 = ContainerVer.cv_202_2;
                    else if (sval == "216.0") uTreeNode3.vercon1 = ContainerVer.cv_216_0;
                    else
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное значение версии контейнера в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo(":") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval1 = SerializationTreeValueType.stv_ver1C;
                    uTreeNode3.ver1C1 = stringtover1C(sval);
                    if (uTreeNode3.ver1C1 == Version1C.v1C_min)
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное значение версии 1C в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo("&") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval1 = SerializationTreeValueType.stv_classpar;
                    uTreeNode3.classpar1 = ClassParameter.GetParam(sval);
                    if (!(uTreeNode3.classpar1 is null))
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное имя параметра класса в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo("~") == 0)
                {
                    typeval1 = SerializationTreeValueType.stv_globalvar;
                    str1 = sval.Substring(2, sval.Length - 1);
                }
                else
                {
                    str = sval.Substring(1, 2);
                    if (str.CompareTo("S'") == 0)
                    {
                        typeval1 = SerializationTreeValueType.stv_string;
                        str1 = sval.Substring(3, sval.Length - 2);
                    }
                    else if (str.CompareTo("N'") == 0)
                    {
                        typeval1 = SerializationTreeValueType.stv_number;
                        uTreeNode3.num1 = Convert.ToInt32(sval.Substring(3, sval.Length - 2));
                    }
                    else if (str.CompareTo("U'") == 0)
                    {
                        typeval1 = SerializationTreeValueType.stv_uid;
                        if (!string_to_GUID(sval.Substring(3, sval.Length - 2), ref uTreeNode3.uid1))
                        {
                            // error(L"Ошибка загрузки статических типов. Ошибка преобразования УИД в условии в описании внешнего файла"
                            //     , L"Тип", owner->name
                            //     , L"УИД", sval);
                        }
                    }
                    else
                    {
                        // error(L"Ошибка загрузки статических типов. Ошибка разбора значения в условии в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }

                tt = tt.Get_Next();
                sval = tt.Get_Value();
                str = sval.Substring(1, 1);
                if (str.CompareTo("%") == 0)
                {
                    typeval2 = SerializationTreeValueType.stv_var;
                    str2 = sval.Substring(2, sval.Length - 1);
                }
                else if (str.CompareTo(".") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval2 = SerializationTreeValueType.stv_prop;
                    uTreeNode4.prop2 = owner.GetProperty(sval);
                    if (!(uTreeNode4.prop2 is null))
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное имя свойства в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Имя свойства", sval);
                    }

                }
                else if (str.CompareTo("*") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval2 = SerializationTreeValueType.stv_value;
                    uTreeNode4.val2 = null;
                    //i = sval.Pos(L".");
                    i = (uint)sval.IndexOf(".");
                    if (i != 0)
                    {
                        str = sval.Substring(1, (int)i - 1);
                        typ = MetaTypeSet.staticTypes.GetTypeByName(str);
                        if (typ != null)
                        {
                            str = sval.Substring((int)i + 1, (int)(sval.Length - i));
                            uTreeNode4.val2 = typ.GetValue(str);
                        }
                    }

                    if (uTreeNode4.val2 is null)
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное значение в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo("v") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval2 = SerializationTreeValueType.stv_vercon;
                    if (sval == "2.0") uTreeNode4.vercon2 = ContainerVer.cv_2_0;
                    else if (sval == "5.0") uTreeNode4.vercon2 = ContainerVer.cv_5_0;
                    else if (sval == "6.0") uTreeNode4.vercon2 = ContainerVer.cv_6_0;
                    else if (sval == "106.0") uTreeNode4.vercon2 = ContainerVer.cv_106_0;
                    else if (sval == "200.0") uTreeNode4.vercon2 = ContainerVer.cv_200_0;
                    else if (sval == "202.2") uTreeNode4.vercon2 = ContainerVer.cv_202_2;
                    else if (sval == "216.0") uTreeNode4.vercon2 = ContainerVer.cv_216_0;
                    else
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное значение версии контейнера в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo(":") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval2 = SerializationTreeValueType.stv_ver1C;
                    uTreeNode4.ver1C2 = stringtover1C(sval);
                    if (uTreeNode4.ver1C2 == Version1C.v1C_min)
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное значение версии 1C в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo("&") == 0)
                {
                    sval = sval.Substring(2, sval.Length - 1);
                    typeval2 = SerializationTreeValueType.stv_classpar;
                    uTreeNode4.classpar2 = ClassParameter.GetParam(sval);
                    if (uTreeNode4.classpar2 == null)
                    {
                        // error(L"Ошибка загрузки статических типов. Некорректное имя параметра класса в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
                else if (str.CompareTo("~") == 0)
                {
                    typeval2 = SerializationTreeValueType.stv_globalvar;
                    str2 = sval.Substring(2, sval.Length - 1);
                }
                else
                {
                    str = sval.Substring(1, 2);
                    if (str.CompareTo("S'") == 0)
                    {
                        typeval2 = SerializationTreeValueType.stv_string;
                        str2 = sval.Substring(3, sval.Length - 2);
                    }
                    else if (str.CompareTo("N'") == 0)
                    {
                        typeval2 = SerializationTreeValueType.stv_number;
                        uTreeNode4.num2 = Convert.ToInt32(sval.Substring(3, sval.Length - 2));
                    }
                    else if (str.CompareTo("U'") == 0)
                    {
                        typeval2 = SerializationTreeValueType.stv_uid;
                        if (!string_to_GUID(sval.Substring(3, sval.Length - 2), ref uTreeNode4.uid2))
                        {
                            // error(L"Ошибка загрузки статических типов. Ошибка преобразования УИД в условии в описании внешнего файла"
                            //     , L"Тип", owner->name
                            //     , L"УИД", sval);
                        }
                    }
                    else
                    {
                        // error(L"Ошибка загрузки статических типов. Ошибка разбора значения в условии в описании внешнего файла"
                        //     , L"Тип", owner->name
                        //     , L"Значение", sval);
                    }
                }
            }
        }
        
        public string typeval1presentation()
        {
            return SerializationTreeNode.typevalpresentation(typeval1);
        }
        public string typeval2presentation()
        {
            return SerializationTreeNode.typevalpresentation(typeval2);
        }

    }
}
