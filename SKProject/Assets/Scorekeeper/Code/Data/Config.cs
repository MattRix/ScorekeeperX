using UnityEngine;
using System;
using System.Collections.Generic;

public class Config
{
	//data
	public static int MAX_PLAYERS = 16;
	public static int MAX_CHARS_PER_NAME = 16;

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

		SLOT_HEIGHT = 45.0f;

		GRID_MARGIN = 10.0f;
		GRID_SPACING = 6.0f;

		RESET_SIZE = 48.0f;
	}
}

