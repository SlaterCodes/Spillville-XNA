using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using SpillvilleDataTypes;

namespace Spillville.MainGame.HUD
{
	class BulletinContainer
	{
		private Bulletin[] _bulletins;
		private static Dictionary<string, List<Bulletin>> _categories = new Dictionary<string, List<Bulletin>>();
		private static Random _random = new Random();

		public static void CallBulletin(string category)
		{
			if(!_categories.ContainsKey(category))
			{
				throw new Exception("Category not found.");
			}
			var list = _categories[category];
			BulletinDrawer.AddBulletin(list[_random.Next(list.Count)]);
		}

		public void LoadContent(ContentManager content)
		{
            
			_bulletins = content.Load<Bulletin[]>("Bulletin\\Bulletins");
			foreach (var bulletin in _bulletins)
			{
				BulletinDrawer.IntializeBulletinText(bulletin);
				bulletin.LoadContent(content);
				if(_categories.ContainsKey(bulletin.MessageClass))
				{
					_categories[bulletin.MessageClass].Add(bulletin);
				}
				else
				{
					var list = new List<Bulletin> {bulletin};
					_categories.Add(bulletin.MessageClass,list);
				}
			}
             
		}



	}
}
