using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Content;
using Indigo.Graphics;
using SNHU.Systems;

namespace SNHU.GameObject.Effects
{
	public class Explosion : Entity
	{
		private Emitter Emitter;
		private int Radius = 150;
		
		public Explosion(float x, float y) : base(x, y)
		{
			Emitter = AddComponent(new Emitter(Library.Get<Texture>("explosion.png"), 60, 60));
			
			for (int i = 0; i < 4; i++)
			{
				var name = i.ToString();
				Emitter.NewType(name, Engine.Frames(i));
				Emitter.SetAlpha(name, 1, 0);
				Emitter.SetMotion(name, 0, 50, 0.4f, 360, 15,  0.15f, Ease.CubeOut);
			}
			
			Width = Height = (int) (Radius * 1.25f);
			CenterOrigin();
			
			var t = 4;
			while (t --> 0)
			{
				var name = t.ToString();
				for (int j = 0; j < Radius; j++)
				{
					var pos = Engine.Random.InCircle(Radius);
					Emitter.Emit(name, pos.X, pos.Y);
				}
			}
		}
		
		public override void Added()
		{
			base.Added();
			World.BroadcastMessage(CameraManager.Message.Shake, 20, 0.5f);
			Mixer.Explode.Play();
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			var all = new List<Entity>();
			CollideInto(Player.Collision, X, Y, all);
			
			for (int i = 0; i < all.Count; i++)
			{
				all[i].OnMessage(Player.Message.Damage);
			}
			
			if (Emitter.ParticleCount == 0)
				World.Remove(this);
		}
	}
}
