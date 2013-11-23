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
using Punk.Tweens.Misc;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of SpawnManager.
	/// </summary>
	public class SpawnManager : Entity
	{
		private List<Entity> spawnList;
		private List<Entity> spawning;
		
		public SpawnManager()
		{
			AddResponse("player_die", onPlayerDie);
			spawnList = new List<Entity>();
			spawning = new List<Entity>();
		}
		
		private void onPlayerDie(params object[] args)
		{
			var player = args[0] as Player;
			
			if (spawning.Contains(player))	return;
			
			spawnList.Clear();
			World.GetType(SpawnPoint.stringID,spawnList);
			for(int x = 0; x < spawnList.Count; x++)
			{
				if(!GameWorld.OnCamera(spawnList[x].X, spawnList[x].Y) && Math.Abs(spawnList[x].Y - FP.Camera.Y) < FP.HalfHeight)
				{
					spawnList.RemoveAt(x);
				}
			}
			
			FP.Shuffle(spawnList);
			var spawn = FP.Choose(spawnList.ToArray());
			
			spawnList.Remove(spawn);
			World.AddTween(new Alarm(0.1f, () => OnFinishSpawn(spawn, player), ONESHOT), true);
			spawning.Add(player);
		}
		
		void OnFinishSpawn(Entity spawn, Player player)
		{
			player.X = spawn.X;
			player.Y = spawn.Y;
			spawnList.Add(spawn);
			spawning.Remove(player);
			
			var p = player.Collide(Player.Collision, X, Y) as Player;
			if (p != null)
			{
				p.dead();
			}
		}
	}
}
