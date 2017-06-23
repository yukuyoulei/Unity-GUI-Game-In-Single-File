using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>
public class UTapAndTurn : MonoBehaviour
{
	private static UTapAndTurn sintance;
	public static UTapAndTurn Instance
	{
		get
		{
			return sintance;
		}
	}
	int Level
	{
		get
		{
			return PlayerPrefs.GetInt("Level", 1);
		}
		set
		{
			PlayerPrefs.SetInt("Level", value);
		}
	}
	int Row
	{
		get
		{
			return (6 + Level) / 2;
		}
	}
	int Col
	{
		get
		{
			return (6 + Level) / 2;
		}
	}
	int _iTap;
	int iTap
	{
		get
		{
			return _iTap;
		}
		set
		{
			_iTap = value;
			titleText = "Tap:" + _iTap;
		}
	}

	List<List<UTapAndTurnCell>> lCells = new List<List<UTapAndTurnCell>>();
	private void Start()
	{
		sintance = this;

		Rerange();
	}

	DateTime startTime;
	public static int top = 100;
	private void Rerange()
	{
		iTap = 0;
		bNowFinished = false;
		startTime = DateTime.Now;

		lCells.Clear();

		for (int i = 0; i < Col; i++)
		{
			List<UTapAndTurnCell> l = new List<UTapAndTurnCell>();
			for (int j = 0; j < Row; j++)
			{
				UTapAndTurnCell c = new UTapAndTurnCell();
				c.width = (Screen.width - 10) / Col - 5;
				c.height = (Screen.height - top - 10) / Row - 5;
				c.x = i * (c.width + 5) + 10;
				c.y = j * (c.height + 5) + 100;
				c.indexX = i;
				c.indexY = j;
				c.bUp = true;
				l.Add(c);
			}
			lCells.Add(l);
		}

		int icount = Row * Col;
		int irandom = icount / 2;
		List<int> lr = new List<int>();
		for (int i = 0; i < irandom; i++)
		{
			int irdm = 0;
			do
			{
				irdm = UnityEngine.Random.Range(0, icount);
			} while (lr.Contains(irdm));
			lr.Add(irdm);
			lCells[irdm / Col][irdm % Row].bUp = false;
		}

	}
	int BestLevel
	{
		get
		{
			return PlayerPrefs.GetInt("BestLevel");
		}
		set
		{
			if (BestLevel > 0 && value < BestLevel)
			{
				return;
			}
			PlayerPrefs.SetInt("BestLevel", value);
		}
	}

	double dTime;
	private void OnGUI()
	{
		GUI.color = Color.white;

		GUILayout.Label(titleText + " 当前等级：" + (Level + 1) + (BestLevel > 0 ? " 最高等级：" + BestLevel : ""));

		GUILayout.Label("倒计时:" + (int)(dTime));

		if (!bNowFinished)
		{
			dTime = 60 - (DateTime.Now - startTime).TotalSeconds;
		}
		else
		{
			GUILayout.Label("Press to rerange the pieces");

		}
		foreach (var lc in lCells)
		{
			foreach (var c in lc)
			{
				c.OnDraw();
			}
		}

		if (bNowFinished)
		{
			if (GUI.Button(new Rect((Screen.width - Screen.width / 5) / 2, (Screen.height - Screen.height / 10) / 2, Screen.width / 5, Screen.height / 10), "Rerange"))
			{
				Rerange();
			}
		}
		else if (!bNowFinished && dTime < 0)
		{
			DoFinish(false);
		}
	}
	public bool bNowFinished = false;
	string titleText = "";
	private void DoFinish(bool bSuccess = true)
	{
		if (bSuccess)
		{
			Level++;
		}
		else
		{
			BestLevel = Level;

			Level = 0;
		}

		bNowFinished = true;

	}

	public void OnClick(int x, int y)
	{
		if (bNowFinished)
		{
			Rerange();
			return;
		}
		iTap++;

		lCells[x][y].bUp = !lCells[x][y].bUp;
		if (x - 1 >= 0)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				lCells[x - 1][y].bUp = !lCells[x - 1][y].bUp;
			}
		}
		if (y - 1 >= 0)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				lCells[x][y - 1].bUp = !lCells[x][y - 1].bUp;
			}
		}
		if (x + 1 < Col)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				lCells[x + 1][y].bUp = !lCells[x + 1][y].bUp;
			}
		}
		if (y + 1 < Row)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				lCells[x][y + 1].bUp = !lCells[x][y + 1].bUp;
			}
		}

		bool? bUp = null;
		foreach (var lg in lCells)
		{
			foreach (var g in lg)
			{
				if (bUp.HasValue)
				{
					if (bUp.Value != g.bUp)
					{
						return;
					}
				}
				else
				{
					bUp = g.bUp;
				}
			}
		}

		DoFinish();
	}
}

public class UTapAndTurnCell
{
	public Texture2D cell;
	public int x, y, width, height;
	public int indexX, indexY;
	private bool _bUp;
	public bool bUp
	{
		get
		{
			return _bUp;
		}
		set
		{
			if (_bUp != value)
			{
				cell = new Texture2D(width, height);
			}
			_bUp = value;
		}
	}
	public Color color
	{
		get
		{
			return bUp ? Color.yellow : Color.green;
		}
	}
	public void OnDraw()
	{
		GUI.color = color;
		if (UTapAndTurn.Instance.bNowFinished)
		{
			GUI.DrawTexture(new Rect(x, y, width, height), cell);
			return;
		}
		else if (GUI.Button(new Rect(x + 1, y + 1, width - 2, height - 2), cell))
		{
			UTapAndTurn.Instance.OnClick(indexX, indexY);
		}
		GUI.DrawTexture(new Rect(x, y, width, height), cell);
	}
}
