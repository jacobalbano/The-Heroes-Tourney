using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of HUD.
	/// </summary>
	public class HUD : Entity
	{
		public enum Message { UpdateDamage }
		
		private GameManager gm;
		
		private List<Entity> players;
		private List<Stack<Entity>> upgradeIcons;
		
		public HUD(GameManager gameManager)
		{
			gm = gameManager;
			players = new List<Entity>();
			upgradeIcons = new List<Stack<Entity>>();
			
			Layer = -10000;
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
			graphics.ScrollX = graphics.ScrollY = 0;
			e.AddComponent(graphics);
			
			if (GameWorld.gameManager.StartingHealth != 0)
			{
				var total = GameWorld.gameManager.StartingHealth.ToString();
				var health = new Text(string.Format("{0}/{0}", total));
				health.Bold = true;
				health.Size = 18;
				health.X = head.X - head.Width / 2;
				health.Y = lives.Y + 30;
				health.ScrollX = health.ScrollY = 0;
				graphics.Add(health);
				
				e.AddResponse(HUD.Message.UpdateDamage, OnDamage(p, health));
			}
			
			e.AddResponse(Player.Message.Die, OnDeath(p, lives, head));
			e.AddResponse(Player.Message.UpgradeAcquired, OnUpgradeAcquired(p, lives, head));
			e.AddResponse(Upgrade.Message.Used, OnUpgradeUsed());
			
			players.Add(e);
			upgradeIcons.Add(new Stack<Entity>());
		}
		
		Action<object[]>  OnDamage(Player p, Text health)
		{
			return args => {
				var player = args[0] as Player;
				if (player != p)	return;
				
				health.String = string.Format("{0}/{1}", player.Health, GameWorld.gameManager.StartingHealth);
				
				health.ScaleY = 1.3f;
				
				Tweener.Tween(health, new { ScaleY = 1 }, 0.15f);
			};
		}
		
		Action<object[]> OnDeath(Player p, Text text, Image image)
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
				
				Tweener.Tween(text, new { Scale = 1 }, 0.5f);
				Tweener.Tween(image, new { Scale = 1 }, 0.5f);
			};
		}
		
		Action<object[]> OnUpgradeAcquired(Player p, Text text, Image image)
		{
			return args => {
				var player = args[0] as Player;
				if (player != p)	return;
				
				var upgrade = args[1] as Upgrade;
				if (upgrade == null)	return;
				
				var hudEnt = players[player.PlayerId];
				
				var icon = upgrade.Icon;
				var upgradeImg = new Image(upgrade.Icon);
				upgradeImg.ScrollX = upgradeImg.ScrollY = 0;
				var upgradeEnt = World.AddGraphic(upgradeImg, Layer, player.X,  player.Top - (FP.Camera.Y - FP.HalfHeight));
				
				upgradeIcons[player.PlayerId].Push(upgradeEnt);
				
				var offsetX = 10;
				var offsetY = GameWorld.gameManager.StartingHealth != 0 ? 80 : 60;
				
				Tweener.Tween(upgradeEnt, new { X = hudEnt.X + offsetX, Y = hudEnt.Y + offsetY }, 0.5f)
					.Ease(Ease.ExpoOut);
				
				Tweener.Tween(upgradeImg, new { Scale = icon.Scale * 0.5f }, 0.5f)
					.Ease(Ease.ExpoOut);
			};
		}
		
		Action<object[]> OnUpgradeUsed()
		{
			return args => {
				var pId = (int)args[0];
				
				if (pId >= 0 && pId < upgradeIcons.Count && upgradeIcons[pId] != null && upgradeIcons[pId].Count > 0)
				{
					World.Remove(upgradeIcons[pId].Pop());
				}
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
	}
}
