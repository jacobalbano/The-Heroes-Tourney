using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Content;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Config;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of HUD.
	/// </summary>
	public class HUD : Entity
	{
		public enum Message { UpdateDamage }
		
		private List<Entity> Players;
		private List<Stack<Image>> upgradeIcons;
		
		private int StartingHealth, StartingLives, PunchDamage;
		
		public HUD()
		{
			Players = new List<Entity>();
			upgradeIcons = new List<Stack<Image>>();
			RenderStep = ObjectLayers.HUD;
			
			var Config = Library.Get<PlayerConfig>("config/player.ini");
			StartingLives = Config.StartingLives;
			StartingHealth = Config.StartingHealth;
			PunchDamage = Config.PunchDamage;
		}
		
		public override void Added()
		{
			base.Added();
			
			var interval = Engine.Width / (Players.Count + 1f);
			for (int i = 0; i < Players.Count; ++i)
			{
				var e = Players[i];
				e.X = (1 + i) * interval;
				e.RenderStep = RenderStep;
				World.Add(e);
			}
		}
		
		public void AddPlayers(List<Player> players)
		{
			foreach (var p in players)
			{
				var e = new Entity();	
				var lives = new Text(StartingLives.ToString());
				lives.Italicized = true;
				lives.Size = 18;
				lives.X = 10;
				lives.Y = 15;
				lives.ScrollX = lives.ScrollY = 0;
						
				var head = new Image(Library.Get<Texture>("players/" + p.ImageName + "_head.png"));
				head.CenterOrigin();
				head.X = -10;
				head.Y = 25;
				head.ScrollX = head.ScrollY = 0;
				
				var graphics = new Graphiclist(lives, head);
				graphics.ScrollX = graphics.ScrollY = 0;
				e.AddComponent(graphics);
				
				if (StartingHealth != 0 && PunchDamage != 0)
				{
					var health = new Text(string.Format("{0}/{0}", StartingHealth));
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
				
				Players.Add(e);
				upgradeIcons.Add(new Stack<Image>());
			}
		}
		
		Action<object[]>  OnDamage(Player p, Text health)
		{
			return args => {
				var player = args[0] as Player;
				if (player != p)	return;
				
				health.String = string.Format("{0}/{1}", player.Health, StartingHealth);
				
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
					text.Color = new Color(0xff0000);
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
				
				var hudEnt = Players[player.PlayerId];
				
				var icon = upgrade.Icon;
				var upgradeImg = new Image(upgrade.Icon);
				upgradeImg.ScrollX = upgradeImg.ScrollY = 0;
				upgradeImg.X = player.X;
				upgradeImg.Y = player.Top - (Engine.World.Camera.Y - Engine.HalfHeight);
				upgradeImg.Relative = false;
				AddComponent(upgradeImg);
				
				upgradeIcons[player.PlayerId].Push(upgradeImg);
				
				var offsetX = 10;
				var offsetY = StartingHealth != 0 ? 80 : 60;
				
				Tweener.Tween(upgradeImg, new { X = hudEnt.X + offsetX, Y = hudEnt.Y + offsetY }, 0.5f)
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
					RemoveComponent(upgradeIcons[pId].Pop());
				}
			};
		}
	}
}
