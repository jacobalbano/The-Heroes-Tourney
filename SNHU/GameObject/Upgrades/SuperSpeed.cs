/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 4:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Utils;
//using SFML.Graphics;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of SuperSpeed.
	/// </summary>
	public class SuperSpeed : Upgrade
	{
		public const float SUPER_SPEED = 12.0f;
		public Emitter emitter;
		
		public SuperSpeed()
		{
			//image = new Image(Library.GetTexture("assets/" +  + ".png"));
			image = Image.CreateCircle(3, FP.Color(0xFF0000));
			
			emitter = new Emitter(Library.GetTexture("assets/speed_particle.png"), 68, 30);
			emitter.Relative = false;
			
			emitter.NewType("l", FP.Frames(0));
			emitter.SetMotion("l", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
			emitter.SetAlpha("l", 0.5f, 0, Ease.QuintOut);
			
			emitter.NewType("r", FP.Frames(1));
			emitter.SetMotion("r", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
			emitter.SetAlpha("r", 0.5f, 0, Ease.QuintOut);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				owner.Speed = SUPER_SPEED;
				Parent.AddGraphic(emitter);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!Activated)	return;
			
			if (lifeTimer.Percent < 1.0f)
			{
				var delta = owner.physics.MoveDelta.X;
				if (delta < 0)
				{
					emitter.Emit("l", Parent.Left - 38, Parent.Top + FP.Rand(Parent.Height));
				}
				else if (delta > 0)
				{
					emitter.Emit("r", Parent.Left, Parent.Top + FP.Rand(Parent.Height));
				}
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			owner.Speed = Player.SPEED;
			
			owner.SetUpgrade(null);
		}
	}
}
