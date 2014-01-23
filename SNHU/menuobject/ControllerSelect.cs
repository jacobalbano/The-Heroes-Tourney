
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU;

namespace SNHU.MenuObject
{
	/// <summary>
	/// Description of ControllerSelect.
	/// </summary>
	public class ControllerSelect : Entity
	{
		private static Dictionary<uint, int> wins;
		
		public int Slot { get; private set; }
		public uint JoyId { get; private set; }
		
		public Controller Controller { get; private set; }
		
		public bool Ready {get; private set; }
		
		public int Color { get; private set; }
		public Image Image {get; private set; }
		private Text pressStart;
		private Image player;
		private Image lArrow, rArrow;
		private Image confirm;
		private Image check;
		public string PlayerImageName { get; private set; }
		private bool changing;
		
		private bool started;
		
		private MenuWorld parent;
		
		private static int[] colors;
		static ControllerSelect()
		{
			wins = new Dictionary<uint, int>();
			
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
			var font = Library.GetFont("assets/Laffayette_Comic_Pro.ttf");
			
			changing = false;
			
			this.parent = parent;
			Height = FP.Height - 20;
			Width = 250;
			
			this.JoyId = joyId;
			this.Slot = slot;
			
			Image = new Image(Library.GetTexture("assets/menu.png"));
			Image.Color = FP.Color(Color = colors[slot]);
			pressStart = new Text("PRESS START");
			pressStart.Font = font;
			pressStart.X -= 80;
			pressStart.Y = Height - 100;
			pressStart.Size = 20;
			pressStart.Color = FP.Color(0);
			
			check = new Image(Library.GetTexture("assets/ready.png"));
			check.CenterOO();
			check.Y = Height * 0.75f;
			check.ScaleY = 0;
			
			OriginX = Width / 2;
			Image.OriginX = Image.Width / 2;
			
			Controller = new Controller(joyId);
			
			Controller.Define("Start", (int) JoyId, (Controller.Button) 9, Controller.Button.Start);
			
			AddGraphic(Image);
			AddGraphic(pressStart);
			if (wins.ContainsKey(JoyId))
			{
				var score = new Text(wins[joyId].ToString("Wins: 0"));
				score.Font = font;
				score.Color = FP.Color(0);
				score.Size = 20;
				score.X = pressStart.X + 32;
				score.Y = pressStart.Y + score.Size;
				AddGraphic(score);
			}
			
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
					list.Remove(pressStart);
					
					if (Joystick.HasAxis(JoyId, Joystick.Axis.PovX) && Joystick.HasAxis(JoyId, Joystick.Axis.PovY))
					{
						confirm = new Image(Library.GetTexture("assets/Xbox_1.png"));
						Controller.Define("A", (int) JoyId, Controller.Button.A);
						Controller.Define("B", (int) JoyId, Controller.Button.B);
					}
					else
					{
						confirm = new Image(Library.GetTexture("assets/Snes_1.png"));
						Controller.Define("A", (int) JoyId, Controller.Button.X);
						Controller.Define("B", (int) JoyId, Controller.Button.B);
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
				if (Ready)
				{
					if (Controller.Pressed("B"))
					{
						ClearTweens();
						
						confirm.Scale = 1f;
						confirm.Y = Height * 0.75f;
					
						var tween = new VarTween(Reselect, ONESHOT);
						tween.Tween(check, "ScaleY", 0, 0.1f, Ease.SineOut);
						AddTween(tween, true);
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
							confirm.Y = Height * 0.75f + 20;
						}
						
						if (Controller.Released("A"))
						{
							confirm.Scale = 1f;
							confirm.Y = Height * 0.75f;
							
							ClearTweens();
							
							var tween = new VarTween(MakeCheckmark, ONESHOT);
							tween.Tween(confirm, "ScaleY", 0, 0.1f, Ease.SineOut);
							AddTween(tween, true);
							
							var yTween = new VarTween(null, ONESHOT);
							yTween.Tween(this, "Y", 0, 0.2f);
							AddTween(yTween, true);
						}
					}
				}
			}
		}
		
		void Reselect()
		{
			Ready = false;
			changing = false;
			
			var list = Graphic as Graphiclist;
			list.Add(confirm);
			list.Remove(check);
			list.Add(lArrow);
			list.Add(rArrow);
			
			var tween = new VarTween(null, ONESHOT);
			tween.Tween(confirm, "ScaleY", 1, 0.1f, Ease.SineOut);
			AddTween(tween, true);
			
		}
		
		void MakeCheckmark()
		{
			var list = Graphic as Graphiclist;
			list.Remove(lArrow);
			list.Remove(rArrow);
			
			changing = true;
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
			
			player = new Image(Library.GetTexture("assets/players/" + PlayerImageName + ".png"));
			player.CenterOrigin();
			
			player.X = 0;
			player.Y = Height / 2;
			
			player.Scale = 0;
			
			var tween = new VarTween(() => changing = false, ONESHOT);
			tween.Tween(player, "Scale", 1, 0.35f);
			AddTween(tween, true);
			
			AddGraphic(player);
		}
		
		public static void IncreaseWin(uint controllerId)
		{
			
			if (!wins.ContainsKey(controllerId))
				wins[controllerId] = 0;
			
			wins[controllerId] = wins[controllerId] + 1;
			
			FP.Log("yolo", controllerId, wins[controllerId]);
			
		}
	}
}
