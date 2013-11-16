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
		public bool GameStarted { get; private set; }
		public bool GamePaused { get; private set; }
		
		private Music GameMusic;
		
		public List<Player> Players;
		private HUD hud;
		public const float SCROLL_SPEED = 1.0f;
		
		public const float METEOR_TIME = 20.0f;
		public float meteorTimeScale = 1.0f;
		private Alarm meteorTimer;
		private bool meteorMode;
		
		public GameManager()
		{
			GameStarted = false;
			GamePaused = false;
			
			GameMusic = Mixer.music;
			
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
			
			World.Add(hud);
		}
		
		public override void Removed()
		{
			World.Remove(hud);
			
			base.Removed();
		}
		
		public override void Update()
		{
			base.Update();
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
			if (!GameStarted)
			{
				GameStarted = true;
				GameMusic.Play();
			}
		}
		
		public void TogglePauseGame(bool affectEnts)
		{
			if (GameStarted)
			{
				List<Entity> entList = new List<Entity>();
				World.GetAll(entList);
				
				if (!GamePaused)
				{
					GamePaused = true;
					GameMusic.Pause();
					
					if (affectEnts)
					{
						foreach (Entity e in entList)
						{
							e.Active = false;
						}
					}
				}
				else
				{
					GamePaused = false;
					GameMusic.Play();
					
					if (affectEnts)
					{
						foreach (Entity e in entList)
						{
							e.Active = true;
						}
					}
				}
			}
		}
		
		public void OnMeteor()
		{
			World.Add(new Meteor());
			meteorTimer.Reset(METEOR_TIME * meteorTimeScale);
			World.AddTween(meteorTimer, true);
		}
	}
}
