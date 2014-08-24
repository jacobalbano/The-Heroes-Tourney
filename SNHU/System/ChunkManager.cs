
using System;
using System.Collections.Generic;
using Indigo;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of ChunkManager.
	/// </summary>
	public class ChunkManager : Entity
	{
		public enum Message
		{
			Advance,
			AdvanceComplete,
			UnloadCurrent,
			SpawnPlayers
		}
		
		private Chunk currentChunk;
		private Chunk nextChunk;
		
		private float y;
		
		public ChunkManager()
		{
			y = (FP.Camera.Y - FP.HalfHeight) - FP.Height;
			
			AddResponse(Message.Advance, OnAdvance);
			AddResponse(Message.UnloadCurrent, OnUnloadCurrent);
			AddResponse(Message.SpawnPlayers, OnSpawnPlayers);
		}
		
		private void OnAdvance(params object[] args)
		{
			var gameManager = GameWorld.gameManager;
			if (!gameManager.GameEnding)
			{
				RemoveResponse(Message.Advance, OnAdvance);
				foreach (var player in gameManager.Players)
				{
					if (player.IsAlive)
					{
						World.Remove(player);
					}
				}
				
				y -= FP.Height;
				
				nextChunk = new Chunk(0, y);
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
					World.BroadcastMessage(HUD.Message.UpdateDamage, player);
				}
			}
			
			AddResponse(Message.Advance, OnAdvance);
			World.BroadcastMessage(Message.AdvanceComplete);
		}
		
		private void OnUnloadCurrent(params object[] args)
		{
			World.Remove(currentChunk);
		}
	}
}
