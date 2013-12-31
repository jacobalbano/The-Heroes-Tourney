
using System;
using Punk;
using Punk.Utils;
using SFML.Window;
using SNHU.GameObject;

namespace SNHU.Components
{
	/// <summary>
	/// Description of Dodge.
	/// </summary>
	public class DodgeController : Logic
	{
		public bool CanDodge;
		public bool IsDodging { get; private set;}
		
		public const string CANCEL_DODGE = "dodge_cancelDodge";
		
		int dodgeDuration;
		Vector2f dodgeDirection;
		Controller controller;
		Axis axis;
		
		public DodgeController(Controller controller, Axis axis)
		{
			CanDodge = true;
			dodgeDirection = new Vector2f();
			this.controller = controller;
			this.axis = axis;
			
			AddResponse(CANCEL_DODGE, OnCancelDodge);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (CanDodge && controller.Pressed("dodge"))
			{
				if (axis.X != 0)
				{
					Parent.OnMessage(PhysicsBody.USE_GRAVITY, false);
					
					CanDodge = false;
					IsDodging = true;
					
					dodgeDuration = 5;
					dodgeDirection.X = (int) Math.Round(axis.X) * 0.7f;
					dodgeDirection.Y = (int) Math.Round(axis.Y) * 0.3f;
					dodgeDirection = dodgeDirection.Normalized(30);
				}
				
//				if (axis.Y != 0) dodgeDirection.X *= 0.7f;
			}
			
			if (dodgeDuration --> 0)
			{
				Parent.OnMessage(PhysicsBody.IMPULSE, dodgeDirection.X, dodgeDirection.Y, true);
				
				if (dodgeDuration == 0)
				{
					OnCancelDodge();
				}
			}
		}
		
		void OnCancelDodge(params object[] args)
		{
			IsDodging = false;
			dodgeDuration = 0;
			Parent.OnMessage(PhysicsBody.USE_GRAVITY, true);
		}
	}
}
