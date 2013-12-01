
using System;
using System.Collections.Generic;
using System.Threading;
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
		public const string PreloadNext = "chunkManager_preloadNext";
		public const string UnloadCurrent = "chunkManager_unloadCurrentChunk";
		public const string SpawnPlayers = "chunkManager_spawnPlayers";
		
		private Chunk currentChunk;
		private Chunk nextChunk;
		private Thread chunkLoader;
		
		public ChunkManager()
		{
			AddResponse(Advance, OnAdvance);
			AddResponse(PreloadNext, OnPreloadNext);
			AddResponse(UnloadCurrent, OnUnloadCurrent);
			AddResponse(SpawnPlayers, OnSpawnPlayers);
		}
		
		private void Loader()
		{
			float x = 180, y = 0;
			if (currentChunk == null)
			{
				y = (FP.Camera.Y - FP.HalfHeight) - FP.Height;
			}
			else
			{
				y = currentChunk.Y - FP.Height;	
			}
			
			nextChunk = new Chunk(x, y);
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
				
				var all = new List<Entity>();
				World.GetType("camerashake", all);
				World.RemoveList(all);
				
				chunkLoader.Join();
				OnPreloadNext();
				
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
					player.X = currentChunk.SpawnPoints[player.PlayerId].X;
					player.Y = currentChunk.SpawnPoints[player.PlayerId].Y;
					World.Add(player);
				}
			}
			
			AddResponse(Advance, OnAdvance);
			World.BroadcastMessage(AdvanceComplete);
		}
		
		private void OnUnloadCurrent(params object[] args)
		{
			World.Remove(currentChunk);
		}
		
		private void OnPreloadNext(params object[] args)
		{
			chunkLoader = new Thread(Loader);
			chunkLoader.Start();
		}
	}
}
