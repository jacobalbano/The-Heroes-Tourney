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
using Punk.Tweens.Misc;

namespace SNHU.GameObject
{
	public class RazorBlade : Entity
	{
		public Image blade;
		
		public RazorBlade()
		{
			blade = new Image(Library.GetTexture("assets/razor.png"));
			blade.Scale = FP.Random + 1f;
			blade.CenterOO();
			SetHitboxTo(blade);
			CenterOrigin();
			
			Graphic = blade;
		}
		
		public override void Update()
		{
			base.Update();
			blade.Angle += 15;
		}
	}
	
	public class Razor : Entity
	{
		public Image myImage;
		
		//How long the razor will be
		private float rotation;
		private float speed;
		
		public Image razorArm;
		
		private RazorBlade theRazor;
		
		
		public Razor()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			myImage = new Image(Library.GetTexture("assets/pivot.png"));
			razorArm = new Image(Library.GetTexture("assets/razorArm.png"));
			
			razorArm.ScaleX = float.Parse(node.Attributes["distance"].Value) * 32 / razorArm.Width;
			
			myImage.CenterOO();
			razorArm.OriginY = razorArm.Height /2;
			
			theRazor = new RazorBlade();
			theRazor.X = X;
			theRazor.Y = Y;
			
			theRazor.X += razorArm.ScaledWidth;

			speed = uint.Parse(node.Attributes["speed"].Value);
			rotation = FP.Rand(360);
			
			speed *= 0.5f;
			
			AddGraphic(razorArm);
			AddGraphic(myImage);
			SetHitbox((int)(myImage.Width), (int)(myImage.Height), (int)(myImage.X + myImage.Width/2), (int)(myImage.Y + myImage.Height/2));	
		}
		
		public override void Added()
		{
			base.Added();
			World.Add(theRazor);
		}
		
		public override void Removed()
		{
			base.Removed();
			World.Remove(theRazor);
		}
		
		public override void Update()
		{
			base.Update();
			rotation += speed;
			FP.AnchorTo(ref theRazor.X, ref theRazor.Y, X, Y, razorArm.ScaledWidth);
			FP.RotateAround(ref theRazor.X, ref theRazor.Y, X, Y, rotation, false);
			razorArm.Angle = rotation;
		}
	}
}
