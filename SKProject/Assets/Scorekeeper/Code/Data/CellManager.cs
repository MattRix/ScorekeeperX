using UnityEngine;
using System;
using System.Collections.Generic;

public class CellManager
{
	public static List<Cell> allCells = new List<Cell>();

	public static Cell megaNewPlayer = new Cell();
	public static Cell megaTimer = new Cell();
	public static Cell megaSort = new Cell();
	public static Cell megaReset = new Cell();
	public static Cell megaSettings = new Cell();

	public static Cell middleCell = new Cell();

	private static Vector2 _gridTopLeft = new Vector2();
	
	private static int _gridCols;
	private static int _gridRows;
	private static float _gridColWidth;
	private static float _gridRowHeight;
	
	public static void Recalculate()
	{
		Config.Setup();
		SetupGridCells();
		SetupMegaCells();
	}

	static void SetupGridCells()
	{
		_gridCols = Config.GRID_COLS;
		_gridRows = Config.GRID_ROWS;

		float colWidth = (Config.WIDTH - Config.GRID_MARGIN*2 - Config.GRID_SPACING*(_gridCols-1)) / _gridCols;
		float rowHeight = (Config.HEIGHT - Config.GRID_MARGIN*2 - Config.GRID_SPACING*(_gridRows-1)) / _gridRows;

		_gridTopLeft.x = -Config.WIDTH/2 + Config.GRID_MARGIN + colWidth/2;
		_gridTopLeft.y = Config.HEIGHT/2 - Config.GRID_MARGIN - rowHeight/2;
		
		_gridColWidth = colWidth + Config.GRID_SPACING;
		_gridRowHeight = rowHeight + Config.GRID_SPACING;
	}

	public static Cell GetCellFromGrid(int leftCol, int rightCol, int topRow, int bottomRow)
	{
		Cell cell = new Cell();

		cell.x = _gridTopLeft.x + (leftCol*_gridColWidth + rightCol*_gridColWidth)/2;
		cell.y = _gridTopLeft.y - (topRow*_gridRowHeight + bottomRow*_gridRowHeight)/2;
		
		cell.width = ((1+rightCol-leftCol) * _gridColWidth) - Config.GRID_SPACING;
		cell.height = ((1+bottomRow-topRow) * _gridRowHeight) - Config.GRID_SPACING;

		return cell;
	}

	static void SetupMegaCells ()
	{
		float padding = Config.PADDING_M;
		float fullHeight = Mathf.Round(Config.HEIGHT - padding*2);
		float fullWidth = Mathf.Round(Config.MEGA_WIDTH - padding*2);
		float halfHeight = Mathf.Round((fullHeight - padding)/2.0f);
		float thirdHeight = Mathf.Round((fullHeight - padding*2)/3.0f);
		
		megaNewPlayer.SetSize(fullWidth,halfHeight);
		megaNewPlayer.SetPosition(-Config.HALF_WIDTH + megaNewPlayer.width/2 + padding,Config.HALF_HEIGHT-megaNewPlayer.height/2-padding);
		allCells.Add(megaNewPlayer);
		
		megaTimer.SetSize(fullWidth,halfHeight);
		megaTimer.SetPosition(-Config.HALF_WIDTH + megaTimer.width/2 + padding,-Config.HALF_HEIGHT+megaTimer.height/2+padding);
		allCells.Add(megaTimer);
		
		megaSort.SetSize(fullWidth,thirdHeight);
		megaSort.SetPosition(Config.HALF_WIDTH - megaSort.width/2-padding,Config.HALF_HEIGHT-megaSort.height/2-padding);
		allCells.Add(megaSort);
		
		megaReset.SetSize(fullWidth,thirdHeight);
		megaReset.SetPosition(Config.HALF_WIDTH - megaReset.width/2-padding,0);
		allCells.Add(megaReset);
		
		megaSettings.SetSize(fullWidth,thirdHeight);
		megaSettings.SetPosition(Config.HALF_WIDTH - megaSettings.width/2-padding,-Config.HALF_HEIGHT+megaSettings.height/2+padding);
		allCells.Add(megaSettings);

		middleCell.SetSize(Futile.screen.halfWidth, Futile.screen.halfHeight);
		allCells.Add(middleCell);
	}

	public static void Refresh ()
	{
		for(int c = 0; c<allCells.Count; c++)
		{
			allCells[c].didHaveMajorChange = false;
		}
	}

	public static float GetGridColWidth()
	{
		return _gridColWidth;
	}

	public static float GetGridRowHeight()
	{
		return _gridRowHeight;
	}
}

