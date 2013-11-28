/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 5:21 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject.Upgrades
{
	public class Bullet : Entity
	{
		public const string Collision = "bullet";
		public const float BULLET_SPEED = 10.0f;
		public int ownerID;
		private Vector2f dir;
		
		private Tween bounceTween;
		
		private Image image;
		private Entity emitterEnt;
		private Emitter emitter;
		
		private static string[] colliders;
		static Bullet()
		{
			colliders = new string[]
			{
				Platform.Collision,
				Player.Collision
			};
		}
		
		public Bullet(Vector2f initialDir, int ownerID)
		{
			image = new Image(Library.GetTexture("assets/bullet.png"));
			Graphic = image;
			image.CenterOO();
			
			var size = Math.Min(image.Width, image.Height);
			SetHitbox(size, size);
			CenterOrigin();
			Type = Collision;
						                           
			dir = initialDir;
			this.ownerID = ownerID;
			
			emitter = new Emitter(Library.GetTexture("assets/bullet_sparkle.png"), 20, 20);
			emitter.Relative = false;
			
			var name = "spark";
			emitter.NewType(name, FP.Frames(0, 1, 2, 3, 4));
			emitter.SetAlpha(name, 1, 0);
			emitter.SetMotion(name, 0, 0, 0.5f, 0, 0, 0.25f, Ease.CircOut);
		}
		
		public override void Added()
		{
			base.Added();
			
			emitterEnt = World.AddGraphic(emitter, -9010);
		}
		
		public override void Update()
		{
			base.Update();
			
			var randX = FP.Rand(50) - 25;
			var randY = FP.Rand(50) - 25;
			emitter.Emit("spark", X + randX, Y + randY);
			
			image.Angle = FP.Angle(0, 0, dir.X, dir.Y);
			
			MoveBy(dir.X * BULLET_SPEED, dir.Y * BULLET_SPEED, colliders, true);
		}
		
		public override void Removed()
		{
			base.Removed();
			
			World.AddTween(new Alarm(3, () => FP.World.Remove(emitterEnt), Tween.ONESHOT), true);
			
			for (int i = 0; i < 10; i++)
			{
				var randX = FP.Rand(100) - 50;
				var randY = FP.Rand(100) - 50;
				emitter.Emit("spark", X + randX, Y + randY);		
			}
		}
		
		public override bool MoveCollideX(Entity e)
		{
			if (ShouldBounce(e))
			{
				dir.X = -dir.X;
				Mixer.Audio["bulletBounce"].Play();
				Bounce();
				
				return true;
			}
				
			return false;
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (ShouldBounce(e))
			{
				dir.Y = -dir.Y;
				Mixer.Audio["bulletBounce"].Play();
				Bounce();
				
				return true;
			}
				
			return false;
		}
		
		bool ShouldBounce(Entity e)
		{
			var p = e as Player;
			if (p != null)
			{
				if (p.PlayerId == ownerID)
				{
					return false;
				}
				
				if (p.Rebounding)
				{
					return true;
				}
				
				p.OnMessage(Player.Damage);
				return false;
			}
			
			e.OnMessage(Platform.PlayerLand);
			
			return true;
		}
		
		private void Bounce()
		{
			if (bounceTween != null)	bounceTween.Cancel();
			image.ScaleX = 0.5f;
			image.ScaleY = 1.5f;
			
			var tween = new MultiVarTween(null, ONESHOT);
			tween.Tween(Graphic, new { ScaleX = 1, ScaleY = 1 }, 0.75f, Ease.ElasticOut);
			bounceTween = FP.Tweener.AddTween(tween, true);
		}
	}
	
	/// <summary>
	/// Description of Bullets.
	/// </summary>
	public class Bullets : Upgrade
	{
		List<Entity> bullets;
		
		public const int BULLET_COUNT = 3;
		
		public Bullets()
		{
			bullets = new List<Entity>();
		}
		
		public override void Added()
		{
			base.Added();
			
			bullets.Add(new Bullet(new Vector2f(-1, -1), (Parent as Player).PlayerId));
			bullets.Add(new Bullet(new Vector2f(FP.Choose(-0.1f, 0.1f), -1), (Parent as Player).PlayerId));
			bullets.Add(new Bullet(new Vector2f(1, -1), (Parent as Player).PlayerId));
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				foreach (var b in bullets)
				{
					b.X = Parent.X;
					b.Y = Parent.Top;
				}
				
				Parent.World.AddList(bullets);
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			
			Parent.World.RemoveList(bullets);
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			if (Parent != null)
			{
				if (Parent.World != null)
				{
					Parent.World.RemoveList(bullets);
				}
				
				(Parent as Player).SetUpgrade(null);
			}
		}
	}
}
