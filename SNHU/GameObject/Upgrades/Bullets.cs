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
using SFML.Window;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject.Upgrades
{
	public class Bullet : Entity
	{
		public const string Collision = "bullet";
		public const float BULLET_SPEED = 4.0f;
		public int ownerID;
		private Vector2f dir;
		
		public Bullet(Vector2f initialDir, int ownerID) : base(0,0,Image.CreateCircle(12, FP.Color(0xFFFF00)))
		{
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
			
			MoveBy(dir.X * BULLET_SPEED, dir.Y * BULLET_SPEED, Platform.Collision, true);
			
			if (Collide(Platform.Collision, X + (dir.X * BULLET_SPEED), Y) != null)
			{
				dir.X = -dir.X;
			}
			if (Collide(Platform.Collision, X, Y + (dir.Y * BULLET_SPEED)) != null)
			{
				dir.Y = -dir.Y;
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
			
			Parent.World.RemoveList(bullets);
			
			(Parent as Player).SetUpgrade(null);
			Parent.RemoveLogic(this);
		}
	}
}
