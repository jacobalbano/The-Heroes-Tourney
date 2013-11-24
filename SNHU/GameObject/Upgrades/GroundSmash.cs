/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 2:50 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of GroundSmash.
	/// </summary>
	public class GroundSmash : Upgrade
	{
		public const string GROUND_SMASH = "groundSmash";
		public const float SMASH_RADIUS = 300.0f;
		const float FALL_SPEED = 50.0f;
		
		public GroundSmash()
		{
			AddResponse(Player.OnLand, OnPlayerLand);
			//image = new Image(Library.GetTexture("assets/" +  + ".png"));
			image = Image.CreateCircle(3, FP.Color(0xFF0000));
		}
		
		public override void Use()
		{
			if (!Activated && !(Parent as Player).OnGround)
			{
				base.Use();
				
				(Parent as Player).physics.OnMessage(PhysicsBody.USE_GRAVITY, false);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Activated)
			{
				Parent.OnMessage(PhysicsBody.IMPULSE, 0, FALL_SPEED, true);
			}
		}
		
		public void OnPlayerLand(params object[] args)
		{
			if (Activated)
			{
				//Parent.World.AddGraphic(Image.CreateRect((int)SMASH_RADIUS * 2, 10, FP.Color(0xFFFFFF)), -9009, Parent.X - SMASH_RADIUS, Parent.Y);
				
				var emitter = new Emitter(Library.GetTexture("assets/groundsmash_particle.png"), 3, 3);
				emitter.Relative = false;
				
				for (int i = 0; i < 4; i++)
				{
					var name = i.ToString();
					emitter.NewType(name, FP.Frames(i));
					emitter.SetAlpha(name, 0, 1f);
					emitter.SetMotion(name, 90, 100, 0.5f, 10, 10, 0.1f, Ease.SineOut);
					emitter.SetGravity(name, 5, 2);
				}
				
				var e = Parent.World.AddGraphic(emitter, -9010);
				e.AddTween(new Alarm(3, () => FP.World.Remove(e), Tween.ONESHOT), true);
				
				for (float i = -SMASH_RADIUS; i < SMASH_RADIUS; i++)
				{
					emitter.Emit(FP.Choose("0", "1", "2", "3"), Parent.X + i + FP.Random - FP.Random, Parent.Y + FP.Rand(10) - 5);
				}
				
				Parent.World.BroadcastMessage(GameManager.SHAKE, 100.0f, 0.5f);
				Parent.World.BroadcastMessage(GROUND_SMASH, Parent, SMASH_RADIUS);
				(Parent as Player).physics.OnMessage(PhysicsBody.USE_GRAVITY, true);
				
				(Parent as Player).SetUpgrade(null);
			}
		}
	}
}
