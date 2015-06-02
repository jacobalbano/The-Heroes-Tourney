using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Components;
using SNHU.Config;
using SNHU.Config.Upgrades;
using SNHU.Systems;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of GroundSmash.
	/// </summary>
	public class GroundSmash : Upgrade
	{
		public float SmashRadius { get; private set; }
		public float FallSpeed { get; private set; }
		
		public GroundSmash()
		{
			Icon = new Image(Library.GetTexture("groundsmash.png"));
			AddResponse(Player.Message.OnLand, OnPlayerLand);
		}
		
		public override void Added()
		{
			base.Added();
			
			var config = Library.GetConfig<GroundSmashConfig>("config/upgrades/groundSmash.ini");
			SmashRadius = config.SmashRadius;
			FallSpeed = config.FallSpeed;
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				if (to == from) return; // sender;
				
				if (to.CollideRect(to.X, to.Y, from.X - SmashRadius, from.Y - 10, SmashRadius * 2, 10))
				{
					to.OnMessage(Player.Message.Damage);
				}
			};
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated && !owner.OnGround)
			{
				base.Use();
				
				owner.OnMessage(PhysicsBody.Message.UseGravity, false);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Activated)
			{
				Parent.OnMessage(PhysicsBody.Message.Impulse, 0, FallSpeed, true);
			}
		}
		
		public void OnPlayerLand(params object[] args)
		{
			if (Activated)
			{
				var emitter = new Emitter(Library.GetTexture("groundsmash_particle.png"), 3, 3);
				emitter.Relative = false;
				
				for (int i = 0; i < 4; i++)
				{
					var name = i.ToString();
					emitter.NewType(name, FP.Frames(i));
					emitter.SetAlpha(name, 0, 1f);
					emitter.SetMotion(name, -90, 100, 0.5f, 10, 10, 0.1f, Ease.SineOut);
					emitter.SetGravity(name, 5, 2);
				}
				
				var emitterEnt = Parent.World.AddGraphic(emitter, 0, 0, ObjectLayers.JustAbove(ObjectLayers.Players));
				emitterEnt.Active = true;
				emitterEnt.Tweener.Timer(3)
					.OnComplete(() => FP.World.Remove(emitterEnt));
				
				for (float i = -SmashRadius; i < SmashRadius; i++)
				{
					var c = FP.Choose.Character("0123");
					emitter.Emit(c, Parent.X + i + FP.Random.Float() - FP.Random.Float(), Parent.Y - FP.Random.Int(10));
				}
				
				Parent.World.BroadcastMessage(CameraManager.Message.Shake, 100.0f, 0.5f);
				Parent.World.BroadcastMessage(EffectMessage.Message.OnEffect, MakeEffect());
				
				owner.OnMessage(PhysicsBody.Message.UseGravity, true);
				
				owner.SetUpgrade(null);
			}
		}
	}
}
