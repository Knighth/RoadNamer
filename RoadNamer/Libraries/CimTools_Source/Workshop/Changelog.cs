using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CimTools.Workshop
{
	public class Changelog
	{
		protected static Changelog m_instance;

		protected WebClient m_webClient = new WebClient();

		protected List<string> m_changeList = new List<string>()
		{
			"<color#f58282>You have not set this up for your mod!</color>",
			"You need to call <color#c8f582>DownloadChangelog</color> or <color#c8f582>DownloadChangelogAsync</color> to get changes!",
			"You might also want to read up on the documentation some more to make the most of this panel!"
		};

		protected string m_rawChanges = "<color#f58282>You have not set this up for your mod!</color>\n\nYou need to call <color#c8f582>DownloadChangelog</color> or <color#c8f582>DownloadChangelogAsync</color> to get changes!\n\nYou might also want to read up on the documentation some more to make the most of this panel!";

		protected bool m_downloadComplete;

		protected bool m_downloadInProgress;

		protected bool m_downloadError;

		public bool m_colouriseTags = true;

		public List<KeyValuePair<string, Color>> m_tagsToColourise = new List<KeyValuePair<string, Color>>()
		{
			new KeyValuePair<string, Color>("b", new Color(0.78f, 0.96f, 0.5f)),
			new KeyValuePair<string, Color>("u", new Color(0.96f, 0.5f, 0.5f))
		};

		public List<string> ChangesList
		{
			get
			{
				return this.m_changeList;
			}
		}

		public string ChangesString
		{
			get
			{
				return this.m_rawChanges;
			}
		}

		public bool DownloadComplete
		{
			get
			{
				return this.m_downloadComplete;
			}
		}

		public bool DownloadError
		{
			get
			{
				return this.m_downloadError;
			}
		}

		public bool DownloadInProgress
		{
			get
			{
				return this.m_downloadInProgress;
			}
		}

		static Changelog()
		{
		}

		public Changelog()
		{
		}

		public void DownloadChangelog(ulong workshopId)
		{
			this.m_changeList.Clear();
			this.m_rawChanges = "";
			this.m_downloadError = true;
			this.ExtractData(this.m_webClient.DownloadString(new Uri(string.Concat("http://steamcommunity.com/sharedfiles/filedetails/changelog/", workshopId.ToString()))));
			this.m_downloadComplete = true;
		}

		public void DownloadChangelogAsync(ulong workshopId)
		{
			this.m_changeList.Clear();
			this.m_rawChanges = "";
			this.m_downloadInProgress = true;
			this.m_downloadComplete = false;
			this.m_downloadError = true;
			try
			{
				this.m_webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.M_webClient_DownloadStringCompleted);
				this.m_webClient.DownloadStringAsync(new Uri(string.Concat("http://steamcommunity.com/sharedfiles/filedetails/changelog/", workshopId.ToString())));
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		protected void ExtractData(string rawData)
		{
			if (rawData != null && rawData != "")
			{
				int num = rawData.IndexOf("<div class=\"headline\"");
				int num1 = rawData.IndexOf("<div class=\"commentsLink changeLog\"");
				if (num > 0 && num1 > 0 && num1 > num)
				{
					string str = rawData.Substring(num, num1 - num);
					Match match = (new Regex("(?:<ul.*?bb_ul.*?>)(.*)(?:<\\/ul>)")).Match(str);
					if (match.Success && match.Groups.Count > 1)
					{
						string value = match.Groups[1].Value;
						value = value.Replace("<li>", "\n\n");
						if (this.m_colouriseTags)
						{
							foreach (KeyValuePair<string, Color> mTagsToColourise in this.m_tagsToColourise)
							{
								string hex = UIMarkupStyle.ColorToHex(mTagsToColourise.Value);
								value = value.Replace(string.Concat("<", mTagsToColourise.Key, ">"), string.Concat("|~|color", hex, "!~|"));
								value = value.Replace(string.Concat("</", mTagsToColourise.Key, ">"), "|~|/color!~|");
							}
						}
						value = (new Regex("<.*?>")).Replace(value, "");
						value = value.Replace("|~|", "<");
						value = value.Replace("!~|", ">");
						value = value.Trim(new char[] { '\n' });
						this.m_rawChanges = value;
						this.m_changeList = value.Split(new string[] { "\n\n" }, StringSplitOptions.None).ToList<string>();
						this.m_downloadError = false;
					}
				}
			}
		}

		public static Changelog Instance()
		{
			if (Changelog.m_instance == null)
			{
				Changelog.m_instance = new Changelog();
			}
			return Changelog.m_instance;
		}

		private void M_webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			this.m_downloadComplete = true;
			this.m_downloadInProgress = false;
			if (!e.Cancelled && e.Result != null && e.Result != "")
			{
				this.ExtractData(e.Result);
			}
		}
	}
}