
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
		public bool RecentlyDodged { get; private set;}
		
		public const string START_DODGE = "dodge_dodged";
		public const string DODGE_COMPLETE = "dodge_complete";
		public const string CANCEL_DODGE = "dodge_cancelDodge";
		public const string CANCEL_COOLDOWN = "dodge_cancelCooldown";
		
		public int DodgeDuration { get; private set; } //= 5;
		
		int duration;
		int cooldown;
		float facing;
		Vector2f dodgeDirection;
		Controller controller;
		Axis axis;
		
		public DodgeController(Controller controller, Axis axis)
		{
			facing = 0;
			CanDodge = true;
			dodgeDirection = new Vector2f();
			this.controller = controller;
			this.axis = axis;
			
			AddResponse(Fist.PUNCH_CONNECTED, SetCooldown);
			AddResponse(Player.OnLand, OnPlayerLand);
			
			AddResponse(CANCEL_DODGE, OnCancelDodge);
			AddResponse(CANCEL_COOLDOWN, OnCancelCooldown);
		}
		
		public override void Added()
		{
			base.Added();
			
			DodgeDuration = int.Parse(GameWorld.gameManager.Config["Player", "DodgeDuration"]);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (axis.X != 0)
			{
				facing = FP.Sign(axis.X);
			}
			
			if (cooldown <= 0 && CanDodge && controller.Pressed("dodge"))
			{
				if (axis.X != 0 && facing != 0)
				{
					Parent.OnMessage(PhysicsBody.USE_GRAVITY, false);
					Parent.OnMessage(START_DODGE);
					
					CanDodge = false;
					IsDodging = true;
					
					duration = DodgeDuration;
					SetCooldown();
					dodgeDirection.X = (int) Math.Round(1f * FP.Sign(facing)) * 0.7f;
					dodgeDirection.Y = (int) Math.Round(1f * FP.Sign(axis.Y)) * 0.3f;
					dodgeDirection = dodgeDirection.Normalized(30);
				}
			}
			
			if (duration --> 0)
			{
				Parent.OnMessage(PhysicsBody.IMPULSE, dodgeDirection.X, dodgeDirection.Y, true);
				
				if (duration == 0)
				{
					Stop();
				}
			}
			
			
			if (cooldown > 0)
			{
				cooldown--;
			}
		}
		
		void Stop()
		{
			duration = 0;
			RecentlyDodged = true;
			OnCancelDodge();
			Parent.OnMessage(DODGE_COMPLETE);
		}
		
		void OnCancelDodge(params object[] args)
		{
			IsDodging = false;
			duration = 0;
			Parent.OnMessage(PhysicsBody.USE_GRAVITY, true);
		}
		
		void SetCooldown(params object[] args)
		{
			cooldown = DodgeDuration + 30;
		}
		
		void OnCancelCooldown(params object[] args)
		{
			cooldown = 0;
		}
		
		void OnPlayerLand(params object[] args)
		{
			RecentlyDodged = false;
			CanDodge = true;
		}
	}
}
