﻿using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Loaders;
using Indigo.Utils;
using SNHU.Config;
using SNHU.GameObject.Upgrades;
using SNHU.Systems;

namespace SNHU.GameObject
{
	public class RazorBlade : Entity
	{
		public Image blade;
		public Emitter emitter;
		
		public RazorBlade(float size)
		{
			blade = new Image(Library.GetTexture("razor.png"));
			blade.Scale = size;
			blade.CenterOO();
			SetHitboxTo(blade);
			CenterOrigin();
			
			emitter = new Emitter(Library.GetTexture("blood.png"), 15, 15);
			emitter.Relative = false;
			
			for (int i = 0; i < 4; ++i)
			{
				var name = i.ToString();
				emitter.NewType(name, FP.Frames(i));
				
				emitter.SetGravity(name, 10, 10);
				emitter.SetMotion(name, 0, 50, 0.5f, 360, 15, 1, Ease.QuintOut);
				emitter.SetAlpha(name, 1, 0, Ease.QuintOut);
			}
			
			AddComponent(blade);
			AddComponent(emitter);
		}
		
		public override void Update()
		{
			base.Update();
			blade.Angle += 15;
			
			var p = Collide(Player.Collision, X, Y) as Player;
			if (p != null)
			{
				for (int i = 0; i < 150; ++i)
				{
					var randX = FP.Random.Int(Width) - HalfWidth;
					var randY = FP.Random.Int(Height) - HalfHeight;
					emitter.Emit(FP.Choose.Character("0123"), X + randX, Y - 50 + randY);
				}
				
				p.OnMessage(EffectMessage.Message.OnEffect, MakeEffect(p));
			}
		}
		
		EffectMessage MakeEffect(Entity p)
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				p.OnMessage(Player.Message.Damage);
				Mixer.SawHit.Play();
			};
			
			return new EffectMessage(this, callback);
		}
	}
	
	public class Razor : Entity, IOgmoNodeHandler
	{
		public Image myImage;
		
		//How long the razor will be
		private float rotation;
		private float speed;
		private float size = 0;
		private float distance = 0;
		
		public Image razorArm;
		private RazorBlade theRazor;
		
		public Razor()
		{
		}
		
		public void NodeHandler(System.Xml.XmlNode entity)
		{
			myImage = new Image(Library.GetTexture("pivot.png"));
			razorArm = new Image(Library.GetTexture("razorArm.png"));
			
			razorArm.ScaleX = distance * 32 / razorArm.Width;
			
			myImage.CenterOO();
			razorArm.OriginY = razorArm.Height /2;
			
			theRazor = new RazorBlade(size);
			theRazor.X = X;
			theRazor.Y = Y;
			
			Layer = ObjectLayers.JustAbove(ObjectLayers.Players);
			theRazor.Layer = ObjectLayers.JustAbove(Layer);
			
			theRazor.X += razorArm.ScaledWidth;

			rotation = -90;
			
			speed *= 0.5f;
			
			AddComponent(razorArm);
			AddComponent(myImage);
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
