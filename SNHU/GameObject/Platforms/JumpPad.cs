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
		public const int POINT_REWARD = 5;
		
		public JumpPad() : base()
		{
			Type = "";
		}
		
		public override void OnLand(Player playerTarget)
		{
		}
		
		public override void Update()
		{
			base.Update();
			var e = Collide(Player.Collision, X, Y - 1);
			
			if(e != null)
			{
				e.OnMessage(PhysicsBody.IMPULSE, 0, Player.JumpForce * 1.5);
				(e as Player).Points += POINT_REWARD;
				Mixer.Audio["jumpPad"].Play();
			}
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint width = uint.Parse(node.Attributes["width"].Value);
			
			Graphic = myImage = Image.CreateRect(width, 3, FP.Color(0x000000));
			myImage.Y -= 3;
			SetHitboxTo(Graphic);
		}
	}
}
