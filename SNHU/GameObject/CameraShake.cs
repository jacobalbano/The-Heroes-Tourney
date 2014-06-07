
using System;
using System.Collections.Generic;
using GlideTween;
using Punk;
using Punk.Utils;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of CameraShake.
	/// </summary>
	public class CameraShake : Entity
	{
		
		public enum Message
		{
			/// <summary>
			/// Broadcast when the camera should shake
			/// Arguments: (float strength = 10.0f, float duration = 1.0f)
			/// </summary>
			Shake
		}
		
		private float OffsetX, OffsetY;
		private Glide shaker;
		
		public CameraShake(float x, float y) : base(x, y)
		{
			Type = "camerashake";
			
			AddResponse(Message.Shake, OnCameraShake);
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
		}
		
		public override void Update()
		{
			base.Update();
			
			FP.Camera.X = X + OffsetX;
			FP.Camera.Y = Y + OffsetY;
		}
		
		public override void Removed()
		{
			base.Removed();
			
			FP.Camera.X = X;
			FP.Camera.Y = Y;
		}
		
		private void OnCameraShake(params object[] args)
		{
			float str = args.Length > 0 ? Convert.ToSingle(args[0]) : 10.0f;
			float dur = args.Length > 1 ? Convert.ToSingle(args[1]) : 1.0f;
			
			// Get a random number [-1..1]
			float randX = ((float) FP.Rand(200) - 100.0f) / 100.0f;
			float randY = ((float) FP.Rand(200) - 100.0f) / 100.0f;
			
			// Scale it by the strength
			OffsetX = str * (randX);
			OffsetY = str * (randY);
			
			if (shaker != null) shaker.Cancel();
			shaker = Tweener.Tween(this, new { OffsetX = 0.0f, OffsetY = 0.0f }, dur)
				.Ease(Ease.ElasticOut)
				.OnComplete(() => shaker = null);
		}
		
		private void OnAdvance(params object[] args)
		{
			World.Remove(this);
		}
	}
}
