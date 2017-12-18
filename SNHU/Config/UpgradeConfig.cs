
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using SNHU.GameObject.Upgrades;

namespace SNHU.Config
{
	public class UpgradeConfig : IniConfig
	{
		public int SpawnChance = 75;
		
		public string Enabled = AllUpgradeNames();
		
		public float RespawnTime;
		public int MaxRespawns;
		
		[Ignore]
		public string[] EnabledUpgrades
		{
			get { return _upgrades ?? (_upgrades = Regex.Split(Enabled, ", ")) ; }
		}
		
		private string[] _upgrades;
		
		private static string AllUpgradeNames()
		{
			var upgrade = typeof(Upgrade);
			var disabled = typeof(Upgrade.DisabledInBuildAttribute);
			
			var types = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t != upgrade && upgrade.IsAssignableFrom(t))
				.Where(t => t.GetCustomAttributes(disabled, false).Length == 0)
				.Select(t => t.Name);
			
			return string.Join(", ", types);
		}
	}
}
