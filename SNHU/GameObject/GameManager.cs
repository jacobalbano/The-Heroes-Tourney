/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/16/2013
 * Time: 12:43 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using SFML.Graphics;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of GameManager.
	/// </summary>
	public class GameManager : Entity
	{
		private MatchTimer matchTimer;
		public List<Player> Players;
		private HUD hud;
		
		public GameManager()
		{
			matchTimer = new MatchTimer(240.0f);
			Players = new List<Player>();
			hud = new HUD(this);
		}
		
		public override void Added()
		{
			base.Added();
			
			foreach (Player p in Players)
			{
				if (p.World == null)
				{
					World.Add(p);
				}
			}
			
			World.Add(matchTimer);
			World.Add(hud);
		}
		
		public override void Removed()
		{
			World.Remove(matchTimer);
			World.Remove(hud);
			
			base.Removed();
		}
		
		public void AddPlayer(float x, float y, uint id)
		{
			FP.Log("here");
			if (Players.Find(p => p.id == id) != null)	return;
			FP.Log("here2");
			
			if (Players.Count < 4 && id <= 4)
			{
				Player p = new Player(x, y, id);
				Players.Add(p);
				
				if (World != null)
				{
					World.Add(p);
				}
			}
		}
		
		public void StartGame()
		{
			matchTimer.Timer.Start();
		}
	}
}
