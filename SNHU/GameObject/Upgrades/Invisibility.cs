using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Invisibility.
	/// </summary>
	public class Invisibility : Upgrade
	{
		public float InvisibleAlpha { get; private set; }
		public float PunchMultiplier { get; private set; }
		
		public Invisibility()
		{
			Icon = new Image(Library.GetTexture("assets/invisibility.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			InvisibleAlpha = float.Parse(GameWorld.gameManager.Config["Invisibility", "InvisibleAlpha"]);
			PunchMultiplier = float.Parse(GameWorld.gameManager.Config["Invisibility", "PunchMultiplier"]);
			Lifetime = float.Parse(GameWorld.gameManager.Config["Invisibility", "Lifetime"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			throw new NotImplementedException();
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
			
				var tweenPlayer = new VarTween(null, Tween.ONESHOT);
				tweenPlayer.Tween(owner.player, "Alpha", InvisibleAlpha, 0.25f);
				AddTween(tweenPlayer, true);
				
				var tweenFist1 = new VarTween(null, Tween.ONESHOT);
				tweenFist1.Tween(owner.left.Graphic, "Alpha", InvisibleAlpha, 0.25f);
				AddTween(tweenFist1, true);
				
				var tweenFist2 = new VarTween(null, Tween.ONESHOT);
				tweenFist2.Tween(owner.right.Graphic, "Alpha", InvisibleAlpha, 0.25f);
				AddTween(tweenFist2, true);
				
				owner.right.ForceMultiplier = owner.left.ForceMultiplier = PunchMultiplier;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			owner.right.ForceMultiplier = owner.left.ForceMultiplier = Fist.DEFAULT_PUNCH_MULT;
		
			var tweenPlayer = new VarTween(() => owner.SetUpgrade(null), Tween.ONESHOT);
			tweenPlayer.Tween(owner.player, "Alpha", 1.0f, 0.25f);
			AddTween(tweenPlayer, true);
			
			var tweenFist1 = new VarTween(null, Tween.ONESHOT);
			tweenFist1.Tween(owner.left.Graphic, "Alpha", 1.0f, 0.25f);
			AddTween(tweenFist1, true);
			
			var tweenFist2 = new VarTween(null, Tween.ONESHOT);
			tweenFist2.Tween(owner.right.Graphic, "Alpha", 1.0f, 0.25f);
			AddTween(tweenFist2, true);
		}
	}
}
