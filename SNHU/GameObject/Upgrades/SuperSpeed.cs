using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SNHU.Components;

//using SFML.Graphics;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of SuperSpeed.
	/// </summary>
	public class SuperSpeed : Upgrade
	{
		public const float SUPER_SPEED = 12.0f;
		public Dictionary<Player, Emitter> emitters;
		public List<Player> players;
		
		public SuperSpeed()
		{
			Icon  = new Image(Library.GetTexture("assets/speed.png"));
			emitters = new Dictionary<Player, Emitter>();
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				players = new List<Player>(GameWorld.gameManager.Players);
				var toRemove = new List<Player>();
				players.RemoveAll(p => !p.IsAlive);
				
				bool anyRebounding = players.Exists(p => p.Rebounding);
				
				foreach (var player in players)
				{
					if (player == owner && !anyRebounding)
					{
						toRemove.Add(player);
						continue;
					}
					
					if (player.Invincible || player.Rebounding)
					{
						toRemove.Add(player);
						continue;
					}
					
					var emitter = new Emitter(Library.GetTexture("assets/speed_particle.png"), 68, 30);
					emitter.Relative = false;
					
					emitter.NewType("l", FP.Frames(0));
					emitter.SetMotion("l", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
					emitter.SetAlpha("l", 0.5f, 0, Ease.QuintOut);
					
					emitter.NewType("r", FP.Frames(1));
					emitter.SetMotion("r", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
					emitter.SetAlpha("r", 0.5f, 0, Ease.QuintOut);
					emitters[player] = emitter;
					
					player.AddGraphic(emitter);
					player.OnMessage(Movement.SPEED, SUPER_SPEED);
				}
				
				foreach (var p in toRemove)
				{
					players.Remove(p);
				}
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!Activated)	return;
			
			foreach (var player in players)
			{
				if (!emitters.ContainsKey(player))
					continue;
				var emitter = emitters[player];
				
				if (lifeTimer.Percent < 1.0f)
				{
					var delta = player.physics.MoveDelta.X;
					if (delta < 0)
					{
						emitter.Emit("l", player.Left - 38, player.Top + FP.Rand(player.Height));
					}
					else if (delta > 0)
					{
						emitter.Emit("r", player.Left, player.Top + FP.Rand(player.Height));
					}
				}
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			for (int i = 0; i < players.Count; i++)
			{
				var player = players[i];
				
				player.OnMessage(Movement.SPEED, Player.SPEED);
				
				if (emitters.ContainsKey(player))
				{
					var emitter = emitters[player];
					FP.Tweener.AddTween(new Alarm(1, () => {
						(player.Graphic as Graphiclist).Remove(emitter);
	              	}, ONESHOT));
				};
			}
			
			owner.SetUpgrade(null);
		}
	}
}
