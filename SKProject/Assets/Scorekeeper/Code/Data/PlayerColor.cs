using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerColor
{
	public static PlayerColor NullGray = new PlayerColor(0x666666);

	public static PlayerColor Red = new PlayerColor(0xFF4444);
	public static PlayerColor Orange = new PlayerColor(0xFF8822);
	public static PlayerColor Yellow = new PlayerColor(0xFFFF33);
	public static PlayerColor Green = new PlayerColor(0x66FF33);
	public static PlayerColor Turquoise = new PlayerColor(0x66FFCC);
	public static PlayerColor Blue = new PlayerColor(0x3388FF);
	public static PlayerColor Purple = new PlayerColor(0xAA55FF);
	public static PlayerColor Pink = new PlayerColor(0xFF88FF);
	public static PlayerColor Gray = new PlayerColor(0xA8A8A8);
	public static PlayerColor White = new PlayerColor(0xF6F6F6);

	public static List<PlayerColor> allColors = new List<PlayerColor> {Red,Orange,Yellow,Green,Turquoise,Blue,Purple,Pink,Gray,White};

	public uint hex;
	public Color color;
	
	public PlayerColor(uint hex)
	{
		this.hex = hex;
		this.color = RXColor.GetColorFromHex(hex);
	}

	public static PlayerColor GetNextUnusedColor(List<PlayerColor> usedColors)
	{
		usedColors = new List<PlayerColor>(usedColors); //make a copy because we'll be destructive

		int c = 0;

		//remove all the usedColors until we find an unused color (which is really the LEAST used color)
		while(true)
		{
			PlayerColor checkColor = allColors[c%allColors.Count]; //get next color in all

			int usedIndex = usedColors.IndexOf(checkColor); //if it's already used, then we don't want to use it

			if(usedIndex == -1)
			{
				return checkColor;
			}
			else 
			{
				usedColors.RemoveAt(usedIndex);
			}
		}
	}
}

