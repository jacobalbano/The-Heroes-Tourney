
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GlideTween;
using Punk;
using Punk.Graphics;
using Punk.Utils;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of PotatoThinker.
	/// </summary>
	public class PotatoThinker : Entity
	{
		Glide alarm;
		
		List<Player> opponents;
		Player target;
		Player parent;
		
		Image image;
		float totalTime;
		
		enum Mode { Furthest, Nearest, Random }
		Mode mode;
		
		public PotatoThinker(Player parent)
		{
			this.parent = parent;
			var modeString = Regex.Replace(GameWorld.gameManager.Config["HotPotato", "Mode"], @"\s", "");
			totalTime = Math.Max(0.01f, float.Parse(GameWorld.gameManager.Config["HotPotato", "Duration"]));
			
			mode = (Mode) Util.GetEnumFromName(typeof(Mode), modeString);
			if (mode == Mode.Random)
				mode = FP.Choose(Mode.Furthest, Mode.Nearest);
			
			opponents = new List<Player>();
			target = parent;
						          
			image = new Image(Library.GetTexture("assets/hotpotato.png"));
			image.CenterOO();
			image.Color = FP.Color(0xff0000);
			AddComponent(image);
			
			CenterOrigin();
			X = parent.X;
			Y = parent.Top - 40;
			
			AddResponse(Player.Message.Die, OnPlayerDie);
			
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
			AddResponse(ChunkManager.Message.AdvanceComplete, OnAdvanceComplete);
			
			alarm = Tweener.Timer(totalTime).OnComplete(OnGoBoom);
			Tick();
		}
		
		private void Tick()
		{
			image.Scale = 1.25f;
			Tweener.Tween(image, new { Scale = 1}, alarm.TimeRemaining / 5)
				.OnComplete(Tick);
			
			Mixer.TimeTick.Play();
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
					x = X;
					y = Y;
				}
			}
			else
			{
				x = parent.X;
				y = parent.Y;
			}
			
			var e = World.AddGraphic(emitter, -9010);
			e.Tweener.Timer(3).OnComplete(() => FP.World.Remove(e));
			
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
			
			FP.World.BroadcastMessageIf(ent => ent.DistanceToPoint(x, y, true) <= radius, EffectMessage.Message.OnEffect, MakeEffect());
			FP.World.BroadcastMessage(CameraShake.Message.Shake, 20, 0.5f);
			Mixer.Explode.Play();
		}
		
		EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				if (to == parent)
				{
					if (from.DistanceToPoint(to.X, to.Y, true) <= 200)
						from.OnMessage(Player.Message.Damage);
				}
				
				to.OnMessage(Player.Message.Damage);
			};
			
			return new EffectMessage(parent, callback);
		}
		
		public override void Update()
		{
			base.Update();
			
			UpdateOpponents();
			
			if (mode == Mode.Furthest)
				GetFurthestTarget();
			else
				GetNearestTarget();
				
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
		}
		
		private void OnAdvanceComplete(params object[] args)
		{
			Active = true;
		}
	}
}
