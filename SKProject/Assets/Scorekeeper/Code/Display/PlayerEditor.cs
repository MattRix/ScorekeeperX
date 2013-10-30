using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerEditor : FContainer
{
	public Slot slot;
	public NameBox nameBox;
	public FContainer keyboardAndSwatchContainer;

	public Box deleteBox;
	public Box okBox;

	public List<SwatchBox> swatchBoxes = new List<SwatchBox>();
	public List<KeyBox> keyBoxes = new List<KeyBox>();

	
	public PlayerEditor()
	{
		AddChild(keyboardAndSwatchContainer = new FContainer());
	}

	public void Setup(Slot slot)
	{
		this.slot = slot;
		this.nameBox = slot.nameBox;

		nameBox.isTouchable = false;

		Vector2 pos = OtherToLocal(nameBox,new Vector2(0,0));

		AddChild(nameBox);
		nameBox.SetPosition(pos);

		Cell nameCell = CellManager.GetCellFromGrid(3,6,2,2);

		Go.to(nameBox, 0.7f, new TweenConfig()
		      .floatProp("x",nameCell.x)
		      .floatProp("y",nameCell.y)
		      .floatProp("width",nameCell.width)
		      .floatProp("height",nameCell.height)
		      .setEaseType(EaseType.ExpoInOut)
		      //.onComplete(() => {})
		);

		////////SETUP DELETE

		Cell deleteCell = CellManager.GetCellFromGrid(1,2,2,2);

		AddChild(deleteBox = new Box());
		deleteBox.Init(slot.player,deleteCell.width,deleteCell.height);
		deleteBox.y = deleteCell.y;
		deleteBox.x = -Config.WIDTH/2 - deleteCell.width - 30.0f; //put if offscreen to the left
		deleteBox.isTouchable = false; //don't allow it to be touched until it builds in

		Go.to(deleteBox, 0.4f, new TweenConfig()
		      .floatProp("x",deleteCell.x)
		      .setEaseType(EaseType.ExpoOut)
		      .setDelay(0.3f)
		      .onComplete(()=>{deleteBox.isTouchable = true;})
		      );

		deleteBox.SignalPress += box =>
		{
			FSoundManager.PlaySound("UI/Button1");
			deleteBox.DoTapEffect();
		};

		////////SETUP OK
		/// 
		Cell okCell = CellManager.GetCellFromGrid(7,8,2,2);
		
		AddChild(okBox = new Box());
		okBox.Init(slot.player,okCell.width,okCell.height);
		okBox.y = okCell.y;
		okBox.x = Config.WIDTH/2 + okCell.width + 30.0f; //put if offscreen to the right
		okBox.isTouchable = false; //don't allow it to be touched until it builds in

		okBox.isEnabled = (slot.player.name.Length > 0);

		Go.to(okBox, 0.4f, new TweenConfig()
		      .floatProp("x",okCell.x)
		      .setEaseType(EaseType.ExpoOut)
		      .setDelay(0.3f)
		      .onComplete(()=>{okBox.isTouchable = true;})
		      );
		
		okBox.SignalPress += box =>
		{
			FSoundManager.PlaySound("UI/Button1");
			okBox.DoTapEffect();
			Close();
		};

		CreateKeyboard(); 
		CreateSwatches();
	}

	void CreateSwatches()
	{
		for(int s = 0; s<10; s++)
		{
			PlayerColor color = PlayerColor.allColors[s];
			SwatchBox swatchBox = new SwatchBox(color);

			if(swatchBox.player.color == slot.player.color)
			{
				swatchBox.isSelected = true;
			}

			swatchBoxes.Add(swatchBox);
			keyboardAndSwatchContainer.AddChild(swatchBox);

			swatchBox.SignalPress += HandleSwatchTap;
			swatchBox.scale = 0.0f;

			Go.to(swatchBox, 0.3f, new TweenConfig()
			      .floatProp("scale",1.0f)
			      .setDelay(0.3f + 0.04f*(float)s)
			      .setEaseType(EaseType.ExpoOut));
		}

		RepositionSwatches();
	}

	void RepositionSwatches()
	{
		for(int s = 0; s<10; s++)
		{
			int col = s%5;
			int row = Mathf.FloorToInt((float)s/5.0f);
			
			Cell cell = CellManager.GetCellFromGrid(col*2,col*2+1,row,row);
			
			swatchBoxes[s].SetToCell(cell);
		}
	}

	void RemoveSwatches()
	{
		for(int s = 0; s<10; s++)
		{
			swatchBoxes[s].isTouchable = false;
			Go.to(swatchBoxes[s], 0.3f, new TweenConfig()
			      .floatProp("scale",0.0f)
			      .setDelay(0.0f + 0.04f*(10.0f-(float)s))
			      .setEaseType(EaseType.ExpoIn)
			      .removeWhenComplete());
		}
	}

	public int noteCount = 0;

	void HandleSwatchTap(Box box)
	{
		box.DoTapEffect();

		box.player.color.PlayNormalSound();
		if(noteCount % 4 == 0) box.player.color.PlayBassSound();
		noteCount++;

		for(int s = 0; s<swatchBoxes.Count; s++)
		{
			swatchBoxes[s].isSelected = (swatchBoxes[s] == box);
		}
	}

	void CreateKeyboard()
	{
		
	}


	void Close()
	{
		RemoveSwatches();
		RemoveKeyboard();

		okBox.isTouchable = false;
		deleteBox.isTouchable = false;

		Go.to(okBox, 0.5f, new TweenConfig()
		      .floatProp("x", Config.WIDTH/2+okBox.width)
		      .setDelay(0.0f)
		      .setEaseType(EaseType.ExpoIn));

		Go.to(deleteBox, 0.5f, new TweenConfig()
		      .floatProp("x", -Config.WIDTH/2-okBox.width)
		      .setDelay(0.0f)
		      .setEaseType(EaseType.ExpoIn)
		      .onComplete(()=>{Keeper.instance.StopEditing(null);}));

		//TODO: disable namebox editing and change its mode

		//add it so the matrix things work properly
		Keeper.instance.AddChild(Keeper.instance.mainContainer);

		Vector2 pos = this.OtherToLocal(slot,slot.nameCell.GetPosition());

		Keeper.instance.RemoveChild(Keeper.instance.mainContainer);

		pos *= 1.0f/Keeper.instance.mainContainer.scale; //because we want to move to where it WILL be when it tweens in

		Go.to(nameBox, 0.8f, new TweenConfig()
		      .floatProp("x",pos.x)
		      .floatProp("y",pos.y)
		      .floatProp("width",slot.nameCell.width)
		      .floatProp("height",slot.nameCell.height)
		      .setDelay(0.2f)
		      .setEaseType(EaseType.ExpoInOut)
		      .onComplete(HandleCloseComplete)
		      );
	}

	void HandleCloseComplete()
	{
		slot.AddChild(nameBox);
		nameBox.isTouchable = true;
		nameBox.SetToCell(slot.nameCell);
		Keeper.instance.RemovePlayerEditor();
	}

	void RemoveKeyboard()
	{
	}


}