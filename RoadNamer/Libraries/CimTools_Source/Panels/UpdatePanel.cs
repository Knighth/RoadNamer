using CimTools.Elements;
using CimTools.Utilities;
using CimTools.Workshop;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace CimTools.Panels
{
	public class UpdatePanel : UIPanel
	{
		protected RectOffset m_UIPadding = new RectOffset(5, 5, 5, 5);

		private UITitleBar m_panelTitle;

		private UILabel m_infoLabel;

		public Changelog m_changelogDownloader = Changelog.Instance();

		public string m_updatedTitleMessage = "I've updated!";

		public string m_updatedContentMessage = "<color#c8f582>Click here</color> to see what's changed";

		public UpdatePanel()
		{
		}

		public override void Awake()
		{
			this.isInteractive = true;
			base.enabled = true;
			base.width = 200f;
			base.height = 100f;
			base.Awake();
		}

		private void M_infoLabel_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
		{
			this.ShowUpdateInfo();
		}

		public void setPositionSpeakyPoint(Vector2 position)
		{
			base.relativePosition = new Vector3(position.x, position.y - base.height);
		}

		private void ShowUpdateInfo()
		{
			float mInfoLabel = this.m_infoLabel.height;
			this.m_infoLabel.text = "Unable to retrieve the latest changes! Check on the workshop for the most recent changes.";
			if (!Changelog.Instance().DownloadComplete)
			{
				Debug.LogError("Failed to download workshop changes!");
			}
			else
			{
				this.m_infoLabel.text = Changelog.Instance().ChangesString;
			}
			float single = this.m_infoLabel.height - mInfoLabel;
			base.height = this.m_infoLabel.relativePosition.y + this.m_infoLabel.height + (float)this.m_UIPadding.bottom + 20f;
			base.relativePosition = base.relativePosition - new Vector3(0f, single);
		}

		public override void Start()
		{
			base.Start();
			this.m_panelTitle = base.AddUIComponent<UITitleBar>();
			this.m_panelTitle.title = this.m_updatedTitleMessage;
			this.m_infoLabel = base.AddUIComponent<UILabel>();
			this.m_infoLabel.width = (float)(200 - this.m_UIPadding.left - this.m_UIPadding.right);
			this.m_infoLabel.wordWrap = true;
			this.m_infoLabel.processMarkup = true;
			this.m_infoLabel.autoHeight = true;
			this.m_infoLabel.text = this.m_updatedContentMessage;
			this.m_infoLabel.textScale = 0.6f;
			this.m_infoLabel.relativePosition = new Vector3((float)this.m_UIPadding.left, this.m_panelTitle.height + (float)this.m_UIPadding.bottom);
			this.m_infoLabel.eventClicked += new MouseEventHandler(this.M_infoLabel_eventClicked);
			base.atlas = UIUtilities.GetAtlas("Ingame");
			base.backgroundSprite = "InfoBubble";
			base.height = this.m_infoLabel.relativePosition.y + this.m_infoLabel.height + (float)this.m_UIPadding.bottom + 20f;
		}
	}
}