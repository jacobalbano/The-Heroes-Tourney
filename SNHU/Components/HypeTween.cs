
using System;
using GlideTween;
using Punk;

namespace SNHU.Components
{
	/// <summary>
	/// Description of HypeTween.
	/// </summary>
	public class HypeTween
	{
		public int Color;
		
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
		
		public HypeTween(float duration, GlideManager tweener)
		{
			StartHype(duration, tweener);
		}
		
		void StartHype(float duration, GlideManager tweener)
		{
			tweener.Tween(this, new {Color = FP.Choose(colors)}, duration)
				.OnComplete(() => StartHype(duration, tweener))
				.HexColor();
		}
	}
}
