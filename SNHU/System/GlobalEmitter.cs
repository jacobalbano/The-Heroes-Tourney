
using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using SNHU.Config;

namespace SNHU.Systems
{
	public class GlobalEmitter : Entity
	{
		public enum Message
		{
			SuperSpeed,
			BulletTrail,
			DoubleJump,
			GroundSmash
		}
		
		public GlobalEmitter()
		{
			AddResponse(Message.GroundSmash, DelegateEmit(InitGroundSmash()));
			AddResponse(Message.BulletTrail, DelegateEmit(InitBulletTrail()));
			AddResponse(Message.SuperSpeed, DelegateEmit(InitSpeed()));
			AddResponse(Message.DoubleJump, DelegateEmit(InitDoubleJump()));
			
			Layer = ObjectLayers.JustAbove(ObjectLayers.Players);
		}
		
		private Emitter InitDoubleJump()
		{
			var emitter = new Emitter(Library.GetTexture("dust.png"), 24, 24);
			
			emitter.NewType("dust", FP.Frames(0, 1));
			emitter.SetAlpha("dust", 0.9f, 0, Ease.QuintOut);
			emitter.SetMotion("dust", 80, 3, 0.5f, 10, 3, 0.2f, Ease.QuintOut);
			emitter.SetScaling("dust", 1.8f, 1.5f, Ease.QuintOut);
			
			return emitter;
		}
		
		private Emitter InitSpeed()
		{
			var emitter = new Emitter(Library.GetTexture("speed_particle.png"), 68, 30);
			
			emitter.NewType("l", FP.Frames(0));
			emitter.SetMotion("l", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
			emitter.SetAlpha("l", 0.5f, 0, Ease.QuintOut);
			
			emitter.NewType("r", FP.Frames(1));
			emitter.SetMotion("r", 0, 50, 0.5f, 0, 15, 1, Ease.QuintOut);
			emitter.SetAlpha("r", 0.5f, 0, Ease.QuintOut);
			
			return emitter;
		}
		
		private Emitter InitBulletTrail()
		{
			var emitter = new Emitter(Library.GetTexture("bullet_sparkle.png"), 20, 20);
			
			var name = "spark";
			emitter.NewType(name, FP.Frames(0, 1, 2, 3, 4));
			emitter.SetAlpha(name, 1, 0);
			emitter.SetMotion(name, 0, 0, 0.5f, 0, 0, 0.25f, Ease.CircOut);
			
			return emitter;
		}
		
		private Emitter InitGroundSmash()
		{
			var emitter = new Emitter(Library.GetTexture("groundsmash_particle.png"), 3, 3);
			emitter.Relative = false;
			
			for (int i = 0; i < 4; i++)
			{
				var name = i.ToString();
				emitter.NewType(name, FP.Frames(i));
				emitter.SetAlpha(name, 0, 1f);
				emitter.SetMotion(name, -90, 100, 0.5f, 10, 10, 0.1f, Ease.SineOut);
				emitter.SetGravity(name, 5, 2);
			}
			
			return emitter;
		}
		
		private Action<object[]> DelegateEmit(Emitter emitter)
		{
			AddComponent(emitter);
			emitter.Relative = false;
			return args => emitter.Emit((string) args[0], Convert.ToSingle(args[1]), Convert.ToSingle(args[2]));
		}
	}
}
