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
		public int ID;
		
		public const string Collision = "teleporter";
		
		public Player owner { get; private set; }
		
		public Image myImage;
		
		
		public Teleporter()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
			Graphic = myImage = Image.CreateRect(16, 16, FP.Color(0xFFAAFF));
			SetHitboxTo(Graphic);
			Type = Collision;
			
			ID += (int)FP.Camera.Y;
		}
		
		public override void Update()
		{
			base.Update();
			
			Player p = (Collide(Player.Collision, X, Y) as Player);
			if (p != null)
			{
				owner = p;
				Teleporter t = GetPartner();
				
				if (t != null)
				{
					owner.X = t.X;
					owner.Y = t.Y;
					
					Mixer.Audio["teleport"].Volume = 60.0f;
					Mixer.Audio["teleport"].Play();
					
					World.Remove(t);
					World.Remove(this);
				}
			}
		}
		
		private Teleporter GetPartner()
		{
			List<Entity> teleList = new List<Entity>();
			World.GetType(Teleporter.Collision, teleList);
			
			for(int x = 0; x < teleList.Count; x++)
			{
				if(teleList[x] != this && (teleList[x] as Teleporter).ID == ID)
				{
					return (teleList[x] as Teleporter);
				}
			}
			
			return null;
		}
	}
}
