using UnityEngine;
using System;
using System.Collections.Generic;

public class Config
{
	//rules
	public static int MAX_PLAYERS = 16;
	public static int MAX_CHARS_PER_NAME = 16;
	public static float MATH_MODE_TIME = 2.0f;
	public static string DEFAULT_NAME = "";

	//visual
	public static float WIDTH;
	public static float HEIGHT;
	public static float HALF_WIDTH;
	public static float HALF_HEIGHT;

	public static float LIST_WIDTH;
	public static float MEGA_WIDTH;

	public static float PADDING_XL;
	public static float PADDING_L;
	public static float PADDING_M;
	public static float PADDING_S;
	public static float PADDING_XS;

	public static float SLOT_HEIGHT;

	public static float GRID_MARGIN;
	public static float GRID_SPACING;
	public static int GRID_COLS;
	public static int GRID_ROWS;

	public static float RESET_SIZE;

	public static void Setup()
	{
		WIDTH = Futile.screen.width;
		HEIGHT = Futile.screen.height;
		HALF_WIDTH = WIDTH/2.0f;
		HALF_HEIGHT = HEIGHT/2.0f;

		LIST_WIDTH = Mathf.Round(WIDTH * (7.0f/10.0f));
		MEGA_WIDTH = Mathf.Round((WIDTH - LIST_WIDTH) / 2.0f);

		PADDING_XL = 12.0f;
		PADDING_L = 10.0f;
		PADDING_M = 4.0f;
		PADDING_S = 2.0f;
		PADDING_XS = 1.0f;

		if(Futile.screen.pixelWidth > 1000.0f)
		{
			SLOT_HEIGHT = 40.0f; //if the screen has over 1000 pixels, it's probably a larger screen
		}
		else 
		{
			SLOT_HEIGHT = 45.0f;
		}

		GRID_MARGIN = 4.0f;
		GRID_SPACING = 3.0f;
		GRID_COLS = 10;
		GRID_ROWS = 6;

		RESET_SIZE = 48.0f;
	}
}

