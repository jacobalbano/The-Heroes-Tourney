/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 10:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Utils;

namespace SNHU.Components
{
	/// <summary>
	/// Description of CheckRestart.
	/// </summary>
	public class CheckRestart : Logic
	{
		private Controller controller;
		
		public CheckRestart(Controller controller)
		{
			this.controller = controller;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (controller.Pressed(Controller.Button.Back))
			{
//				FP.World = new GameWorld();
			}
		}
	}
}
