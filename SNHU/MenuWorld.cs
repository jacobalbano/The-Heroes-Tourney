
using System;
using System.Collections.Generic;
using SNHU.menuobject;
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
		
		public MenuWorld()
		{
			allImages = new List<string>()
			{
				"player",
				"batman",
				"dragonborn",
				"faith",
				"gomez",
				"gordon",
				"punk",
				"jade",
				"maggy"
			};
			
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
			
			AddTween(new Alarm(0.1f, () => BroadcastMessage(ControllerSelect.ControllerAdded, GetSlot()), ONESHOT), true);
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
			
			if (readying)	return;
			
			if (Input.Pressed(Keyboard.Key.F5))
			{
				FP.World = new MenuWorld();
			}
			
			foreach (var menu in controllerMenus.Values)
			{
				if (!menu.Ready)
				{
					return;
				}
			}
			
			if (controllerMenus.Count > 0)
			{
				AddTween(new Alarm(0.5f, Ready, ONESHOT), true);
			}
		}
		
		void Ready()
		{
			readying = true;
			
			var i = Image.CreateRect(FP.Width, FP.Height, FP.Color(0));
			i.Alpha = 0;
			
			AddGraphic(i, -100);
			
			var tween = new VarTween(StartTheGame, ONESHOT);
			tween.Tween(i, "Alpha", 1, 0.25f, Ease.SineOut);
			AddTween(tween, true);
		}
		
		void StartTheGame()
		{
			var playerGraphics = new Dictionary<uint, string>();
			foreach (var menu in controllerMenus.Values)
			{
				playerGraphics[menu.JoyId] = menu.PlayerImageName;
			}
			
			FP.World = new GameWorld(playerGraphics);
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
			tween.Tween(menu, "X", (menu.Width / 2) + slot * menu.Width, 1.6f + FP.Random, Ease.ElasticOut);
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
			
			BroadcastMessage(ControllerSelect.ControllerRemoved, menu.Slot);
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
