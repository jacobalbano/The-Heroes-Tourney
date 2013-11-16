/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/15/2013
 * Time: 10:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SNHU.Components;

namespace SNHU.GameObject.Platforms
{
	/// <summary>
	/// Description of JumpPad.
	/// </summary>
	public class JumpPad : Platform
	{
		public JumpPad() : base()
		{
			
		}
		
		public override void OnLand(Player playerTarget)
		{
			playerTarget.OnMessage(PhysicsBody.IMPULSE, Player.JumpForce * 2);
		}
	}
}
