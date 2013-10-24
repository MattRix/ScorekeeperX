using UnityEngine;
using System;
using System.Collections.Generic;

public class Config
{
	//data
	public static int MAX_PLAYERS = 16;
	public static int MAX_CHARS_PER_NAME = 16;

	//visual
	public static float SCREEN_WIDTH;
	public static float SCREEN_HEIGHT;
	public static float HALF_SCREEN_WIDTH;
	public static float HALF_SCREEN_HEIGHT;

	public static float LIST_WIDTH;
	public static float SIDE_WIDTH;

	public static float PADDING_XL;
	public static float PADDING_L;
	public static float PADDING_M;
	public static float PADDING_S;
	public static float PADDING_XS;

	public static float PLAYER_HEIGHT;

	public static float GRID_MARGIN;
	public static float GRID_SPACING;

	public static float RESET_SIZE;

	public static void Setup()
	{
		SCREEN_WIDTH = Futile.screen.width;
		SCREEN_HEIGHT = Futile.screen.height;
		HALF_SCREEN_WIDTH = SCREEN_WIDTH/2.0f;
		HALF_SCREEN_HEIGHT = SCREEN_HEIGHT/2.0f;

		LIST_WIDTH = Mathf.Round(SCREEN_WIDTH * (7.0f/10.0f));
		SIDE_WIDTH = Mathf.Round((SCREEN_WIDTH - LIST_WIDTH) / 2.0f);

		PADDING_XL = 20.0f;
		PADDING_L = 15.0f;
		PADDING_M = 10.0f;
		PADDING_S = 5.0f;
		PADDING_XS = 2.0f;

		PLAYER_HEIGHT = 90.0f;

		GRID_MARGIN = 10.0f;
		GRID_SPACING = 6.0f;

		RESET_SIZE = 96.0f;
	}
}

