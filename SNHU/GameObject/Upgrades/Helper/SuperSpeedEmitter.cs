
using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades.Helper
{
	/// <summary>
	/// Description of SuperSpeedEmitter.
	/// </summary>
	public class SuperSpeedEmitter : Component
	{
		private Emitter emitter;
		private float lastX;
		
		public SuperSpeedEmitter(float duration)
		{
			emitter = new Emitter(Library.GetTexture("assets/speed_particle.png"), 68, 30);
			emitter.Relative = false;
			
			emitter.NewType("l", FP.Frames(0));
			emitter.SetMotion("l", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
			emitter.SetAlpha("l", 0.5f, 0, Ease.QuintOut);
			
			emitter.NewType("r", FP.Frames(1));
			emitter.SetMotion("r", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
			emitter.SetAlpha("r", 0.5f, 0, Ease.QuintOut);
			
			FP.Tweener.Timer(duration)
				.OnComplete(LifetimeComplete);
		}
		
		public override void Added()
		{
			base.Added();
			
			Parent.AddComponent(emitter);
			lastX = Parent.X;
		}
		
		public override void Update()
		{
			base.Update();
			
			var delta = FP.Sign(Parent.X - lastX);
			lastX = Parent.X;
			if (delta < 0)
			{
				emitter.Emit("l", Parent.Left - 38, Parent.Top + FP.Random.Int(Parent.Height));
			}
			else if (delta > 0)
			{
				emitter.Emit("r", Parent.Left, Parent.Top + FP.Random.Int(Parent.Height));
			}
		}
		
		private void LifetimeComplete()
		{
			Parent.OnMessage(Movement.Message.Speed, Player.SPEED);
			Parent.RemoveComponent(this);
			
			var parent = Parent;	//	gg ty
			FP.Tweener.Timer(1).OnComplete(() => parent.RemoveComponent(emitter));
		}
	}
}
