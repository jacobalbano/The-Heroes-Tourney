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
using Punk.Utils;
using SFML.Graphics;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of GameManager.
	/// </summary>
	public class GameManager : Entity
	{
		/// <summary>
		/// Broadcast when the camera should shake
		/// Arguments: (float strength = 10.0f, float duration = 1.0f)
		/// </summary>
		public const string SHAKE = "cameraShake";
		
		private float offsetX, offsetY;
		private MultiVarTween prevShaker;
		
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
			
			AddResponse("player_die", OnPlayerDie);
			AddResponse(SHAKE, OnCameraShake);
		}
		
		public override void Added()
		{
			base.Added();
			
			//foreach (Player p in Players)
			//{
			//	if (p.World == null)
			//	{
			//		World.Add(p);
			//	}
			//}
			
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
			
			FP.Camera.X += offsetX;
			FP.Camera.Y += offsetY;
		}
		
		public void AddPlayer(float x, float y, int id)
		{
			FP.Log(id);
			if (Players.Find(p => p.id == id) != null)	return;
			
			if (Players.Count < 4 && id <= 4)
			{
				Player p = new Player(x, y, id);
				Players.Add(p);
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
		
		public void OnPlayerDie(params object[] args)
		{
			foreach (Player p in Players)
			{
				if (p.IsAlive)
				{
					return;
				}
			}
			
			World.AddTween(new Alarm(1.0f, OnNextLevelTimer, ONESHOT), true);
		}
		
		public void OnNextLevelTimer()
		{
			(World as GameWorld).AdvanceLevel();
		}
		private void OnCameraShake(params object[] args)
		{
			FP.Log("SHAAAAAAKE");
			float str = args.Length > 0 ? (float)args[0] : 10.0f;
			float dur = args.Length > 1 ? (float)args[1] : 1.0f;
			
			// Get a random number [-1..1]
			float randX = ((float)FP.Rand(200) - 100.0f) / 100.0f;
			float randY = ((float)FP.Rand(200) - 100.0f) / 100.0f;
			
			// Scale it by the strength
			offsetX = str * (randX);
			offsetY = str * (randY);
			
			if (prevShaker != null)
			{
				prevShaker.Cancel();
			}
			
			var shaker = new MultiVarTween(OnShakeDone, Tween.ONESHOT);
			AddTween(shaker);
			shaker.Tween(this, new { offsetX = 0.0f, offsetY = 0.0f }, dur, Ease.ElasticOut);
			shaker.Start();
			
			prevShaker = shaker;
		}
		
		private void OnShakeDone()
		{
		}
	}
}
