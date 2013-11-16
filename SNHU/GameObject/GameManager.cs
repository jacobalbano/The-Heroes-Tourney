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
using Punk.Tweens.Misc;
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
		public const float SCROLL_SPEED = 1.0f;
		
		public const float METEOR_TIME = 20.0f;
		public float meteorTimeScale = 1.0f;
		private Alarm meteorTimer;
		private bool meteorMode;
		
		public GameManager()
		{
			matchTimer = new MatchTimer(240.0f);
			Players = new List<Player>();
			hud = new HUD(this);
			
			meteorTimer = new Alarm(METEOR_TIME * meteorTimeScale, OnMeteor, Tween.ONESHOT);
			meteorMode = false;
			
			FP.Camera.Zoom = 0.75f;
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
		
		public override void Update()
		{
			base.Update();
			
			if (matchTimer.Timer.Active)
			{
				FP.Camera.Y -= SCROLL_SPEED;
				
				if (matchTimer.Timer.Percent >= 0.5f)
				{
					if (World != null && ! meteorMode)
					{
						meteorMode = true;
						World.Add(new Meteor());
						World.AddTween(meteorTimer, true);
					}
					
					if (matchTimer.Timer.Percent >= 0.66f)
					{
						meteorTimeScale = 0.75f;
					}
					
					if (matchTimer.Timer.Percent >= 0.75f)
					{
						meteorTimeScale = 0.5f;
					}
					
					if (matchTimer.Timer.Percent >= 0.9f)
					{
						meteorTimeScale = 0.1f;
					}
				}
				
				if (matchTimer.Timer.Percent == 1.0f)
				{
					World.RemoveTween(meteorTimer);
				}
			}
		}
		
		public void AddPlayer(float x, float y, uint id)
		{
			if (Players.Find(p => p.id == id) != null)	return;
			
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
		
		public void OnMeteor()
		{
			World.Add(new Meteor());
			meteorTimer.Reset(METEOR_TIME * meteorTimeScale);
			World.AddTween(meteorTimer, true);
		}
	}
}
