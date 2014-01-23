using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of GroundSmash.
	/// </summary>
	public class GroundSmash : Upgrade
	{
		public const float SMASH_RADIUS = 300.0f;
		const float FALL_SPEED = 50.0f;
		
		public GroundSmash()
		{
			Icon = new Image(Library.GetTexture("assets/groundsmash.png"));
			AddResponse(Player.OnLand, OnPlayerLand);
		}
		
		public override EffectMessage MakeEffect()
		{
			FP.Log("create");
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				if (to.CollideRect(to.X, to.Y, from.X - SMASH_RADIUS, from.Y - 10, SMASH_RADIUS * 2, 10))
				{
					to.OnMessage(Player.Damage);
				}
			};
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated && !owner.OnGround)
			{
				base.Use();
				
				owner.physics.OnMessage(PhysicsBody.USE_GRAVITY, false);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Activated)
			{
				Parent.OnMessage(PhysicsBody.IMPULSE, 0, FALL_SPEED, true);
			}
		}
		
		public void OnPlayerLand(params object[] args)
		{
			if (Activated)
			{
				var emitter = new Emitter(Library.GetTexture("assets/groundsmash_particle.png"), 3, 3);
				emitter.Relative = false;
				
				for (int i = 0; i < 4; i++)
				{
					var name = i.ToString();
					emitter.NewType(name, FP.Frames(i));
					emitter.SetAlpha(name, 0, 1f);
					emitter.SetMotion(name, 90, 100, 0.5f, 10, 10, 0.1f, Ease.SineOut);
					emitter.SetGravity(name, 5, 2);
				}
				
				var emitterEnt = Parent.World.AddGraphic(emitter, -9010);
				emitterEnt.AddTween(new Alarm(3, () => FP.World.Remove(emitterEnt), Tween.ONESHOT), true);
				
				for (float i = -SMASH_RADIUS; i < SMASH_RADIUS; i++)
				{
					emitter.Emit(FP.Choose("0", "1", "2", "3"), Parent.X + i + FP.Random - FP.Random, Parent.Y + FP.Rand(10) - 5);
				}
				
				Parent.World.BroadcastMessage(CameraShake.SHAKE, 100.0f, 0.5f);
				Parent.World.BroadcastMessageIf(e => e != owner, EffectMessage.ON_EFFECT, MakeEffect());
				
				owner.physics.OnMessage(PhysicsBody.USE_GRAVITY, true);
				
				owner.SetUpgrade(null);
			}
		}
	}
}
