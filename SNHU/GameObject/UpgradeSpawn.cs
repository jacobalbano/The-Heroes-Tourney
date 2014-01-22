using System;
using System.Text.RegularExpressions;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of UpgradeSpawn.
	/// </summary>
	public class UpgradeSpawn : Entity
	{
		Upgrade upgrade;
		Player owner;
		
		public UpgradeSpawn()
		{
			AddResponse("Upgrade Used", OnPlayerUsed);
		}
		
		private void OnPlayerUsed(params object[] args)
		{
			int id = (int)args[0];
			if(owner != null)
			{
				if(owner.PlayerId == id)
				{
					World.Remove(this);
				}
			}
		}
		
		public override void Added()
		{
			base.Added();
			
			var spawnChance = int.Parse(GameWorld.gameManager.Config["Upgrades", "SpawnChance"]);
			if (FP.Rand(100) < spawnChance)
			{
				var upgrades = Regex.Split(GameWorld.gameManager.Config["Upgrades", "Enabled"], ", ");
				
				var name = FP.Choose(upgrades);
				name = Regex.Replace(name, @"\s", "");
				var type = GetTypeFromAllAssemblies(name);
				if (type == null)
					throw new Exception(string.Format("Invalid upgrade type: '{0}'", name));
				
				upgrade = (Upgrade) type.GetConstructor(System.Type.EmptyTypes).Invoke(null);
				Graphic = upgrade.Icon;
				
				(Graphic as Image).CenterOO();
				SetHitboxTo(Graphic);
				CenterOrigin();
			}
			else
			{
				World.Remove(this);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (upgrade != null)
			{
				var p = Collide(Player.Collision, X, Y) as Player;
				if (p != null && p.Upgrade == null)
				{
					if(owner == null)
					{
						owner = p;
						p.SetUpgrade(upgrade);
						
						AddTween(new Alarm(2.0f, () => World.Remove(this), ONESHOT), true);
					}
				}
				
				if(owner != null)
				{
					X = owner.X;
					Y = owner.Top - 20;
				}
			}
		}
		
		/// <summary>
        /// Searches all known assemblies for a type and returns that type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>The type found.  Null if no match.</returns>
        public static Type GetTypeFromAllAssemblies(string type)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var t in types)
                {
                    if (t.Name == type)
                        return t;
                }
            }
            
            return null;
        }
	}
}
