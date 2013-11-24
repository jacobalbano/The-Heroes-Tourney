/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 5:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of FUS.
	/// </summary>
	public class FUS : Upgrade
	{
		public const string BE_FUS = "fusYou";
		const float FUS_STRENGTH = 32.0f;
			
		public FUS()
		{
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				if (Parent.World != null)
				{
					Parent.World.BroadcastMessage(BE_FUS, FUS_STRENGTH, Parent.X, Parent.Y);
					Parent.World.BroadcastMessage(GameManager.SHAKE, 60.0f, 0.5f);
					(Parent as Player).SetUpgrade(null);
					Parent.RemoveLogic(this);
				}
			}
		}
	}
}
