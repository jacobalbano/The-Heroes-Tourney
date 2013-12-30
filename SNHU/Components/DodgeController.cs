
using System;
using Punk;
using Punk.Utils;

namespace SNHU.Components
{
	/// <summary>
	/// Description of Dodge.
	/// </summary>
	public class DodgeController : Logic
	{
		Controller controller;
		public DodgeController(Controller controller)
		{
			this.controller = controller;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (controller.Pressed("dodge"))
			{
				FP.Log("doge");
			}
		}
	}
}
