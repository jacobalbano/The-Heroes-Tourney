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
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of UpgradeSpawn.
	/// </summary>
	public class UpgradeSpawn : Entity
	{
		Upgrade upgrade;
		
		public UpgradeSpawn()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
			Graphic = Image.CreateCircle(10, FP.Color(0xFF66AA));
			(Graphic as Image).CenterOO();
			SetHitboxTo(Graphic);
			CenterOrigin();
		}
		
		public override void Added()
		{
			base.Added();
			if (FP.Rand(6) == 1)
			{
				switch (FP.Rand(3))
				{
					case 0:
						upgrade = new Invisibility();
						break;
					case 1:
						upgrade = new Shield();
						break;
					case 2:
						upgrade = new GroundSmash();
						break;
					default:
						break;
				}
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
				if (p != null)
				{
					if (p.upgrade == null)
					{
						FP.Log("GIVING PLAYER ", p.id, " A ", upgrade.GetType());
						p.SetUpgrade(upgrade);
						World.Remove(this);
					}
				}
			}
		}
	}
}
