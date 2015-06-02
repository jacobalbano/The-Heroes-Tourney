
using System;
using Indigo.Content.Data;

namespace SNHU.Config
{
	public class PlayerConfig : IniConfig
	{
		public int DodgeDuration = 5;
		public int UpgradeCapacity = 1;
		public int StartingLives = 5;
		public int StartingHealth = 100;
		public int PunchDamage = 0;
	}
}
