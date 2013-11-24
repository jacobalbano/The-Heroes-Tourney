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
using Punk.Tweens.Motion;
using Punk.Utils;
using Punk.Graphics;

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
		public bool GameEnding { get; private set; }
		
		private Music GameMusic;
		
		public List<Player> Players;
		public int PlayersInMatch { get; private set; }
		
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
			GameEnding = false;
			
			GameMusic = Mixer.music;
			
			Players = new List<Player>();
			hud = new HUD(this);
			
			meteorTimer = new Alarm(METEOR_TIME * meteorTimeScale, OnMeteor, Tween.ONESHOT);
			meteorMode = false;
			
			FP.Camera.Zoom = 0.75f;
			
			AddResponse("player_die", OnPlayerDie);
			AddResponse("player_lose", OnPlayerLose);
			AddResponse(SHAKE, OnCameraShake);
		}
		
		public override void Added()
		{
			base.Added();
			
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
		
		public void AddPlayer(float x, float y, uint jid, string imageName)
		{
			if (Players.Find(p => p.jid == jid) != null)	return;
			
			if (Players.Count < 4)
			{
				Player p = new Player(x, y, jid, Players.Count, imageName);
				Players.Add(p);
			}
		}
		
		public void StartGame()
		{
			if (!GameStarted)
			{
				GameStarted = true;
				GameMusic.Play();
				PlayersInMatch = Players.Count;
				(World as GameWorld).AdvanceLevel();
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
			// If there is more than one player playing...
			if (PlayersInMatch > 1)
			{
				// Find out how many players are still alive...
				int remainingPlayers = 0;
				foreach (Player p in Players)
				{
					if (p.IsAlive)
					{
						remainingPlayers++;
					}
				}
				
				// If there is more than one player alive, don't go to the next level.
				if (remainingPlayers > 1)
				{
					return;
				}
			}
			
			World.AddTween(new Alarm(1.0f, OnNextLevelTimer, ONESHOT), true);
		}
		
		public void OnPlayerLose(params object[] args)
		{
			List<Player> remainingPlayers = new List<Player>();
			foreach (Player p in Players)
			{
				if (p.Lives > 0)
				{
					remainingPlayers.Add(p);
				}
			}
			
			if (remainingPlayers.Count == 1)
			{
				GameEnding = true;
				(World as GameWorld).UnloadCurrentChunk();
				remainingPlayers[0].Active = false;
				remainingPlayers[0].Layer = -9002;
				remainingPlayers[0].SetGlovesLayer(-9002);
				
				// YOU WIN!
				FP.Log("PLAYER ", remainingPlayers[0].id, " WINS!");
				
				Image black = Image.CreateRect(FP.Width, FP.Height, FP.Color(0x00000000));
				black.Alpha = 0.0f;
				black.ScrollX = black.ScrollY = 0;
				World.AddGraphic(black, -9001, 0, 0);
				
				var blackTween = new VarTween(null, ONESHOT);
				blackTween.Tween(black, "Alpha", 1.0f, 0.5f);
				World.AddTween(blackTween, true);
				
				Text txt = new Text("     PLAYER " + (remainingPlayers[0].id + 1) + "\nIS THE TRUE HERO!!!");
				txt.Size = 64;
				txt.ScrollX = txt.ScrollY = 0;
				World.AddGraphic(txt, -9001, 0, 50);
				
				var txtTween = new VarTween(null, ONESHOT);
				txtTween.Tween(txt, "X", FP.HalfWidth - txt.Width * 2, 0.25f, Ease.BounceOut);
				AddTween(txtTween, true);
				
				var playerTween = new MultiVarTween(null, ONESHOT);
				playerTween.Tween(remainingPlayers[0], new { X = FP.Camera.X, Y = FP.Camera.Y },
				                  1.5f, Ease.ElasticOut);
				AddTween(playerTween, true);
				
				World.Add(new Victory(remainingPlayers[0].Layer + 1));
				
			}
			else if (remainingPlayers.Count <= 0)
			{
				GameEnding = true;
				(World as GameWorld).UnloadCurrentChunk();
				
				// DRAW!
				FP.Log("IT'S A DRAW!");
				
				Image black = Image.CreateRect(FP.Width, FP.Height, FP.Color(0x00000000));
				black.Alpha = 0.0f;
				black.ScrollX = black.ScrollY = 0;
				World.AddGraphic(black, -9001, 0, 0);
				
				var blackTween = new VarTween(null, ONESHOT);
				blackTween.Tween(black, "Alpha", 1.0f, 0.5f);
				World.AddTween(blackTween, true);
				
				Text txt = new Text("IT'S A DRAW!");
				txt.Size = 64;
				txt.ScrollX = txt.ScrollY = 0;
				World.AddGraphic(txt, -9001, 0, 50);
				
				var txtTween = new VarTween(null, ONESHOT);
				txtTween.Tween(txt, "X", FP.HalfWidth - txt.Width * 2, 0.25f, Ease.BounceOut);
				AddTween(txtTween, true);
			}
			else if (remainingPlayers.Count > 1)
			{
				return;
			}
		}
		
		public void OnNextLevelTimer()
		{
			(World as GameWorld).AdvanceLevel();
		}
		
		private void OnCameraShake(params object[] args)
		{
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
