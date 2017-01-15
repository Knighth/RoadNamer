using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CimTools.Utilities
{
	public class SpriteUtilities
	{
		internal static Dictionary<string, UITextureAtlas> m_atlasStore;

		static SpriteUtilities()
		{
			SpriteUtilities.m_atlasStore = new Dictionary<string, UITextureAtlas>();
		}

		public SpriteUtilities()
		{
		}

		public static bool AddSpriteToAtlas(Rect dimensions, string spriteName, string atlasName)
		{
			bool flag = false;
			if (SpriteUtilities.m_atlasStore.ContainsKey(atlasName))
			{
				UITextureAtlas item = SpriteUtilities.m_atlasStore[atlasName];
				Texture2D texture2D = item.texture;
				Vector2 vector2 = new Vector2((float)texture2D.width, (float)texture2D.height);
				Rect rect = new Rect(new Vector2(dimensions.position.x / vector2.x, dimensions.position.y / vector2.y), new Vector2(dimensions.width / vector2.x, dimensions.height / vector2.y));
				Texture2D texture2D1 = new Texture2D((int)Math.Round((double)dimensions.width), (int)Math.Round((double)dimensions.height));
				texture2D1.SetPixels(texture2D.GetPixels((int)dimensions.position.x, (int)dimensions.position.y, (int)dimensions.width, (int)dimensions.height));
				item.AddSprite(new UITextureAtlas.SpriteInfo()
				{
					name = spriteName,
					region = rect,
					texture = texture2D1
				});
				flag = true;
			}
			return flag;
		}

		public static void FixTransparency(Texture2D texture)
		{
			Color32[] pixels32 = texture.GetPixels32();
			int _width = texture.width;
			int _height = texture.height;
			for (int i = 0; i < _height; i++)
			{
				for (int j = 0; j < _width; j++)
				{
					int num = i * _width + j;
					Color32 color32 = pixels32[num];
					if (color32.a == 0)
					{
						bool flag = false;
						if (!flag && j > 0)
						{
							flag = SpriteUtilities.TryAdjacent(ref color32, pixels32[num - 1]);
						}
						if (!flag && j < _width - 1)
						{
							flag = SpriteUtilities.TryAdjacent(ref color32, pixels32[num + 1]);
						}
						if (!flag && i > 0)
						{
							flag = SpriteUtilities.TryAdjacent(ref color32, pixels32[num - _width]);
						}
						if (!flag && i < _height - 1)
						{
							flag = SpriteUtilities.TryAdjacent(ref color32, pixels32[num + _width]);
						}
						pixels32[num] = color32;
					}
				}
			}
			texture.SetPixels32(pixels32);
			texture.Apply();
		}

		public static UITextureAtlas GetAtlas(string atlasName)
		{
			UITextureAtlas item = null;
			if (SpriteUtilities.m_atlasStore.ContainsKey(atlasName))
			{
				item = SpriteUtilities.m_atlasStore[atlasName];
			}
			return item;
		}

		public static bool InitialiseAtlas(string texturePath, string atlasName)
		{
			bool flag = false;
			if (texturePath == null)
			{
				Debug.LogError("Road Namer: Could not find the mod path, which is odd.");
			}
			else
			{
				Shader shader = Shader.Find("UI/Default UI Shader");
				if (shader == null)
				{
					Debug.LogError("Road Namer: Couldn't find the default UI Shader!");
				}
				else if (!System.IO.File.Exists(texturePath))
				{
					Debug.LogError(string.Concat("Road Namer: Could not find atlas at ", texturePath));
				}
				else
				{
					Texture2D texture2D = new Texture2D(2, 2);
					FileStream fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
                    byte[] numArray = new byte[fileStream.Length];
					fileStream.Read(numArray, 0, (int)fileStream.Length);
					texture2D.LoadImage(numArray);
					SpriteUtilities.FixTransparency(texture2D);
					Material material = new Material(shader);
					material.mainTexture = texture2D;
					Material material1 = material;
					UITextureAtlas uITextureAtla = ScriptableObject.CreateInstance<UITextureAtlas>();
					uITextureAtla.name = atlasName;
					uITextureAtla.material = material1;
					SpriteUtilities.m_atlasStore.Add(atlasName, uITextureAtla);
					flag = true;
				}
			}
			return flag;
		}

		private static bool TryAdjacent(ref Color32 pixel, Color32 adjacent)
		{
			if (adjacent.a == 0)
			{
				return false;
			}
			pixel.r = adjacent.r;
			pixel.g = adjacent.g;
			pixel.b = adjacent.b;
			return true;
		}
	}
}