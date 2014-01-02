﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SNHU.MenuObject;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;

namespace SNHU
{
	/// <summary>
	/// Description of MenuWorld.
	/// </summary>
	public class MenuWorld : World
	{
		private Dictionary<uint, ControllerSelect> controllerMenus;
		private Stack<int> pool;
		
		private List<string> allImages, takenImages;
		
		private bool readying;
		
		private GameWorld gameWorld;
		
		public MenuWorld()
		{
			allImages = LoadAllPlayers();
			takenImages = new List<string>();
			
			controllerMenus = new Dictionary<uint, ControllerSelect>();
			pool = new Stack<int>();
			
			for (uint i = 0; i < Joystick.Count; i++)
			{
				if (Joystick.IsConnected(i))
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
			Input.ControllerConnected += OnControllerAdded;
			Input.ControllerDisconnected += OnControllerRemoved;
		}
		
		public override void End()
		{
			base.End();
			Input.ControllerConnected -= OnControllerAdded;
			Input.ControllerDisconnected -= OnControllerRemoved;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Input.Down(Keyboard.Key.LAlt) && Input.Pressed(Keyboard.Key.F4))
			{
				FP.Screen.Close();
			}
			
			if (Input.Pressed(Keyboard.Key.F5))
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
					var tween = new VarTween(null, ONESHOT);
					tween.Tween(menus[i], "Y", -FP.Height, 0.25f, Ease.QuadOut);
					AddTween(tween, true);
				}
				
				AddTween(new Alarm(0.25f, Ready, ONESHOT), true);
			}
		}
		
		void Ready()
		{
			var i = Image.CreateRect(FP.Width, FP.Height, FP.Color(0));
			i.Alpha = 0;
			
			AddGraphic(i, -100);
			
			var tween = new VarTween(AllReady, ONESHOT);
			tween.Tween(i, "Alpha", 1, 0.25f, Ease.SineOut);
			AddTween(tween, true);
		}
		
		void AllReady()
		{
			var playerGraphics = new Dictionary<uint, string>();
			foreach (var menu in controllerMenus.Values)
			{
				playerGraphics[menu.JoyId] = menu.PlayerImageName;
			}
			
			FP.World = gameWorld = new GameWorld();
			gameWorld.Init(playerGraphics);
		}
		
		private void OnControllerAdded(object sender, JoystickConnectEventArgs e)
		{
			if (controllerMenus.ContainsKey(e.JoystickId))	return;
			
			AddController(e.JoystickId);
		}
		
		void AddController(uint joystickId)
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
			
			var tween = new VarTween(null, ONESHOT);
			tween.Tween(menu, "X", 10 + (menu.Width / 2) + slot * menu.Width, 1.6f + FP.Random, Ease.ElasticOut);
			AddTween(tween, true);
		}
		
		private int GetSlot()
		{
			if (pool.Count > 0)
			{
				return pool.Pop();
			}
			
			return controllerMenus.Count;
		}
		
		private void OnControllerRemoved(object sender, JoystickConnectEventArgs e)
		{
			if (!controllerMenus.ContainsKey(e.JoystickId))	return;
			
			var menu = controllerMenus[e.JoystickId];
			controllerMenus.Remove(e.JoystickId);
			
			pool.Push(menu.Slot);
			
			var tween = new VarTween(() => Remove(menu), ONESHOT);
			tween.Tween(menu, "Y", -menu.Height, 0.75f + FP.Random / 2, Ease.ElasticOut);
			AddTween(tween, true);
		}
		
		public string GetImageName()
		{
			var choice = "";
			do {
				choice = FP.Choose(allImages.ToArray());
			} while (takenImages.IndexOf(choice) >= 0);
			
			takenImages.Add(choice);
			return choice;
		}
		
		public string NextImage(string current)
		{
			var c = current;
			var choice = "";
			do {
				choice = FP.Next(c, allImages, true);
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
				choice = FP.Prev(c, allImages, true);
				c = choice;
			} while (takenImages.IndexOf(choice) >= 0);
			
			takenImages.Remove(current);
			takenImages.Add(choice);
			return choice;
		}
	}
}
