
using System;
using Punk;
using Punk.Tweens.Misc;

namespace SNHU.Components
{
	/// <summary>
	/// Description of HypeTween.
	/// </summary>
	public class HypeTween : ColorTween
	{
		private float duration;
		
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
		
		public HypeTween(float duration) : base(null, Tweener.LOOPING)
		{
			Complete = NewColorPls;
			this.duration = duration;
			Color = FP.Color(FP.Choose(colors));
			NewColorPls();
		}
		
		void NewColorPls()
		{
			Tween(duration, Color, FP.Color(FP.Choose(colors)));
		}
	}
}
