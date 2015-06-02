using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.GameObject.Upgrades.Helper;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of HotPotato.
	/// </summary>
	public class HotPotato : Upgrade
	{
		public HotPotato()
		{
			Icon = new Image(Library.GetTexture("hotpotato.png"));
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				Parent.World.Add(new PotatoThinker(owner));
			}
		}
	}
}
