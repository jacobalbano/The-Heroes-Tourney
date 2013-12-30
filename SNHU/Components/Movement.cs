
using System;
using Punk;
using Punk.Utils;
using SNHU.GameObject;

namespace SNHU.Components
{
	/// <summary>
	/// Description of Movement.
	/// </summary>
	public class Movement : Logic
	{
		PhysicsBody physics;
		Axis axis;
		
		float Speed;
		
		public const string SPEED = "movement_speed";
		
		public Movement(PhysicsBody physics, Axis axis)
		{
			this.physics = physics;
			this.axis = axis;
			
			Speed = Player.SPEED;
			AddResponse(SPEED, onChangeSpeed);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Math.Abs(physics.MoveDelta.X) < Speed)
			{
				var delta = FP.Sign(physics.MoveDelta.X);
				var ax = FP.Sign(axis.X);
				
				if (delta == 0 || delta == ax)
				{
					Parent.OnMessage(PhysicsBody.IMPULSE, axis.X * Speed, 0, true);
				}
				else
				{
					Parent.OnMessage(PhysicsBody.IMPULSE, axis.X * (Speed / 3f), 0, true);
				}
			}
		}
		
		private void onChangeSpeed(params object[] args)
		{
			Speed = Convert.ToSingle(args[0]);
		}
	}
}
