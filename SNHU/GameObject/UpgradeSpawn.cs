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
			AddResponse("Upgrade Used", OnPlayerUsed);
		}
		
		private void OnPlayerUsed(params object[] args)
		{
			int id = (int)args[0];
			if(owner != null)
			{
				if(owner.id == id)
				{
					World.Remove(this);
				}
			}
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
		}
		
		public override void Added()
		{
			base.Added();
			if (FP.Rand(3) == 1)
			{
				switch (FP.Rand(8))
				{
					case 0:
						upgrade = new Invisibility();
						Graphic = new Image(Library.GetTexture("assets/invisibility.png"));
						break;
					case 1:
						upgrade = new Shield();
						Graphic = new Image(Library.GetTexture("assets/shield.png"));
						break;
					case 2:
						upgrade = new GroundSmash();
						Graphic = new Image(Library.GetTexture("assets/groundsmash.png"));
						break;
					case 3:
						upgrade = new SuperSpeed();
						Graphic = new Image(Library.GetTexture("assets/speed.png"));
						break;
					case 4:
						upgrade = new Rebound();
						Graphic = new Image(Library.GetTexture("assets/rebound.png"));
						break;
					case 5:
						upgrade = new FUS();
						Graphic = new Image(Library.GetTexture("assets/fus.png"));
						break;
					case 6:
						upgrade = new Bullets();
						Graphic = new Image(Library.GetTexture("assets/bullets.png"));
						break;
					case 7:
						upgrade = new HotPotato();
						Graphic = new Image(Library.GetTexture("assets/hotpotato.png"));
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
