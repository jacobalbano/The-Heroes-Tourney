using System;
using Indigo;

namespace SNHU.Components
{
	/// <summary>
	/// Freezes a player for a fraction of a second after a successful punch;
	/// </summary>
	public class HitFreeze : Component
	{
		private const int MaxFrameTimer = 6;
		
		private PhysicsBody physics;
		private float x, y;
		private int frameTimer;
		
		public HitFreeze(float x, float y)
		{
			this.x = x;
			this.y = y;
			frameTimer = MaxFrameTimer;
		}
		
		public override void Added()
		{
			base.Added();
			
			physics = Parent.GetComponent<PhysicsBody>();
			physics.OnMessage(PhysicsBody.Message.Cancel);
			physics.Active = false;
		}
		
		public override void Removed()
		{
			base.Removed();
			physics.Active = true;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (frameTimer --> 0)
			{
				Parent.X = x;
				Parent.Y = y;
				return;
			}
			
			Parent.RemoveComponent(this);
		}
	}
}
