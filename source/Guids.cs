using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaRead
{
    public class Guids
    {
        //public static SortedDictionary<Guid, KeyValuePair<string, string>> PredefinedMeta = new SortedDictionary<Guid, KeyValuePair<string, string>>();
        public static Dictionary<Guid, KeyValuePair<string, string>> PredefinedMetaUnions = new Dictionary<Guid, KeyValuePair<string, string>>();
        public static Dictionary<Guid, KeyValuePair<string, string>> PredefinedMeta = new Dictionary<Guid, KeyValuePair<string, string>>();

        public static void InitGuids()
        {
            #region исходные гуиды
            // ==> Классы-коллекции метаданных
            //  ("HTTPСервисы", "HTTPServices",                                  new Guid("0fffc09c-8f4c-47cc-b41c-8d5c5a221d79");
            //  ("WebСервисы", "WebServices",                                    new Guid("8657032e-7740-4e1d-a3ba-5dd6e8afb78f");
            //  ("WSСсылки", "WSReferences",                                     new Guid("d26096fb-7a5d-4df9-af63-47d04771fa9b");
            //  ("БизнесПроцессы", "BusinessProcesses",                          new Guid("fcd3404e-1523-48ce-9bc0-ecdb822684a1");
            //  ("ВнешниеИсточникиДанных", "ExternalDataSources",                new Guid("5274d9fc-9c3a-4a71-8f5e-a0db8ab23de5");
            //  ("ГруппыКоманд", "CommandGroups",                                new Guid("1c57eabe-7349-44b3-b1de-ebfeab67b47d");
            //  ("Документы", "Documents",                                       new Guid("061d872a-5787-460e-95ac-ed74ea3a3e84");
            //  ("ЖурналыДокументов", "DocumentJournals",                        new Guid("4612bd75-71b7-4a5c-8cc5-2b0b65f9fa0d");
            //  ("Задачи", "Tasks",                                              new Guid("3e63355c-1378-4953-be9b-1deb5fb6bec5");
            //  ("Интерфейсы", "Interfaces",                                     new Guid("39bddf6a-0c3c-452b-921c-d99cfa1c2f1b");
            //  ("Константы", "Constants",                                       new Guid("0195e80c-b157-11d4-9435-004095e12fc7");
            //  ("КритерииОтбора", "FilterCriteria",                             new Guid("3e7bfcc0-067d-11d6-a3c7-0050bae0a776");
            //  ("НумераторыДокументов", "DocumentNumerators",                   new Guid("36a8e346-9aaa-4af9-bdbd-83be3c177977");
            //  ("Обработки", "DataProcessors",                                  new Guid("bf845118-327b-4682-b5c6-285d2a0eb296");
            //  ("ОбщиеКартинки", "CommonPictures",                              new Guid("7dcd43d9-aca5-4926-b549-1842e6a4e8cf");
            //  ("ОбщиеКоманды", "CommonCommands",                               new Guid("2f1a5187-fb0e-4b05-9489-dc5dd6412348");
            //  ("ОбщиеМакеты", "CommonTemplates",                               new Guid("0c89c792-16c3-11d5-b96b-0050bae0a95d");
            //  ("ОбщиеМодули", "CommonModules",                                 new Guid("0fe48980-252d-11d6-a3c7-0050bae0a776");
            //  ("ОбщиеРеквизиты", "CommonAttributes",                           new Guid("15794563-ccec-41f6-a83c-ec5f7b9a5bc1");
            //  ("ОбщиеФормы", "CommonForms",                                    new Guid("07ee8426-87f1-11d5-b99c-0050bae0a95d");
            //  ("ОпределяемыеТипы", "DefinedTypes",                             new Guid("c045099e-13b9-4fb6-9d50-fca00202971e");
            //  ("Отчеты", "Reports",                                            new Guid("631b75a0-29e2-11d6-a3c7-0050bae0a776");
            //  ("ПакетыXDTO", "XDTOPackages",                                   new Guid("cc9df798-7c94-4616-97d2-7aa0b7bc515e");
            //  ("ПараметрыСеанса", "SessionParameters",                         new Guid("24c43748-c938-45d0-8d14-01424a72b11e");
            //  ("ПараметрыФункциональныхОпций", "FunctionalOptionsParameters",  new Guid("30d554db-541e-4f62-8970-a1c6dcfeb2bc");
            //  ("Перечисления", "Enums",                                        new Guid("f6a80749-5ad7-400b-8519-39dc5dff2542");
            //  ("ПланыВидовРасчета", "ChartsOfCalculationTypes",                new Guid("30b100d6-b29f-47ac-aec7-cb8ca8a54767");
            //  ("ПланыВидовХарактеристик", "ChartsOfCharacteristicTypes",       new Guid("82a1b659-b220-4d94-a9bd-14d757b95a48");
            //  ("ПланыОбмена", "ExchangePlans",                                 new Guid("857c4a91-e5f4-4fac-86ec-787626f1c108");
            //  ("ПланыСчетов", "ChartsOfAccounts",                              new Guid("238e7e88-3c5f-48b2-8a3b-81ebbecb20ed");
            //  ("ПодпискиНаСобытия", "EventSubscriptions",                      new Guid("4e828da6-0f44-4b5b-b1c0-a2b3cfe7bdcc");
            //  ("Подсистемы", "Subsystems",                                     new Guid("37f2fa9a-b276-11d4-9435-004095e12fc7");
            //  ("Последовательности", "Sequences",                              new Guid("bc587f20-35d9-11d6-a3c7-0050bae0a776");
            //  ("РегистрыБухгалтерии", "AccountingRegisters",                   new Guid("2deed9b8-0056-4ffe-a473-c20a6c32a0bc");
            //  ("РегистрыНакопления", "AccumulationRegisters",                  new Guid("b64d9a40-1642-11d6-a3c7-0050bae0a776");
            //  ("РегистрыРасчета", "CalculationRegisters",                      new Guid("f2de87a8-64e5-45eb-a22d-b3aedab050e7");
            //  ("РегистрыСведений", "InformationRegisters",                     new Guid("13134201-f60b-11d5-a3c7-0050bae0a776");
            //  ("РегламентныеЗадания", "ScheduledJobs",                         new Guid("11bdaf85-d5ad-4d91-bb24-aa0eee139052");
            //  ("Роли", "Roles",                                                new Guid("09736b02-9cac-4e3f-b4f7-d3e9576ab948");
            //  ("Справочники", "Catalogs",                                      new Guid("cf4abea6-37b2-11d4-940f-008048da11f9");
            //  ("Стили", "Styles",                                              new Guid("3e5404af-6ef8-4c73-ad11-91bd2dfac4c8");
            //  ("ФункциональныеОпции", "FunctionalOptions",                     new Guid("af547940-3268-434f-a3e7-e47d6d2638c3");
            //  ("ХранилищаНастроек", "SettingsStorages",                        new Guid("46b4cd97-fd13-4eaa-aba2-3bddd7699218");
            //  ("ЭлементыСтиля", "StyleItems",                                  new Guid("58848766-36ea-4076-8800-e91eb49590d7");
            //  ("Языки", "Languages",                                           new Guid("9cd510ce-abfc-11d4-9434-004095e12fc7");
            // <== Классы-коллекции метаданных

            #endregion

            
            #region Ветка общие
            PredefinedMetaUnions.Add(new Guid("37f2fa9a-b276-11d4-9435-004095e12fc7"), new KeyValuePair<string, string>("Подсистемы", "Subsystems"));
            PredefinedMetaUnions.Add(new Guid("0fe48980-252d-11d6-a3c7-0050bae0a776"), new KeyValuePair<string, string>("Общие модули", "CommonModules"));
            PredefinedMetaUnions.Add(new Guid("24c43748-c938-45d0-8d14-01424a72b11e"), new KeyValuePair<string, string>("Параметры сеанса", "SessionParameters"));
            PredefinedMetaUnions.Add(new Guid("09736b02-9cac-4e3f-b4f7-d3e9576ab948"), new KeyValuePair<string, string>("Роли", "Roles"));
            PredefinedMetaUnions.Add(new Guid("15794563-ccec-41f6-a83c-ec5f7b9a5bc1"), new KeyValuePair<string, string>("Общие реквизиты", "CommonAttributes"));
            PredefinedMetaUnions.Add(new Guid("857c4a91-e5f4-4fac-86ec-787626f1c108"), new KeyValuePair<string, string>("Планы обмена", "ExchangePlans"));
            PredefinedMetaUnions.Add(new Guid("3e7bfcc0-067d-11d6-a3c7-0050bae0a776"), new KeyValuePair<string, string>("Критерии отбора", "FilterCriteria"));
            PredefinedMetaUnions.Add(new Guid("4e828da6-0f44-4b5b-b1c0-a2b3cfe7bdcc"), new KeyValuePair<string, string>("Подписки на события", "EventSubscriptions"));
            PredefinedMetaUnions.Add(new Guid("11bdaf85-d5ad-4d91-bb24-aa0eee139052"), new KeyValuePair<string, string>("Регламентные задания", "ScheduledJobs"));
            PredefinedMetaUnions.Add(new Guid("af547940-3268-434f-a3e7-e47d6d2638c3"), new KeyValuePair<string, string>("Функциональные опции", "FunctionalOptions"));
            PredefinedMetaUnions.Add(new Guid("30d554db-541e-4f62-8970-a1c6dcfeb2bc"), new KeyValuePair<string, string>("Параметры функциональных опций", "FunctionalOptionsParameters"));
            PredefinedMetaUnions.Add(new Guid("c045099e-13b9-4fb6-9d50-fca00202971e"), new KeyValuePair<string, string>("Определяемые типы", "DefinedTypes"));
            PredefinedMetaUnions.Add(new Guid("46b4cd97-fd13-4eaa-aba2-3bddd7699218"), new KeyValuePair<string, string>("Хранилища настроек", "SettingsStorages"));
            PredefinedMetaUnions.Add(new Guid("07ee8426-87f1-11d5-b99c-0050bae0a95d"), new KeyValuePair<string, string>("Общие формы", "CommonForms"));
            PredefinedMetaUnions.Add(new Guid("2f1a5187-fb0e-4b05-9489-dc5dd6412348"), new KeyValuePair<string, string>("Общие команды", "CommonCommands"));
            PredefinedMetaUnions.Add(new Guid("1c57eabe-7349-44b3-b1de-ebfeab67b47d"), new KeyValuePair<string, string>("Группы команд", "CommandGroups"));
            PredefinedMetaUnions.Add(new Guid("39bddf6a-0c3c-452b-921c-d99cfa1c2f1b"), new KeyValuePair<string, string>("Интерфейсы", "Interfaces"));
            PredefinedMetaUnions.Add(new Guid("0c89c792-16c3-11d5-b96b-0050bae0a95d"), new KeyValuePair<string, string>("Общие макеты", "CommonTemplates"));
            PredefinedMetaUnions.Add(new Guid("7dcd43d9-aca5-4926-b549-1842e6a4e8cf"), new KeyValuePair<string, string>("Общие картинки", "CommonPictures"));
            PredefinedMetaUnions.Add(new Guid("cc9df798-7c94-4616-97d2-7aa0b7bc515e"), new KeyValuePair<string, string>("XDTO-пакеты", "XDTOPackages"));
            PredefinedMetaUnions.Add(new Guid("8657032e-7740-4e1d-a3ba-5dd6e8afb78f"), new KeyValuePair<string, string>("Web-сервисы", "WebServices"));
            PredefinedMetaUnions.Add(new Guid("0fffc09c-8f4c-47cc-b41c-8d5c5a221d79"), new KeyValuePair<string, string>("HTTP-сервисы",                   "HTTPServices"));
            PredefinedMetaUnions.Add(new Guid("d26096fb-7a5d-4df9-af63-47d04771fa9b"), new KeyValuePair<string, string>("WS-ссылки", "WSReferences"));
            PredefinedMetaUnions.Add(new Guid("58848766-36ea-4076-8800-e91eb49590d7"), new KeyValuePair<string, string>("Элементы стиля", "StyleItems"));
            PredefinedMetaUnions.Add(new Guid("3e5404af-6ef8-4c73-ad11-91bd2dfac4c8"), new KeyValuePair<string, string>("Стили", "Styles"));
            PredefinedMetaUnions.Add(new Guid("9cd510ce-abfc-11d4-9434-004095e12fc7"), new KeyValuePair<string, string>("Языки", "Languages"));
            #endregion

            #region Основное дерево метаданных
            // Основное дерево метаданных
            PredefinedMeta.Add(new Guid("0195e80c-b157-11d4-9435-004095e12fc7"), new KeyValuePair<string, string>("Константы", "Constants"));
            PredefinedMeta.Add(new Guid("cf4abea6-37b2-11d4-940f-008048da11f9"), new KeyValuePair<string, string>("Справочники", "Catalogs"));

            PredefinedMeta.Add(new Guid("061d872a-5787-460e-95ac-ed74ea3a3e84"), new KeyValuePair<string, string>("Документы", "Documents"));
            PredefinedMeta.Add(new Guid("36a8e346-9aaa-4af9-bdbd-83be3c177977"), new KeyValuePair<string, string>("Нумераторы", "DocumentNumerators"));
            PredefinedMeta.Add(new Guid("bc587f20-35d9-11d6-a3c7-0050bae0a776"), new KeyValuePair<string, string>("Последовательности", "Sequences"));
            
            PredefinedMeta.Add(new Guid("4612bd75-71b7-4a5c-8cc5-2b0b65f9fa0d"), new KeyValuePair<string, string>("Журналы документов", "DocumentJournals"));
            PredefinedMeta.Add(new Guid("f6a80749-5ad7-400b-8519-39dc5dff2542"), new KeyValuePair<string, string>("Перечисления", "Enums"));
            PredefinedMeta.Add(new Guid("631b75a0-29e2-11d6-a3c7-0050bae0a776"), new KeyValuePair<string, string>("Отчеты", "Reports"));
            PredefinedMeta.Add(new Guid("bf845118-327b-4682-b5c6-285d2a0eb296"), new KeyValuePair<string, string>("Обработки", "DataProcessors"));
            PredefinedMeta.Add(new Guid("82a1b659-b220-4d94-a9bd-14d757b95a48"), new KeyValuePair<string, string>("Планы видов характеристик", "ChartsOfCharacteristicTypes"));
            PredefinedMeta.Add(new Guid("238e7e88-3c5f-48b2-8a3b-81ebbecb20ed"), new KeyValuePair<string, string>("Планы счетов", "ChartsOfAccounts"));
            PredefinedMeta.Add(new Guid("30b100d6-b29f-47ac-aec7-cb8ca8a54767"), new KeyValuePair<string, string>("Планы видов расчета", "ChartsOfCalculationTypes"));
            PredefinedMeta.Add(new Guid("13134201-f60b-11d5-a3c7-0050bae0a776"), new KeyValuePair<string, string>("Регистры сведений", "InformationRegisters"));
            PredefinedMeta.Add(new Guid("b64d9a40-1642-11d6-a3c7-0050bae0a776"), new KeyValuePair<string, string>("Регистры накопления", "AccumulationRegisters"));
            PredefinedMeta.Add(new Guid("2deed9b8-0056-4ffe-a473-c20a6c32a0bc"), new KeyValuePair<string, string>("Регистры бухгалтерии", "AccountingRegisters"));
            PredefinedMeta.Add(new Guid("f2de87a8-64e5-45eb-a22d-b3aedab050e7"), new KeyValuePair<string, string>("Регистры расчета", "CalculationRegisters"));
            PredefinedMeta.Add(new Guid("fcd3404e-1523-48ce-9bc0-ecdb822684a1"), new KeyValuePair<string, string>("Бизнес процессы",                "BusinessProcesses"));
            PredefinedMeta.Add(new Guid("3e63355c-1378-4953-be9b-1deb5fb6bec5"), new KeyValuePair<string, string>("Задачи", "Tasks"));
            PredefinedMeta.Add(new Guid("5274d9fc-9c3a-4a71-8f5e-a0db8ab23de5"), new KeyValuePair<string, string>("Внешние источники данных",        "ExternalDataSources"));
            #endregion

        }

        public Guids()
        {
            InitGuids();
        }


    }
}
