using System;
using Indigo;
using Indigo.Graphics;
using Indigo.Loaders;
using Indigo.Utils;
using SNHU.Components;
using SNHU.Config;

namespace SNHU.GameObject.Platforms
{
	public class JumpPad : Entity, IOgmoNodeHandler
	{
		private Nineslice image;
		private Emitter emitter;
		
		public const string Collision = "JumpPad";
		
		public JumpPad() : base()
		{
			Type = Collision;
			Tweener.Timer(0.5f)
				.OnComplete(() => emitter.Emit("p", FP.Random.Float(Width), -5))
				.Repeat();
		}
		
		public override void Added()
		{
			base.Added();
			Layer = ObjectLayers.JustAbove(ObjectLayers.Platforms);
		}
		
		public void NodeHandler(System.Xml.XmlNode entity)
		{
			image = new Nineslice(Library.GetTexture("bouncepad.png"), 3, 3);
			image.Columns = (int) (Width / 5f);
			image.ScaleX = Width / image.Width;
			image.Y -= 3;
			
			emitter = new Emitter(Library.GetTexture("jumpparticle.png"));
			emitter.NewType("p");
			emitter.SetMotion("p", 90, 32, 1, 45, 0.5f, 0.25f);
			emitter.SetAlpha("p", 1, 0);
			
			Height = 3;
			
			AddComponent(emitter);
			AddComponent(image);
		}
	}
}
