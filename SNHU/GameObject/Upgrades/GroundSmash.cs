using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Components;
using SNHU.Config;
using SNHU.Config.Upgrades;
using SNHU.GameObject.Platforms;
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
			if (!Activated && owner.Collide(Platform.Collision, owner.X, owner.Y + 1) == null)	//	TODO: fix me
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
				for (float i = -SmashRadius; i < SmashRadius; i++)
				{
					var x = Parent.X + i + FP.Random.Float() - FP.Random.Float();
					var y = Parent.Y - FP.Random.Int(10);
					var type = FP.Choose.Character("0123");
					Parent.World.BroadcastMessage(GlobalEmitter.Message.GroundSmash, type, x, y);
				}
				
				Parent.World.BroadcastMessage(CameraManager.Message.Shake, 100.0f, 0.5f);
				Parent.World.BroadcastMessage(EffectMessage.Message.OnEffect, MakeEffect());
				
				owner.OnMessage(PhysicsBody.Message.UseGravity, true);
				
				owner.SetUpgrade(null);
			}
		}
	}
}
