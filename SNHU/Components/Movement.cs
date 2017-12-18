
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
	public class HtMovement : Component
	{
		PhysicsBody physics;
		Directional axis;
		
		public float Speed { get; private set; }
		
		public enum Message { Speed }
		
		public HtMovement(PhysicsBody physics, Directional axis)
		{
			this.physics = physics;
			this.axis = axis;
			
			Speed = Player.SPEED;
			AddResponse(Message.Speed, onChangeSpeed);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			if (Math.Abs(physics.MoveDelta.X) < Speed)
			{
				var delta = Math.Sign(physics.MoveDelta.X);
				var ax = Math.Sign(axis.X);
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
