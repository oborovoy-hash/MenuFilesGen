using MenuFilesGen.Models;
using System.Xml.Linq;

namespace MenuFilesGen.CFG
{
    public partial class CfgCreator
    {
        /// <summary>
        /// Заполняем ленту на основе декларативных скобок { } и [ ]
        /// </summary>
        void Ribbon(bool isDuplicatePanel)
        {
            #region Ribbon
            XDoc = new XDocument();

            XElement ribbonRoot = new XElement("RibbonRoot");
            XDoc.Add(ribbonRoot);

            XElement ribbonPanelSourceCollection = new XElement("RibbonPanelSourceCollection");
            ribbonRoot.Add(ribbonPanelSourceCollection);

            XElement ribbonTabSourceCollection = new XElement("RibbonTabSourceCollection");
            ribbonRoot.Add(ribbonTabSourceCollection);

            XElement ribbonTabSource = new XElement("RibbonTabSource");
            ribbonTabSource.Add(new XAttribute("Text", addonNameGlobal));
            ribbonTabSource.Add(new XAttribute("UID", $"{addonNameGlobal.Replace(" ", "")}_Tab"));
            ribbonTabSourceCollection.Add(ribbonTabSource);

            foreach (IGrouping<string, CommandDefinition> cmd in groupsPanel)
            {
                XElement ribbonPanelSource = new XElement("RibbonPanelSource");
                ribbonPanelSource.Add(new XAttribute("UID", cmd.Key));
                ribbonPanelSource.Add(new XAttribute("Text", cmd.Key));
                ribbonPanelSourceCollection.Add(ribbonPanelSource);

                XElement panelButtons = new XElement("Temp");

                XElement currentSubPanel = null;
                XElement currentSubRow = null;

                List<IGrouping<string, CommandDefinition>> unitedCommands = cmd.GroupBy(c => c.RibbonSplitButtonName).ToList();

                foreach (IGrouping<string, CommandDefinition> unitedCommandGroup in unitedCommands)
                {
                    XElement container = ribbonPanelSource;
                    bool isSplit = !string.IsNullOrWhiteSpace(unitedCommandGroup.Key);

                    if (isSplit)
                    {
                        currentSubPanel = null;
                        currentSubRow = null;

                        XElement ribbonSplitButton = new XElement("RibbonSplitButton");
                        ribbonSplitButton.Add(new XAttribute("Text", unitedCommandGroup.Key));
                        ribbonSplitButton.Add(new XAttribute("Behavior", "SplitFollowStaticText"));
                        ribbonSplitButton.Add(new XAttribute("ButtonStyle", unitedCommandGroup.First().RibbonSize));

                        ribbonPanelSource.Add(ribbonSplitButton);
                        panelButtons.Add(ribbonSplitButton);
                        container = ribbonSplitButton;
                    }

                    foreach (CommandDefinition commandData in unitedCommandGroup)
                    {
                        if (commandData.HideCommand) continue;

                        if (!isSplit)
                        {
                            // 1. Сначала проверяем скобоки { и [
                            if (commandData.HasRowPanelStart)
                            {
                                currentSubPanel = new XElement("RibbonRowPanel");
                                ribbonPanelSource.Add(currentSubPanel);
                            }

                            if (commandData.HasRowStart)
                            {
                                currentSubRow = new XElement("RibbonRow");
                                if (currentSubPanel != null)
                                    currentSubPanel.Add(currentSubRow);
                                else
                                    ribbonPanelSource.Add(currentSubRow);
                            }

                            // Определяем контейнер для кнопки
                            if (currentSubRow != null)
                                container = currentSubRow;
                            else if (currentSubPanel != null)
                                container = currentSubPanel;
                            else
                                container = ribbonPanelSource;
                        }

                        // 2. Создаем и добавляем саму кнопку в контейнер
                        XElement generatedButton = CreateButton(commandData);
                        container.Add(generatedButton);

                        if (!isSplit)
                        {
                            panelButtons.Add(new XElement(generatedButton));
                        }

                        // 3. после того как кнопка добавлена закрываем контейнеры
                        if (!isSplit)
                        {
                            if (commandData.HasRowEnd)
                            {
                                currentSubRow = null;
                            }

                            if (commandData.HasRowPanelEnd)
                            {
                                currentSubPanel = null;
                                currentSubRow = null;
                            }
                        }
                    }
                }

                if (!isDuplicatePanel)
                {
                    XElement ribbonPanelBreak = new XElement("RibbonPanelBreak");
                    ribbonPanelSource.Add(ribbonPanelBreak);
                    XElement ribbonRowDuplicatePanel = new XElement("RibbonRowPanel");
                    ribbonPanelSource.Add(ribbonRowDuplicatePanel);

                    XElement[] items = panelButtons.Elements().ToArray();
                    int nameSymbolsCountMax = 0;

                    for (int itemIndex = 0; itemIndex < items.Count(); itemIndex += 2)
                    {
                        if (items[itemIndex].Attributes().Any(attr => attr.Name == "Text"))
                        {
                            int nameSymbolsCount = items[itemIndex].Attributes().First(attr => attr.Name == "Text").Value.Count();
                            if (nameSymbolsCount > nameSymbolsCountMax)
                                nameSymbolsCountMax = nameSymbolsCount;
                        }
                    }

                    for (int itemIndex = 0; itemIndex < items.Count(); itemIndex++)
                    {
                        XElement item = items[itemIndex];
                        XElement[] itemButtons;

                        if (item.Name == "RibbonSplitButton")
                            itemButtons = item.Elements().ToArray();
                        else
                        {
                            itemButtons = new[] { item };
                        }

                        for (int buttonIndex = 0; buttonIndex < itemButtons.Count(); buttonIndex++)
                        {
                            var button = itemButtons[buttonIndex];
                            var buttonStyleAttr = button.Attributes().FirstOrDefault(attr => attr.Name == "ButtonStyle");
                            if (buttonStyleAttr != null)
                            {
                                buttonStyleAttr.Value = "LargeWithHorizontalText";
                            }
                            ribbonRowDuplicatePanel.Add(button);

                            if (itemIndex < items.Count() - 1 || buttonIndex < itemButtons.Count() - 1)
                            {
                                XElement separator = new XElement("RibbonSeparator");
                                ribbonRowDuplicatePanel.Add(separator);
                            }
                        }
                    }
                }

                XElement ribbonPanelSourceReference = new XElement("RibbonPanelSourceReference");
                ribbonPanelSourceReference.Add(new XAttribute("PanelId", cmd.Key));
                ribbonTabSource.Add(ribbonPanelSourceReference);
            }
            #endregion
        }

        public static XElement CreateButton(CommandDefinition commandData)
        {
            XElement ribbonCommandButton = new XElement("RibbonCommandButton");
            ribbonCommandButton.Add(new XAttribute("Text", commandData.DispName));
            ribbonCommandButton.Add(new XAttribute("ButtonStyle", commandData.RibbonSize));
            ribbonCommandButton.Add(new XAttribute("MenuMacroID", commandData.InterName));
            return ribbonCommandButton;
        }

        public XDocument XDoc { get; private set; }
    }
}
