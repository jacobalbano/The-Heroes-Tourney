using System;
using System.Collections.Generic;
using System.Linq;
using Glide;
using Indigo;
using Indigo.Audio;
using Indigo.Utils;
using Indigo.Graphics;
using SNHU.MenuObject;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of GameManager.
	/// </summary>
	public class GameManager : Entity
	{
		public bool GameStarted { get; private set; }
		public bool GamePaused { get; private set; }
		public bool GameEnding { get; private set; }
		public bool NobodyWon { get; private set; }
		
		public int StartingLives { get; private set; }
		public int StartingHealth { get; private set; }
		public int PunchDamage { get; private set; }
		
		private Sound GameMusic;
		private GameWorld gameWorld;
		
		public List<Player> Players;
		public int PlayersInMatch { get; private set; }
		
		private HUD hud;
		
		public Ini Config { get; private set; }
		
		public GameManager()
		{	
			GameStarted = false;
			GamePaused = false;
			GameEnding = false;
			
			Config = new Ini(Library.GetText("assets/cfg.ini"));
			StartingLives = Math.Max(1, int.Parse(Config["Game", "Lives"]));
			StartingHealth = int.Parse(Config["Game", "Health"]);
			PunchDamage = int.Parse(Config["Damage", "Punch"]);
			
			GameMusic = Mixer.music;
			
			Players = new List<Player>();
			hud = new HUD(this);
			
			AddResponse(Player.Message.Die, OnPlayerDie);
			AddResponse(Player.Message.Lose, OnPlayerLose);
		}
		
		public override void Added()
		{
			base.Added();

			World.Add(hud);
			gameWorld = World as GameWorld;
		}
		
		public override void Removed()
		{
			World.Remove(hud);
			
			base.Removed();
		}
		
		public override void Update()
		{
			base.Update();
			
			if (GameEnding)
			{
				foreach (var p in Players)
				{
					if (p.Start.Pressed)
					{
						if (NobodyWon || p.Lives > 0)
						{
							FP.World = new MenuWorld();
						}
					}
				}
			}
		}
		
		public void AddPlayer(float x, float y, int controllerId, string imageName)
		{
			if (Players.Find(p => p.ControllerId == controllerId) != null)	return;
			
			if (Players.Count < 4)
			{
				Player p = new Player(x, y, controllerId, Players.Count, imageName);
				Players.Add(p);
				hud.AddPlayer(p);
			}
		}
		
		public void StartGame()
		{
			if (!GameStarted)
			{
				GameStarted = true;
				GameMusic.Looping = true;
				GameMusic.Play();
				PlayersInMatch = Players.Count;
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
			
			World.Tweener.Timer(1).OnComplete(() => World.BroadcastMessage(ChunkManager.Message.Advance));
		}
		
		public void OnPlayerLose(params object[] args)
		{
			World.Tweener.Timer(FP.Elapsed)
				.OnComplete(CheckWinner);
		}
		
		private void CheckWinner()
		{
			var remainingPlayers = Players
				.Where(p => p.Lives > 0)
				.ToList();
			
			if (remainingPlayers.Count <= 1)
			{
				Image black = Image.CreateRect(FP.Width, FP.Height, FP.Color(0x00000000));
				black.Alpha = 0.0f;
				black.ScrollX = black.ScrollY = 0;
				World.AddGraphic(black, -9001, 0, 0);
				
				World.Tweener.Tween(black, new { Alpha = 1 }, 0.5f);
			}
			
			if (remainingPlayers.Count == 1)
			{
				var winner = remainingPlayers[0];
				if (winner.World == null)
				{
					World.Add(winner);
				}
				
				GameEnding = true;
				World.BroadcastMessage(ChunkManager.Message.UnloadCurrent);
				winner.Active = false;
				winner.Layer = -9002;
				winner.SetGlovesLayer(-9002);
				
				// YOU WIN!
				Text txt = new Text("     PLAYER " + (winner.PlayerId + 1) + "\nIS THE TRUE HERO!!!");
				txt.Size = 64;
				txt.ScrollX = txt.ScrollY = 0;
				txt.CenterOrigin();
				World.AddGraphic(txt, -9001, 0, 50);
				
				Tweener.Tween(txt, new { X = FP.HalfWidth }, 0.25f)
					.Ease(Ease.BounceOut);
				
				Tweener.Tween(winner, new { X = FP.Camera.X, Y = FP.Camera.Y }, 1.5f)
					.Ease(Ease.ElasticOut);
				
				World.Add(new Victory(winner.Layer + 1));
				
				ControllerSelect.IncreaseWin(winner.ControllerId);
			}
			else if (remainingPlayers.Count <= 0)
			{
				GameEnding = true;
				World.BroadcastMessage(ChunkManager.Message.UnloadCurrent);
				
				// DRAW!
				Text txt = new Text("IT'S A DRAW!");
				txt.Size = 64;
				txt.ScrollX = txt.ScrollY = 0;
				txt.CenterOrigin();
				World.AddGraphic(txt, -9001, 0, 50);
				
				World.Tweener.Tween(txt, new { X = FP.HalfWidth }, 0.25f)
					.Ease(Ease.BounceOut);
				
				NobodyWon = true;
			}
			else if (remainingPlayers.Count > 1)
			{
				return;
			}
		}
	}
}
