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
		public float NewSpeed { get; private set; }// = 12.0f;
		public Dictionary<Player, Emitter> emitters;
		
		public SuperSpeed()
		{
			Icon  = new Image(Library.GetTexture("assets/speed.png"));
			emitters = new Dictionary<Player, Emitter>();
		}
		
		public override void Added()
		{
			base.Added();
			NewSpeed = float.Parse(GameWorld.gameManager.Config["SuperSpeed", "NewSpeed"]);
			Lifetime = float.Parse(GameWorld.gameManager.Config["SuperSpeed", "Lifetime"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				var emitter = new Emitter(Library.GetTexture("assets/speed_particle.png"), 68, 30);
				emitter.Relative = false;
				
				emitter.NewType("l", FP.Frames(0));
				emitter.SetMotion("l", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
				emitter.SetAlpha("l", 0.5f, 0, Ease.QuintOut);
				
				emitter.NewType("r", FP.Frames(1));
				emitter.SetMotion("r", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
				emitter.SetAlpha("r", 0.5f, 0, Ease.QuintOut);
				emitters[to as Player] = emitter;
				
				to.AddGraphic(emitter);
				to.OnMessage(Movement.SPEED, NewSpeed);
			};
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				Parent.World.BroadcastMessageIf(e => e != owner, EffectMessage.ON_EFFECT, MakeEffect());	
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!Activated)	return;
			
			foreach (var pair in emitters)
			{
				var player = pair.Key;
				var emitter = pair.Value;
				
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
			
			foreach (var pair in emitters)
			{
				var player = pair.Key;
				var emitter = pair.Value;
				
				player.OnMessage(Movement.SPEED, Player.SPEED);
				
				if (emitters.ContainsKey(player))
				{
					FP.Tweener.AddTween(new Alarm(1, () => {
						(player.Graphic as Graphiclist).Remove(emitter);
	              	}, ONESHOT));
				};
			}
			
			owner.SetUpgrade(null);
		}
	}
}
