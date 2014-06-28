
using System;
using System.Collections.Generic;
using Glide;
using Punk;
using Punk.Graphics;
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
		
		public bool Ready { get; private set; }
		
		public int Color { get; private set; }
		public Image Image { get; private set; }
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
			
			AddComponent(Image);
			AddComponent(pressStart);
			if (wins.ContainsKey(JoyId))
			{
				var score = new Text(wins[joyId].ToString("Wins: 0"));
				score.Font = font;
				score.Color = FP.Color(0);
				score.Size = 20;
				score.X = pressStart.X + 32;
				score.Y = pressStart.Y + score.Size;
				AddComponent(score);
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
					
					RemoveComponent(pressStart);
					
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
					
					AddComponent(confirm);
					
					Y -= 50;
					Tweener.Cancel();
					
					Tweener.Tween(this, new { Y = 1 }, 0.75f)
						.Ease(Ease.BounceOut);
					
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
						Tweener.Cancel();
						
						confirm.Scale = 1f;
						confirm.Y = Height * 0.75f;
					
						Tweener.Tween(check, new { ScaleY = 0 }, 0.1f)
							.Ease(Ease.SineOut)
							.OnComplete(Reselect);
					}
				}
				else
				{
					if (!changing)
					{
						if (Controller.LeftStick.X < 0)
						{
							lArrow.Scale = 0.8f;
							Tweener.Tween(lArrow, new { Scale = 1 }, 0.3f);
							
							PlayerImageName = parent.NextImage(PlayerImageName);
							MakePlayerImage();
						}
						else if (Controller.LeftStick.X > 0)
						{
							rArrow.Scale = 0.8f;
							Tweener.Tween(rArrow, new { Scale = 1 }, 0.3f);
							
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
							
							Tweener.Cancel();
							
							Tweener.Tween(confirm, new { ScaleY = 0 }, 0.1f)
								.Ease(Ease.SineOut)
								.OnComplete(MakeCheckmark);
							
							Tweener.Tween(this, new { Y = 0 }, 0.2f);
						}
					}
				}
			}
		}
		
		void Reselect()
		{
			Ready = false;
			changing = false;
			
			AddComponent(confirm);
			AddComponent(lArrow);
			AddComponent(rArrow);
			RemoveComponent(check);
			
			Tweener.Tween(confirm, new { ScaleY = 1 }, 0.1f)
				.Ease(Ease.SineOut);
			
		}
		
		void MakeCheckmark()
		{
			RemoveComponent(lArrow);
			RemoveComponent(rArrow);
			
			changing = true;
			AddComponent(check);
			
			Tweener.Tween(check, new { ScaleY = 1 }, 0.1f)
				.Ease(Ease.SineOut);
			
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
			
			AddComponent(lArrow);
			AddComponent(rArrow);
		}
		
		void MakePlayerImage()
		{
			changing = true;
			
			if (player != null)
			{
				Tweener.Tween(player, new { Scale = 0 }, 0.1f)
					.OnComplete(ScalePlayerIn);
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
				RemoveComponent(player);
			}
			
			player = new Image(Library.GetTexture("assets/players/" + PlayerImageName + ".png"));
			player.CenterOrigin();
			
			player.X = 0;
			player.Y = Height / 2;
			
			player.Scale = 0;
			
			Tweener.Tween(player, new { Scale = 1 }, 0.35f)
				.OnComplete(() => changing = false)
				.Ease(Ease.ExpoOut);
			
			AddComponent(player);
		}
		
		public static void IncreaseWin(uint controllerId)
		{
			
			if (!wins.ContainsKey(controllerId))
				wins[controllerId] = 0;
			
			wins[controllerId] = wins[controllerId] + 1;
			
		}
	}
}
