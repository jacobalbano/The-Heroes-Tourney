using System;
using Glide;
using Indigo;
using Indigo.Content;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Components;
using SNHU.Config;
using SNHU.GameObject;

namespace SNHU.MenuObject
{
	/// <summary>
	/// Description of Victory.
	/// </summary>
	public class VictoryEnding : Entity
	{
		private Player Winner;
		
		public VictoryEnding(Player winner)
		{
			Winner = winner;
			X = Engine.World.Camera.X;
			Y = Engine.World.Camera.Y;
			RenderStep = ObjectLayers.JustAbove(ObjectLayers.Background);
			
			var bg = AddComponent(Image.CreateRect(Engine.Width, Engine.Height, new Color()));
          	bg.CenterOrigin();

			var image = AddComponent(new Image(Library.Get<Texture>("happy.png")));
			image.CenterOrigin();
			
			// YOU WIN!
			var text = AddComponent(new Text("\tPlayer " + (Winner.PlayerId + 1) + "\nis the true hero!"));
			text.Y = -(Engine.Height / 4);
			text.Font = Library.Get<Font>("fonts/Laffayette_Comic_Pro.ttf");
			text.Size = 64;
			text.CenterOrigin();
			
			Tweener.Tween(Winner, new { X = Engine.World.Camera.X, Y = Engine.World.Camera.Y }, 1.2f)
				.Ease(Ease.ElasticOut);
			
			HypeTween.StartHype(Tweener, image, 1);
			Tweener.Tween(image, new { Angle = 359 }, 20)
				.Repeat();
			
			Tweener.Tween(image, new { Scale = 1.7f }, 0.7f)
				.From(new { Scale = 0.7f })
				.Ease(Ease.SineOut)
				.Repeat()
				.Reflect();
			
			Winner.OnMessage(PhysicsBody.Message.Deactivate);
		}
		
		public override void Added()
		{
			base.Added();
			
			if (Winner.World == null)
				World.Add(Winner);
		}
	}
}
