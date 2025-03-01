﻿
using System;
using Indigo;
using Indigo.Core;
using Indigo.Inputs;
using Indigo.Utils;
using SNHU.Config;
using SNHU.GameObject;

namespace SNHU.Components
{
	/// <summary>
	/// Description of Dodge.
	/// </summary>
	public class DodgeController : Component
	{
		public bool CanDodge;
		public bool IsDodging { get; private set;}
		public bool RecentlyDodged { get; private set;}
		
		public int DodgeDuration { get; private set; } //= 5;
		
		int duration;
		int cooldown;
		float facing;
		float dodgeDirection;
		Input Button;
		Directional axis;
		
		public DodgeController(Input button, Directional axis)
		{
			facing = 0;
			CanDodge = true;
			Button = button;
			this.axis = axis;
			
			AddResponse(Fist.Message.PunchConnected, SetCooldown);
			AddResponse(Player.Message.OnLand, OnPlayerLand);
		}
		
		public override void Added()
		{
			base.Added();
			
			DodgeDuration = Library.GetConfig<PlayerConfig>("config/player.ini").DodgeDuration;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (axis.X != 0)
			{
				facing = FP.Sign(axis.X);
			}
			
			if (cooldown <= 0 && CanDodge && Button.Pressed)
			{
				if (axis.X != 0 && facing != 0)
				{
					Parent.OnMessage(PhysicsBody.Message.UseGravity, false);
					
					CanDodge = false;
					IsDodging = true;
					
					duration = DodgeDuration;
					SetCooldown();
					dodgeDirection = FP.Sign(facing) * 0.7f;
					dodgeDirection *= 30;
				}
			}
			
			if (duration --> 0)
			{
				Parent.OnMessage(PhysicsBody.Message.Impulse, dodgeDirection, 0, true);
				
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
			IsDodging = false;
			duration = 0;
			Parent.OnMessage(PhysicsBody.Message.UseGravity, true);
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
