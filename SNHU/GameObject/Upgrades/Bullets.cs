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
		
		public Bullet(Vector2f initialDir, int ownerID)
		{
			Graphic = new Image(Library.GetTexture("assets/bullet.png"));
			
			(Graphic as Image).CenterOO();
			SetHitboxTo(Graphic);
			CenterOrigin();
			Type = Collision;
						                           
			dir = initialDir;
			this.ownerID = ownerID;
		}
		
		public override void Update()
		{
			base.Update();
			
			var bounce = false;
			
			(Graphic as Image).Angle = FP.Angle(0, 0, dir.X, dir.Y);
			
			MoveBy(dir.X * BULLET_SPEED, dir.Y * BULLET_SPEED, Platform.Collision, true);
			
			if (Collide(Platform.Collision, X + (dir.X * BULLET_SPEED), Y) != null)
			{
				dir.X = -dir.X;
				World.BroadcastMessage(GameManager.SHAKE, 20.0f, 0.25f);
				Mixer.Audio["bulletBounce"].Play();
				bounce = true;
			}
			if (Collide(Platform.Collision, X, Y + (dir.Y * BULLET_SPEED)) != null)
			{
				dir.Y = -dir.Y;
				World.BroadcastMessage(GameManager.SHAKE, 20.0f, 0.25f);
				Mixer.Audio["bulletBounce"].Play();
				bounce = true;
			}
			
			var p = Collide(Player.Collision, X + (dir.X * BULLET_SPEED), Y);
			if (p != null && (p as Player).Rebounding)
			{
				dir.X = -dir.X;
				World.BroadcastMessage(GameManager.SHAKE, 20.0f, 0.25f);
				Mixer.Audio["bulletBounce"].Play();
			}
			
			p = Collide(Player.Collision, X, Y + (dir.Y * BULLET_SPEED));
			if (p != null && (p as Player).Rebounding)
			{
				dir.Y = -dir.Y;
				World.BroadcastMessage(GameManager.SHAKE, 20.0f, 0.25f);
				Mixer.Audio["bulletBounce"].Play();
			}
			
			if (bounce)
			{
				if (bounceTween != null)	bounceTween.Cancel();
				(Graphic as Image).ScaleX = 0.5f;
				(Graphic as Image).ScaleY = 1.5f;
				
				var tween = new MultiVarTween(null, ONESHOT);
				tween.Tween(Graphic, new {ScaleX = 1, ScaleY = 1}, 0.75f, Ease.ElasticOut);
				bounceTween = FP.Tweener.AddTween(tween, true);
			}
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
			
			//for (int i = 0; i < BULLET_COUNT; i++)
			//{
			bullets.Add(new Bullet(new Vector2f(-1, -1), (Parent as Player).id));
			bullets.Add(new Bullet(new Vector2f(0, -1), (Parent as Player).id));
			bullets.Add(new Bullet(new Vector2f(1, -1), (Parent as Player).id));
			//}
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
