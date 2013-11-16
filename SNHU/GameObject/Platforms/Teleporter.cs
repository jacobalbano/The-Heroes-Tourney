/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/16/2013
 * Time: 2:40 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;

namespace SNHU.GameObject.Platforms
{
	public class Teleporter : Entity
	{
		private int ID;
		public int teleporterID
		{
			get
			{
				return ID;
			}
		}
		
		public const string Collision = "teleporter";
		
		private Player owner;
		public Player myOwner
		{
			set
			{
				owner = value;
			}
		}
		public Image myImage;
		
		
		public Teleporter()
		{
			Graphic = myImage = Image.CreateRect(32, 32, FP.Color(0x00FF00));
			SetHitboxTo(Graphic);
		}
		
		public void onHit(Player targetPlayer)
		{
			List<Entity> teleList = new List<Entity>();
			World.GetType(Teleporter.Collision, teleList);
			for(int x = 0; x < teleList.Count; x++)
			{
				if(teleList[x] is TeleporterOUT)
				{
					if((teleList[x] as Teleporter).ID == teleporterID)
					{
						targetPlayer.X = teleList[x].X;
						targetPlayer.Y = teleList[x].Y;
					}
				}
			}
		}
	}
}
