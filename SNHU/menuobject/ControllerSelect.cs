
using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Inputs;
using Indigo.Inputs.Gamepads;
using Indigo.Utils;
using SFML.Window;
using SNHU;

namespace SNHU.MenuObject
{
	/// <summary>
	/// Description of ControllerSelect.
	/// </summary>
	public class ControllerSelect : Entity
	{
		private static Dictionary<int, int> wins;
		private static Dictionary<int, string> skins;
		
		public int PlayerSlot { get; private set; }
		public int JoyId { get; private set; }
		
		private Input Start, Back, Confirm;
		private Directional Cursor;
		
		public bool Ready { get; private set; }
		public bool Started { get; private set; }
		
		public int Color { get; private set; }
		public Image Image { get; private set; }
		private Text pressStart;
		private Image player;
		private Image lArrow, rArrow;
		private Image confirm;
		private Image check;
		public string PlayerImageName { get; private set; }
		private bool changing;
		
		private MenuWorld parent;
		
		private static int[] colors;
		static ControllerSelect()
		{
			wins = new Dictionary<int, int>();
			skins = new Dictionary<int, string>();
			
			colors = new int[]
			{
				0xFF1919,
				0x4CFF4C,
				0x4C4CFF,
				0xFFFF4C
			};
		}
		
		public ControllerSelect(MenuWorld parent, int playerSlot, int joyId)
		{
			var font = Library.GetFont("fonts/Laffayette_Comic_Pro.ttf");
			
			changing = false;
			
			this.parent = parent;
			Height = FP.Height - 20;
			Width = 250;
			
			this.JoyId = joyId;
			this.PlayerSlot = playerSlot;
			
			Image = new Image(Library.GetTexture("menu.png"));
			Image.Color = new Color(Color = colors[PlayerSlot]);
			pressStart = new Text("PRESS START");
			pressStart.Font = font;
			pressStart.X -= 80;
			pressStart.Y = Height - 100;
			pressStart.Size = 20;
			pressStart.Color = new Color(0);
			
			check = new Image(Library.GetTexture("ready.png"));
			check.CenterOO();
			check.Y = Height * 0.75f;
			check.ScaleY = 0;
			
			OriginX = Width / 2;
			Image.OriginX = Image.Width / 2;
			
			var slot = GamepadManager.GetSlot(joyId);
			if (SnesController.IsMatch(slot))
			{
				var snes = new SnesController(slot);
				confirm = new Image(Library.GetTexture("Snes_1.png"));
				Cursor = snes.Dpad;
				Start = snes.Start;
				Back = snes.A;
				Confirm = snes.B;
			}
			else if (Xbox360Controller.IsMatch(slot))
			{
				var xbox = new Xbox360Controller(slot);
				confirm = new Image(Library.GetTexture("Xbox_1.png"));
				Cursor = xbox.LeftStick;
				Start = xbox.Start;
				Confirm = xbox.A;
				Back = xbox.B;
			}
			else
			{
				throw new Exception("Invalid gamepad type, sorry D:");
			}
			
			AddComponent(Image);
			AddComponent(pressStart);
			if (wins.ContainsKey(JoyId))
			{
				var score = new Text(wins[joyId].ToString("Wins: 0"));
				score.Font = font;
				score.Color = new Color(0);
				score.Size = 20;
				score.X = pressStart.X + 32;
				score.Y = pressStart.Y + score.Size;
				AddComponent(score);
			}
			
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!Started)
			{
				if (Start.Pressed)
				{
					Started = true;
					
					RemoveComponent(pressStart);
					
					confirm.CenterOO();
					confirm.Y = Height * 0.75f;
					
					AddComponent(confirm);
					
					Y -= 50;
					Tweener.Cancel();
					
					Tweener.Tween(this, new { Y = 1 }, 0.75f)
						.Ease(Ease.BounceOut);
					
					PlayerImageName = parent.GetImageName(GetLastSkin(PlayerSlot));
					MakePlayerImage();
					
					StartCursors();
				}
			}
			else
			{
				if (Ready)
				{
					if (Back.Pressed)
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
						if (Cursor.X < 0)
						{
							lArrow.Scale = 0.8f;
							Tweener.Tween(lArrow, new { Scale = 1 }, 0.3f);
							
							PlayerImageName = parent.NextImage(PlayerImageName);
							MakePlayerImage();
						}
						else if (Cursor.X > 0)
						{
							rArrow.Scale = 0.8f;
							Tweener.Tween(rArrow, new { Scale = 1 }, 0.3f);
							
							PlayerImageName = parent.PrevImage(PlayerImageName);
							MakePlayerImage();
						}
						
						if (Confirm.Pressed)
						{
							confirm.Scale = 0.8f;
							confirm.Y = Height * 0.75f + 20;
						}
						
						if (Confirm.Pressed)
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
			RemoveComponent(confirm);
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
			var texture = Library.GetTexture("change.png");
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
			
			player = new Image(Library.GetTexture("players/" + PlayerImageName + ".png"));
			player.CenterOrigin();
			
			player.X = 0;
			player.Y = Height / 2;
			
			player.Scale = 0;
			
			Tweener.Tween(player, new { Scale = 1 }, 0.35f)
				.OnComplete(() => changing = false)
				.Ease(Ease.ExpoOut);
			
			AddComponent(player);
		}
		
		public static void IncreaseWin(int controllerId)
		{
			if (!wins.ContainsKey(controllerId))
				wins[controllerId] = 0;
			
			wins[controllerId] = wins[controllerId] + 1;
		}
		
		public static void SetLastSkin(int playerId, string imageName)
		{
			skins[playerId] = imageName;
		}
		
		public static string GetLastSkin(int playerId)
		{
			var result = "";
			skins.TryGetValue(playerId, out result);
			return result;
		}
	}
}
