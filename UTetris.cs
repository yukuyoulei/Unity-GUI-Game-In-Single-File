using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>
public class UTetris : MonoBehaviour
{
	const int cRow = 16;
	const int cCol = 10;
	List<int>[] aTable = new List<int>[cRow];
	void DoInit()
	{
		lineCount = 0;
		score = 0;
		curSpeed = 1;

		for (int i = 0; i < aTable.Length; i++)
		{
			aTable[i] = new List<int>();
			for (int j = 0; j < cCol; j++)
			{
				aTable[i].Add(0);
			}
		}
	}
	void DoStart()
	{
		DoInit();
		DoNextCell();

		bStart = true;
		bGameOver = false;
	}

	private void DetectIsFail()
	{
		if (CanMoveTo(curCellCol, curCellRow))
		{
			return;
		}
		bGameOver = true;
	}
	int nextCellID = 0;
	int curCellID = 0;
	void DoNextCell()
	{
		if (nextCell == null)
		{
			nextCellID = rdm.Next(aCells.Length);
			nextCell = aCells[nextCellID];
		}
		curCellCol = cCol / 2 - 2;
		curCellRow = -4;
		curCell = nextCell;
		curCellID = nextCellID;
		nextCellID = rdm.Next(aCells.Length);
		nextCell = aCells[nextCellID];

		DetectIsFail();
	}

	bool bStart = false;
	bool bGameOver = false;

	System.Random rdm = new System.Random();
	const int cLeft = 60;
	const int cTop = 150;
	const int cSize = 30;
	const int cButtonWidth = 200;
	const int cButtonHeight = 80;
	int[][] nextCell = null;
	int[][] curCell = null;
	int curCellCol = 0;
	int curCellRow = 0;
	void OnGUI()
	{
		if (!bStart)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - cButtonWidth / 2, Screen.height / 2 - cButtonHeight / 2, cButtonWidth, cButtonHeight), "Start"))
			{
				DoStart();
			}
			string sDirection = "";
#if UNITY_EDITOR
			sDirection = "ASWD或上下左右键操作\r\n打包到手机上之后可以点击屏幕操作，点击当前下落的形状的左侧左移，点击右侧右移。";
#else
			sDirection = "点击屏幕操作，点击当前下落的形状的左侧左移，点击右侧右移。";
#endif
			GUI.Label(new Rect(Screen.width / 2 - cButtonWidth, Screen.height / 2 + cButtonHeight / 2 + 5, cButtonWidth * 2, Screen.height / 2), sDirection);
			return;
		}
		if (GUI.Button(new Rect(Screen.width - cButtonWidth - 5, 5, cButtonWidth, cButtonHeight), bPause ? "Continue" : "Pause"))
		{
			bPause = !bPause;
		}
		GUI.Label(new Rect(5, 5, Screen.width, 30), "Score:" + score);
		GUI.Label(new Rect(5, 35, Screen.width, 30), "Line:" + lineCount);

#if !UNITY_EDITOR1
		//display the operations areas
		GUI.Label(new Rect(5, Screen.height * 0.45f, 100, 20), "Left");
		GUI.Label(new Rect(Screen.width - 30, Screen.height * 0.45f, 100, 20), "Right");
		GUI.Box(new Rect(0, Screen.height * 0.8f, Screen.width, 2), "");
		GUI.Box(new Rect(Screen.width / 2 - 1, Screen.height * 0.8f + 2, 2, Screen.height * 0.2f), "");
		GUI.Label(new Rect(Screen.width / 4, Screen.height * 0.9f, 100, 20), "Down");
		GUI.Label(new Rect(Screen.width / 4 * 3, Screen.height * 0.9f, 100, 20), "Transform");
#endif

		for (int i = 0; i < aTable.Length; i++)
		{
			for (int j = 0; j < cCol; j++)
			{
				GUI.Box(new Rect(cLeft + cSize * j, cTop + cSize * i, cSize, cSize), "");
				if (aTable[i][j] == 0)
				{
					continue;
				}
				GUI.Box(new Rect(cLeft + cSize * j, cTop + cSize * i, cSize, cSize), GetContent(aTable[i][j]));
			}
		}
		for (int i = 0; i < curCell.Length; i++)
		{
			for (int j = 0; j < curCell[i].Length; j++)
			{
				if (curCell[i][j] == 0)
				{
					continue;
				}
				if (i + curCellRow < 0)
				{
					continue;
				}
				GUI.Box(new Rect(cLeft + cSize * j + curCellCol * cSize, cTop + cSize * i + curCellRow * cSize, cSize, cSize), GetContent(curCell[i][j]));
			}
		}
		for (int i = 0; i < nextCell.Length; i++)
		{
			for (int j = 0; j < nextCell[i].Length; j++)
			{
				if (nextCell[i][j] == 0)
				{
					continue;
				}
				GUI.Box(new Rect(cLeft + cSize * j + (cCol + 0.5f) * cSize, cTop + cSize * i, cSize, cSize), GetContent(nextCell[i][j]));
			}
		}

		if (bGameOver)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - cButtonWidth / 2, Screen.height / 2 - cButtonHeight / 2, cButtonWidth, cButtonHeight), "Restart"))
			{
				DoStart();
			}
		}
	}
	string GetContent(int cellContent)
	{
		switch (cellContent)
		{
			case 0:
				return "";
			default:
				return cellContent.ToString();
		}
	}

	float curSpeed = 1;
	float fCurSpeed = 1;
	float fDeltaTime = 0;
	bool bPause = false;
	void Update()
	{
		if (!bStart)
		{
			return;
		}

		if (bGameOver)
		{
			return;
		}

		if (bPause)
		{
			return;
		}

		fDeltaTime += Time.deltaTime;
		if (fDeltaTime > fCurSpeed)
		{
			fDeltaTime -= fCurSpeed;
			if (CanMoveTo(curCellCol, curCellRow + 1))
			{
				curCellRow++;
			}
			else
			{
				DoSetCurCellDown();
				return;
			}
		}

		if (bGameOver)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (Input.mousePosition.y < Screen.height * 0.2f)
			{
				if (Input.mousePosition.x < Screen.width / 2)
				{
					DoSpeedUp();
				}
				else
				{
					DoTransform();
				}
			}
			else if (Input.mousePosition.y < Screen.height - cButtonHeight - 10)
			{
				if (Input.mousePosition.x < cLeft + cSize * 3 + curCellCol * cSize)
				{
					DoLeft();
				}
				else
				{
					DoRight();
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			DoSpeedRestore();
		}

		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			DoLeft();
		}
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			DoRight();
		}
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			DoTransform();
		}
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
		{
			DoSpeedUp();
		}
		if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
		{
			DoSpeedRestore();
		}
	}
	private void DoSpeedRestore()
	{
		fCurSpeed = curSpeed;
	}
	private void DoSpeedUp()
	{
		fCurSpeed = curSpeed / 8;
		fDeltaTime = fCurSpeed;
	}
	private void DoRight()
	{
		if (CanMoveTo(curCellCol + 1, curCellRow))
		{
			curCellCol++;
		}
	}
	private void DoLeft()
	{
		if (CanMoveTo(curCellCol - 1, curCellRow))
		{
			curCellCol--;
		}
	}
	private void DoTransform()
	{
		int transformedCellID = curCellID / 4 * 4 + (curCellID % 4 + 1) % 4;
		if (CanMoveTo(curCellCol, curCellRow, aCells[transformedCellID]))
		{
			curCellID = transformedCellID;
			curCell = aCells[transformedCellID];
		}
	}
	private void DoSetCurCellDown()
	{
		for (int i = 0; i < curCell.Length; i++)
		{
			for (int j = 0; j < curCell[i].Length; j++)
			{
				if (curCell[i][j] == 0)
				{
					continue;
				}
				if (curCellRow + i < 0)
				{
					bGameOver = true;
					return;
				}
				aTable[curCellRow + i][curCellCol + j] = curCell[i][j];
			}
		}

		DoSpeedRestore();
		DoLine();

		DoNextCell();
	}

	int lineCount = 0;
	int score = 0;
	private void DoLine()
	{
		List<int> fulledLines = new List<int>();
		for (int i = curCellRow; i < curCellRow + 4; i++)
		{
			if (i >= cRow)
			{
				continue;
			}
			if (i < 0)
			{
				bGameOver = true;
				return;
			}
			for (int j = 0; j < cCol; j++)
			{
				if (aTable[i][j] == 0)
				{
					break;
				}
				if (j == cCol - 1)
				{
					fulledLines.Add(i);
				}
			}
		}

		if (fulledLines.Count == 0)
		{
			return;
		}

		for (int i = 0; i < fulledLines.Count; i++)
		{
			for (int j = 0; j < cCol; j++)
			{
				aTable[fulledLines[i]][j] = 0;
			}
		}

		int ilastEmptyRow = fulledLines[fulledLines.Count - 1];
		int ilastNotEmptyRow = ilastEmptyRow - 1;
		while (true)
		{
			if (ilastNotEmptyRow < 0)
			{
				break;
			}

			if (!IsRowEmpty(ilastNotEmptyRow))
			{
				for (int j = 0; j < cCol; j++)
				{
					aTable[ilastEmptyRow][j] = aTable[ilastNotEmptyRow][j];
					aTable[ilastNotEmptyRow][j] = 0;
				}
				ilastEmptyRow--;
			}
			ilastNotEmptyRow--;
		}

		for (int i = 0; i < fulledLines.Count; i++)
		{
			for (int j = 0; j < cCol; j++)
			{
				aTable[i][j] = 0;
			}
		}

		lineCount += fulledLines.Count;
		score += fulledLines.Count * fulledLines.Count;

		curSpeed *= 0.99f;
	}
	bool IsRowEmpty(int irow)
	{
		for (int j = 0; j < cCol; j++)
		{
			if (aTable[irow][j] != 0)
			{
				return false;
			}
		}
		return true;
	}
	bool CanMoveTo(int x, int y, int[][] cell = null)
	{
		if (cell == null)
		{
			cell = curCell;
		}
		for (int i = 0; i < cell.Length; i++)
		{
			for (int j = 0; j < cell[i].Length; j++)
			{
				if (cell[i][j] == 0)
				{
					continue;
				}
				if (x + j < 0 || x + j >= cCol)
				{
					return false;
				}
				if (y + i >= cRow)
				{
					return false;
				}
				if (y + i < 0)
				{
					continue;
				}
				if (aTable[i + y][j + x] != 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	private int[][][] aCells = new int[][][]
	{
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,1,0},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,1,0},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,1,0},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,1,0},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},

		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,1,1},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,0,0},
			new int[]{0,1,1,0},
			new int[]{0,0,1,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,1,1},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,0,0},
			new int[]{0,1,1,0},
			new int[]{0,0,1,0},
		},

		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,1,0,0},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,1,0},
			new int[]{0,1,1,0},
			new int[]{0,1,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,1,0,0},
			new int[]{0,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,1,0},
			new int[]{0,1,1,0},
			new int[]{0,1,0,0},
		},

		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,1,1,1},
			new int[]{0,0,0,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,1,1,1},
			new int[]{0,0,0,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
		},

		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,0,0},
			new int[]{1,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,0,0},
			new int[]{0,1,1,0},
			new int[]{0,1,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,0,0},
			new int[]{1,1,1,0},
			new int[]{0,1,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,0,0},
			new int[]{1,1,0,0},
			new int[]{0,1,0,0},
		},

		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,1,0},
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,1,0},
			new int[]{1,1,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,0,0},
			new int[]{0,1,0,0},
			new int[]{0,1,1,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,1,1,0},
			new int[]{1,0,0,0},
			new int[]{0,0,0,0},
		},

		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,1,1,0},
			new int[]{0,1,0,0},
			new int[]{0,1,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,1,1,0},
			new int[]{0,0,1,0},
			new int[]{0,0,0,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{0,0,1,0},
			new int[]{0,0,1,0},
			new int[]{0,1,1,0},
		},
		new int[][]
		{
			new int[]{0,0,0,0},
			new int[]{1,0,0,0},
			new int[]{1,1,1,0},
			new int[]{0,0,0,0},
		},
	};
}
