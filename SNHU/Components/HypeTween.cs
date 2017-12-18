
using System;
using Glide;
using Indigo;
using Indigo.Utils;
using Indigo.Graphics;

namespace SNHU.Components
{
	/// <summary>
	/// Description of HypeTween.
	/// </summary>
	public class HypeTween
	{
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
		
		public static void StartHype(Tweener tweener, Image image, float duration)
		{
			ContinueHype(tweener, image, duration, new HypeTween());
		}
		
		private static void ContinueHype(Tweener tweener, Image image, float duration, HypeTween hype)
		{
			tweener.Tween(hype, new { Color = new Color(Engine.Choose.From(colors)) }, duration)
				.OnComplete(() => ContinueHype(tweener, image, duration, hype))
				.OnUpdate(() => image.Color = hype.Color);
		}
		
		private Color Color = Color.White;
	}
}
