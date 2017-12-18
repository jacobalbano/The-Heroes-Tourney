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
			var result = Library.GetFilenames("players/", "*.ini")
				.ToList();
			
			var regex = new Regex(@"players/(?<Name>.+).ini");
			
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
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			if (Keyboard.R.Pressed)
				Engine.World = new MenuWorld();
			
			if (readying)	return;
			if (controllerMenus.Count == 0) return;
			
			int total = controllerMenus.Count, ready = 0, started = 0;
			foreach (var menu in controllerMenus.Values)
			{
				if (menu.Ready) ready++;
				if (menu.Started) started++;
			}
			
			if ((started > 1 && ready == started) || total == ready)
			{
				readying = true;
				var menus = controllerMenus.Values
					.Where(menu => menu.Ready)
					.ToList();
				
				for (int i = 0; i < menus.Count; i++)
				{
					Tweener.Tween(menus[i], new { Y = -Engine.Height }, 0.25f)
						.Ease(Ease.QuadOut);
				}
				
				Tweener.Timer(0.25f).OnComplete(Ready);
			}
		}
		
		void Ready()
		{
			var i = Image.CreateRect(Engine.Width, Engine.Height, new Color());
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
				if (!menu.Ready) continue;
				playerGraphics[menu.JoyId] = menu.PlayerImageName;
				ControllerSelect.SetLastSkin(menu.PlayerSlot, menu.PlayerImageName);
			}

			Engine.World = new GameWorld(playerGraphics);
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
				return;
			
			var menu = new ControllerSelect(this, slot, joystickId);
			
			menu.X = -menu.Width;
			
			controllerMenus[joystickId] = menu;
			Add(menu);
			
			Tweener.Tween(menu, new { X = 10 + (menu.Width / 2) + slot * menu.Width}, 1.6f + Engine.Random.Float())
				.Ease(Ease.ElasticOut);
		}
		
		private int GetSlot()
		{
			if (pool.Count > 0)
				return pool.Pop();
			
			return controllerMenus.Count;
		}
		
		private void OnControllerRemoved(int joystickID)
		{
			if (!controllerMenus.ContainsKey(joystickID))
				return;
			
			var menu = controllerMenus[joystickID];
			controllerMenus.Remove(joystickID);
			
			pool.Push(menu.PlayerSlot);
			
			Tweener.Tween(menu, new { Y = -menu.Height}, 0.75f + Engine.Random.Float() / 2f)
				.Ease(Ease.ElasticOut)
				.OnComplete(() => Remove(menu));
		}
		
		public string GetImageName(string preferredChoice)
		{
			var choice = preferredChoice;
			if (choice == null || takenImages.IndexOf(choice) >= 0)
			{
				do {
					choice = Engine.Choose.From(allImages);
				} while (takenImages.IndexOf(choice) >= 0);
			}
			
			takenImages.Add(choice);
			return choice;
		}
		
		public string NextImage(string current)
		{
			var c = current;
			var choice = "";
			do {
				choice = Engine.Choose.Next(c, allImages, true);
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
				choice = Engine.Choose.Prev(c, allImages, true);
				c = choice;
			} while (takenImages.IndexOf(choice) >= 0);
			
			takenImages.Remove(current);
			takenImages.Add(choice);
			return choice;
		}
	}
}
