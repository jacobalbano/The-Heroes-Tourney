
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Config.Upgrades;
using SNHU.GameObject.Effects;

namespace SNHU.GameObject.Upgrades.Helper
{
	/// <summary>
	/// Description of PotatoThinker.
	/// </summary>
	public class PotatoThinker : Entity
	{
		Tween alarm;
		
		Image image;
		float totalTime;
		
		Player Parent, Target;
		
		public PotatoThinker(Player target)
		{
			Parent = Target = target;
			X = Target.X;
			Y = Target.Top - 40;
		
			var config = Library.GetConfig<HotPotatoConfig>("assets/config/upgrades/hotpotato.ini");
			totalTime = Math.Max(0.01f, config.Duration);
			
			image = AddComponent(new Image(Library.GetTexture("assets/hotpotato.png")));
			image.Color = new Color(0xff0000);
			image.CenterOrigin();
			CenterOrigin();
			
			AddResponse(Player.Message.Die, OnPlayerDie);
			AddResponse(Player.Message.Hit, OnPlayerHit);
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
			AddResponse(ChunkManager.Message.AdvanceComplete, OnAdvanceComplete);
			
			alarm = Tweener.Timer(totalTime).OnComplete(OnExplode);
			Tick();
		}
		
		private void Tick()
		{
			image.Scale = 1.25f;
			Tweener.Tween(image, new { Scale = 1}, alarm.TimeRemaining / 5)
				.OnComplete(Tick);
			
			Mixer.TimeTick.Play();
		}
		
		public override void Removed()
		{
			base.Removed();
			World.Add(new Explosion(X, Y));
			Parent.SetUpgrade(null);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Target != null)
			{
				float x = Target.X, y = Target.Top - 40;
				MoveTowards(x, y, FP.Distance(X, Y, x, y) * 0.3f);
			}
		}
		
		private void OnPlayerDie(params object[] args)
		{
			var p = args[0] as Player;
			if (p == Target)
				World.Remove(this);
		}
		
		public void OnPlayerHit(params object[] args)
		{
			var from = args[0] as Player;
			var to = args[1] as Player;
			
			if (from == Target)
				Target = to;
		}
		
		private void OnExplode()
		{
			World.Remove(this);
		}
		
		private void OnAdvance(params object[] args)
		{
			Active = false;
		}
		
		private void OnAdvanceComplete(params object[] args)
		{
			Active = true;
		}
	}
}
