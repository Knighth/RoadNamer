using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CimTools.Utilities
{
	public class UIUtilities
	{
		private static Dictionary<string, UITextureAtlas> sm_atlases;

		public static UITextureAtlas defaultAtlas
		{
			get
			{
				return UIUtilities.GetAtlas("Ingame");
			}
		}

		static UIUtilities()
		{
			UIUtilities.sm_atlases = new Dictionary<string, UITextureAtlas>();
		}

		public UIUtilities()
		{
		}

		public static UIButton CreateButton(UIComponent parent)
		{
			UIButton vector2 = parent.AddUIComponent<UIButton>();
			vector2.atlas = UIUtilities.defaultAtlas;
			vector2.size = new Vector2(90f, 30f);
			vector2.textScale = 0.9f;
			vector2.normalBgSprite = "ButtonMenu";
			vector2.hoveredBgSprite = "ButtonMenuHovered";
			vector2.pressedBgSprite = "ButtonMenuPressed";
			vector2.canFocus = false;
			return vector2;
		}

		public static UICheckBox CreateCheckBox(UIComponent parent)
		{
			UICheckBox vector2 = parent.AddUIComponent<UICheckBox>();
			vector2.width = 300f;
			vector2.height = 20f;
			vector2.clipChildren = true;
			UISprite _zero = vector2.AddUIComponent<UISprite>();
			_zero.spriteName = "ToggleBase";
			_zero.size = new Vector2(16f, 16f);
			_zero.relativePosition = Vector3.zero;
			vector2.checkedBoxObject = _zero.AddUIComponent<UISprite>();
			((UISprite)vector2.checkedBoxObject).atlas = UIUtilities.defaultAtlas;
			((UISprite)vector2.checkedBoxObject).spriteName = "ToggleBaseFocused";
			vector2.checkedBoxObject.size = new Vector2(16f, 16f);
			vector2.checkedBoxObject.relativePosition = Vector3.zero;
			vector2.label = vector2.AddUIComponent<UILabel>();
			vector2.label.atlas = UIUtilities.defaultAtlas;
			vector2.label.text = " ";
			vector2.label.textScale = 0.9f;
			vector2.label.relativePosition = new Vector3(22f, 2f);
			return vector2;
		}

		public static UIColorField CreateColorField(UIComponent parent)
		{
			UIColorField component = UnityEngine.Object.Instantiate<GameObject>(UnityEngine.Object.FindObjectOfType<UIColorField>().gameObject).GetComponent<UIColorField>();
			parent.AttachUIComponent(component.gameObject);
			component.size = new Vector2(40f, 26f);
			component.normalBgSprite = "ColorPickerOutline";
			component.hoveredBgSprite = "ColorPickerOutlineHovered";
			component.selectedColor = Color.white;
			component.pickerPosition = UIColorField.ColorPickerPosition.RightAbove;
			return component;
		}

		public static UIDropDown CreateDropDown(UIComponent parent)
		{
			UIDropDown vector2 = parent.AddUIComponent<UIDropDown>();
			vector2.atlas = UIUtilities.defaultAtlas;
			vector2.size = new Vector2(90f, 30f);
			vector2.listBackground = "GenericPanelLight";
			vector2.itemHeight = 30;
			vector2.itemHover = "ListItemHover";
			vector2.itemHighlight = "ListItemHighlight";
			vector2.normalBgSprite = "ButtonMenu";
			vector2.disabledBgSprite = "ButtonMenuDisabled";
			vector2.hoveredBgSprite = "ButtonMenuHovered";
			vector2.focusedBgSprite = "ButtonMenu";
			vector2.listWidth = 90;
			vector2.listHeight = 500;
			vector2.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
			vector2.popupColor = new Color32(45, 52, 61, 255);
			vector2.popupTextColor = new Color32(170, 170, 170, 255);
			vector2.zOrder = 1;
			vector2.textScale = 0.8f;
			vector2.verticalAlignment = UIVerticalAlignment.Middle;
			vector2.horizontalAlignment = UIHorizontalAlignment.Left;
			vector2.selectedIndex = 0;
			vector2.textFieldPadding = new RectOffset(8, 0, 8, 0);
			vector2.itemPadding = new RectOffset(14, 0, 8, 0);
			UIButton vector3 = vector2.AddUIComponent<UIButton>();
			vector2.triggerButton = vector3;
			vector3.atlas = UIUtilities.defaultAtlas;
			vector3.text = "";
			vector3.size = vector2.size;
			vector3.relativePosition = new Vector3(0f, 0f);
			vector3.textVerticalAlignment = UIVerticalAlignment.Middle;
			vector3.textHorizontalAlignment = UIHorizontalAlignment.Left;
			vector3.normalFgSprite = "IconDownArrow";
			vector3.hoveredFgSprite = "IconDownArrowHovered";
			vector3.pressedFgSprite = "IconDownArrowPressed";
			vector3.focusedFgSprite = "IconDownArrowFocused";
			vector3.disabledFgSprite = "IconDownArrowDisabled";
			vector3.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
			vector3.horizontalAlignment = UIHorizontalAlignment.Right;
			vector3.verticalAlignment = UIVerticalAlignment.Middle;
			vector3.zOrder = 0;
			vector3.textScale = 0.8f;
			vector2.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((UIComponent c, Vector2 t) => {
				vector3.size = t;
				vector2.listWidth = (int)t.x;
			});
			return vector2;
		}

		public static UITextField CreateTextField(UIComponent parent)
		{
			UITextField vector2 = parent.AddUIComponent<UITextField>();
			vector2.atlas = UIUtilities.defaultAtlas;
			vector2.size = new Vector2(90f, 20f);
			vector2.padding = new RectOffset(6, 6, 3, 3);
			vector2.builtinKeyNavigation = true;
			vector2.isInteractive = true;
			vector2.readOnly = false;
			vector2.horizontalAlignment = UIHorizontalAlignment.Center;
			vector2.selectionSprite = "EmptySprite";
			vector2.selectionBackgroundColor = new Color32(0, 172, 234, 255);
			vector2.normalBgSprite = "TextFieldPanelHovered";
			vector2.textColor = new Color32(0, 0, 0, 255);
			vector2.disabledTextColor = new Color32(0, 0, 0, 128);
			vector2.color = new Color32(255, 255, 255, 255);
			return vector2;
		}

		public static UITextureAtlas GetAtlas(string name)
		{
			UITextureAtlas[] uITextureAtlasArray = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
			for (int i = 0; i < (int)uITextureAtlasArray.Length; i++)
			{
				if (uITextureAtlasArray[i].name == name)
				{
					return uITextureAtlasArray[i];
				}
			}
			return null;
		}

		public static void ResizeIcon(UISprite icon, Vector2 maxSize)
		{
			if (icon.height == 0f)
			{
				return;
			}
			float single = icon.width / icon.height;
			if (icon.width > maxSize.x)
			{
				icon.width = maxSize.x;
				icon.height = maxSize.x / single;
			}
			if (icon.height > maxSize.y)
			{
				icon.height = maxSize.y;
				icon.width = maxSize.y * single;
			}
		}
	}
}