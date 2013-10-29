using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerColor
{
	public static PlayerColor NullGray = new PlayerColor("NullGray", 0x999999,0);

	public static PlayerColor Red = new PlayerColor("Red",0xFF4444,0);
	public static PlayerColor Orange = new PlayerColor("Orange",0xFF8822,1);
	public static PlayerColor Yellow = new PlayerColor("Yellow",0xFFFF33,2);
	public static PlayerColor Green = new PlayerColor("Green",0x66FF33,3);
	public static PlayerColor Turquoise = new PlayerColor("Turquoise",0x66FFCC,4);
	public static PlayerColor Blue = new PlayerColor("Blue",0x3388FF,5);
	public static PlayerColor Purple = new PlayerColor("Purple",0xAA55FF,6);
	public static PlayerColor Pink = new PlayerColor("Pink",0xFF88FF,7);
	public static PlayerColor Gray = new PlayerColor("Gray",0xA8A8A8,8);
	public static PlayerColor White = new PlayerColor("White",0xF6F6F6,9);

	public static List<PlayerColor> allColors = new List<PlayerColor> {Red,Orange,Yellow,Green,Turquoise,Blue,Purple,Pink,Gray,White};

	public string name;
	public uint hex;
	public Color color;
	public int soundIndex;
	public string soundNormalName;
	public string soundBassName;
	
	public PlayerColor(string name, uint hex, int soundIndex)
	{
		this.name = name;
		this.hex = hex;
		this.color = RXColor.GetColorFromHex(hex);
		this.soundNormalName = "Musical/Note"+soundIndex+"_normal";
		this.soundBassName = "Musical/Note"+soundIndex+"_bass";
	}

	public void PlayNormalSound()
	{
		FSoundManager.PlaySound(soundNormalName);
	}

	public void PlayBassSound()
	{
		FSoundManager.PlaySound(soundBassName);
	}

	public static PlayerColor GetNextUnusedColor()
	{
		List<PlayerColor> usedColors = SKDataManager.GetUsedColors();

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

			c++;
		}
	}

	public static PlayerColor GetColor(string name)
	{
		for(int a = 0; a<allColors.Count;a++)
		{
			if(allColors[a].name == name)
			{
				return allColors[a];
			}
		}

		return NullGray;
	}
}

