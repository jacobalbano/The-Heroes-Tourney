/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/16/2013
 * Time: 1:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of HUD.
	/// </summary>
	public class HUD : Entity
	{
		private Entity p1, p2, p3, p4;
		private Text p1Txt, p2Txt, p3Txt, p4Txt;
		private GameManager gm;
		
		public HUD(GameManager gameManager)
		{
			gm = gameManager;
			
			p1Txt = new Text("_");
			p1Txt.Color = FP.Color(0xFF8888);
			p1Txt.Italicized = true;
			p1Txt.Size = 18;
			p1 = new Entity(10, 10, p1Txt);
			
			p2Txt = new Text("_");
			p2Txt.Color = FP.Color(0x88FF88);
			p2Txt.Italicized = true;
			p2Txt.Size = 18;
			p2 = new Entity(200, 10, p2Txt);
			
			p3Txt = new Text("_");
			p3Txt.Color = FP.Color(0x8888FF);
			p3Txt.Italicized = true;
			p3Txt.Size = 18;
			p3 = new Entity(670, 10, p3Txt);
			
			p4Txt = new Text("_");
			p4Txt.Color = FP.Color(0xFFFF88);
			p4Txt.Italicized = true;
			p4Txt.Size = 18;
			p4 = new Entity(860, 10, p4Txt);
		}
		
		public override void Added()
		{
			base.Added();
			
			p1Txt.ScrollX = p1Txt.ScrollY = 0;
			p2Txt.ScrollX = p2Txt.ScrollY = 0;
			p3Txt.ScrollX = p3Txt.ScrollY = 0;
			p4Txt.ScrollX = p4Txt.ScrollY = 0;
			
			p1.Layer = p2.Layer = p3.Layer = p4.Layer = -1000;
			
			World.Add(p1);
			World.Add(p2);
			World.Add(p3);
			World.Add(p4);
		}
		
		public override void Removed()
		{
			World.Remove(p1);
			World.Remove(p2);
			World.Remove(p3);
			World.Remove(p4);
			
			base.Removed();
		}
		
		public override void Update()
		{
			base.Update();
			
			if (gm.Players.Count >= 1)
			{
				p1Txt.String = "Player 1\n" + gm.Players[0].Points.ToString();
			}
			if (gm.Players.Count >= 2)
			{
				p2Txt.String = "Player 2\n" + gm.Players[1].Points.ToString();
			}
			if (gm.Players.Count >= 3)
			{
				p3Txt.String = "Player 3\n" + gm.Players[2].Points.ToString();
			}
			if (gm.Players.Count >= 4)
			{
				p4Txt.String = "Player 4\n" + gm.Players[3].Points.ToString();
			}
		}
	}
}
