
using System;
using Indigo;
using System.Xml.Serialization;
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
		public PlayerConfig PlayerConfig { get; set; }

		private int duration;
		private int cooldown;
		private float facing;
		private Input Button;
		private Directional axis;

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

			PlayerConfig = Library.Get<PlayerConfig>("config/player.ini");

			

			DodgeDuration = .DodgeDuration;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			if (axis.X != 0)
			{
				facing = Math.Sign(axis.X);
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
					dodgeDirection = Math.Sign(facing) * 0.7f;
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
