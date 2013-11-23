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
using Punk.Tweens.Misc;
using Punk.Utils;
using SNHU.Components;

namespace SNHU.GameObject.Platforms
{
	public class JumpPad : Entity
	{
		private Nineslice image;
		private Emitter emitter;
		
		public JumpPad() : base()
		{
			Type = "";
			AddTween(new Alarm(0.5f, () => emitter.Emit("p", FP.Rand((uint) Width), -5), LOOPING), true);
		}
		
		public override void Update()
		{
			base.Update();
			var e = Collide(Player.Collision, X, Y - 1);
			
			if(e != null)
			{
				for (int i = 0; i < 5; ++i)
				{
					emitter.Emit("p", FP.Rand((uint) Width), -5);
				}
				
				e.OnMessage(PhysicsBody.IMPULSE, 0, Player.JumpForce * 1.4, true);
				Mixer.Audio["jumpPad"].Play();
			}
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			float width = float.Parse(node.Attributes["width"].Value);
			width = FP.Clamp(width, 15, 640);	//	lol game jam
			
			image = new Nineslice(Library.GetTexture("assets/bouncepad.png"), 3, 3);
			image.Columns = (uint) (width / 5f);
			image.ScaleX = width / image.Width;
			image.Y -= 3;
			
			emitter = new Emitter(Library.GetTexture("assets/jumpparticle.png"));
			emitter.NewType("p");
			emitter.SetMotion("p", 90, 32, 1, 45, 0.5f, 0.25f);
			emitter.SetAlpha("p", 1, 0);
			
			Width = (int) width;
			Height = 3;
			
			AddGraphic(emitter);
			AddGraphic(image);
		}
	}
}
