using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpillvilleDataTypes
{

	public class Bulletin
	{

		private static readonly XBoxFriendlyDictionary<String,Texture2D> _globalBulletinIcons = new XBoxFriendlyDictionary<string, Texture2D>();
		public Texture2D BulletinIcon { get; private set;  }
		
		public String MessageClass;
		public String From;
		public String Message;

		public String IconFileName;


		public Bulletin()
		{
			IconFileName = string.Empty;
			From = string.Empty;
			Message = string.Empty;
			MessageClass = string.Empty;
		}

		public void UseLiveIDIcon(string gamerTag, Texture2D texture)
		{
			if (!_globalBulletinIcons.ContainsKey(gamerTag))
			{
				_globalBulletinIcons.Add(gamerTag, texture);
			}
			IconFileName = gamerTag;
		}


		public void LoadContent(ContentManager content)
		{
			if(_globalBulletinIcons.ContainsKey(IconFileName))
			{
				BulletinIcon = _globalBulletinIcons[IconFileName];
			}
			else if (!IconFileName.Equals(String.Empty))
			{
				BulletinIcon = content.Load<Texture2D>(IconFileName);
				_globalBulletinIcons.Add(IconFileName,BulletinIcon);
			}
			else
			{
				BulletinIcon = null;
			}

		}





	}
}
