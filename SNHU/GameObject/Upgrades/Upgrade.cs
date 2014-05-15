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
		
		public bool Activated { get; protected set; }
		private float _lifetime;
		public float Lifetime
		{
			get
			{
				return _lifetime;
			}
			
			protected set
			{
				_lifetime = value;
				lifeTimer = new Alarm(_lifetime, OnLifetimeComplete, Tween.ONESHOT);
			}
		}
		
		protected Player owner;
		public Image Icon;
		
		protected Alarm lifeTimer;
		public Upgrade()
		{
			Lifetime = 5.0f;
			Activated = false;
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
