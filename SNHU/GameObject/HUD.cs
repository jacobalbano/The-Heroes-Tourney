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
		public const string UpdateDamage = "player_takeDamage";
		
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
			var lives = new Text(GameWorld.gameManager.StartingLives.ToString("x 0"));
			lives.Italicized = true;
			lives.Size = 18;
			lives.X = 10;
			lives.Y = 15;
			lives.ScrollX = lives.ScrollY = 0;
					
			var head = new Image(Library.GetTexture("assets/players/" + p.ImageName + "_head.png"));
			head.CenterOO();
			head.X = -10;
			head.Y = 25;
			head.ScrollX = head.ScrollY = 0;
			
			var graphics = new Graphiclist(lives, head);
			e.Graphic = graphics;
			e.Graphic.ScrollX = e.Graphic.ScrollY = 0;
			
			if (GameWorld.gameManager.StartingHealth != 0)
			{
				var total = GameWorld.gameManager.StartingHealth.ToString();
				var health = new Text(string.Format("{0}/{0}", total));
				health.Bolded = true;
				health.Size = 18;
				health.X = head.X - head.Width / 2;
				health.Y = lives.Y + 25;
				health.ScrollX = health.ScrollY = 0;
				graphics.Add(health);
				
				e.AddResponse(HUD.UpdateDamage, OnDamage(p, health));
			}
			
			e.AddResponse(Player.Die, OnDeath(p, lives, head));
			
			players.Add(e);
		}
		
		Entity.MessageResponse OnDamage(Player p, Text health)
		{
			return args => {
				var player = args[0] as Player;
				if (player != p)	return;
				
				health.String = string.Format("{0}/{1}", player.Health, GameWorld.gameManager.StartingHealth);
				
				health.ScaleY = 1.3f;
				
				var textTween = new VarTween(null, ONESHOT);
				textTween.Tween(health, "ScaleY", 1, 0.15f);
				AddTween(textTween, true);
				
//				if (player.Health < GameWorld.gameManager.StartingHealth / 25)
			};
		}
		
		Entity.MessageResponse OnDeath(Player p, Text text, Image image)
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
