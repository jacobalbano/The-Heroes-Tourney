using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Upgrade.
	/// </summary>
	public class Upgrade : Logic
	{
		public const string Used = "Upgrade Used";
		
		protected const float LIFETIME = 5.0f;
		public bool Activated { get; protected set; }
		
		protected Player owner;
		public Image Icon;
		
		protected Alarm lifeTimer;
		public Upgrade()
		{
			Activated = false;
			lifeTimer = new Alarm(LIFETIME, OnLifetimeComplete, Tween.ONESHOT);
		}
		
		public override void Added()
		{
			base.Added();
			owner = Parent as Player;
		}
		
		public virtual void Use()
		{
			Activated = true;
			AddTween(lifeTimer, true);
		}
		
		public virtual EffectMessage MakeEffect()
		{
			return new EffectMessage();
		}
		
		public virtual void OnLifetimeComplete()
		{
		}
	}
}
