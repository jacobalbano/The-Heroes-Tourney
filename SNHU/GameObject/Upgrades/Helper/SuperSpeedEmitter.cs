
using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using SNHU.Components;
using SNHU.Systems;

namespace SNHU.GameObject.Upgrades.Helper
{
	public class SuperSpeedEmitter : Component
	{
		private float lastX, Lifetime;
		
		public SuperSpeedEmitter(float lifetime)
		{
			Lifetime = lifetime;
		}
		
		public override void Update()
		{
			base.Update();
			
			Lifetime -= FP.Elapsed;
			if (Lifetime <= 0)
				Parent.RemoveComponent(this);
			
			var delta = FP.Sign(Parent.X - lastX);
			if (delta != 0)
			{
				var x = Parent.X;
				var y = Parent.Top + FP.Random.Int(Parent.Height);
				var type = delta < 0 ? "l" : "r";
				
				Parent.World.BroadcastMessage(GlobalEmitter.Message.SuperSpeed, type, x, y);
			}
			
			lastX = Parent.X;
		}
	}
}
