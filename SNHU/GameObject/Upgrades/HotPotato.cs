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
using Punk.Tweens.Misc;
using Punk.Utils;

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
		
		private int prevTime;
		
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
			prevTime = 5;
		
			//image = new Image(Library.GetTexture("assets/" +  + ".png"));
			image = Image.CreateCircle(3, FP.Color(0xFF0000));
						                           
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
			
			var emitter = new Emitter(Library.GetTexture("assets/explosion.png"), 60, 60);
			emitter.Relative = false;
			
			for (int i = 0; i < 4; i++)
			{
				var name = i.ToString();
				emitter.NewType(name, FP.Frames(i));
				emitter.SetAlpha(name, 1, 0);
				emitter.SetMotion(name, 0, 150, 0.25f, 360, 100,  0.1f, Ease.CubeOut);
			}
			
			float x = 0, y = 0;
			
			if(closestOpponent != null)
			{
				if (closestOpponent.Rebounding)
				{
					x = Parent.X;
					y = Parent.Y;
					
					owner.Kill();
				}
				else
				{
					x = closestOpponent.X;
					y = closestOpponent.Y;
					
					closestOpponent.Kill();
				}
			}
			else
			{
				owner.Kill();
				
				x = Parent.X;
				y = Parent.Y;
			}
			
			var e = Parent.World.AddGraphic(emitter, -9010);
			e.AddTween(new Alarm(3, () => FP.World.Remove(e), Tween.ONESHOT), true);
			
			var t = 4;
			while (t --> 0)
			{
				var r = t * 100;
				var name = t.ToString();
				for (int j = 0; j < r; j++)
				{
					var randX = FP.Rand(r) - r / 2;
					var randY = FP.Rand(r) - r / 2;
					emitter.Emit(name, x + randX, y + randY);
				}
			}
			
			Parent.World.BroadcastMessage(CameraShake.SHAKE, 20, 0.5f);
			Mixer.Audio["explode"].Play();
		}
		
		public override void Update()
		{
			base.Update();
			
			var text = countdown.Graphic as Text;
			text.String = lifeTimer.Remaining.ToString("0");
			if (prevTime != int.Parse(text.String))
			{
				prevTime = int.Parse(text.String);
				Mixer.Audio["timeTick"].Play();
			}
			
			float minDist = 9999.0f;
			foreach (var p in opponents)
			{
				var currentDist = FP.Distance(Parent.X, Parent.Y - Parent.HalfHeight, p.X, p.Y - p.HalfHeight);
				
				if (currentDist < minDist)
				{
					minDist = currentDist;
					closestOpponent = (Player) p;
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
			
			owner.SetUpgrade(null);
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
