﻿using ColossalFramework.UI;
using ICities;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using ColossalFramework;
using RoadNamer.Utilities;

namespace RoadNamer.Managers
{
    /// <summary>
    /// Manages all ingame options. Handles the interface between ingame options
    /// storage and saving/loading from disk.
    /// </summary>
    public class OptionsManager : MonoBehaviour
    {
        public static bool m_isIngame = false;
        public static string m_randomNamesLocation = FileUtilities.GetModPath() + "/Names/";

        /// <summary>
        /// Contains all options that can be set using a checkbox. These
        /// options automatically generate a checkbox in the options panel
        /// </summary>
        private static RoadCheckBoxOption[] checkboxOptions = new RoadCheckBoxOption[]
        {
            new RoadCheckBoxOption() { uniqueName = "showCamera", readableName = "Show road names in camera mode", value = false, enabled = true }
        };

        /// <summary>
        /// Contains all options that can be set using a slider. These
        /// options automatically generate a slider in the options panel
        /// </summary>
        private static RoadSliderOption[] sliderOptions = new RoadSliderOption[]
        {
            new RoadSliderOption() { uniqueName = "textDisappearDistance", readableName = "Rendering distance", min = 100f, max = 2000f, value = 1000f, step = 10f, enabled = true },
            new RoadSliderOption() { uniqueName = "textScale", readableName = "Text scale", min = 0.2f, max = 2f, value = 0.5f, step = 0.1f, enabled = true }
        };

        /// <summary>
        /// Contains all options that can be set using a slider. These
        /// options automatically generate a slider in the options panel
        /// </summary>
        private static RoadDropdownOption[] dropdownOptions = new RoadDropdownOption[]
        {
            new RoadDropdownOption() { uniqueName = "randomNameLocalisation", readableName = "Random name localisation", value = 0, enabled = true }
        };

        /// <summary>
        /// Creates options on a panel using the helper
        /// </summary>
        /// <param name="helper">The UIHelper to put the options on</param>
        public void CreateOptions(UIHelperBase helper)
        {
            Populate();

            UIHelperBase optionGroup = helper.AddGroup("Road Namer Options");

            foreach(RoadCheckBoxOption checkboxOption in checkboxOptions)
            {
                UICheckBox checkBox = optionGroup.AddCheckbox(checkboxOption.readableName, checkboxOption.value, OptionChanged) as UICheckBox;
                checkBox.readOnly = !checkboxOption.enabled;
                checkBox.name = checkboxOption.uniqueName;
                checkBox.eventCheckChanged += CheckBox_eventCheckChanged;
            }

            foreach(RoadSliderOption sliderOption in sliderOptions)
            {
                UISlider slider = optionGroup.AddSlider(sliderOption.readableName, sliderOption.min, sliderOption.max, sliderOption.step, sliderOption.value, OptionChanged) as UISlider;
                slider.enabled = sliderOption.enabled;
                slider.name = sliderOption.uniqueName;
                slider.eventValueChanged += Slider_eventValueChanged;
                slider.tooltip = sliderOption.value.ToString();
            }

            foreach (RoadDropdownOption dropdownOption in dropdownOptions)
            {
                UIDropDown dropdown = optionGroup.AddDropdown(dropdownOption.readableName, dropdownOption.options, dropdownOption.value, OptionChanged) as UIDropDown;
                dropdown.enabled = dropdownOption.enabled;
                dropdown.name = dropdownOption.uniqueName;
                dropdown.eventSelectedIndexChanged += Dropdown_eventSelectedIndexChanged;
                dropdown.tooltip = dropdownOption.readableName;
            }

            UIButton saveButton = optionGroup.AddButton("Apply", SaveButtonClicked) as UIButton;
        }

        private void Dropdown_eventSelectedIndexChanged(UIComponent component, int value)
        {
            UIDropDown dropdown = component as UIDropDown;

            if(dropdown != null)
            {
                RoadDropdownOption foundOption = null;

                foreach (RoadDropdownOption option in dropdownOptions)
                {
                    if (dropdown.name == option.uniqueName)
                    {
                        foundOption = option;
                    }
                }

                if (foundOption != null)
                {
                    foundOption.value = value;
                    dropdown.tooltip = value.ToString();
                    dropdown.RefreshTooltip();
                }
            }
        }

        private void Slider_eventValueChanged(UIComponent component, float value)
        {
            UISlider slider = component as UISlider;

            if (slider != null)
            {
                RoadSliderOption foundOption = null;

                foreach (RoadSliderOption option in sliderOptions)
                {
                    if (slider.name == option.uniqueName)
                    {
                        foundOption = option;
                    }
                }

                if (foundOption != null)
                {
                    foundOption.value = value;
                    slider.tooltip = value.ToString();
                    slider.RefreshTooltip();
                }
            }
        }

        private void CheckBox_eventCheckChanged(UIComponent component, bool value)
        {
            UICheckBox checkBox = component as UICheckBox;

            if(checkBox != null) //Should bloody well not be null!
            {
                RoadCheckBoxOption foundOption = null;

                foreach(RoadCheckBoxOption option in checkboxOptions)
                {
                    if(checkBox.name == option.uniqueName)
                    {
                        foundOption = option;
                    }
                }

                if(foundOption != null)
                {
                    foundOption.value = value;
                }
            }
        }

        private void OptionChanged(bool value)
        {
        }

        private void OptionChanged(float value)
        {
        }

        private void OptionChanged(int sel)
        {
        }

        private void SaveButtonClicked()
        {
            SaveOptions();
            UpdateEverything();
        }

        /// <summary>
        /// Gets a bool from an option that was set using a checkbox
        /// </summary>
        /// <param name="uniqueName">The unique name of the checkbox option</param>
        /// <param name="returnValue">The value to replace</param>
        /// <returns>Whether the value was found and set</returns>
        public static bool GetCheckBoxValue(string uniqueName, ref bool returnValue)
        {
            bool successful = false;

            foreach(RoadCheckBoxOption option in checkboxOptions)
            {
                if(option.uniqueName == uniqueName)
                {
                    returnValue = option.value;
                    successful = true;
                }
            }

            return successful;
        }

        /// <summary>
        /// Gets a float from an option that was set using a slider
        /// </summary>
        /// <param name="uniqueName">The unique name of the slider option</param>
        /// <param name="returnValue">The value to replace</param>
        /// <returns>Whether the value was found and set</returns>
        public static bool GetSliderValue(string uniqueName, ref float returnValue)
        {
            bool successful = false;

            foreach (RoadSliderOption option in sliderOptions)
            {
                if (option.uniqueName == uniqueName)
                {
                    Debug.Log(option.value);
                    returnValue = option.value;
                    successful = true;
                }
            }

            return successful;
        }

        /// <summary>
        /// Gets a string from an option that was set using a dropdown
        /// </summary>
        /// <param name="uniqueName">The unique name of the dropdown option</param>
        /// <param name="returnValue">The value to replace</param>
        /// <returns>Whether the value was found and set</returns>
        public static bool GetDropdownValue(string uniqueName, ref int returnValue)
        {
            bool successful = false;

            foreach (RoadDropdownOption option in dropdownOptions)
            {
                if (option.uniqueName == uniqueName)
                {
                    Debug.Log(option.value);
                    returnValue = option.value;
                    successful = true;
                }
            }

            return successful;
        }

        /// <summary>
        /// Gets a string from an option that was set using a dropdown
        /// </summary>
        /// <param name="uniqueName">The unique name of the dropdown option</param>
        /// <param name="returnValue">The value to replace</param>
        /// <returns>Whether the value was found and set</returns>
        public static bool GetDropdownValue(string uniqueName, ref string returnValue)
        {
            bool successful = false;

            foreach (RoadDropdownOption option in dropdownOptions)
            {
                if (option.uniqueName == uniqueName)
                {
                    if (option.options != null && option.options.Length >= option.value + 1)
                    {
                        returnValue = option.options[option.value];
                        successful = returnValue != null && returnValue != "";
                    }
                }
            }

            return successful;
        }

        /// <summary>
        /// Save options to disk
        /// </summary>
        public static void SaveOptions()
        {
            SavedOptionManager.Instance().SetCheckBoxOptions(checkboxOptions);
            SavedOptionManager.Instance().SetSliderOptions(sliderOptions);
            SavedOptionManager.Instance().SetDropdownOptions(dropdownOptions);
            SavedOptionManager.SaveOptions();
        }

        /// <summary>
        /// Load options from disk and replace default values
        /// with stored ones.
        /// </summary>
        public static void LoadOptions()
        {
            SavedOptionManager.LoadOptions();

            if (SavedOptionManager.Instance().m_toggles != null)
            {
                foreach (StoredCheckBox storedCheckBox in SavedOptionManager.Instance().m_toggles)
                {
                    foreach (RoadCheckBoxOption option in checkboxOptions)
                    {
                        if (option.uniqueName == storedCheckBox.linkedOption)
                        {
                            option.value = storedCheckBox.data;
                        }
                    }
                }
            }

            if (SavedOptionManager.Instance().m_sliders != null)
            {
                foreach (StoredSlider storedSlider in SavedOptionManager.Instance().m_sliders)
                {
                    foreach (RoadSliderOption option in sliderOptions)
                    {
                        if (option.uniqueName == storedSlider.linkedOption)
                        {
                            if (storedSlider.data <= option.max && storedSlider.data >= option.min)
                            {
                                option.value = storedSlider.data;
                            }
                        }
                    }
                }
            }

            if (SavedOptionManager.Instance().m_dropdowns != null)
            {
                foreach (StoredDropdown storedDropdown in SavedOptionManager.Instance().m_dropdowns)
                {
                    foreach (RoadDropdownOption option in dropdownOptions)
                    {
                        if (option.uniqueName == storedDropdown.linkedOption)
                        {
                            option.value = storedDropdown.data;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populates everything that needs any additional data loading
        /// </summary>
        public static void Populate()
        {
            foreach(RoadDropdownOption option in dropdownOptions)
            {
                if(option.uniqueName == "randomNameLocalisation")
                {
                    if(Directory.Exists(m_randomNamesLocation))
                    {
                        List<string> fileNames = new List<string>();

                        foreach(string fileLocation in Directory.GetFiles(m_randomNamesLocation))
                        {
                            string fileExtension = Path.GetExtension(fileLocation);
                            string fileName = Path.GetFileNameWithoutExtension(fileLocation);

                            if(fileExtension != null && fileName != null && fileExtension.ToLower() == ".xml" && fileName != "")
                            {
                                fileNames.Add(fileName);
                            }
                        }

                        option.options = fileNames.ToArray();
                    }
                    else
                    {
                        Debug.LogError("Road Namer: Random names location doesn't exist! " + m_randomNamesLocation);
                    }
                }
            }
        }

        /// <summary>
        /// Updates everything ingame. Should be used when one of the options has updated
        /// and ingame elements need immediately refreshing.
        /// </summary>
        public static void UpdateEverything()
        {
            if (m_isIngame)
            {
                RoadRenderingManager renderingManager = Singleton<RoadRenderingManager>.instance;

                if (renderingManager != null)
                {
                    GetCheckBoxValue("showCamera", ref renderingManager.m_alwaysShowText);
                    GetSliderValue("textDisappearDistance", ref renderingManager.m_renderHeight);
                    GetSliderValue("textScale", ref renderingManager.m_textScale);

                    renderingManager.ForceUpdate();
                }

                GetDropdownValue("randomNameLocalisation", ref RandomNameManager.m_fileName);
            }
        }
    }

    public class RoadCheckBoxOption
    {
        public string uniqueName = "";
        public string readableName = "";
        public bool value = false;
        public bool enabled = false;
    }

    public class RoadSliderOption
    {
        public string uniqueName = "";
        public string readableName = "";
        public float min = 0f;
        public float max = 1f;
        public float value = 0f;
        public float step = 1f;
        public bool enabled = false;
    }

    public class RoadDropdownOption
    {
        public string uniqueName = "";
        public string readableName = "";
        public int value = 0;
        public string[] options = null;
        public bool enabled = false;
    }
}