
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU;

namespace SNHU.menuobject
{
	/// <summary>
	/// Description of ControllerSelect.
	/// </summary>
	public class ControllerSelect : Entity
	{
		public const string ControllerAdded = "controllerselect_added";
		public const string ControllerRemoved = "controllerselect_removed";
		
		public int Slot { get; private set; }
		public uint JoyId { get; private set; }
		
		public Controller Controller { get; private set; }
		
		public bool Ready {get; private set; }
		
		private Image image;
		private Text text;
		private Image player;
		private Image lArrow, rArrow;
		private Image confirm;
		public string PlayerImageName { get; private set; }
		private bool changing;
		
		private bool started;
		
		private MenuWorld parent;
		
		private static int[] colors;
		static ControllerSelect()
		{
			colors = new int[]
			{
				0xFF1919,
				0x4CFF4C,
				0x4C4CFF,
				0xFFFF4C
			};
		}
		
		public ControllerSelect(MenuWorld parent, int slot, uint joyId)
		{
			changing = false;
			
			this.parent = parent;
			Height = FP.Height;
			Width = 200;
			
			this.JoyId = joyId;
			this.Slot = slot;
			
			image = new Image(Library.GetTexture("assets/menu.png"));
			image.Color = FP.Color(colors[slot]);
			text = new Text("PRESS START");
			text.X -= 70;
			text.Y = Height - 100;
			text.Size = 20;
			text.Color = FP.Color(0);
			
			OriginX = Width / 2;
			image.OriginX = image.Width / 2;
			
			Controller = MakeController(JoyId);
			
			Controller.Define("Start", (int) JoyId, (Controller.Button) 9, Controller.Button.Start);
			
			AddGraphic(image);
			AddGraphic(text);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!started)
			{
				if (Controller.Pressed("Start"))
				{
					started = true;
					
					var list = Graphic as Graphiclist;
					list.Remove(text);
					
					if (Joystick.HasAxis(JoyId, Joystick.Axis.PovX) && Joystick.HasAxis(JoyId, Joystick.Axis.PovY))
					{
						confirm = new Image(Library.GetTexture("assets/Xbox_1.png"));
						Controller.Define("A", (int) JoyId, Controller.Button.A);
					}
					else
					{
						confirm = new Image(Library.GetTexture("assets/Snes_1.png"));
						Controller.Define("A", (int) JoyId, Controller.Button.X);
					}
					
					confirm.CenterOO();
					confirm.Y = Height * 0.75f;
					
					AddGraphic(confirm);
					
					Y -= 50;
					ClearTweens();
					var tween = new VarTween(null, ONESHOT);
					tween.Tween(this, "Y", 1, 0.75f, Ease.BounceOut);
					AddTween(tween, true);
					
					PlayerImageName = parent.GetImageName();
					MakePlayerImage();
					
					StartCursors();
				}
			}
			else
			{
				if (!changing)
				{
					if (Controller.LeftStick.X < 0)
					{
						lArrow.Scale = 0.8f;
						
						var tween = new VarTween(null, ONESHOT);
						tween.Tween(lArrow, "Scale", 1, 0.3f);
						AddTween(tween, true);
						
						PlayerImageName = parent.NextImage(PlayerImageName);
						MakePlayerImage();
					}
					else if (Controller.LeftStick.X > 0)
					{
						rArrow.Scale = 0.8f;
						
						var tween = new VarTween(null, ONESHOT);
						tween.Tween(rArrow, "Scale", 1, 0.3f);
						AddTween(tween, true);
						
						PlayerImageName = parent.PrevImage(PlayerImageName);
						MakePlayerImage();
					}
					
					if (Controller.Pressed("A"))
					{
						confirm.Scale = 0.8f;
						confirm.Y += 20;
					}
					else if (Controller.Released("A"))
					{
						confirm.Scale = 1f;
						confirm.Y -= 20;
						
						var tween = new VarTween(MakeCheckmark, ONESHOT);
						tween.Tween(confirm, "ScaleY", 0, 0.1f, Ease.SineOut);
						AddTween(tween, true);
					}
				}
			}
		}
		
		void MakeCheckmark()
		{
			var list = Graphic as Graphiclist;
			list.Remove(lArrow);
			list.Remove(rArrow);
			
			changing = true;
			
			var check = new Image(Library.GetTexture("assets/ready.png"));
			check.CenterOO();
			check.Y = Height * 0.75f;
			check.ScaleY = 0;
			list.Add(check);
			
			var tween = new VarTween(null, ONESHOT);
			tween.Tween(check, "ScaleY", 1, 0.1f, Ease.SineOut);
			AddTween(tween, true);
			
			Ready = true;
		}
		
		void StartCursors()
		{
			var texture = Library.GetTexture("assets/change.png");
			lArrow = new Image(texture);
			rArrow = new Image(texture);
			
			lArrow.CenterOO();
			rArrow.CenterOO();
			
			lArrow.FlippedX = true;
			
			lArrow.X = -60;
			rArrow.X = 60;
			
			lArrow.Y = Height / 2;
			rArrow.Y = Height / 2;
			
			AddGraphic(lArrow);
			AddGraphic(rArrow);
		}
		
		void MakePlayerImage()
		{
			changing = true;
			
			if (player != null)
			{
				var tween = new VarTween(ScalePlayerIn, ONESHOT);
				tween.Tween(player, "Scale", 0, 0.1f);
				AddTween(tween, true);
			}
			else
			{
				ScalePlayerIn();
			}
		}
		
		void ScalePlayerIn()
		{
			if (player != null)
			{
				var list = Graphic as Graphiclist;
				list.Remove(player);
			}
			
			player = new Image(Library.GetTexture("assets/" + PlayerImageName + ".png"));
			player.CenterOrigin();
			
			player.X = 0;
			player.Y = Height / 2;
			
			player.Scale = 0;
			
			var tween = new VarTween(() => changing = false, ONESHOT);
			tween.Tween(player, "Scale", 1, 0.35f);
			AddTween(tween, true);
			
			AddGraphic(player);
		}
		
		Controller MakeController(uint joyId)
		{
			return new Controller(joyId);
		}
	}
}
