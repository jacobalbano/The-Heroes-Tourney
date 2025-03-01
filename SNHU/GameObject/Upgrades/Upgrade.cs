﻿using System;
using Glide;
using Indigo;
using Indigo.Graphics;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Upgrade.
	/// </summary>
	public class Upgrade : Component
	{
		[AttributeUsage(AttributeTargets.Class)]
		public class DisabledInBuildAttribute : Attribute {}
		
		public enum Message { Used }
		
		protected Tween lifeTimer;
		private float _lifetime;
		public float Lifetime
		{
			get { return _lifetime; }
			protected set {
				_lifetime = value;
				if (lifeTimer != null)
					lifeTimer.Cancel();
				
				lifeTimer = Tweener.Timer(Lifetime).OnComplete(OnLifetimeComplete);
				lifeTimer.Pause();
			}
		}
		
		protected Player owner;
		public Image Icon { get; protected set; }
		public bool Activated { get; protected set; }
		
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
			lifeTimer.Resume();
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
