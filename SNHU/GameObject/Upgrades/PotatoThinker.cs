
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of PotatoThinker.
	/// </summary>
	public class PotatoThinker : Entity
	{
		List<Player> opponents;
		Player target;
		Player parent;
		
		Image image;
		float totalTime;
		Alarm alarm;
		
		public PotatoThinker(Player parent)
		{
			this.parent = parent;
			
			opponents = new List<Player>();
			target = parent;
						          
			image = new Image(Library.GetTexture("assets/hotpotato.png"));
			image.CenterOO();
			image.Color = FP.Color(0xff0000);
			Graphic = image;
			
			CenterOrigin();
			X = parent.X;
			Y = parent.Top - 40;
			
			AddResponse(Player.Die, OnPlayerDie);
			
			AddResponse(ChunkManager.Advance, OnAdvance);
			AddResponse(ChunkManager.AdvanceComplete, OnAdvanceComplete);
			
			totalTime = 5;
			AddTween(alarm = new Alarm(totalTime, OnGoBoom, ONESHOT), true);
			Tick();
		}
		
		private void Tick()
		{
			image.Scale = 1.25f;
			
			var tween = new VarTween(Tick, ONESHOT);
			tween.Tween(image, "Scale", 1, alarm.Remaining / 5f);
			AddTween(tween, true);
			
			Mixer.Audio["timeTick"].Play();
		}
		
		public override void Added()
		{
			base.Added();
			UpdateOpponents();
		}
		
		public override void Removed()
		{
			base.Removed();
			
			var emitter = new Emitter(Library.GetTexture("assets/explosion.png"), 60, 60);
			emitter.Relative = false;
			
			for (int i = 0; i < 4; i++)
			{
				var name = i.ToString();
				emitter.NewType(name, FP.Frames(i));
				emitter.SetAlpha(name, 1, 0);
				emitter.SetMotion(name, 0, 50, 0.4f, 360, 15,  0.1f, Ease.CubeOut);
			}
			
			float x = 0, y = 0;
			
			if(target != null)
			{
				if (target.Rebounding)
				{
					x = parent.X;
					y = parent.Y;
				}
				else
				{
					x = target.X;
					y = target.Y;
				}
			}
			else
			{
				x = parent.X;
				y = parent.Y;
			}
			
			var e = World.AddGraphic(emitter, -9010);
			e.AddTween(new Alarm(3, () => FP.World.Remove(e), Tween.ONESHOT), true);
			
			var radius = 200;
			var t = 4;
			while (t --> 0)
			{
				var name = t.ToString();
				for (int j = 0; j < radius; j++)
				{
					var randX = FP.Rand(radius) - radius / 2;
					var randY = FP.Rand(radius) - radius / 2;
					emitter.Emit(name, x + randX, y + randY);
				}
			}
			
			FP.World.BroadcastMessageInCircle(x, y, radius, Player.Damage);
			FP.World.BroadcastMessage(CameraShake.SHAKE, 20, 0.5f);
			Mixer.Audio["explode"].Play();
		}
		
		public override void Update()
		{
			base.Update();
			
			UpdateOpponents();
			GetFurthestTarget();
				
			if (target != null)
			{
				float x = target.X, y = target.Top - 40;
				MoveTowards(x, y, FP.Distance(X, Y, x, y) * 0.2f);
			}
		}
		
		private void UpdateOpponents()
		{
			opponents.Clear();
			opponents = GameWorld.gameManager.Players.FindAll(p => p.World != null);
			
			for (int i = 0; i < opponents.Count; i++)
			{
				if (opponents[i] == parent)
				{
					opponents.RemoveAt(i);
				}
			}
		}
		
		private void GetNearestTarget()
		{
			float minDist = 9999.0f;
			foreach (var p in opponents)
			{
				var currentDist = FP.Distance(parent.X, parent.Y - parent.HalfHeight, p.X, p.Y - p.HalfHeight);
				
				if (currentDist < minDist)
				{
					minDist = currentDist;
					target = p;
				}
			}
		}
		
		private void GetFurthestTarget()
		{
			float maxDist = 0f;
			foreach (var p in opponents)
			{
				var currentDist = FP.Distance(parent.X, parent.Y - parent.HalfHeight, p.X, p.Y - p.HalfHeight);
				
				if (currentDist > maxDist)
				{
					maxDist = currentDist;
					target = p;
				}
			}
		}
		
		private void OnPlayerDie(params object[] args)
		{
			var p = args[0] as Player;
			opponents.Remove(p);
			
			if (p != parent)
			{
				if (opponents.Count == 0)
				{
					//	no more enemies
					if (parent.World == null)
					{
						//	the player is no longer in the world either
						Active = false;
					}
					else
					{
						target = parent;
					}
				}
			}
		}
		
		private void OnGoBoom()
		{
			parent.SetUpgrade(null);
			World.Remove(this);
		}
		
		private void OnAdvance(params object[] args)
		{
			Active = false;
			FP.Log("advance pause");
		}
		
		private void OnAdvanceComplete(params object[] args)
		{
			Active = true;
			FP.Log("advance unpause");
		}
	}
}
