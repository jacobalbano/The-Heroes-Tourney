using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SFML.Window;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject.Upgrades
{
	public class Bullet : Entity
	{
		public const string Collision = "bullet";
		public const float BULLET_SPEED = 10.0f;
		public Player owner;
		private Entity lastBounce;
		private Vector2f dir;
		private MessageResult result;
		
		private Image image;
		private Entity emitterEnt;
		private Emitter emitter;
		private Tween bounceTween;
		
		private static string[] colliders;
		static Bullet()
		{
			colliders = new string[]
			{
				Platform.Collision,
				Player.Collision
			};
		}
		
		public Bullet(Vector2f initialDir, Player owner)
		{	
			image = new Image(Library.GetTexture("assets/bullet.png"));
			AddComponent(image);
			image.CenterOO();
			
			var size = Math.Min(image.Width, image.Height);
			SetHitbox(size, size);
			CenterOrigin();
			Type = Collision;
						                           
			dir = initialDir;
			this.owner = owner;
			
			emitter = new Emitter(Library.GetTexture("assets/bullet_sparkle.png"), 20, 20);
			emitter.Relative = false;
			
			var name = "spark";
			emitter.NewType(name, FP.Frames(0, 1, 2, 3, 4));
			emitter.SetAlpha(name, 1, 0);
			emitter.SetMotion(name, 0, 0, 0.5f, 0, 0, 0.25f, Ease.CircOut);
			
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
			
			result = new MessageResult();
		}
		
		public override void Added()
		{
			base.Added();
			
			emitterEnt = World.AddGraphic(emitter, -9010);
			emitterEnt.Active = true;
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
			
			World.Tweener.Timer(3).OnComplete(() => FP.World.Remove(emitterEnt));
			
			for (int i = 0; i < 10; i++)
			{
				var randX = FP.Rand(100) - 50;
				var randY = FP.Rand(100) - 50;
				emitter.Emit("spark", X + randX, Y + randY);		
			}
		}
		
		public override bool MoveCollideX(Entity e)
		{
			if (e == owner)	return false;
			if (ShouldBounce(e))
			{
				dir.X = -dir.X;
				Mixer.BulletBounce.Play();
				Bounce();
				
				return true;
			}
				
			return false;
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e == owner)	return false;
			if (ShouldBounce(e))
			{
				dir.Y = -dir.Y;
				Mixer.BulletBounce.Play();
				Bounce();
				
				return true;
			}
				
			return false;
		}
		
		bool ShouldBounce(Entity e)
		{
			result.Value = false;
			e.OnMessage(EffectMessage.Message.OnEffect, MakeEffect(result));
			e.OnMessage(Platform.Message.Bump);
			
			var bounce = e != lastBounce && (bool) result.Value;
			lastBounce = e;
			
			return bounce || e.Type == Platform.Collision;
		}
		
		private EffectMessage MakeEffect(MessageResult result)
		{
			EffectMessage.Callback callback = delegate(Entity to, Entity from, float scalar)
			{
				if (from != null)	//	hit
				{
					from.OnMessage(Player.Message.Damage);
				}
				else //	rebound
				{
					result.Value = true;
				}
			};
			
			return new EffectMessage(null, callback);
		}
		
		private void Bounce()
		{	
			if (bounceTween != null)	bounceTween.Cancel();
			image.ScaleX = 0.5f;
			image.ScaleY = 1.5f;
			
			bounceTween = Tweener.Tween(image, new {ScaleX = 1, ScaleY = 1}, 0.75f)
				.Ease(Ease.ElasticOut);
		}
		
		private void OnAdvance(params object[] args)
		{
			World.Remove(this);
		}
	}
	
	/// <summary>
	/// Description of Bullets.
	/// </summary>
	public class Bullets : Upgrade
	{
		List<Entity> bullets;
		
		public int BulletCount { get; private set; }
		
		private enum Mode { Cone, Radial, Random };
		
		public Bullets()
		{
			Icon = new Image(Library.GetTexture("assets/bullets.png"));
			bullets = new List<Entity>();
			
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
		}
		
		public override void Added()
		{
			base.Added();
			
			BulletCount = int.Parse(GameWorld.gameManager.Config["Bullets", "Count"]);
			Lifetime = float.Parse(GameWorld.gameManager.Config["Bullets", "Lifetime"]);
			
			var modeString = Regex.Replace(GameWorld.gameManager.Config["Bullets", "Mode"], @"\s", "");
			
			Mode mode = (Mode) Util.GetEnumFromName(typeof(Mode), modeString);
			if (mode == Mode.Random)
				mode = FP.Choose(Mode.Radial, Mode.Cone);
			
			for (int i = 0; i < BulletCount; i++)
			{
				var vec = new Vector2f();
				if (mode == Mode.Radial)
				{
					var angle = FP.Scale(i, 0, BulletCount, 0, 360);
					angle += 45;
					if ((angle %= 360) < 0)	angle += 360;
				
					FP.AngleXY(ref vec.X, ref vec.Y, angle, 1);
				}
				else
				{
					vec.X = FP.Scale(i, 0, BulletCount - 1, -1, 1);
					vec.Y = -1;
				}
				
				bullets.Add(new Bullet(vec, owner));
			}
		}
		
		
		public override EffectMessage MakeEffect()
		{
			throw new NotSupportedException("Don't be using this pls");
		}
		
		public override void Update()
		{
			base.Update();
			
			if (bullets.Count == 0) return;
 			
			int count = 0;
			foreach (var bullet in bullets)
			{
				if (bullet.World != null && !bullet.OnCamera)	count++;
			}
			
			if (count == bullets.Count)
			{
				OnLifetimeComplete();
			}
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
				
				owner.SetUpgrade(null);
			}
		}
		
		private void OnAdvance(params object[] args)
		{
			if (Activated)
				OnLifetimeComplete();
		}
	}
}
