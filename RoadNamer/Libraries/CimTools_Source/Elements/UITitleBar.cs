using CimTools.Utilities;
using ColossalFramework.UI;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CimTools.Elements
{
	public class UITitleBar : UIPanel
	{
		private UISprite m_icon;

		private UILabel m_title;

		private UIButton m_close;

		private UIDragHandle m_drag;

		public UIButton closeButton
		{
			get
			{
				return this.m_close;
			}
		}

		public UITextureAtlas iconAtlas
		{
			get
			{
				return this.m_icon.atlas;
			}
			set
			{
				this.m_icon.atlas = value;
			}
		}

		public string iconSprite
		{
			get
			{
				return this.m_icon.spriteName;
			}
			set
			{
				if (this.m_icon != null)
				{
					this.m_icon.spriteName = value;
					if (this.m_icon.atlas == null)
					{
						this.m_icon.atlas = UIUtilities.defaultAtlas;
					}
					if (this.m_icon.spriteInfo != null)
					{
						this.m_icon.size = this.m_icon.spriteInfo.pixelSize;
						UIUtilities.ResizeIcon(this.m_icon, new Vector2(32f, 32f));
						this.m_icon.relativePosition = new Vector3(10f, 5f);
					}
				}
			}
		}

		public string title
		{
			get
			{
				return this.m_title.text;
			}
			set
			{
				this.m_title.text = value;
			}
		}

		public UITitleBar()
		{
		}

		public override void Awake()
		{
			base.Awake();
			this.m_icon = base.AddUIComponent<UISprite>();
			this.m_title = base.AddUIComponent<UILabel>();
			this.m_close = base.AddUIComponent<UIButton>();
			this.m_drag = base.AddUIComponent<UIDragHandle>();
			base.height = 40f;
			base.width = 450f;
			this.title = "(None)";
			this.iconSprite = "";
		}

		public override void Start()
		{
			base.Start();
			base.width = base.parent.width;
			base.relativePosition = Vector3.zero;
			base.isVisible = true;
			this.canFocus = true;
			this.isInteractive = true;
			this.m_drag.width = base.width - 50f;
			this.m_drag.height = base.height;
			this.m_drag.relativePosition = Vector3.zero;
			this.m_drag.target = base.parent;
			this.m_icon.spriteName = this.iconSprite;
			this.m_icon.relativePosition = new Vector3(10f, 5f);
			this.m_title.relativePosition = new Vector3(50f, 13f);
			this.m_title.text = this.title;
			this.m_title.textAlignment = UIHorizontalAlignment.Center;
			this.m_close.atlas = UIUtilities.defaultAtlas;
			this.m_close.relativePosition = new Vector3(base.width - 35f, 2f);
			this.m_close.normalBgSprite = "buttonclose";
			this.m_close.hoveredBgSprite = "buttonclosehover";
			this.m_close.pressedBgSprite = "buttonclosepressed";
			this.m_close.eventClick += new MouseEventHandler((UIComponent component, UIMouseEventParameter param) => base.parent.Hide());
			this.m_title.width = base.parent.width - base.relativePosition.x - this.m_close.width - 10f;
		}
	}
}