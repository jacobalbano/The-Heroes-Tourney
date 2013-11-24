/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 1:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of UpgradeSpawn.
	/// </summary>
	public class UpgradeSpawn : Entity
	{
		Upgrade upgrade;
		Player owner;
		
		public UpgradeSpawn()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
		}
		
		public override void Added()
		{
			base.Added();
			if (FP.Rand(6) == 1)
			{
				switch (FP.Rand(8))
				{
					case 0:
						upgrade = new Invisibility();
						Graphic = Image.CreateCircle(10, FP.Color(0x0000FF));
						break;
					case 1:
						upgrade = new Shield();
						Graphic = Image.CreateCircle(10, FP.Color(0x00FF00));
						break;
					case 2:
						upgrade = new GroundSmash();
						Graphic = Image.CreateCircle(10, FP.Color(0xFF0000));
						break;
					case 3:
						upgrade = new SuperSpeed();
						Graphic = Image.CreateCircle(10, FP.Color(0xFFFF00));
						break;
					case 4:
						upgrade = new Rebound();
						Graphic = Image.CreateCircle(10, FP.Color(0xFFFFFF));
						break;
					case 5:
						upgrade = new FUS();
						Graphic = Image.CreateCircle(10, FP.Color(0x00FFFF));
						break;
					case 6:
						upgrade = new Bullets();
						Graphic = Image.CreateCircle(10, FP.Color(0xFF00FF));
						break;
					case 7:
						upgrade = new HotPotato();
						Graphic = Image.CreateCircle(10, FP.Color(0xF0F0F0));
						break;
					default:
						break;
				}
				
				(Graphic as Image).CenterOO();
				SetHitboxTo(Graphic);
				CenterOrigin();
			}
			else
			{
				World.Remove(this);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (upgrade != null)
			{
				var p = Collide(Player.Collision, X, Y) as Player;
				if (p != null && p.upgrade == null)
				{
					if(owner == null)
					{
						owner = p;
						FP.Log("GIVING PLAYER ", p.id, " A ", upgrade.GetType());
						p.SetUpgrade(upgrade);
						
						AddTween(new Alarm(2.0f, () => World.Remove(this), ONESHOT), true);
					}
				}
				if(owner != null)
				{
					X = owner.X;
					Y = owner.Top - 20;
				}
			}
		}
	}
}
