using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Utils;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of HotPotato.
	/// </summary>
	public class HotPotato : Upgrade
	{
		public HotPotato()
		{
			Icon = new Image(Library.GetTexture("assets/hotpotato.png"));
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
