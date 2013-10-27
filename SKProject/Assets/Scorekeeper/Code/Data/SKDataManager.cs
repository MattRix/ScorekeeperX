using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class SKDataManager
{
	private static List<Player>_players = new List<Player>();
	private static SortType _sortType = SortType.HighestAtTop;

	static void LoadDefaults()
	{
		_players.Clear();
		_sortType = SortType.HighestAtTop;
	}

	public static void LoadData()
	{
		string dataString = PlayerPrefs.GetString("Game_0");
		
		if(dataString == null || dataString == "")
		{
			LoadDefaults();
			return;
		}
		
		var rootDict = Json.Deserialize(dataString) as Dictionary<string,object>;
		
		if(rootDict == null)
		{
			LoadDefaults();
			return;
		}

		if(rootDict.ContainsKey("sortType"))
		{
			_sortType = SortType.GetByName(rootDict["sortType"].ToString());
		}

		List<object> playerDatas = rootDict["players"] as List<object>;
		
		for(int p = 0; p<playerDatas.Count; p++)
		{
			Dictionary<string,object> playerData = playerDatas[p] as Dictionary<string,object>;
			
			Player player = new Player();
			
			player.name = playerData["name"].ToString();
			player.color = PlayerColor.GetColor(playerData["color"].ToString());
			player.score = int.Parse(playerData["score"].ToString());

			_players.Add(player);
		}
	}
	
	public static void SaveData()
	{
		if(Keeper.instance.slotList == null) return; //don't bother saving before the slot list has even finished initializing

		_players = Keeper.instance.slotList.GetPlayers();

		Dictionary<string, object> rootDict = new Dictionary<string,object>();

		rootDict["sortType"] = _sortType.name;

		List<object> playerDatas = new List<object>();
		rootDict["players"] = playerDatas;
		
		for(int p = 0; p<_players.Count; p++)
		{
			Player player = _players[p];
			var playerDict = new Dictionary<string,object>()
			{
				{"name",player.name},
				{"score",player.score},
				{"color",player.color.name}
			};
			
			playerDatas.Add(playerDict);
		}
		
		string output = Json.Serialize(rootDict);
		
		output = RXUtils.PrettyifyJson(output);
		
		File.WriteAllText(Application.dataPath+"\\testData.txt",output);
		
		PlayerPrefs.SetString("Game_0",output);
		PlayerPrefs.Save();
	}

	public static List<Player> GetPlayers()
	{
		return _players;
	}

	public static List<PlayerColor> GetUsedColors()
	{
		List<PlayerColor> usedColors = new List<PlayerColor>(_players.Count);

		for(int p = 0; p<_players.Count; p++)
		{
			usedColors.Add(_players[p].color);
		}

		return usedColors;
	}

	public static SortType sortType
	{
		get {return _sortType;}
		set {_sortType = value;}
	}
}

public class SortType
{
	public static List<SortType> all = new List<SortType>();
	public static SortType HighestAtTop = new SortType("HighestAtTop");
	public static SortType LowestAtTop = new SortType("LowestAtTop");
	public static SortType Manual = new SortType("Manual");

	public string name;

	public SortType(string name)
	{
		this.name = name;
		all.Add(this);
	}

	public static SortType GetByName(string name)
	{
		for(int s = 0; s<all.Count; s++)
		{
			if(all[s].name == name) return all[s];
		}
		return null;
	}
}




