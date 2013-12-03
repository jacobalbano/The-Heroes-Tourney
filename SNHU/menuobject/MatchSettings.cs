
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SNHU.MenuObject;

namespace Tourney.MenuObject
{
	/// <summary>
	/// Description of MatchSettings.
	/// </summary>
	public class MatchSettings : Entity
	{
		private Controller controller;
		
		public MatchSettings(Controller controller, int color)
		{
			this.controller = controller;
			
			var image = new Image(Library.GetTexture("assets/menu.png"));
			image.Color = FP.Color(color);
			
			Graphic = new Graphiclist(image, new Image(Library.GetTexture("assets/matchmenu.png")));
		}
		
		public override void Added()
		{
			base.Added();
			
			X = 10;
			Y = FP.Height;
			
			var cameraTween = new VarTween(null, ONESHOT);
			cameraTween.Tween(this, "Y", 0, 0.75f, Ease.QuadOut);
			AddTween(cameraTween, true);
		}
	}
}
