
using System;
using Punk;
using SNHU.GameObject;

namespace SNHU.Components
{
	/// <summary>
	/// Description of GrabLock.
	/// </summary>
	public class GrabLock : Logic
	{
		const int MAX_BUFFER = 5;
		const int MAX_COUNTDOWN = 12;
		const int WINDOW = MAX_COUNTDOWN / 2;
		
		private DodgeController dodge;
		private int lastPunch, lastDodge, countdown;
		
		private Player target;
		
		public GrabLock(DodgeController dodge)
		{
			this.dodge = dodge;
			
			AddResponse(Fist.PUNCH_SUCCESS, OnPunch);
			AddResponse(DodgeController.START_DODGE, OnDodge);
			AddResponse(Player.OnLand, OnLand);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (countdown > 0)
			{
				countdown--;
				
				if (countdown < WINDOW && countdown != 0)
				{
					InComboWindow();
				}
				else if (countdown == 0)
				{
					ExitComboWindow();
				}
//				else if (countdown > WINDOW)
//				{
//					PreComboWindow();
//				}
			}
			
			lastDodge--;
			lastPunch--;
		}
		
//		void PreComboWindow()
//		{
//			if (lastPunch == MAX_BUFFER)
//			{
//				FP.Log("cancel");
//			}
//		}
		
		void InComboWindow()
		{
			if (target != null)
			{
				if (Math.Abs(lastDodge - lastPunch) > 5)
				{
					countdown = 0;
					target.OnMessage(PhysicsBody.ACTIVATE);
					target = null;
					ExitComboWindow();
					return;
				}
				
				Parent.MoveTowards(Parent.X, target.Y, 5, Parent.Type);
				target.OnMessage(PhysicsBody.DEACTIVATE);
			}
			
			Parent.OnMessage(PhysicsBody.DEACTIVATE);
			Parent.OnMessage(DodgeController.CANCEL_COOLDOWN);
			dodge.CanDodge = true;
		}
		
		void ExitComboWindow()
		{
			countdown = 0;
			if (target != null)
			{
				target.OnMessage(PhysicsBody.CANCEL);
				target.OnMessage(PhysicsBody.ACTIVATE);
			}
			
			Parent.OnMessage(PhysicsBody.ACTIVATE);
			Parent.OnMessage(PhysicsBody.CANCEL);
		}
		
		private void OnPunch(params object[] args)
		{
			if (target != null)
			{
				target.OnMessage(PhysicsBody.ACTIVATE);
			}
			
			target = args[0] as Player;
			lastPunch = MAX_BUFFER;
			CheckStatus();
		}
		
		private void OnDodge(params object[] args)
		{
			lastDodge = MAX_BUFFER;
			CheckStatus();
		}
		
		private void OnLand(params object[] args)
		{
			target = null;
		}
		
		private void CheckStatus()
		{
			if (lastPunch > 0 && lastDodge > 0)
			{
				countdown = MAX_COUNTDOWN;
			}
		}
	}
}
