
using System;
using Indigo;
using Indigo.Inputs;
using Indigo.Utils;
using SNHU.GameObject;

namespace SNHU.Components
{
	/// <summary>
	/// Description of Movement.
	/// </summary>
	public class Movement : Component
	{
		PhysicsBody physics;
		Directional axis;
		
		public float Speed { get; private set; }
		
		public enum Message { Speed }
		
		public Movement(PhysicsBody physics, Directional axis)
		{
			this.physics = physics;
			this.axis = axis;
			
			Speed = Player.SPEED;
			AddResponse(Message.Speed, onChangeSpeed);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Math.Abs(physics.MoveDelta.X) < Speed)
			{
				var delta = FP.Sign(physics.MoveDelta.X);
				var ax = FP.Sign(axis.X);
				float speed = Speed;
				
				if (Parent.Collide(Parent.Type, Parent.X, Parent.Y) != null)
				{
					speed *= 0.5f;
				}
				
				if (delta == 0 || delta == ax)
				{
					Parent.OnMessage(PhysicsBody.Message.Impulse, ax * speed, 0, true);
				}
				else
				{
					Parent.OnMessage(PhysicsBody.Message.Impulse, ax * (speed / 3f), 0, true);
				}
			}
		}
		
		private void onChangeSpeed(params object[] args)
		{
			Speed = Convert.ToSingle(args[0]);
		}
	}
}
