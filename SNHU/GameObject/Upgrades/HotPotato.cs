using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of HotPotato.
	/// </summary>
	public class HotPotato : Upgrade
	{
		public const string GO_BOOM = "goBoom";
		
		public HotPotato()
		{
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				Parent.World.Add(new PotatoThinker(Parent as Player));
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			FP.World.BroadcastMessage(GO_BOOM, owner);
			owner.SetUpgrade(null);
		}
	}
}
