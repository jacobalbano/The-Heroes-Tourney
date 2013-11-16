﻿/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 7:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using SNHU.GameObject;

namespace SNHU
{
	/// <summary>
	/// Description of GameWorld.
	/// </summary>
	public class GameWorld : World
	{
		public GameWorld() : base()
		{
			AddGraphic(new Text("HI GUISE"));
			
			Add(new Player(FP.HalfWidth, FP.HalfHeight));
		}
	}
}
