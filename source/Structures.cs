﻿using System;
using System.IO;
using static MetaRead.APIcfBase;

namespace MetaRead
{
    public static class Structures
    {
        #region Структуры
        public struct IndexRecord
        {
            private V8Field field;
            private Int32 len;

            public V8Field Field { get { return field; } set { field = value; } }

            public int Len { get { return len; } set { len = value; } }
        }

        public struct UnpackIndexRecord
        {
            private UInt32 _record_number; // номер (индекс) записи в таблице записей
                                           //unsigned char _index[1]; // значение индекса записи. Реальная длина значения определяется полем length класса index
            private byte[] index;

            public byte[] Index { get { return index; } set { index = value; } }
        }

        /// <summary>
        /// Структура заголовка
        /// </summary>
        public struct Fat_item
        {
            private UInt32 header_start;
            private UInt32 data_start;
            private UInt32 ff;            // всегда 7fffffff

            public uint Header_start { get { return header_start; } set { header_start = value; } }
            public uint Data_start { get { return data_start; } set { data_start = value; } }
            public uint Ff { get { return ff; } set { ff = value; } }
        }

        // структура одного блока в файле file_blob
        public struct Blob_block
        {
            private UInt32 nextblock;
            private Int16 length;
            //public char[] data[BLOB_RECORD_DATA_LEN];
            private char[] data;

            public uint Nextblock { get { return nextblock; } set { nextblock = value; } }

            public short Length { get { return length; } set { length = value; } }

            public char[] Data { get { return data; } set { data = value; } }
        }

        // структура root файла экспорта/импорта таблиц
        public struct Export_import_table_root
        {
            private bool has_data;
            private bool has_blob;
            private bool has_index;
            private bool has_descr;
            private Int32 data_version_1; // версия реструктуризации
            private Int32 data_version_2; // версия изменения
            private Int32 blob_version_1; // версия реструктуризации
            private Int32 blob_version_2; // версия изменения
            private Int32 index_version_1; // версия реструктуризации
            private Int32 index_version_2; // версия изменения
            private Int32 descr_version_1; // версия реструктуризации
            private Int32 descr_version_2; // версия изменения

            public bool Has_data { get { return has_data; } set { has_data = value; } }
            public bool Has_blob { get { return has_blob; } set { has_blob = value; } }

            public bool Has_index { get { return has_index; } set { has_index = value; } }

            public bool Has_descr { get { return has_descr; } set { has_descr = value; } }

            public int Data_version_1 { get { return data_version_1; } set { data_version_1 = value; } }
            public int Data_version_2 { get { return data_version_2; } set { data_version_2 = value; } }

            public int Blob_version_1 { get { return blob_version_1; } set { blob_version_1 = value; } }
            public int Blob_version_2 { get { return blob_version_2; } set { blob_version_2 = value; } }

            public int Index_version_1 { get { return index_version_1; } set { index_version_1 = value; } }
            public int Index_version_2 { get { return index_version_2; } set { index_version_2 = value; } }

            public int Descr_version_1 { get { return descr_version_1; } set { descr_version_1 = value; } }
            public int Descr_version_2 { get { return descr_version_2; } set { descr_version_2 = value; } }
        }

        /// <summary>
        /// структура версии
        /// </summary>
        public struct _VersionRec
        {
            private UInt32 version_1; // версия реструктуризации
            private UInt32 version_2; // версия изменения

            //public uint Version_1 { get { return version_1; } set { version_1 = value; } }
            //public uint Version_2 { get { return version_2; } set { version_2 = value; } }
            public uint Version_1 { get; set; }
            public uint Version_2 { get; set; }

        }

        /// <summary>
        /// структура версии
        /// </summary>
        public struct _Version
        {
            private UInt32 version_1; // версия реструктуризации
            private UInt32 version_2; // версия изменения
            private UInt32 version_3; // версия изменения 2

            public uint Version_1 { get { return version_1; } set { version_1 = value; } }
            public uint Version_2 { get { return version_2; } set { version_2 = value; } }
            public uint Version_3 { get { return version_3; } set { version_3 = value; } }
            public _Version(UInt32 v1, UInt32 v2, UInt32 v3)
            {
                version_1 = v1;
                version_2 = v2;
                version_3 = v3;
            }
        }

        /// <summary>
        /// Структура страницы размещения уровня 1 версий от 8.3.8 
        /// </summary>
        public struct Objtab838
        {
            private UInt32[] blocks; // реальное количество блоков зависит от размера страницы (pagesize)

            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        /// <summary>
        /// структура заголовочной страницы файла данных или файла свободных страниц 
        /// </summary>
        public struct V8ob
        {
            private char[] sig; // сигнатура SIG_OBJ
            private UInt32 len; // длина файла
            private _Version version;
            private UInt32[] blocks;

            public char[] Sig { get { return sig; } set { sig = value; } }
            public uint Len { get { return len; } set { len = value; } }
            public _Version Version { get { return version; } set { version = value; } }
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        /// <summary>
        /// структура заголовочной страницы файла данных начиная с версии 8.3.8 
        /// </summary>
        public struct V838ob_data
        {
            //public char[] sig;       // сигнатура 0x1C 0xFD (1C File Data?)
            private byte[] sig;
            private Int16 fatlevel;   // уровень таблицы размещения (0x0000 - в таблице blocks номера страниц с данными, 0x0001 - в таблице blocks номера страниц с таблицами размещения второго уровня, в которых уже, в свою очередь, находятся номера страниц с данными)
            private _Version version;
            private UInt64 len;       // длина файла
            private UInt32[] blocks;  // Реальная длина массива зависит от размера страницы и равна pagesize/4-6 (от это 1018 для 4К до 16378 для 64К)

            public byte[] Sig { get { return sig; } set { sig = value; } }

            public short Fatlevel { get { return fatlevel; } set { fatlevel = value; } }
            public _Version Version { get { return version; } set { version = value; } }
            public ulong Len { get { return len; } set { len = value; } }
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        /// <summary>
        /// структура заголовочной страницы файла свободных страниц начиная с версии 8.3.8 
        /// </summary>
        public struct V838ob_free
        {
            //public char[] sig;     // сигнатура 0x1C 0xFF (1C File Free?)
            private byte[] sig;     // сигнатура 0x1C 0xFF (1C File Free?)
            private Int16 fatlevel; // 0x0000 пока! но может ... уровень таблицы размещения (0x0000 - в таблице blocks номера страниц с данными, 0x0001 - в таблице blocks номера страниц с таблицами размещения второго уровня, в которых уже, в свою очередь, находятся номера страниц с данными)
            private UInt32 version;        // ??? предположительно...
            private UInt32[] blocks;       // Реальная длина массива зависит от размера страницы и равна pagesize/4-6 (от это 1018 для 4К до 16378 для 64К)

            public byte[] Sig { get { return sig; } set { sig = value; } }
            public short Fatlevel { get { return fatlevel; } set { fatlevel = value; } }
            public uint Version { get { return version; } set { version = value; } }
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        /// <summary>
        /// Заголовок страницы
        /// </summary>
        public struct LeafPageHeader
        {
            private Int16 flags; // offset 0
            private UInt16 number_indexes; // offset 2
            private UInt32 prev_page; // offset 4 // для 8.3.8 - это номер страницы (реальное смещение = prev_page * pagesize), до 8.3.8 - это реальное смещение
            private UInt32 next_page; // offset 8 // для 8.3.8 - это номер страницы (реальное смещение = next_page * pagesize), до 8.3.8 - это реальное смещение
            private UInt16 freebytes; // offset 12
            private UInt32 numrecmask; // offset 14
            private UInt16 leftmask; // offset 18
            private UInt16 rightmask; // offset 20
            private UInt16 numrecbits; // offset 22
            private UInt16 leftbits; // offset 24
            private UInt16 rightbits; // offset 26
            private UInt16 recbytes; // offset 28

            public short Flags { get { return flags; } set { flags = value; } }

            public ushort Number_indexes { get { return number_indexes; } set { number_indexes = value; } }

            public uint Prev_page { get { return prev_page; } set { prev_page = value; } }
            public uint Next_page { get { return next_page; } set { next_page = value; } }
            public ushort Freebytes { get { return freebytes; } set { freebytes = value; } }
            public uint Numrecmask { get { return numrecmask; } set { numrecmask = value; } }
            public ushort Leftmask { get { return leftmask; } set { leftmask = value; } }
            public ushort Rightmask { get { return rightmask; } set { rightmask = value; } }
            public ushort Numrecbits { get { return numrecbits; } set { numrecbits = value; } }
            public ushort Leftbits { get { return leftbits; } set { leftbits = value; } }
            public ushort Rightbits { get { return rightbits; } set { rightbits = value; } }
            public ushort Recbytes { get { return recbytes; } set { recbytes = value; } }
        }

        public static LeafPageHeader ByteArrayToLeafPageHeader(byte[] src)
        {

            LeafPageHeader Res = new LeafPageHeader();

            Res.Flags = BitConverter.ToInt16(src, 0);
            Res.Number_indexes = BitConverter.ToUInt16(src, 2);
            Res.Prev_page = BitConverter.ToUInt32(src, 4);
            Res.Next_page = BitConverter.ToUInt32(src, 8);
            Res.Freebytes = BitConverter.ToUInt16(src, 12);
            Res.Numrecmask = BitConverter.ToUInt32(src, 14);
            Res.Leftmask = BitConverter.ToUInt16(src, 18);
            Res.Rightmask = BitConverter.ToUInt16(src, 20);
            Res.Numrecbits = BitConverter.ToUInt16(src, 22);
            Res.Leftbits = BitConverter.ToUInt16(src, 24);
            Res.Rightbits = BitConverter.ToUInt16(src, 26);
            Res.Recbytes = BitConverter.ToUInt16(src, 28);

            return Res;

        }

        public struct Field_type_declaration
        {
            private TypeFields type;
            private bool null_exists;
            private Int32 length;
            private Int32 precision;
            private bool case_sensitive;

            public TypeFields Type { get { return type; } set { type = value; } }

            public bool Null_exists { get { return null_exists; } set { null_exists = value; } }

            public int Length { get { return length; } set { length = value; } }

            public int Precision { get { return precision; } set { precision = value; } }

            public bool Case_sensitive { get { return case_sensitive; } set { case_sensitive = value; } }

            public static Field_type_declaration Parse_tree(Tree field_tree) { return new Field_type_declaration(); }
        }

        /// <summary>
        /// Cтруктура первой страницы контейнера
        /// </summary>
        public struct V8Con
        {
            // восемь символов
            public Char[] sig; // сигнатура SIG_CON
            public Byte ver1;
            public Byte ver2;
            public Byte ver3;
            public Byte ver4;
            public UInt32 length;
            public UInt32 firstblock;
            public UInt32 pagesize;

            public String getver()
            {
                String ss = ver1.ToString();
                ss += ".";
                ss += ver2;
                ss += ".";
                ss += ver3;
                ss += ".";
                ss += ver4;
                return ss;
            }
        }

        /// <summary>
        /// Структура страницы размещения уровня 1 версий от 8.0 до 8.2.14
        /// </summary>
        //public struct ObjTab
        public class ObjTab
        {
            private Int32 numblocks;
            private UInt32[] blocks;

            public int Numblocks
            {
                get { return numblocks; }
                set { numblocks = value; }
            }

            public uint[] Blocks
            {
                get { return blocks; }
                set { blocks = value; }
            }

            public ObjTab(Int32 _numblocks, UInt32[] _blocks)
            {
                Numblocks = _numblocks;
                Blocks = _blocks;
            }

        }

        /// <summary>
        /// структура заголовочной страницы файла свободных страниц начиная с версии 8.3.8 
        /// </summary>
        public struct V838ObjFree
        {
            //public char[] sig;     // сигнатура 0x1C 0xFF (1C File Free?)
            private byte[] sig;     // сигнатура 0x1C 0xFF (1C File Free?)
            private Int16 fatlevel; // 0x0000 пока! но может ... уровень таблицы размещения (0x0000 - в таблице blocks номера страниц с данными, 0x0001 - в таблице blocks номера страниц с таблицами размещения второго уровня, в которых уже, в свою очередь, находятся номера страниц с данными)
            private UInt32 version;        // ??? предположительно...
            private UInt32[] blocks;       // Реальная длина массива зависит от размера страницы и равна pagesize/4-6 (от это 1018 для 4К до 16378 для 64К)

            public byte[] Sig { get { return sig; } set { sig = value; } }
            public short FatLevel { get { return fatlevel; } set { fatlevel = value; } }
            public uint Version { get { return version; } set { version = value; } }
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }

        /// <summary>
        /// структура заголовочной страницы файла данных начиная с версии 8.3.8 
        /// </summary>
        public struct V838ObjData
        {
            //public char[] sig;       // сигнатура 0x1C 0xFD (1C File Data?)
            private byte[] sig;
            private Int16 fatlevel;   // уровень таблицы размещения (0x0000 - в таблице blocks номера страниц с данными, 0x0001 - в таблице blocks номера страниц с таблицами размещения второго уровня, в которых уже, в свою очередь, находятся номера страниц с данными)
            private _Version version;
            private UInt64 len;       // длина файла
            private UInt32[] blocks;  // Реальная длина массива зависит от размера страницы и равна pagesize/4-6 (от это 1018 для 4К до 16378 для 64К)

            public byte[] Sig { get { return sig; } set { sig = value; } }

            public short Fatlevel { get { return fatlevel; } set { fatlevel = value; } }
            public _Version Version { get { return version; } set { version = value; } }
            public ulong Len { get { return len; } set { len = value; } }
            public uint[] Blocks { get { return blocks; } set { blocks = value; } }
        }


        public struct Root80
        {
            private char[] lang; // 8
            private UInt32 numblocks;
            private UInt32[] blocks;

            public char[] Lang
            {
                get { return lang; }
                set { lang = value; }
            }

            public uint Numblocks
            {
                get { return numblocks; }
                set { numblocks = value; }
            }

            public uint[] Blocks
            {
                get { return blocks; }
                set { blocks = value; }
            }
        }

        public struct Root81
        {
            private char[] lang; //32
            private UInt32 numblocks;
            private UInt32[] blocks;

            public char[] Lang
            {
                get { return lang; }
                set { lang = value; }
            }

            public uint Numblocks
            {
                get { return numblocks; }
                set { numblocks = value; }
            }

            public uint[] Blocks
            {
                get { return blocks; }
                set { blocks = value; }
            }
        }

        /// <summary>
        /// Структура принадлежности страницы
        /// </summary>
        public class PageMapRec
        {
            private Int32 tab;     // Индекс в Tools1CD::tables, -1 - страница не относится к таблицам
            private PageType type; // тип страницы
            private UInt32 number; // номер страницы в своем типе

            public int Tab
            {
                get { return tab; }
                set { tab = value; }
            }

            public PageType Type
            {
                get { return type; }
                set { type = value; }
            }

            public uint Number
            {
                get { return number; }
                set { number = value; }
            }

            public PageMapRec(Int32 _tab = -1, PageType _type = PageType.lost, UInt32 _number = 0)
            {
                Tab = -1;
                Type = _type;
                Number = 0;
            }
        }

        /// <summary>
        /// Структура файла таблицы контейнера файлов
        /// </summary>
        public class TableFile
        {
            private V8Table t;
            private String name; // Имя, как оно хранится в таблице
            public UInt32 maxpartno;
            public table_blob_file[] addr = null;
            private DateTime ft_create;
            private DateTime ft_modify;

            public V8Table T
            {
                get { return t; }
                set { t = value; }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public DateTime Ft_create
            {
                get { return ft_create; }
                set { ft_create = value; }
            }

            public DateTime Ft_modify
            {
                get { return ft_modify; }
                set { ft_modify = value; }
            }

            /*
            public table_file(Table _t, String _name, UInt32 _maxpartno)
            {
                t = _t;
                name = _name;
                maxpartno = _maxpartno;
            }
            */
            public TableFile(V8Table _t, string _name, UInt32 _maxpartno)
            {
                t = _t;
                name = _name;
                maxpartno = _maxpartno;
            }

        }

        /// <summary>
        /// Структура открытого файла адаптера контейнера конфигурации
        /// </summary>
        public struct ConfigFile
        {
            private Stream str;
            private char[] addin;

            public Stream Str { get { return str; } set { str = value; } }

            public char[] Addin { get { return addin; } set { addin = value; } }
        }

        /// <summary>
        /// Структура адреса файла таблицы-контейнера файлов
        /// </summary>
        public struct table_blob_file
        {
            private UInt32 blob_start;
            private UInt32 blob_length;

            public uint Blob_start { get { return blob_start; } set { blob_start = value; } }
            public uint Blob_length { get { return blob_length; } set { blob_length = value; } }
        }

        /// <summary>
        /// Структура записи таблицы контейнера файлов
        /// </summary>
        public struct table_rec
        {
            private String name;
            private table_blob_file addr;
            private Int32 partno;
            private DateTime ft_create;
            private DateTime ft_modify;

            public string Name { get { return name; } set { name = value; } }
            public table_blob_file Addr { get { return addr; } set { addr = value; } }
            public int Partno { get { return partno; } set { partno = value; } }
            public DateTime Ft_create { get { return ft_create; } set { ft_create = value; } }
            public DateTime Ft_modify { get { return ft_modify; } set { ft_modify = value; } }
        }

        public struct ConfigStorageTable_addin
        {
            public ConfigStorageTableAddinVariant variant;
            public Container_file tf;
            public V8File f;
        }

        public class stBlockHeader
        {
            public static int size()
            {
                // EOL_0D + EOL_0A + data_size_hex + space1 + page_size_hex + space2 + next_page_addr_hex + space3 + EOL2_0D + EOL2_0A
                // Размер в байтах = 31
                return 1 + 1 + 8 + 1 + 8 + 1 + 8 + 1 + 1 + 1;
            }

            public stBlockHeader()
            {
                EOL_0D =  (char)0xD;
                EOL_0A =  (char)0xA;
                space1 =  (char)0x20; // ' ';
                space2 =  (char)0x20; // ' ';
                space3 =  (char)0x20; // ' ';
                EOL2_0D = (char)0xD;
                EOL2_0A = (char)0xA;
            }

            public static stBlockHeader create(int block_data_size, int page_size, int next_page_addr)
            {
                stBlockHeader blockHeader = new stBlockHeader();

                blockHeader.set_data_size(block_data_size);
                blockHeader.set_page_size(page_size);
                blockHeader.set_next_page_addr(next_page_addr);

                return blockHeader;

            }

            public int get_data_size()
            {
                string hex = "0x";

                foreach (var sym in data_size_hex)
                {
                    if (sym == '\0')
                        hex += "0";
                    else
                        hex += sym;
                }

                try
                {
                    return Convert.ToInt32(hex, 16);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public void set_data_size(int value)
            {
                string hex = int_to_hex(value);
                Array.Copy(hex.ToCharArray(), data_size_hex, 8);
            }

            public int get_page_size()
            {
                string hex = "0x";
                foreach (var sym in page_size_hex)
                {
                    if (sym == '\0')
                        hex += "0";
                    else
                        hex += sym;
                }
                try
                {
                    return Convert.ToInt32(hex, 16);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public void set_page_size(int value)
            {
                string hex = int_to_hex(value);
                Array.Copy(hex.ToCharArray(), page_size_hex, 8);
            }

            public int get_next_page_addr()
            {
                string hex = "0x";

                foreach (var sym in next_page_addr_hex)
                {
                    if (sym == '\0')
                        hex += "0";
                    else
                        hex += sym;
                }
                try
                {
                    return Convert.ToInt32(hex, 16);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            public void set_next_page_addr(int value)
            {
                string hex = int_to_hex(value);
                Array.Copy(hex.ToCharArray(), next_page_addr_hex, 8);
            }

            public bool is_correct()
            {
                return EOL_0D == (char)0x0d && EOL_0A == (char)0x0a && space1 == (char)0x20 &&
                       space2 == (char)0x20 && space3 == (char)0x20 && EOL2_0D == (char)0x0d && EOL2_0A == (char)0x0a;
            }

            public char EOL_0D;
            public char EOL_0A;
            public char[] data_size_hex = new char[8];
            public char space1;
            public char[] page_size_hex = new char[8];
            public char space2;
            public char[] next_page_addr_hex = new char[8];
            public char space3;
            public char EOL2_0D;
            public char EOL2_0A;

        }

        #endregion

        #region Перечисления

        /// <summary>
        /// Версии формата базы 1С
        /// </summary>
        public enum DBVer
        {
            ver8_0_3_0 = 1,
            ver8_0_5_0 = 2,
            ver8_1_0_0 = 3,
            ver8_2_0_0 = 4,
            ver8_2_14_0 = 5,
            ver8_3_8_0 = 6
        }

        public enum NodeType
        {
            nd_empty = 0, // пусто
            nd_string = 1, // строка
            nd_number = 2, // число
            nd_number_exp = 3, // число с показателем степени
            nd_guid = 4, // уникальный идентификатор
            nd_list = 5, // список
            nd_binary = 6, // двоичные данные (с префиксом #base64:)
            nd_binary2 = 7, // двоичные данные формата 8.2 (без префикса)
            nd_link = 8, // ссылка
            nd_binary_d = 9, // двоичные данные (с префиксом #data:)
            nd_unknown         // неизвестный тип
        }

        public enum TableInfo
        {
            ti_description,
            ti_fields,
            ti_indexes,
            ti_physical_view,
            ti_logical_view
        }

        // типы измененных записей
        public enum ChangedRecType
        {
            not_changed,
            changed,
            inserted,
            deleted
        }

        /// <summary>
        /// типы внутренних файлов
        /// </summary>
        public enum V8ObjType
        {
            unknown = 0, // тип неизвестен
            data80 = 1, // файл данных формата 8.0 (до 8.2.14 включительно)
            free80 = 2, // файл свободных страниц формата 8.0 (до 8.2.14 включительно)
            data838 = 3, // файл данных формата 8.3.8
            free838 = 4  // файл свободных страниц формата 8.3.8
        }

        public enum FileIsCatalog
        {
            Unknown,
            Yes,
            No
        }

        public enum TypeFields
        {
            tf_binary,   // B   // длина = length
            tf_bool,     // L   // длина = 1
            tf_numeric,  // N   // длина = (length + 2) / 2
            tf_char,     // NC  // длина = length * 2
            tf_varchar,  // NVC // длина = length * 2 + 2
            tf_version,  // RV  // 16, 8 версия создания и 8 версия модификации ? каждая версия int32_t(изменения) + int32_t(реструктуризация)
            tf_string,   // NT  // 8 (unicode text)
            tf_text,     // T   // 8 (ascii text)
            tf_image,    // I   // 8 (image = bynary data)
            tf_datetime, // DT  // 7
            tf_version8, // 8, скрытое поле при recordlock == false и отсутствии поля типа tf_version
            tf_varbinary // VB  // длина = length + 2
        }


        public enum Node_type
        {
            nd_empty = 0,       // пусто
            nd_string = 1,      // строка
            nd_number = 2,      // число
            nd_number_exp = 3,  // число с показателем степени
            nd_guid = 4,        // уникальный идентификатор
            nd_list = 5,        // список
            nd_binary = 6,      // двоичные данные (с префиксом #base64:)
            nd_binary2 = 7,     // двоичные данные формата 8.2 (без префикса)
            nd_link = 8,        // ссылка
            nd_binary_d = 9,    // двоичные данные (с префиксом #data:)
            nd_unknown          // неизвестный тип
        }

        public enum _state
        {
            s_value,              // ожидание начала значения
            s_delimitier,         // ожидание разделителя
            s_string,             // режим ввода строки
            s_quote_or_endstring, // режим ожидания конца строки или двойной кавычки
            s_nonstring           // режим ввода значения не строки
        }

        /// <summary>
        /// Типы страниц
        /// </summary>
        public enum PageType
        {
            lost,          // потерянная страница (не относится ни к одному объекту)
            root,          // корневая страница (страница 0)
            freeroot,      // корневая страница таблицы свободных блоков (страница 1)
            freealloc,     // страница размещения таблицы свободных блоков
            free,          // свободная страница
            rootfileroot,  // корневая страница корневого файла (страница 2)
            rootfilealloc, // страница размещения корневого файла
            rootfile,      // страница данных корневого файла
            descrroot,     // корневая страница файла descr таблицы
            descralloc,    // страница размещения файла descr таблицы
            descr,         // страница данных файла descr таблицы
            dataroot,      // корневая страница файла data таблицы
            dataalloc,     // страница размещения файла data таблицы
            data,          // страница данных файла data таблицы
            indexroot,     // корневая страница файла index таблицы
            indexalloc,    // страница размещения файла index таблицы
            index,         // страница данных файла index таблицы
            blobroot,      // корневая страница файла blob таблицы
            bloballoc,     // страница размещения файла blob таблицы
            blob           // страница данных файла blob таблицы
        }

        /// <summary>
        /// Версии файлов shapshot
        /// </summary>
        public enum SnapshotVersion
        {
            Ver1 = 1,
            Ver2 = 2
        }

        /// <summary>
        /// Известные версии хранилища конфигурации
        /// </summary>
        public enum DepotVer
        {
            UnknownVer = 0,
            Ver3 = 3, // 0300000000000000
            Ver5 = 5, // 0500000000000000
            Ver6 = 6, // 0600000000000000
            Ver7 = 7  // 0700000000000000
        }

        /// <summary>
        /// Перечисление признака упакованности файла
        /// </summary>
        public enum table_file_packed
        {
            unknown,
            no,
            yes
        }

        public enum ConfigStorageTableAddinVariant
        {
            cstav_container_file,
            cstav_v8file
        }

        #endregion

    }
}
