﻿using ColossalFramework;
using ColossalFramework.UI;
using RoadNamer.Managers;
using RoadNamer.Utilities;
using UnityEngine;
using CimTools.Elements;
using CimTools.Utilities;
using CimTools.Panels;
using CimTools.Workshop;

namespace RoadNamer.Panels
{
    public class RoadNamePanel : UIPanel
    {
        protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

        private UpdatePanel m_infoPanel;
        private UITitleBar m_panelTitle;
        private UITextField m_textField;
        private UIColorField m_colourSelector;
        private UIButton m_randomNameButton;
        private string m_initialRoadName;

        public ushort m_netSegmentId = 0;
        public string initialRoadName
        {
            set
            {
                m_initialRoadName = value;

                if (m_textField != null)
                {
                    UpdateTextField(value);
                }
            }
            get
            {
                return m_initialRoadName;
            }
        }

        public override void Awake()
        {
            this.isInteractive = true;
            this.enabled = true;
            this.width = 250;
            this.height = 100;

            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            m_infoPanel = this.AddUIComponent<UpdatePanel>();
            m_infoPanel.Hide();

            Changelog changelogDownloader = m_infoPanel.m_changelogDownloader;

            if(!changelogDownloader.DownloadComplete && !changelogDownloader.DownloadInProgress)
            {
                changelogDownloader.DownloadChangelogAsync(OptionsManager.m_workshopId);
            }

            m_panelTitle = this.AddUIComponent<UITitleBar>();
            m_panelTitle.title = "Set a name";
            m_panelTitle.iconAtlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_panelTitle.iconSprite = "ToolbarFGIcon";

            CreatePanelComponents();
            CreateUpdatePanel();

            this.relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
            this.backgroundSprite = "MenuPanel2";
            this.atlas = UIUtilities.GetAtlas("Ingame");
            this.eventKeyPress += RoadNamePanel_eventKeyPress;
        }

        private void RoadNamePanel_eventKeyPress(UIComponent component, UIKeyEventParameter eventParam)
        {
            if (eventParam.keycode == KeyCode.KeypadEnter || eventParam.keycode == KeyCode.Return)
            {
                SetRoadData();
            }
        }

        private void CreatePanelComponents()
        {
            m_infoPanel.setPositionSpeakyPoint(new Vector2(this.width - 20, -20));

            m_textField = UIUtilities.CreateTextField(this);
            m_textField.relativePosition = new Vector3(m_UIPadding.left, m_panelTitle.height + m_UIPadding.bottom);
            m_textField.height = 21;
            m_textField.width = this.width - m_UIPadding.left - (m_UIPadding.right * 2) - m_textField.height;
            m_textField.eventKeyDown += M_textField_eventKeyDown;
            m_textField.processMarkup = false; //Might re-implement this eventually (needs work to stop it screwing up with markup)
            m_textField.textColor = Color.white;
            
            m_randomNameButton = UIUtilities.CreateButton(this);
            m_randomNameButton.text = "";
            m_randomNameButton.size = new Vector2(m_textField.height, m_textField.height);
            m_randomNameButton.relativePosition = new Vector3(m_textField.relativePosition.x + m_textField.width + m_UIPadding.left, m_textField.relativePosition.y);
            m_randomNameButton.atlas = SpriteUtilities.GetAtlas("RoadNamerIcons");
            m_randomNameButton.disabledBgSprite = "DiceIcon";
            m_randomNameButton.normalFgSprite = "DiceIcon";
            m_randomNameButton.focusedFgSprite = "DiceIcon";
            m_randomNameButton.hoveredFgSprite = "DiceIcon";
            m_randomNameButton.pressedFgSprite = "DiceIcon";
            m_randomNameButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            //m_randomNameButton.tooltip = RandomNameManager.m_fileName;
            m_randomNameButton.eventClicked += RandomNameButton_eventClicked;
            
            UIPanel colourSelectorPinPanel = this.AddUIComponent<UIPanel>();
            colourSelectorPinPanel.relativePosition = new Vector3(m_UIPadding.left, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            
            m_colourSelector = UIUtilities.CreateColorField(colourSelectorPinPanel);
            m_colourSelector.pickerPosition = UIColorField.ColorPickerPosition.LeftBelow;
            m_colourSelector.eventColorChanged += ColourSelector_eventColorChanged;
            m_colourSelector.eventColorPickerClose += ColourSelector_eventColorPickerClose;
            m_colourSelector.tooltip = "Set the text colour";
            m_colourSelector.relativePosition = new Vector3(0, 0);

            UIButton nameRoadButton = UIUtilities.CreateButton(this);
            nameRoadButton.text = "Set";
            nameRoadButton.size = new Vector2(60, 30);
            nameRoadButton.relativePosition = new Vector3(this.width - nameRoadButton.width - m_UIPadding.right, m_textField.relativePosition.y + m_textField.height + m_UIPadding.bottom);
            nameRoadButton.eventClicked += NameRoadButton_eventClicked;
            nameRoadButton.tooltip = "Create the label";

            this.height = nameRoadButton.relativePosition.y + nameRoadButton.height + m_UIPadding.bottom;
        }

        private void CreateUpdatePanel()
        {
            if (OptionsManager.m_versionStringFull != SavedOptionManager.Instance().m_lastSavedVersion)
            {
                OptionsManager.SaveOptions();
                m_infoPanel.Show();
            }
        }

        private void RandomNameButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            string randomName = RandomNameManager.GenerateRandomRoadName(m_netSegmentId);

            if(randomName != null)
            {
                m_randomNameButton.tooltip = "";
                m_textField.text = randomName;
            }
            else
            {
                m_randomNameButton.tooltip = "Could not find any road names :(";
                m_randomNameButton.RefreshTooltip();
                m_randomNameButton.bringTooltipToFront = true;
                m_randomNameButton.tooltipBox.Show();
            }
        }

        private void ColourSelector_eventColorPickerClose(UIColorField dropdown, UIColorPicker popup, ref bool overridden)
        {
            m_textField.textColor = popup.color;
        }

        private void ColourSelector_eventColorChanged(UIComponent component, Color32 value)
        {
            m_textField.textColor = value;
        }

        private void M_textField_eventKeyDown(UIComponent component, UIKeyEventParameter eventParam)
        {
            if (eventParam.keycode == KeyCode.KeypadEnter || eventParam.keycode == KeyCode.Return)
            {
                SetRoadData();
            }
        }

        private void NameRoadButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            SetRoadData();
        }

        /// <summary>
        /// Gets the colour from the panel and sets it to be rendered/saved
        /// </summary>
        private void SetRoadData()
        {
            if (m_netSegmentId != 0)
            {
                string roadName = m_textField.text;

                if (roadName != null)
                {
                    string hexColour = UIMarkupStyle.ColorToHex(m_textField.textColor);
                    roadName = "<color" + hexColour + ">" + roadName + "</color>";

                    RoadRenderingManager roadRenderingManager = Singleton<RoadRenderingManager>.instance;
                    RoadNameManager.Instance().SetRoadName(m_netSegmentId, roadName);
                    Hide();

                    roadRenderingManager.ForceUpdate();
                }
            }
        }

        /// <summary>
        /// Sets the panel text field to road data. Converts colours.
        /// </summary>
        /// <param name="text"></param>
        private void UpdateTextField(string text)
        {
            if (text != null)
            {
                Color textFieldColour = StringUtilities.ExtractColourFromTags(text, new Color(1, 1, 1));
                string sanitisedLabel = StringUtilities.RemoveTags(text);

                m_textField.textColor = textFieldColour;
                m_textField.text = sanitisedLabel;

                m_colourSelector.selectedColor = textFieldColour;
            }
            else
            {
                m_textField.text = "";
            }
        }
    }
}
