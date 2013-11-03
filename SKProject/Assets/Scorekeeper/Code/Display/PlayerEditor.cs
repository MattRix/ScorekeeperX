using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerEditor : FContainer, SKDestroyable
{
	public static string THE_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	public static string THE_KEYBOARD = "QWERTYUIOPASDFGHJKLZXCVBNM"; 
	public static int SPACE_KEY = 26;
	public static int BACKSPACE_KEY = 27;

	public static float SKULL_WIDTH = 25.0f;
	public static float QUESTION_MARK_WIDTH = 15.0f;

	public Slot slot;
	public NameBox nameBox;
	public FContainer keyboardAndSwatchContainer;

	public Box deleteBox;
	public Box okBox;

	public List<SwatchBox> swatchBoxes = new List<SwatchBox>();
	public List<KeyBox> keyBoxes = new List<KeyBox>();

	public RXTweenable deleteModeTweenable;
	public Box deleteOkBox;
	public Box deleteCancelBox;
	public FSprite deleteQMark;
	public FSprite deleteSkull;
	public bool isOpeningDelete = false;
	
	public PlayerEditor()
	{
		deleteModeTweenable = new RXTweenable(0.0f);
		deleteModeTweenable.SignalChange += HandleDeleteModeChange;
		AddChild(keyboardAndSwatchContainer = new FContainer());
	}

	public void Destroy()
	{
		this.RemoveFromContainer();
		if(deleteBox != null) deleteBox.Destroy();
		if(okBox != null) okBox.Destroy();
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

		nameBox.anchorCell = nameCell;

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
		deleteBox.anchorCell = deleteCell;

		deleteBox.contentContainer.AddChild(deleteSkull = new FSprite("Icons/Placeholder"));
		deleteSkull.width = SKULL_WIDTH;

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
			StartDelete();
		};

		////////SETUP OK
		/// 
		Cell okCell = CellManager.GetCellFromGrid(7,8,2,2);
		
		AddChild(okBox = new Box());
		okBox.Init(slot.player,okCell.width,okCell.height);
		okBox.y = okCell.y;
		okBox.x = Config.WIDTH/2 + okCell.width + 30.0f; //put if offscreen to the right
		okBox.isTouchable = false; //don't allow it to be touched until it builds in
		okBox.anchorCell = okCell;

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
			slot.player.color.PlayNormalSound();
			okBox.DoTapEffect();
			Close();
		};

		nameBox.isEditMode = true;

		CreateKeyboard(0.0f); 
		CreateSwatches(0.3f);
	}

	void CreateSwatches(float delay)
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
			      .setDelay(delay + 0.04f*(float)s)
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
			      .destroyWhenComplete());
		}

		swatchBoxes.Clear();
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

		slot.player.color = box.player.color;
	}

	void CreateKeyboard(float delay)
	{
		bool isNameAtMaxLength = (slot.player.name.Length >= Config.MAX_CHARS_PER_NAME);
		bool isNameEmpty = (slot.player.name.Length == 0);

		FSoundManager.PlaySound("UI/LetterIn",0.5f);

		for(int k = 0; k<28; k++)
		{
			KeyBox keyBox = new KeyBox(slot.player,k);

			if(k == SPACE_KEY)
			{
				keyBox.shouldRepeat = false;
			}

			keyBoxes.Add(keyBox);
			keyboardAndSwatchContainer.AddChild(keyBox);

			if(k == SPACE_KEY)
			{
				keyBox.normalSoundName = "UI/ButtonTick";
			}
			else if(k == BACKSPACE_KEY)
			{
				keyBox.normalSoundName = "UI/Backspace";
			}
			else //the normal letters
			{
				FLabel keyLabel = new FLabel("Raleway",THE_KEYBOARD[k].ToString());
				keyLabel.color = Color.black;
				keyLabel.scale = 0.75f;
				keyBox.contentContainer.AddChild(keyLabel);
				keyBox.normalSoundName = "UI/ButtonTick";
			}

			//disable backspace if it's empty, disable everything BUT backspace if it's full
			if((k == BACKSPACE_KEY && !isNameEmpty) || (k != BACKSPACE_KEY && !isNameAtMaxLength)) 
			{
				keyBox.isEnabled = true;
			}
			else 
			{
				keyBox.isEnabled = false;
			}

			keyBox.SignalTick += HandleKeyTick;

			keyBox.scale = 0.0f;

			Go.to(keyBox, 0.25f, new TweenConfig()
			      .floatProp("scale",1.0f)
			      .setDelay(delay + 0.025f*(28f-(float)k))
			      .setEaseType(EaseType.ExpoOut));

		}

		RepositionKeyboard();
	}

	private void HandleKeyTick(Box box, int ticks)
	{
		KeyBox keyBox = box as KeyBox;

		bool isNameAtMaxLength = (slot.player.name.Length >= Config.MAX_CHARS_PER_NAME);
		bool isNameEmpty = (slot.player.name.Length == 0);

		if(keyBox.index == SPACE_KEY)
		{
			if(!isNameAtMaxLength) slot.player.name += " ";
		}
		else if(keyBox.index == BACKSPACE_KEY)
		{
			if(!isNameEmpty) slot.player.name = slot.player.name.Substring(0,slot.player.name.Length-1);
		}
		else //normal letters
		{
			if(!isNameAtMaxLength) slot.player.name += THE_KEYBOARD[keyBox.index];
		}

		string name = slot.player.name;

		if
		(
			(!isNameAtMaxLength && (name.Length >= Config.MAX_CHARS_PER_NAME)) || 	// if it's now too long
			(isNameAtMaxLength && !(name.Length >= Config.MAX_CHARS_PER_NAME)) || 	//it just stopped being too long
			(!isNameEmpty && (name.Length == 0)) ||									//it just became empty
			(isNameEmpty && !(name.Length == 0))  									//it just stopped being empty
		)
		{
			isNameAtMaxLength = (name.Length >= Config.MAX_CHARS_PER_NAME);
			isNameEmpty = (name.Length == 0);

			okBox.isEnabled = !isNameEmpty;

			for(int k = 0; k<28; k++)
			{
				if((k == BACKSPACE_KEY && !isNameEmpty) || (k != BACKSPACE_KEY && !isNameAtMaxLength)) 
				{
					keyBoxes[k].isEnabled = true;
				}
				else 
				{
					keyBoxes[k].isEnabled = false;
				}
			}

		}

	}

	void RepositionKeyboard()
	{
		for(int k = 0; k<28; k++)
		{
			int col;
			int row;

			if(k < 10)
			{
				col = k;
				row = 3; 
			}
			else if(k < 19)
			{
				col = k-10;
				row = 4; 
			}
			else 
			{
				col = k-19;
				row = 5; 
			}

			Cell cell;

			if(k == SPACE_KEY)
			{
				cell = CellManager.GetCellFromGrid(col,col+1,row,row);
			}
			else if(k == BACKSPACE_KEY)
			{
				cell = CellManager.GetCellFromGrid(col+1,col+1,row-1,row);
			}
			else //the normal letters
			{
				cell = CellManager.GetCellFromGrid(col,col,row,row);
			}

			keyBoxes[k].SetToCell(cell);
		}
	}

	void RemoveKeyboard()
	{
		FSoundManager.PlaySound("UI/LetterIn",0.5f);

		for(int k = 0; k<28; k++)
		{
			keyBoxes[k].isTouchable = false;
			Go.to(keyBoxes[k], 0.25f, new TweenConfig()
			      .floatProp("scale",0.0f)
			      .setDelay(0.0f + 0.015f*(float)k)
			      .setEaseType(EaseType.ExpoIn)
			      .destroyWhenComplete());
		}

		keyBoxes.Clear();
	}

	void StartDelete()
	{
		FSoundManager.PlaySound("UI/MathOpen");

		isOpeningDelete = true;

		okBox.isTouchable = false;
		deleteBox.isTouchable = false;
		RemoveSwatches();
		RemoveKeyboard();

		Cell deleteCancelCell = CellManager.GetCellFromGrid(2,4,3,3);
		Cell deleteOkCell = CellManager.GetCellFromGrid(5,7,3,3);

		deleteCancelBox = new Box();
		deleteCancelBox.Init(slot.player);
		deleteCancelBox.contentContainer.AddChild(new FSprite("Icons/Placeholder"));
		deleteCancelBox.SetToCell(deleteCancelCell);
		deleteCancelBox.anchorCell = deleteCancelCell;
		AddChild(deleteCancelBox);

		deleteOkBox = new Box();
		deleteOkBox.Init(slot.player);
		deleteOkBox.contentContainer.AddChild(new FSprite("Icons/Placeholder"));
		deleteOkBox.SetToCell(deleteOkCell);
		deleteOkBox.anchorCell = deleteOkCell;
		AddChild(deleteOkBox);

		deleteOkBox.isTouchable = false;
		deleteCancelBox.isTouchable = false;

		nameBox.fixedScale = nameBox.nameLabel.scale;
		nameBox.contentContainer.AddChild(deleteQMark = new FSprite("Icons/Placeholder"));
		deleteQMark.width = QUESTION_MARK_WIDTH;
		nameBox.questionMark = deleteQMark;

		deleteModeTweenable.To(1.0f,0.5f, new TweenConfig().onComplete(HandleDeleteModeOpen));
		HandleDeleteModeChange();
	}

	void HandleDeleteModeChange()
	{
		float amount = deleteModeTweenable.amount;

		//swap easing depennding on whether we're opening or closing
		RXEase.Delegate easeOut = isOpeningDelete ? RXEase.ExpoInOut : RXEase.ExpoInOut;
		RXEase.Delegate easeIn = isOpeningDelete ? RXEase.ExpoInOut : RXEase.ExpoInOut;

		Cell fullCell = CellManager.GetCellFromGrid(2,7,2,2);

		float qMarkWidth = QUESTION_MARK_WIDTH; //question mark width
		float skullWidth = SKULL_WIDTH; //skull width
		float skullPadding = Config.PADDING_L; //padding between skull and right side

		float innerWidth = 0;
		innerWidth += skullWidth;
		innerWidth += skullPadding; 
		innerWidth += Config.GRID_SPACING; //grid spacing
		innerWidth += Config.PADDING_L; //left side text padding
		innerWidth += nameBox.nameLabel.textRect.width * nameBox.nameLabel.scale; 
		innerWidth += Config.PADDING_L * nameBox.nameLabel.scale; //padding between text and question mark
		innerWidth += qMarkWidth * nameBox.nameLabel.scale;

		float remainingWidth = fullCell.width - innerWidth;
		float halfRemaining = remainingWidth/2;

		float deleteBoxTargetWidth = Mathf.Max(CellManager.GetGridColWidth(),halfRemaining + skullWidth + skullPadding);
		float nameBoxTargetWidth = fullCell.width - deleteBoxTargetWidth;

		float deleteBoxTargetX = fullCell.x - fullCell.width/2 + deleteBoxTargetWidth/2;
		float nameBoxTargetX = fullCell.x + fullCell.width/2 - nameBoxTargetWidth/2;

		//make sure the gap is filled
		deleteBoxTargetWidth += 2;
		deleteBoxTargetX += 1;

		//make the delete cell fill the gap
		
		float deletePercent = RXMath.GetSubPercent(amount, 0.0f,1.0f);
		float namePercent = RXMath.GetSubPercent(amount, 0.0f,1.0f);
		
		deleteBox.x = deleteBox.anchorCell.x + (deleteBoxTargetX - deleteBox.anchorCell.x) * easeOut(deletePercent);
		deleteBox.width = deleteBox.anchorCell.width + (deleteBoxTargetWidth - deleteBox.anchorCell.width) * easeOut(deletePercent);
		
		nameBox.x = nameBox.anchorCell.x + (nameBoxTargetX - nameBox.anchorCell.x) * easeOut(namePercent);
		nameBox.width = nameBox.anchorCell.width + (nameBoxTargetWidth - nameBox.anchorCell.width) * easeOut(namePercent);

		float skullPercent = RXMath.GetSubPercent(amount, 0.0f,1.0f);
		float skullTargetX = deleteBoxTargetWidth/2 - skullPadding - skullWidth/2;

		deleteSkull.x = 0 + (skullTargetX - 0) * easeOut(skullPercent);

		deleteQMark.alpha = RXMath.GetSubPercent(amount, 0.5f,1.0f);

		//////////
		//ok and cancel buttons
		//////////

		float okPercent = RXMath.GetSubPercent(amount, 0.0f,0.3f);
		float okX = Config.HALF_WIDTH + okBox.anchorCell.width;

		okBox.x = okBox.anchorCell.x + (okX - okBox.anchorCell.x) * easeIn(okPercent);

		float deleteCancelPercent = RXMath.GetSubPercent(amount, 0.6f,1.0f);
		float deleteOkPercent = RXMath.GetSubPercent(amount, 0.7f,1.0f);

		float bottomY = -Config.HALF_HEIGHT - CellManager.GetGridRowHeight() - 10.0f;

		deleteCancelBox.y = bottomY + (deleteCancelBox.anchorCell.y - bottomY) * RXEase.ExpoOut(deleteCancelPercent);
		deleteOkBox.y = bottomY + (deleteOkBox.anchorCell.y - bottomY) * RXEase.ExpoOut(deleteOkPercent);
	}

	void HandleDeleteModeOpen(AbstractTween obj)
	{
		deleteOkBox.isTouchable = true;
		deleteCancelBox.isTouchable = true;

		deleteCancelBox.SignalRelease += (box) => 
		{
			deleteCancelBox.isTouchable = false;
			deleteOkBox.isTouchable = false;

			deleteCancelBox.DoTapEffect();
			FSoundManager.PlaySound("UI/Cancel",0.5f);
			FSoundManager.PlaySound("UI/MathClose");
			isOpeningDelete = false;
			deleteModeTweenable.To(0.0f,0.5f, new TweenConfig().onComplete(HandleDeleteModeClose));
			CreateKeyboard(0.2f);
			CreateSwatches(0.5f);
		};

		deleteOkBox.SignalRelease += (box) => 
		{
			deleteCancelBox.isTouchable = false;
			deleteOkBox.isTouchable = false;

			okBox.Destroy();//it's offscreen anyway!

			deleteOkBox.DoTapEffect();
			FSoundManager.PlaySound("UI/Button1");

			Go.to(this, 0.4f, new TweenConfig()
			      .floatProp("scale",0.0f)
			      .setEaseType(EaseType.ExpoIn)
			      .onComplete(HandleDeleteComplete));
		};
	}

	void HandleDeleteComplete() //if they've actually deleted something
	{
		FSoundManager.PlaySound("UI/ResetOk",1.0f);
		Keeper.instance.StopEditing(slot.player);

		deleteBox.Destroy();
		slot.Destroy();
		nameBox.Destroy();

		slot = null;
		Keeper.instance.RemovePlayerEditor();
	}

	void HandleDeleteModeClose()
	{
		nameBox.questionMark.RemoveFromContainer();
		nameBox.questionMark = null;

		deleteCancelBox.Destroy();
		deleteOkBox.Destroy();
		deleteCancelBox = null;
		deleteOkBox = null;

		okBox.isTouchable = true;
		deleteBox.isTouchable = true;

		nameBox.fixedScale = -1;
	}

	void Close()
	{
		nameBox.isEditMode = false;

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


}