/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/16/2013
 * Time: 12:38 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of SpawnManager.
	/// </summary>
	public class SpawnManager : Entity
	{
		private List<Entity> spawnList;
		
		public SpawnManager()
		{
			AddResponse("player_die", onPlayerDie);
			spawnList = new List<Entity>();
		}
		
		private void onPlayerDie(params object[] args)
		{
			var player = args[0] as Player;
			spawnList.Clear();
			World.GetType(SpawnPoint.stringID,spawnList);
			for(int x = 0; x < spawnList.Count; x++)
			{
				if(!GameWorld.OnCamera(spawnList[x].X, spawnList[x].Y) && spawnList[x].Y > FP.Camera.Y - 20)
			   {
			   		spawnList.RemoveAt(x);
			   }
			}
			var spawn = FP.Choose(spawnList.ToArray());
			
			player.X = spawn.X;
			player.Y = spawn.Y;
		}
	}
}
