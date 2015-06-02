using System;
using Glide;
using Indigo;
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
			X = FP.Camera.X;
			Y = FP.Camera.Y;
			Layer = ObjectLayers.JustAbove(ObjectLayers.Background);
			
			var bg = AddComponent(Image.CreateRect(FP.Width, FP.Height, new Color()));
          	bg.CenterOO();

			var image = AddComponent(new Image(Library.GetTexture("happy.png")));
			image.CenterOO();
			
			// YOU WIN!
			var text = AddComponent(new Text("\tPlayer " + (Winner.PlayerId + 1) + "\nis the true hero!"));
			text.Y = -(FP.Height / 4);
			text.Font = Library.GetFont("fonts/Laffayette_Comic_Pro.ttf");
			text.Size = 64;
			text.CenterOrigin();
			
			Tweener.Tween(Winner, new { X = FP.Camera.X, Y = FP.Camera.Y }, 1.2f)
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
