/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 8/23/2014
 * Time: 2:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Indigo;
using Indigo.Graphics;
using SNHU.GameObject.Platforms;
using Indigo.Loaders;

namespace SNHU.GameObject.Triggers
{
	public class Triggerable : Entity
	{
		public string Group { get; set; }
		
		public Triggerable(string group = "default")
		{
			Group = group;
			Type = Platform.Collision;
			
			AddResponse(Trigger.Message.On, OnTriggerOn);
			AddResponse(Trigger.Message.Off, OnTriggerOff);
		}
		
		protected virtual void TriggerOn()
		{
		}
		
		protected virtual void TriggerOff()
		{
		}
		
		private void OnTriggerOn(params object[] args)
        {
			if (String.Equals(args[0] as string, Group))
			{
				FP.Log("I was triggered on. Group: " + Group);
				TriggerOn();
			}
        }
		
		private void OnTriggerOff(params object[] args)
        {
			if (String.Equals(args[0] as string, Group))
			{
				FP.Log("I was triggered off. Group: " + Group);
				TriggerOff();
			}
        }
		
	}
}
