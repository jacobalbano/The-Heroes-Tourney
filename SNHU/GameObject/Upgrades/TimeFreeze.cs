/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 7/12/2014
 * Time: 5:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of TimeFreeze.
	/// </summary>
	public class TimeFreeze : Upgrade
	{
		public List<Player> players;
		
		public TimeFreeze()
		{
			Icon  = new Image(Library.GetTexture("assets/timeFreeze.png"));
			players = new List<Player>();
		}
		
		public override void Added()
		{
			base.Added();
			Lifetime = float.Parse(GameWorld.gameManager.Config["TimeFreeze", "Lifetime"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				(to as Player).Frozen = true;
				players.Add((Player)to);
			};
			
			FP.Log("FROZEN FOR ", Lifetime, " SECONDS YO");
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				Parent.World.BroadcastMessage(EffectMessage.Message.OnEffect, MakeEffect());	
			}
		}
	
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			FP.Log("u can b unfrozen now bro");
			
			players.ForEach(p => (p as Player).Frozen = false);
			players.Clear();
			
			owner.SetUpgrade(null);
		}
	}
}
