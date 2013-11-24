/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 5:51 AM
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
	/// Description of HotPotato.
	/// </summary>
	public class HotPotato : Upgrade
	{
		Entity countdown;
		List<Entity> opponents;
		Player closestOpponent;
		
		public const string GO_BOOM = "goBoom";
		
		public HotPotato()
		{
			var txt = new Text("0.0",0,0);
			txt.Size = 30;
			txt.Bolded = true;
			txt.CenterOO();
			
			countdown = new Entity(0,0,txt);
			countdown.CenterOrigin();
			
			opponents = new List<Entity>();
			closestOpponent = (Player)Parent;
			
			AddResponse("player_die", OnPlayerDie);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				Parent.World.GetType(Player.Collision, opponents);
				
				for (int i = 0; i < opponents.Count; i++)
				{
					if (opponents[i] == Parent)
					{
						opponents.RemoveAt(i);
					}
				}
				
				countdown.X = Parent.X;
				countdown.Y = Parent.Top - 40;
				Parent.World.Add(countdown);
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			
			Parent.World.Remove(countdown);
			closestOpponent.Kill();
		}
		
		public override void Update()
		{
			base.Update();
			
			(countdown.Graphic as Text).String = lifeTimer.Remaining.ToString("0");
			
			float minDist = 9999.0f;
			foreach (var p in opponents)
			{
				var currentDist = FP.Distance(Parent.X, Parent.Y - Parent.HalfHeight, p.X, p.Y - p.HalfHeight);
				
				if (currentDist < minDist)
				{
					minDist = currentDist;
					closestOpponent = (Player)p;
				}
			}
				
			if (closestOpponent != null)
			{
				countdown.X = closestOpponent.X;
				countdown.Y = closestOpponent.Top - 40;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			(Parent as Player).SetUpgrade(null);
			Parent.RemoveLogic(this);
		}
		
		public void OnPlayerDie(params object[] args)
		{
			var p = args[0] as Player;
			
			if (opponents.Contains(p))
			{
				opponents.Remove(p);
			}
		}
	}
}
