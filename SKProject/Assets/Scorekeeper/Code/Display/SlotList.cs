using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class SlotList : FContainer
{
	private List<Slot> _slots = new List<Slot>();

	public Action SignalPlayerChange;

	public FContainer slotContainer;

	public SlotList()
	{
		AddChild(slotContainer = new FContainer());

		LoadPlayerData();
	}

	public void AddSlotForPlayer(Player player)
	{
		Slot slot = new Slot(player);

		slotContainer.AddChild(slot);

		_slots.Add(slot);

		Reorder(false,false,false);

		if(SignalPlayerChange != null) SignalPlayerChange();
	}

	public void Reorder(bool shouldWaitUntilMathModeFinishes, bool isFlip, bool shouldScrollToTop)
	{
		List<Slot> originalSlots = new List<Slot>(_slots); //create a copy

		if(shouldWaitUntilMathModeFinishes)
		{
			for(int s = 0; s<_slots.Count; s++)
			{
				if(_slots[s].isMathMode) return; //don't sort if one of them is in math mode
			}
		}

		for(int s = 0; s<_slots.Count; s++)
		{
			_slots[s].y = s * _slots[s].height;
		}
	}

	private void LoadPlayerData()
	{
		string dataString = PlayerPrefs.GetString("Game_0");

		if(dataString == null || dataString == "")
		{
//			_slots.Add(new Slot(new Player("Matt",PlayerColor.Orange,0)));
//			_slots.Add(new Slot(new Player("Susan",PlayerColor.Purple,27)));
//			_slots.Add(new Slot(new Player("Rory Six",PlayerColor.Red,123)));
//			_slots.Add(new Slot(new Player("Wilbur Fentington",PlayerColor.Yellow,0)));
//			_slots.Add(new Slot(new Player("Asymptote",PlayerColor.Green,96)));
//			_slots.Add(new Slot(new Player("  Live  Wire  ",PlayerColor.Pink,44)));
			return;
		}

		var rootDict = Json.Deserialize(dataString) as Dictionary<string,object>;
		
		if(rootDict == null)
		{
			return;
		}
		 
		List<object> playerDatas = rootDict["players"] as List<object>;

		for(int p = 0; p<playerDatas.Count; p++)
		{
			Dictionary<string,object> playerData = playerDatas[p] as Dictionary<string,object>;

			Player player = new Player();

			player.name = playerData["name"].ToString();
			player.color = PlayerColor.GetColor(playerData["color"].ToString());
			player.score = int.Parse(playerData["score"].ToString());

			AddSlotForPlayer(player);
		}
	}

	private void SavePlayerData()
	{
		Dictionary<string, object> rootDict = new Dictionary<string,object>();

		List<object> players = new List<object>();
		rootDict["players"] = players;

		for(int s = 0; s<_slots.Count; s++)
		{
			Player player = _slots[s].player;

			var playerDict = new Dictionary<string,object>()
			{
				{"name",player.name},
				{"score",player.score},
				{"color",player.color.name}
			};

			players.Add(playerDict);
		}

		string output = Json.Serialize(rootDict);

		output = RXUtils.PrettyifyJson(output);

		File.WriteAllText(Application.dataPath+"\\testData.txt",output);

		PlayerPrefs.SetString("Game_0",output);
		PlayerPrefs.Save();
	}
}










