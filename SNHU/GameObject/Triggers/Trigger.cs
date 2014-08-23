/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 8/23/2014
 * Time: 2:52 PM
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
	/// Description of Trigger.
	/// </summary>
	[OgmoConstructor("group")]
	public class Trigger : Entity
	{
		public string Group { get; set; }
		
		public enum Message
		{
			On,
			Off
		}
		
		public Trigger(string group = "default")
		{
			Group = group;
			Type = Platform.Collision;
			
			var img = Image.CreateRect(32, 32, FP.Color(0xFFFFFF));
			SetHitboxTo(img);
			AddComponent(img);
		}
	}
}
