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
			myImage = Image.CreateRect(16, 16, FP.Color(0x00FF00));
			razorArm = Image.CreateRect(uint.Parse(node.Attributes["distance"].Value) * 32, 8, FP.Color(0xFFFFFF));
			theRazor = Image.CreateCircle(uint.Parse(node.Attributes["size"].Value), FP.Color(0xFF0000));
			
			theRazor.CenterOO();
			myImage.CenterOO();
			razorArm.OriginY = razorArm.Height /2;
			
			razorArm.X = myImage.X;
			razorArm.Y = myImage.Y;
			
			theRazor.X += razorArm.Width;

			speed = uint.Parse(node.Attributes["speed"].Value);
			rotation = FP.Rand(360);
			
			AddGraphic(razorArm);
			AddGraphic(theRazor);
			AddGraphic(myImage);
			SetHitbox((int)(myImage.Width), (int)(myImage.Height), (int)(myImage.X + myImage.Width/2), (int)(myImage.Y + myImage.Height/2));	
		}
		
		public override void Update()
		{
			base.Update();
			rotation += speed;
			FP.RotateAround(ref theRazor.X, ref theRazor.Y, myImage.X, myImage.Y, rotation, false);
			razorArm.Angle = rotation;
			
			
		}
	}
}
