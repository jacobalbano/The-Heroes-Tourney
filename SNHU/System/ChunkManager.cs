
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
			Unload,
		}
		
		private Chunk currentChunk;
		private Chunk nextChunk;
		
		private float position;
		
		public ChunkManager()
		{
			position = (FP.Camera.Y - FP.HalfHeight) - FP.Height;
			
			AddResponse(Message.Advance, OnAdvance);
			AddResponse(Message.Unload, OnUnload);
		}
		
		private void OnAdvance(params object[] args)
		{
			position -= FP.Height;
			
			if (currentChunk != null && currentChunk.World != null)
				World.Remove(currentChunk);
			
			nextChunk = new Chunk(0, position);
			World.Add(nextChunk);
			currentChunk = nextChunk;
		}
		
		private void OnAdvanceComplete(params object[] args)
		{
			AddResponse(Message.Advance, OnAdvance);
		}
		
		private void OnUnload(params object[] args)
		{
			World.Remove(currentChunk);
		}
	}
}
