/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 1:42 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		const float LIFETIME = 5.0f;
		public bool Activated { get; protected set; }
		public Image image {get; set;}
		
		protected Alarm lifeTimer;
		public Upgrade()
		{
			Activated = false;
			lifeTimer = new Alarm(LIFETIME, OnLifetimeComplete, Tween.ONESHOT);
		}
		
		public virtual void Use()
		{
			Activated = true;
			
			if (Parent.World != null)
			{
				Parent.World.AddTween(lifeTimer, true);
			}
		}
		
		public override void Added()
		{
			base.Added();
			//Parent.World.AddGraphic(image);
		}
		
		public virtual void OnLifetimeComplete()
		{
			
		}
	}
}
