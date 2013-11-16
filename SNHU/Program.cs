/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 6:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;

namespace SNHU
{
	class Program : Engine
	{
		public Program() : base(1000, 600, 60)
		{
		}
		
		public override void Init()
		{
			base.Init();
			
			FP.World = new GameWorld();
			
			FP.Log(Library.GetText("Program.cs"));
		}
		
		public static void Main(string[] args)
		{
			var game = new Program();
		}
	}
}