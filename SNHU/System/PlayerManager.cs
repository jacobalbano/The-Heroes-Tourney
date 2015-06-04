using System;
using System.Collections.Generic;
using System.Linq;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Inputs;
using SNHU.Config;
using SNHU.GameObject;
using SNHU.MenuObject;

namespace SNHU.Systems
{
	/// <summary>
	/// Description of PlayerManager.
	/// </summary>
	public class PlayerManager : EntityManager<Player>
	{
		public enum Message
		{
			SpawnPlayers
		}
		
		public List<Player> ActivePlayers { get; private set; }
		private List<Player> AllPlayers;
		
		private int StartingLives;
		private int StartingHealth;
		private int PunchDamage;
		
		public PlayerManager()
		{
			var Config = Library.GetConfig<PlayerConfig>("config/player.ini");
			StartingLives = Config.StartingLives;
			StartingHealth = Config.StartingHealth;
			PunchDamage = Config.PunchDamage;
			
			ActivePlayers = new List<Player>();
			AllPlayers = new List<Player>();
			
			AddResponse(Message.SpawnPlayers, SpawnPlayers);
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
			AddResponse(ChunkManager.Message.AdvanceComplete, OnFinishAdvance);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (GameEnding)
			{
				var players = AllPlayers;
				if (ActivePlayers.Count != 0)
					players = ActivePlayers;
				
				foreach (var player in players)
					if (player.Start.Pressed) FP.World = new MenuWorld();
			}
		}
		
		public void CreatePlayers(Dictionary<int, string> playerImageNames)
		{
			foreach (var id in playerImageNames.Keys)
			{
				var imageName = playerImageNames[id];
				if (GamepadManager.GetSlot(id).IsConnected)
				{
					if (ActivePlayers.Find(p => p.ControllerId == id) != null) continue;
					if (ActivePlayers.Count < 4)
					{
						Player p = new Player(id, ActivePlayers.Count, imageName);
						
						p.Health = StartingHealth;
						p.PunchDamage = PunchDamage;
						p.Lives = StartingLives;
						
						ActivePlayers.Add(p);
						AllPlayers.Add(p);
					}
				}
			}
		}
		
		public override void EntityAdded(Player e)
		{
			base.EntityAdded(e);
			if (!ActivePlayers.Contains(e))
				ActivePlayers.Add(e);
		}
		
		public override void EntityRemoved(Player e)
		{
			base.EntityRemoved(e);
			
			if (e.Lives == 0)
			{
				if (Timeout != null)
					Timeout.Cancel();
				
				Timeout = World.Tweener.Timer(0.2f)
					.OnComplete(CheckWinner);
				
				ActivePlayers.Remove(e);
			}
			
			if (ActivePlayers.Count > 0)
			{
				var remaining = ActivePlayers.Count(p => p.IsAlive);
				if (remaining <= 1)
					World.Tweener.Timer(1).OnComplete(CheckAdvance);
			}
		}
		
		private void OnAdvance(params object[] args)
		{
			foreach (var player in ActivePlayers)
			{
				if (player.World != null)
					World.Remove(player);
			}
		}
		
		private void SpawnPlayers(params object[] args)
		{
			var SpawnPoints = args[0] as List<Entity>;
			FP.Random.Shuffle(SpawnPoints);
			
			foreach (var player in ActivePlayers)
			{
				if (player.Health <= 0)
					player.Health = StartingHealth;
				
				player.X = SpawnPoints[player.PlayerId].X;
				player.Y = SpawnPoints[player.PlayerId].Y;
				World.BroadcastMessage(HUD.Message.UpdateDamage, player);
				World.Add(player);
			}
			
			World.BroadcastMessage(ChunkManager.Message.AdvanceComplete);
		}
		
		private void CheckWinner()
		{
			if (GameEnding) return;
			
			if (ActivePlayers.Count <= 1)
			{
				var flash = Image.CreateRect(FP.Width, FP.Height, Color.White);
				flash.ScrollX = flash.ScrollY = 0;
				World.Tweener.Tween(flash, new { Alpha = 0 }, 1f)
					.Ease(Ease.SineOut);
				
				World.BroadcastMessage(ChunkManager.Message.Unload);
				World.AddGraphic(flash, 0, 0, ObjectLayers.Foreground);
				
				GameEnding = true;
			}
			
			if (ActivePlayers.Count == 1)
			{
				var winner = ActivePlayers[0];
				
				if (winner.World == null)
					World.Add(winner);
				
				World.Add(new VictoryEnding(winner));
				ControllerSelect.IncreaseWin(winner.ControllerId);
			}
			else if (ActivePlayers.Count == 0)
			{
				World.Add(new DrawEnding());
			}
			
			Timeout = null;
		}
		
		private void CheckAdvance()
		{
			if (!GameEnding && !Advancing)
			{
				World.BroadcastMessage(ChunkManager.Message.Advance);
				Advancing = true;
			}
		}
		
		private void OnFinishAdvance(params object[] args)
		{
			Advancing = false;
		}
		
		private Tween Timeout;
		private bool GameEnding, Advancing;
	}
}
