using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Glide;
using Indigo.Inputs;
using SNHU.Config;
using SNHU.MenuObject;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;

namespace SNHU
{
	/// <summary>
	/// Description of MenuWorld.
	/// </summary>
	public class MenuWorld : World
	{
		private Dictionary<int, ControllerSelect> controllerMenus;
		private Stack<int> pool;
		
		private List<string> allImages, takenImages;
		
		private bool readying;
		
		private GameWorld gameWorld;
		
		public MenuWorld()
		{
			allImages = LoadAllPlayers();
			takenImages = new List<string>();
			
			controllerMenus = new Dictionary<int, ControllerSelect>();
			pool = new Stack<int>();
			
			for (int i = 0; i < 4; i++)
			{
				if (GamepadManager.GetSlot(i).IsConnected)
				{
					AddController(i);
				}
			}
		}
		
		List<string> LoadAllPlayers()
		{
			List<string> result = new List<string>();
			
			try
			{
				result.AddRange(Directory.GetFiles("assets/players", "*.ini"));
			}
			catch
			{
				FP.Log("error");
				// Could not open the directory for some inexplicable reason
			}
			
			var regex = new Regex(@"assets/players/(?<Name>.+).ini");
			
			for (int i = 0; i < result.Count; i++)
			{
				var file = result[i];
				
				file = file.Replace('\\', '/');
				
				var match = regex.Match(file);
				
				file = match.Groups["Name"].Value;
				result[i] = file;
			}
			
			return result;
		}
		
		public override void Begin()
		{
			base.Begin();
			
			GamepadManager.Connected += OnControllerAdded;
			GamepadManager.Disconnected += OnControllerRemoved;
		}
		
		public override void End()
		{
			base.End();
			
			GamepadManager.Connected -= OnControllerAdded;
			GamepadManager.Disconnected -= OnControllerRemoved;
		}
		
		public override void Update()
		{
			base.Update();
			
			if ((Keyboard.LAlt.Down || Keyboard.RAlt.Down) && Keyboard.F4.Pressed)
			{
				FP.Engine.Quit();
			}
			
			if (Keyboard.F.Pressed)
			{
				FP.Screen.Fullscreen = !FP.Screen.Fullscreen;
			}
			
			if (Keyboard.R.Pressed)
			{
				FP.World = new MenuWorld();
			}
			
			if (readying)	return;
			
			foreach (var menu in controllerMenus.Values)
			{
				if (!menu.Ready)
				{
					return;
				}
			}
			
			if (controllerMenus.Count > 0)
			{
				readying = true;
				var menus = new List<ControllerSelect>(controllerMenus.Values);
				
				for (int i = 0; i < menus.Count; i++)
				{
					Tweener.Tween(menus[i], new { Y = -FP.Height }, 0.25f)
						.Ease(Ease.QuadOut);
				}
				
				Tweener.Timer(0.25f).OnComplete(Ready);
			}
		}
		
		void Ready()
		{
			var i = Image.CreateRect(FP.Width, FP.Height, new Color());
			i.Alpha = 0;
			
			AddGraphic(i, 0, 0, ObjectLayers.Foreground);
			
			Tweener.Tween(i, new { Alpha = 1 }, 0.25f)
				.Ease(Ease.SineOut)
				.OnComplete(AllReady);
		}
		
		void AllReady()
		{
			var playerGraphics = new Dictionary<int, string>();
			foreach (var menu in controllerMenus.Values)
			{
				playerGraphics[menu.JoyId] = menu.PlayerImageName;
			}
			
			FP.World = new GameWorld(playerGraphics);
		}
		
		private void OnControllerAdded(int joystickID)
		{
			if (controllerMenus.ContainsKey(joystickID))
				return;
			
			AddController(joystickID);
		}
		
		void AddController(int joystickId)
		{
			var slot = GetSlot();
			if (slot > 3)
			{
				return;
			}
			
			var menu = new ControllerSelect(this, slot, joystickId);
			
			menu.X = -menu.Width;
			
			controllerMenus[joystickId] = menu;
			Add(menu);
			
			Tweener.Tween(menu, new { X = 10 + (menu.Width / 2) + slot * menu.Width}, 1.6f + FP.Random.Float())
				.Ease(Ease.ElasticOut);
		}
		
		private int GetSlot()
		{
			if (pool.Count > 0)
			{
				return pool.Pop();
			}
			
			return controllerMenus.Count;
		}
		
		private void OnControllerRemoved(int joystickID)
		{
			if (!controllerMenus.ContainsKey(joystickID))
				return;
			
			var menu = controllerMenus[joystickID];
			controllerMenus.Remove(joystickID);
			
			pool.Push(menu.PlayerSlot);
			
			Tweener.Tween(menu, new { Y = -menu.Height}, 0.75f + FP.Random.Float() / 2f)
				.Ease(Ease.ElasticOut)
				.OnComplete(() => Remove(menu));
		}
		
		public string GetImageName()
		{
			var choice = "";
			do {
				choice = FP.Choose.From(allImages);
			} while (takenImages.IndexOf(choice) >= 0);
			
			takenImages.Add(choice);
			return choice;
		}
		
		public string NextImage(string current)
		{
			var c = current;
			var choice = "";
			do {
				choice = FP.Choose.Next(c, allImages, true);
				c = choice;
			} while (takenImages.IndexOf(choice) >= 0);
			
			takenImages.Remove(current);
			takenImages.Add(choice);
			return choice;
		}
		
		public string PrevImage(string current)
		{
			var c = current;
			var choice = "";
			do {
				choice = FP.Choose.Prev(c, allImages, true);
				c = choice;
			} while (takenImages.IndexOf(choice) >= 0);
			
			takenImages.Remove(current);
			takenImages.Add(choice);
			return choice;
		}
	}
}
