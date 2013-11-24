/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 4:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of SuperSpeed.
	/// </summary>
	public class SuperSpeed : Upgrade
	{
		public const float SUPER_SPEED = 12.0f;
		
		public SuperSpeed()
		{
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				(Parent as Player).Speed = SUPER_SPEED;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			(Parent as Player).Speed = Player.SPEED;
			
			(Parent as Player).SetUpgrade(null);
			Parent.RemoveLogic(this);
		}
	}
}
