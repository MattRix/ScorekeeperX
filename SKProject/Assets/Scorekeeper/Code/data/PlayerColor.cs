using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerColor
{
	public static List<PlayerColor> allColors = new List<PlayerColor>();

	public static PlayerColor Red = new PlayerColor(0xFF4444);

	public uint hex;
	public Color color;
	
	public PlayerColor(uint hex)
	{
		this.hex = hex;
		this.color = RXColor.GetColorFromHex(hex);
		allColors.Add(this);
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

