using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.PlatformServices;
using System;
using System.Collections.Generic;

namespace CimTools.File
{
	public class Path
	{
		internal static string m_savedModPath;

		static Path()
		{
		}

		public Path()
		{
		}

		public static string GetModPath(ulong workshopId, string modName)
		{
			if (Path.m_savedModPath == null)
			{
				foreach (PluginManager.PluginInfo pluginsInfo in Singleton<PluginManager>.instance.GetPluginsInfo())
				{
					if (!(pluginsInfo.name == modName) && pluginsInfo.publishedFileID.AsUInt64 != workshopId)
					{
						continue;
					}
					Path.m_savedModPath = pluginsInfo.modPath;
				}
			}
			return Path.m_savedModPath;
		}
	}
}