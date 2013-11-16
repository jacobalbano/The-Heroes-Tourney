/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 7/26/2013
 * Time: 10:01 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using SFML.Window;
using SNHU.GameObject;

namespace SNHU.Components
{
	/// <summary>
	/// Description of PhysicsBody.
	/// </summary>
	public class PhysicsBody : Logic
	{
		/// <summary>
		/// <para>Used to send a physics impulse.</para>
		/// <para>Arguments: (float X = 0, float Y = 0, bool absolute = false).</para>
		/// <para>Arguments' value will be added to the movement vector.</para>
		/// <para>If absolute, the movement vector will be set to the values of the arguments for any value but zero.</para>
		/// </summary>
		public const string IMPULSE = "impulse";
		
		/// <summary>
		/// <para>Used to set whether the body will move when impulses are applied.</para>
		/// <para>Arguments: (bool fixPosition)</para>
		/// </summary>
		public const string FIX_POSITION = "fixPosition";
		
		/// <summary>
		/// <para>Used to set whether gravity is active on the body.</para>
		/// <para>Arguments: (bool useGravity)</para>
		/// </summary>
		public const string USE_GRAVITY = "useGravity";
		
		/// <summary>
		/// <para>Used to apply friction to the body.</para>
		/// <para>Arguments: (float frictionFactor)</para>
		/// <para>1 means no slowdown, 0 means complete slowdown.</para>
		/// </summary>
		public const string FRICTION = "friction";
		
		// Movement/Physics
		/// <summary>
		/// The types to collide against.
		/// </summary>
		public List<string> Colliders;
		
		/// <summary>
		/// The value that will be accumulated as the body accelerates downwards.
		/// </summary>
		public float Gravity;
		
		/// <summary>
		/// The amount that the body has moved since the last update.
		/// </summary>
		public Vector2f MoveDelta;
		public Vector2f LastMoveDelta;
		
		private Vector2f movement;
		private bool hasGravity;
		private bool canMove;
		
		private bool applyGroundFriction;
		private float frictionFactor;
		private const float airFriction = 0.9f;
		
		public PhysicsBody()
		{
			Colliders = new List<string>();
			MoveDelta = new Vector2f();
			Gravity = 0.95f;
			
			movement = new Vector2f();
			hasGravity = true;
			canMove = true;
			
			AddResponse(IMPULSE, OnImpulse);
			AddResponse(FIX_POSITION, OnFixPosition);
			AddResponse(USE_GRAVITY, OnUseGravity);
			AddResponse(FRICTION, OnApplyFriction);
			AddResponse(Player.OnLand, OnLand);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (hasGravity)
			{
				movement.Y += Gravity;
			}
			
			if (!canMove)
			{
				movement.X = 0;
			}
			
			if (applyGroundFriction)
			{
				movement.X *= frictionFactor;
				applyGroundFriction = false;
			}
			else
			{
				movement.X *= airFriction;
			}
			
			float x = Parent.X, y = Parent.Y;
			Parent.MoveBy(movement.X, movement.Y, Colliders, true);
			
			LastMoveDelta.X = MoveDelta.X;
			LastMoveDelta.Y = MoveDelta.Y;
			
			MoveDelta.X = Parent.X - x;
			MoveDelta.Y = Parent.Y - y;
			
			//	stop movement if we hit a wall or something
			if (MoveDelta.X == 0)
			{
				movement.X = 0;
			}
		}
		
		private void OnImpulse(params object[] args)
		{
			float x = args.Length > 0 ? Convert.ToSingle(args[0]) : 0;
			float y = args.Length > 1 ? Convert.ToSingle(args[1]) : 0;
			
			if (args.Length > 2 && (bool) args[2])
			{
				movement.X = x == 0 ? movement.X : x;
				movement.Y = y == 0 ? movement.Y : y;
			}
			else
			{
				movement.X += x;
				movement.Y += y;
			}
		}
		
		private void OnFixPosition(params object[] args)
		{
			if (args.Length == 0)
				throw new Exception("Must supply a value!");
			
			canMove = !((bool) args[0]);
		}
		
		private void OnUseGravity(params object[] args)
		{
			if (args.Length == 0)
				throw new Exception("Must supply a value!");
			
			hasGravity = (bool) args[0];
			
			if (!hasGravity)
				movement.Y = 0;
		}
		
		private void OnLand(params object[] args)
		{
			movement.Y = 0;
		}
		
		private void OnApplyFriction(params object[] args)
		{
			frictionFactor = Convert.ToSingle(args[0]);
			applyGroundFriction = true;
		}
	}
}