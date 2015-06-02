using System;
using System.Text.RegularExpressions;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils.Reflect;
using SNHU.Config;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of UpgradeSpawn.
	/// </summary>
	public class UpgradeSpawn : Entity
	{
		Upgrade upgrade;
		public float RespawnTime { get; private set; }
		public int MaxRespawns { get; private set; }
		public string[] Upgrades { get; private set; }
		private int numRespawns;
		
		public UpgradeSpawn()
		{
			numRespawns = -1;
			AddResponse(Upgrade.Message.Used, OnPlayerUsed);
		}
		
		private void OnPlayerUsed(params object[] args)
		{
		}
		
		public override void Added()
		{
			base.Added();
			
			// Determine if this upgrade spawner will be spawning upgrades for this chunk
			var config = Library.GetConfig<UpgradeConfig>("assets/config/upgrades.ini");
			Upgrades = config.EnabledUpgrades;
			if (FP.Random.Chance(config.SpawnChance))
			{
				RespawnTime = config.RespawnTime;
				MaxRespawns = config.MaxRespawns;
				
				SpawnUpgrade();
			}
			else
			{
				// If it won't be, remove it from the world
				World.Remove(this);
			}
		}
		
		private void SpawnUpgrade()
		{
			var name = FP.Choose.From(Upgrades);
			name = Regex.Replace(name, @"\s", "");
			var type = Reflect.GetTypeFromAllAssemblies(name);
			if (type == null)
				throw new Exception(string.Format("Invalid upgrade type: '{0}'", name));
			
			upgrade = (Upgrade) type.GetConstructor(System.Type.EmptyTypes).Invoke(null);
			AddComponent(upgrade.Icon);
			
			SetHitbox((int) upgrade.Icon.ScaledWidth, (int) upgrade.Icon.ScaledHeight);
			upgrade.Icon.CenterOO();
			CenterOrigin();
			
			numRespawns++;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (upgrade != null)
			{
				// If player collides with this spawner
				var p = Collide(Player.Collision, X, Y) as Player;
				if (p != null && p.CurrentUpgrade == null)
				{
					// Give the player the upgrade and remove the spawners <-- you spelled this with an apostrophe Chris :(
					p.SetUpgrade(upgrade);
					RemoveComponent(upgrade.Icon);
					upgrade = null;
					
					// If the max respawns is 0, remove it from the world now
					if (MaxRespawns == 0)
					{
						World.Remove(this);
					}
					else
					{
						// If the max respawns is infinite (< 0) or if we have not yet reached max respawns
						if (MaxRespawns < 0 || numRespawns < MaxRespawns)
						{
							// Start the respawn timer
							Tweener.Timer(RespawnTime).OnComplete(SpawnUpgrade);
						}
						else if (numRespawns >= MaxRespawns)
						{
							// If the respawn limit has been reached, remove the spawner
							World.Remove(this);
						}
					}
				}
			}
		}
	}
}
