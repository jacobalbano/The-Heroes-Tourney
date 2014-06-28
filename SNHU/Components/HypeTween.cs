
using System;
using Glide;
using Punk;
using Punk.Utils;
using SFML.Graphics;

namespace SNHU.Components
{
	/// <summary>
	/// Description of HypeTween.
	/// </summary>
	public class HypeTween
	{
		public Color Color;
		
		static int[] colors;
		static HypeTween()
		{
			colors = new int[]
			{
				0xff0000,
				0x00ff00,
				0x0000ff,
				0xff00ff,
				0x00ffff,
				0xffaa00,
				0x00ffaa,
				0xff00aa,
				0xaaff00,
				0x00aaff
			};
		}
		
		public HypeTween(float duration, Tweener tweener)
		{
			StartHype(duration, tweener);
			Color = Color.White;
		}
		
		void StartHype(float duration, Tweener tweener)
		{
			tweener.ColorTween(this, "Color", FP.Color(FP.Choose(colors)), duration)
				.OnComplete(() => StartHype(duration, tweener));
		}
	}
}
