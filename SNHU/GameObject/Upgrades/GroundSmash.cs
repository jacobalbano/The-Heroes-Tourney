/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 2:50 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of GroundSmash.
	/// </summary>
	public class GroundSmash : Upgrade
	{
		public const string GROUND_SMASH = "groundSmash";
		public const float SMASH_RADIUS = 300.0f;
		const float FALL_SPEED = 50.0f;
		
		public GroundSmash()
		{
			AddResponse(Player.OnLand, OnPlayerLand);
		}
		
		public override void Use()
		{
			if (!Activated && !(Parent as Player).OnGround)
			{
				base.Use();
				
				(Parent as Player).physics.OnMessage(PhysicsBody.USE_GRAVITY, false);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Activated)
			{
				Parent.OnMessage(PhysicsBody.IMPULSE, 0, FALL_SPEED, true);
			}
		}
		
		public void OnPlayerLand(params object[] args)
		{
			if (Activated)
			{
				//Parent.World.AddGraphic(Image.CreateRect((int)SMASH_RADIUS * 2, 10, FP.Color(0xFFFFFF)), -9009, Parent.X - SMASH_RADIUS, Parent.Y);
				
				Parent.World.BroadcastMessage(GameManager.SHAKE, 100.0f, 0.5f);
				Parent.World.BroadcastMessage(GROUND_SMASH, Parent, SMASH_RADIUS);
				(Parent as Player).physics.OnMessage(PhysicsBody.USE_GRAVITY, true);
				
				(Parent as Player).SetUpgrade(null);
			}
		}
	}
}
