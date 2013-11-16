/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/16/2013
 * Time: 12:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;

namespace SNHU.GameObject
{
	public class Razor : Entity
	{
		public Image myImage;
		
		//How long the razor will be
		private float rotation;
		private float speed;
		
		public Image razorArm;
		public Image theRazor;
		
		
		public Razor()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			razorArm = Image.CreateRect(uint.Parse(node.Attributes["distance"].Value) * 32, 10, FP.Color(0x333333));
			razorArm.OriginX = 5;
			razorArm.OriginY = 5;
			theRazor = Image.CreateCircle(uint.Parse(node.Attributes["size"].Value) * 20, FP.Color(0xFF0000));
			speed = 0.5f;//uint.Parse(node.Attributes["speed"].Value);
			rotation = FP.Rand(360);
			Graphic = myImage = Image.CreateRect(16, 16, FP.Color(0x00FF00));
			AddGraphic(theRazor);
			
			SetHitboxTo(myImage);
		}
		
		public override void Update()
		{
			base.Update();
			rotation += speed;
			FP.RotateAround(ref theRazor.X, ref theRazor.Y, this.X, this.Y, rotation, false);
			
		}
	}
}
