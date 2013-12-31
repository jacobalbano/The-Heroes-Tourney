
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
		
		private const int DODGE_DURATION = 5;
		
		int duration;
		int cooldown;
		Vector2f dodgeDirection;
		Controller controller;
		Axis axis;
		
		public DodgeController(Controller controller, Axis axis)
		{
			CanDodge = true;
			dodgeDirection = new Vector2f();
			this.controller = controller;
			this.axis = axis;
			
			AddResponse(Player.OnLand, OnPlayerLand);
			AddResponse(CANCEL_DODGE, OnCancelDodge);
			AddResponse(Fist.PUNCH_CONNECTED, SetCooldown);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (cooldown <= 0 && CanDodge && controller.Pressed("dodge"))
			{
				if (axis.X != 0)
				{
					Parent.OnMessage(PhysicsBody.USE_GRAVITY, false);
					
					CanDodge = false;
					IsDodging = true;
					
					duration = DODGE_DURATION;
					SetCooldown();
					dodgeDirection.X = (int) Math.Round(axis.X) * 0.7f;
					dodgeDirection.Y = (int) Math.Round(axis.Y) * 0.3f;
					dodgeDirection = dodgeDirection.Normalized(30);
				}
			}
			
			if (duration --> 0)
			{
				Parent.OnMessage(PhysicsBody.IMPULSE, dodgeDirection.X, dodgeDirection.Y, true);
				
				if (duration == 0)
				{
					OnCancelDodge();
				}
			}
			
			if (cooldown > 0)
			{
				cooldown--;
			}
		}
		
		void OnCancelDodge(params object[] args)
		{
			IsDodging = false;
			duration = 0;
			Parent.OnMessage(PhysicsBody.USE_GRAVITY, true);
		}
		
		void SetCooldown(params object[] args)
		{
			cooldown = DODGE_DURATION + 30;
		}
		
		void OnPlayerLand(params object[] args)
		{
			CanDodge = true;
		}
	}
}
