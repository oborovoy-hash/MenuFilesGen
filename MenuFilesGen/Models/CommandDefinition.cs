using MenuFilesGen.Enums;
using MenuFilesGen.Service;

namespace MenuFilesGen.Models
{
    /// <summary> Общие данные по команде </summary>
    public class CommandDefinition
    {
        /// <summary> Имя команды, как оно будет показываться в меню </summary>
        public string DispName { get; set; }

        #region InterName
        /// <summary> Внутреннее имя команды, очищенное от скобок разметки { } [ ] и знака | </summary>
        public string InterName
        {
            get
            {
                if (string.IsNullOrEmpty(InterNameRaw)) return "";

                // Работаем строго от строки из Excel, чтобы обойти любые группировок
                string raw = InterNameRaw.Trim();

                // Если в ячейке есть палочка |, отрезаем всё, что идет после неё
                int pipeIndex = raw.IndexOf('|');
                if (pipeIndex >= 0)
                {
                    raw = raw.Substring(0, pipeIndex).Trim();
                }

                // Полностью очищаем имя макроса/команды от управляющих символов верстки ленты
                return raw.Replace("{", "")
                          .Replace("}", "")
                          .Replace("[", "")
                          .Replace("]", "");
            }
        }

        /// <summary> Флаги разметки структуры для генератора ленты (сканируем сырую строку на лету) </summary>
        public bool HasRowPanelStart => !string.IsNullOrEmpty(InterNameRaw) && InterNameRaw.Contains("{");
        public bool HasRowPanelEnd => !string.IsNullOrEmpty(InterNameRaw) && InterNameRaw.Contains("}");
        public bool HasRowStart => !string.IsNullOrEmpty(InterNameRaw) && InterNameRaw.Contains("[");
        public bool HasRowEnd => !string.IsNullOrEmpty(InterNameRaw) && InterNameRaw.Contains("]");

        // Оригинальный список, как он был заложен автором изначально
        List<string> InterNames => InterNameRaw.RawSplit();
        public string InterNameRaw { get; set; } = "";

        /// <summary> 
        /// Добавлять перед командой разделитель.
        /// Если в сырой ячейке из Excel есть символ '|', 
        /// этот флаг вернет true, независимо от списков InterNames!
        /// </summary>
        public bool IsCommandSeparator => !string.IsNullOrEmpty(InterNameRaw) && InterNameRaw.Contains("|");
        #endregion

        /// <summary> Описание команды, показываемое в качестве всплывающей подсказки </summary>
        public string StatusText { get; set; }

        #region PanelName
        /// <summary> имя панели/подменю </summary>
        public string PanelName => PanelNames.Count > 0 ? PanelNames[0].Trim() : "";

        List<string> PanelNames => PanelNameRaw.RawSplit();
        public string PanelNameRaw { get; set; } = "";

        public bool IsPanelSeparator => PanelNames.Count > 1;
        
        #endregion

        /// <summary> Флаг виртуальной панели </summary>
        /// </summary>
        /// <value>
        ///   <c>true</c> Не включать под панель меню, в вкладках ленты выпадающий список; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtualPanel { get; set; }

        /// <summary> Размер кнопки на ленте. None - кнопки не будет </summary>
        public string RibbonSize { get; set; }

        /// <summary>название сплитера ленты </summary>
        public string RibbonSplitButtonName { get; set; }

        /// <summary> не регистрировать команду </summary>
        public bool DontTake { get; set; }

        /// <summary> Команду зарегистрировать, но не показывать в UI </summary>
        public bool HideCommand { get; set; }

        /// <summary> Имя ресурсной dll. Обязательна, если установлен IconName </summary>
        public string ResourceDllName { get; set; }

        /// <summary> Имя иконки </summary>
        public string IconName { get; set; }

        /// <summary> приложение </summary>
        public string AppName { get; set; }

        /// <summary> Локальное имя команды </summary>
        public string LocalName { get; set; }

        /// <summary> Реальное имя команды </summary>
        public string RealCommandName { get; set; }

        /// <summary> Ключевое слово </summary>
        public string Keyword { get; set; }

        /// <summary> вес команды)) </summary>
        public int Weight { get; set; }

        /// <summary> Тип команды, контекст выполнения </summary>
        /// <br>документ-1</br>
        /// <br>приложение-0</br> </summary>
        public int CmdType { get; set; }

        /// <summary>подсказки </summary>
        public string ToolTipText { get; set; }

        /// <summary> хоткеи </summary>
        public string Accelerators { get; set; }

        #region AddonName
        /// <summary> аддон </summary>
        //public string AddonName => AddonNames[0].Trim();//x

        //List<string> AddonNames => AddonNameRaw.RawSplit();
        public string AddonNameRaw { get; set; } = "";
        #endregion
    }

    /// <summary> Общие данные по команде not Used </summary>
    public class CommandDefinitionKpc
    {
        /// <summary> Имя команды, как оно будет показываться в меню </summary>
        public string MenuCommandName { get; set; }
        /// <summary> Внутреннее имя команды, как оно определено в dll / nrx / lsp </summary>
        public string InternalName { get; set; }
        /// <summary> Описание команды, показываемое в качестве всплывающей подсказки </summary>
        public string Description { get; set; }
        /// <summary> Имя иконки </summary>
        public string IconName { get; set; }
        /// <summary> Имя ресурсной dll. Обязательна, если установлена иконка (?) </summary>
        public string ResourceDllName { get; set; }
        /// <summary> Команду ввести в меню, но не показывать нигде </summary>
        public bool HideCommand { get; set; }
        /// <summary> В каких панелях инструментов будет участвовать команда </summary>
        public IEnumerable<PanelDefinition> Panel { get; set; }
        /// <summary> В каких кусках ленты будет участвовать команда </summary>
        public IEnumerable<RibbonPaletteDefinition> RibbonPanel { get; set; }
        /// <summary> Размер кнопки на ленте. None - кнопки не будет </summary>
        public RibbonButtonSize RibbonSize { get; set; }
    }
}
