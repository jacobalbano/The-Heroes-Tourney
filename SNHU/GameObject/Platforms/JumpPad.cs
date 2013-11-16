/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/15/2013
 * Time: 10:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Platforms
{
	public class JumpPad : Platform
	{
		public JumpPad() : base()
		{
			
		}
		
		public override void OnLand(Player playerTarget)
		{
			playerTarget.OnMessage(PhysicsBody.IMPULSE, Player.JumpForce * 2);
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint width = uint.Parse(node.Attributes["width"].Value);
			//uint height = uint.Parse(node.Attributes["height"].Value);
			
			Graphic = myImage = Image.CreateRect(width, 5, FP.Color(0x000000));
			myImage.Y -= 5;
			SetHitboxTo(Graphic);
		}
	}
}
