
using System;
using System.Collections.Generic;
using Punk;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of ChunkManager.
	/// </summary>
	public class ChunkManager : Entity
	{
		public const string Advance = "chunkManager_advance";
		public const string AdvanceComplete = "chunkManager_advanceComplete";
		public const string UnloadCurrent = "chunkManager_unloadCurrentChunk";
		public const string SpawnPlayers = "chunkManager_spawnPlayers";
		
		private Chunk currentChunk;
		private Chunk nextChunk;
		
		private float x, y;
		
		public ChunkManager()
		{
			y = (FP.Camera.Y - FP.HalfHeight) - FP.Height;
			
			AddResponse(Advance, OnAdvance);
			AddResponse(UnloadCurrent, OnUnloadCurrent);
			AddResponse(SpawnPlayers, OnSpawnPlayers);
		}
		
		private void OnAdvance(params object[] args)
		{
			var gameManager = GameWorld.gameManager;
			if (!gameManager.GameEnding)
			{
				RemoveResponse(Advance, OnAdvance);
				foreach (var player in gameManager.Players)
				{
					if (player.IsAlive)
					{
						World.Remove(player);
					}
				}
				
				y -= FP.Height;
				
				nextChunk = new Chunk(x, y);
				World.Add(nextChunk);
				
				if (currentChunk != null && currentChunk.World != null)
				{
					World.Remove(currentChunk);
				}
				
				currentChunk = nextChunk;
			}
		}
		
		private void OnSpawnPlayers(params object[] args)
		{
			FP.Shuffle(currentChunk.SpawnPoints);
			
			foreach (var player in GameWorld.gameManager.Players)
			{
				if (player.Lives > 0)
				{
					if (player.Health <= 0) player.Health = GameWorld.gameManager.StartingHealth;
					player.X = currentChunk.SpawnPoints[player.PlayerId].X;
					player.Y = currentChunk.SpawnPoints[player.PlayerId].Y;
					World.Add(player);
					World.BroadcastMessage(HUD.UpdateDamage, player);
				}
			}
			
			AddResponse(Advance, OnAdvance);
			World.BroadcastMessage(AdvanceComplete);
		}
		
		private void OnUnloadCurrent(params object[] args)
		{
			World.Remove(currentChunk);
		}
	}
}
