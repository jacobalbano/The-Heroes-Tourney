
using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Core;
using Indigo.Utils;
using SNHU.GameObject;

namespace SNHU.Systems
{
	public class CameraManager : Entity
	{
		public enum Message
		{
			/// <summary>
			/// Broadcast when the camera should shake
			/// Arguments: (float strength = 10.0f)
			/// </summary>
			Shake
		}
		
		private Point velocity;
		private Point position;
		private float drag = 0.1f;
		private float elasticity = 0.1f;
		
		public CameraManager(float x, float y) : base(x, y)
		{
			Type = "camerashake";
			
			position = new Point();
			velocity = new Point();
			
			AddResponse(Message.Shake, OnCameraShake);
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			velocity.X -= velocity.X * drag;
			velocity.Y -= velocity.Y * drag;
			
			velocity.X -= position.X * elasticity;
			velocity.Y -= position.Y * elasticity;
			
			position.X += velocity.X;
			position.Y += velocity.Y;
			
			World.Camera.X = X + (Math.Abs(position.X) < 0.5f ? 0f : position.X);
			World.Camera.Y = Y + (Math.Abs(position.Y) < 0.5f ? 0f : position.Y);
		}
		
		private void OnCameraShake(params object[] args)
		{
			float strength = args.Length > 0 ? 10f : Convert.ToSingle(args[0]);
			velocity = Point.FromAngle(Engine.Random.Angle(), strength / 3f);
		}
		
		private void OnAdvance(params object[] args)
		{
			World.Remove(this);
		}
		
		public override void Removed()
		{
			base.Removed();
			
			Engine.World.Camera.X = X;
			Engine.World.Camera.Y = Y;
		}
	}
}
