/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/16/2013
 * Time: 12:43 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using SFML.Graphics;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of GameManager.
	/// </summary>
	public class GameManager : Entity
	{
		private MatchTimer matchTimer;
		
		public GameManager()
		{
			matchTimer = new MatchTimer(240.0f);
		}
		
		public override void Added()
		{
			base.Added();
			
			World.Add(matchTimer);
		}
		
		public override void Removed()
		{
			World.Remove(matchTimer);
			
			base.Removed();
		}
		
		public void StartGame()
		{
			matchTimer.Timer.Start();
		}
	}
}
