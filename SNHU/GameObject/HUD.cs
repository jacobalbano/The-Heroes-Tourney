using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of HUD.
	/// </summary>
	public class HUD : Entity
	{
		private GameManager gm;
		
		private List<Entity> players;
		
		public HUD(GameManager gameManager)
		{
			gm = gameManager;
			players = new List<Entity>();
		}
		
		public void AddPlayer(Player p)
		{
			var e = new Entity();	
			var text = new Text(GameWorld.gameManager.StartingLives.ToString("x 0"));
			text.Italicized = true;
			text.Size = 18;
			text.X = 10;
			text.Y = 15;
			text.ScrollX = text.ScrollY = 0;
			
			var head = new Image(Library.GetTexture("assets/players/" + p.ImageName + "_head.png"));
			head.CenterOO();
			head.X = -10;
			head.Y = 25;
			head.ScrollX = head.ScrollY = 0;
			
			e.Graphic = new Graphiclist(text, head);
			e.Graphic.ScrollX = e.Graphic.ScrollY = 0;
			
			e.AddResponse(Player.Die, OnDeathClosure(p, text, head));
			
			players.Add(e);
		}
		
		Entity.MessageResponse OnDeathClosure(Player p, Text text, Image image)
		{
			return args => {
				var player = args[0] as Player;
				if (player != p)	return;
				
				text.String = player.Lives.ToString("x 0");
				if (player.Lives == 1)
				{
					text.Color = FP.Color(0xff0000);
				}
				
				text.Scale = 1.5f;
				image.Scale = 1.5f;
				
				var textTween = new VarTween(null, ONESHOT);
				textTween.Tween(text, "Scale", 1, 0.5f);
				AddTween(textTween, true);
				
				var imageTween = new VarTween(null, ONESHOT);
				imageTween.Tween(image, "Scale", 1, 0.5f);
				AddTween(imageTween, true);
			};
		}
		
		public override void Added()
		{
			base.Added();
			
			var interval = FP.Width / (players.Count + 1);
			for (int i = 0; i < players.Count; ++i)
			{
				var e = players[i];
				
				e.X = (1 + i) * interval;
				
				e.Layer = -1000;
				
				World.Add(e);
			}
		}
		
		public override void Removed()
		{
//			World.Remove(p1);
//			World.Remove(p2);
//			World.Remove(p3);
//			World.Remove(p4);
			
			base.Removed();
		}
		
		public override void Update()
		{
			base.Update();
			
//			if (gm.Players.Count >= 1)
//			{
//				p1Txt.String = "Player 1\n" + gm.Players[0].Lives.ToString();
//			}
//			if (gm.Players.Count >= 2)
//			{
//				p2Txt.String = "Player 2\n" + gm.Players[1].Lives.ToString();
//			}
//			if (gm.Players.Count >= 3)
//			{
//				p3Txt.String = "Player 3\n" + gm.Players[2].Lives.ToString();
//			}
//			if (gm.Players.Count >= 4)
//			{
//				p4Txt.String = "Player 4\n" + gm.Players[3].Lives.ToString();
//			}
		}
	}
}
