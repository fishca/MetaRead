using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MetaRead.Constants;



namespace MetaRead
{
    #region Перечисления

    public enum _state
    {
        s_value              = 0, // ожидание начала значения
        s_delimitier         = 1, // ожидание разделителя
        s_string             = 2, // режим ввода строки
        s_quote_or_endstring = 3, // режим ожидания конца строки или двойной кавычки
        s_nonstring          = 4  // режим ввода значения не строки
    }

    public enum Node_Type
    {
        nd_empty      = 0, // пусто
        nd_string     = 1, // строка
        nd_number     = 2, // число
        nd_number_exp = 3, // число с показателем степени
        nd_guid       = 4, // уникальный идентификатор
        nd_list       = 5, // список
        nd_binary     = 6, // двоичные данные (с префиксом #base64:)
        nd_binary2    = 7, // двоичные данные формата 8.2 (без префикса)
        nd_link       = 8, // ссылка
        nd_binary_d   = 9, // двоичные данные (с префиксом #data:)
        nd_unknown         // неизвестный тип
    }

    //---------------------------------------------------------------------------
    // Версии контейнера 1С
    // значения версий должны обязательно располагаться по возрастанию, чтобы можно было сравнивать версии на >, < и =
    public enum ContainerVer
    {
        cv_2_0   = 1,
        cv_5_0   = 2,
        cv_6_0   = 3,
        cv_106_0 = 4,
        cv_200_0 = 5,
        cv_202_2 = 6,
        cv_216_0 = 7
    }

    //---------------------------------------------------------------------------
    // Версии 1С
    // значения версий должны обязательно располагаться по возрастанию, чтобы можно было сравнивать версии на >, < и =
    public enum Version1C
    {
        v1C_min    = 0,
        v1C_8_0    = 1,
        v1C_8_1    = 2,
        v1C_8_2    = 3,
        v1C_8_2_14 = 4,
        v1C_8_3_1  = 5,
        v1C_8_3_2  = 6,
        v1C_8_3_3  = 7,
        v1C_8_3_4  = 8,
        v1C_8_3_5  = 9,
        v1C_8_3_6  = 10,
        v1C_8_3_7  = 11,
        v1C_8_3_8  = 12,
        v1C_8_3_9  = 13,
        v1C_8_3_10 = 14,
        v1C_8_3_11 = 15,
        v1C_8_3_12 = 16,
        v1C_8_3_13 = 17,
        v1C_8_3_14 = 18
    }

    //---------------------------------------------------------------------------
    // Способы выгрузки
    public enum ExportType
    {
        et_default = 0,
        et_catalog = 1,
        et_file    = 2
    }

    //---------------------------------------------------------------------------
    // Виды значений по умолчанию
    public enum DefaultValueType
    {
        dvt_novalue = 0, // Нет значения по умолчанию
        dvt_bool    = 1, // Булево
        dvt_number  = 2, // Число
        dvt_string  = 3, // Строка
        dvt_date    = 4, // Дата
        dvt_undef   = 5, // Неопределено
        dvt_null    = 6, // Null
        dvt_type    = 7, // Тип
        dvt_enum    = 8  // Значение системного перечисления
    }

    //---------------------------------------------------------------------------
    // Типы узлов дерева сериализации
    public enum SerializationTreeNodeType
    {
        stt_min      = 0, // Минимум, для проверки

        stt_const    = 1, // Константа
        stt_var      = 2, // Переменная
        stt_list     = 3, // Список
        stt_prop     = 4, // Свойство
        stt_elcol    = 5, // ЭлементКоллекции
        stt_gentype  = 6, // ГенерируемыйТип
        stt_cond     = 7, // Условие
        stt_metaid   = 8, // МетаИД, идентификатор объекта метаданных
        stt_classcol = 9, // Коллекция классов
        stt_class    = 10, // Класс
        stt_idcol    = 11, // Коллекция ИД-элементов
        stt_idel     = 12, // ИД-элемент

        stt_max // Максимум, для проверки
    }

    //---------------------------------------------------------------------------
    // Типы значений дерева сериализации
    public enum SerializationTreeValueType
    {
        //stv_min     = 0, // Минимум, для проверки

        stv_string    = 1,  // Строка
        stv_number    = 2,  // Число
        stv_uid       = 3,  // УникальныйИдентификатор
        stv_value     = 4,  // Значение (MetaValue)
        stv_var       = 5,  // Переменная (SerializationTreeVar)
        stv_prop      = 6,  // Свойство (MetaProperty)
        stv_vercon    = 7,  // Версия контейнера
        stv_ver1C     = 8,  // Версия 1C
        stv_classpar  = 9,  // Параметр класса
        stv_globalvar = 10, // Глобальная переменная (SerializationTreeVar)
        stv_none      = 11  // Нет значения

        //stv_max // Максимум, для проверки
    }

    //---------------------------------------------------------------------------
    // Виды условий дерева сериализации
    public enum SerializationTreeCondition
    {
        stc_min = 0, // Минимум, для проверки

        stc_e   = 1, // Равно
        stc_ne  = 2, // НеРавно
        stc_l   = 3, // Меньше
        stc_g   = 4, // Больше
        stc_le  = 5, // МеньшеИлиРавно
        stc_ge  = 6, // БольшеИлиРавно
        stc_bs  = 7, // УстановленБит
        stc_bn  = 8, // НеУстановленБит

        stc_max // Максимум, для проверки
    }

    //---------------------------------------------------------------------------
    // Виды коллекций классов дерева сериализации
    public enum SerializationTreeClassType
    {
        stct_min       = 0, // Минимум, для проверки

        stct_inlist    = 1, // Классы в списке
        stct_notinlist = 2, // Классы не в списке

        stct_max // Максимум, для проверки
    }

    //---------------------------------------------------------------------------
    // Форматы внешних файлов типа (форматы двоичных данных)
    public enum ExternalFileFormat
    {
        eff_min       = 0,

        eff_servalue  = 1,  // СериализованноеЗначение
        eff_text      = 2,  // Текст (ИсходныйТекст)
        eff_tabdoc    = 3,  // ТабличныйДокумент
        eff_binary    = 4,  // ДвоичныеДанные
        eff_activedoc = 5,  // ActiveДокумент
        eff_htmldoc   = 6,  // HTMLДокумент
        eff_textdoc   = 7,  // ТекстовыйДокумент
        eff_geo       = 8,  // ГеографическаяСхема
        eff_kd        = 9,  // СхемаКомпоновкиДанных
        eff_mkd       = 10, // МакетОформленияКомпоновкиДанных
        eff_graf      = 11, // ГрафическаяСхема
        eff_xml       = 12, // XML
        eff_wsdl      = 13, // WSDL
        eff_picture   = 14, // Картинка
        eff_string    = 15, // Строка (строка длиной > maxStringLength)

        eff_max
    }

    //---------------------------------------------------------------------------
    // Типы сериализации двоичных данных
    public enum BinarySerializationType
    {
        bst_min            = 0,

        bst_empty          = 1, // БезПрефикса
        bst_base64         = 2, // Префикс base64
        bst_data           = 3, // Префикс data
        bst_base64_or_data = 4, // Префикс base64 или data (Преимущество base64)

        bst_max
    }

    //---------------------------------------------------------------------------
    // Виды значений 1С (для Value1C)
    public enum KindOfValue1C
    {
        kv_unknown,    // Неинициализированное значение
        kv_bool,       // Булево
        kv_string,     // Строка
        kv_number,     // Целое число
        kv_number_exp, // Число с плавающей запятой
        kv_date,       // Дата
        kv_null,       // Null
        kv_undef,      // Неопределено
        kv_type,       // Тип
        kv_uid,        // Уникальный идентификатор
        kv_enum,       // Системное перечисление
        kv_stdattr,    // Стандартный реквизит
        kv_stdtabsec,  // Стандартная табличная часть
        kv_obj,        // Объект
        kv_metaobj,    // Объект, являющийся объектом метаданных
        kv_refobj,     // Ссылка на объект, являющийся объектом метаданных
        kv_refpre,     // Ссылка на предопределенный элемент
        kv_right,      // Право
        kv_binary,     // Двоичные данные
        kv_extobj      // Внешний объект
    }

    #endregion

    #region Преобразование_в_Union
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/how-to-create-a-c-cpp-union-by-using-attributes
    //    typedef union byte_array
    //    {
    //     struct{
    //        byte byte1; byte byte2; byte byte3; byte byte4;
    //           };
    //     struct{
    //        int int1; int int2;
    //           };
    //   };byte_array

    // Add a using directive for System.Runtime.InteropServices.  

    // [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    // struct TestUnion
    // {
    //     [System.Runtime.InteropServices.FieldOffset(0)]
    //     public int i;
    // 
    //     [System.Runtime.InteropServices.FieldOffset(0)]
    //     public double d;
    // 
    //     [System.Runtime.InteropServices.FieldOffset(0)]
    //     public char c;
    // 
    //     [System.Runtime.InteropServices.FieldOffset(0)]
    //     public byte b;
    // }



    #endregion

    //---------------------------------------------------------------------------
    // Допустимое значение переменной дерева сериализации
    public struct VarValidValue
    {
        public int value;
        public Version1C ver1C;
        public int globalvalue;
    }



    public class Value1C_metaobj
    {
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
    public struct SerializationTreeNode
    { }

    //---------------------------------------------------------------------------
    // Внешний файл типа
    public struct ExternalFile
    { }




    class MetaData
    {
    }
}
