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
	/// <summary>
	/// Description of Triggerable.
	/// </summary>
	[OgmoConstructor("group")]
	public class Triggerable : Entity
	{
		public string Group { get; set; }
		
		public Triggerable(string group = "default")
		{
			Group = group;
			Type = Platform.Collision;
			
			var img = Image.CreateRect(32, 32, FP.Color(0xFF00FF));
			SetHitboxTo(img);
			AddComponent(img);
			
			AddResponse(Trigger.Message.On, OnTriggerOn);
			AddResponse(Trigger.Message.Off, OnTriggerOff);
		}
		
		public void OnTriggerOn(params object[] args)
        {
			FP.Log("I was triggered on. Group: " + Group);
        }
		
		public void OnTriggerOff(params object[] args)
        {
			FP.Log("I was triggered off. Group: " + Group);
        }
		
	}
}
