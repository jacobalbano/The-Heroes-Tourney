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
			
			emitter = new Emitter(Library.GetTexture("assets/superSpeed.png"), 10, 3);
			emitter.Relative = false;
			
			for (int i = 0; i < 4; ++i)
			{
				var name = i.ToString();
				emitter.NewType(name, FP.Frames(i));
				
				emitter.SetMotion(name, 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
				emitter.SetAlpha(name, 1, 0, Ease.QuintOut);
			}
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				(Parent as Player).Speed = SUPER_SPEED;
				Parent.AddGraphic(emitter);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (lifeTimer.Percent < 1.0f)
			{
				//for (int i = 0; i < 4; ++i)
				//{
					emitter.Emit(FP.Choose("0", "1", "2", "3"), Parent.Left, Parent.Top + FP.Rand(Parent.Height));
				//}
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			(Parent as Player).Speed = Player.SPEED;
			
			(Parent as Player).SetUpgrade(null);
			Parent.RemoveLogic(this);
		}
	}
}
