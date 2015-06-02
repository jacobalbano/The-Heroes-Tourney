
using System;

namespace SNHU.Config
{
	public static class ObjectLayers
	{
		public static int Background = 1000;
		
		public static int Platforms = 100;
		
		public static int Upgrades = 50;
		
		public static int Players = JustAbove(Upgrades);
	
		public static int HUD = JustBelow(Foreground);
			
		public static int Foreground = -1000;
		
		public static int JustAbove(int layer) { return layer - 1; }
		public static int JustBelow(int layer) { return layer + 1; }
	}
}
