
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
		const int MAX_COUNTDOWN = 20;
		const int WINDOW = MAX_COUNTDOWN / 2;
		
		private DodgeController dodge;
		private int lastPunch, lastDodge, countdown;
		
		private Player target;
		
		public GrabLock(DodgeController dodge)
		{
			this.dodge = dodge;
			
			AddResponse(Fist.PUNCH_SUCCESS, OnPunch);
			AddResponse(DodgeController.START_DODGE, OnDodge);
//			AddResponse(DodgeController.DODGE_COMPLETE, OnDodgeComplete);
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
					if (target != null)
					{
						Parent.MoveTowards(target.X, target.Y, 5, target.Type);
						target.OnMessage(PhysicsBody.DEACTIVATE);
					}
					Parent.OnMessage(PhysicsBody.DEACTIVATE);
					(Parent as Player).dodge.CanDodge = true;
					Parent.OnMessage(DodgeController.CANCEL_COOLDOWN);
				}
				else
				{
					if (countdown == 0)
					{
						if (target != null)
						{
							target.OnMessage(PhysicsBody.CANCEL);
							target.OnMessage(PhysicsBody.ACTIVATE);
						}
						
						Parent.OnMessage(PhysicsBody.ACTIVATE);
						Parent.OnMessage(PhysicsBody.CANCEL);
					}
				}
			}
			
			lastDodge--;
			lastPunch--;
		}
		
		private void OnPunch(params object[] args)
		{
			target = args[0] as Player;
			lastPunch = 5;
			CheckStatus();
		}
		
		private void OnDodge(params object[] args)
		{
			lastDodge = 5;
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
