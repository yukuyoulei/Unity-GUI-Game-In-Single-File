using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// Written by FS
/// 
/// </summary>
public class UHanoi : MonoBehaviour
{
	int CurLevel
	{
		get
		{
			return PlayerPrefs.GetInt("CurLevel") + 1;
		}
		set
		{
			PlayerPrefs.SetInt("CurLevel", value - 1);
		}
	}
	int towerCount = 3;
	int floorCount
	{
		get
		{
			return 3 + CurLevel - 1;
		}
	}
	float towerWidth;
	float towerHeight;
	List<List<int>> lTowers = new List<List<int>>();
	void Start()
	{
	}
	void DoInit()
	{
		lTowers.Clear();
		for (int i = 0; i < towerCount; i++)
		{
			lTowers.Add(new List<int>());
		}
		for (int i = floorCount; i > 0; i--)
		{
			lTowers[0].Add(i);
		}

		floorWidth = Screen.width / (towerCount + 1);
		floorHeight = Screen.height * 0.5f / (floorCount + 1);
		towerWidth = floorWidth * 1.2f;
		towerHeight = floorHeight * (floorCount + 2);
	}
	void DoStart()
	{
		bStarting = true;
		ioperationCount = 0;
		selectedTower = null;
	}
	bool bStarting;
	float btnWidth = 200;
	float btnHeight = 100;
	float floorWidth = 0;
	float floorHeight = 0;

	int? selectedTower;
	int ioperationCount;
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 1000, 20),"Level:" + CurLevel);
		if (!bStarting)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - btnWidth, Screen.height / 2 - btnHeight, btnWidth, btnHeight), "Start"))
			{
				DoInit();
				DoStart();
			}
			return;
		}

		if (bGameOver)
		{
			GUI.Label(new Rect(Screen.width / 2 - btnWidth, Screen.height / 2 - btnHeight - 30, btnWidth * 2, 30), "Success! You spend " + ioperationCount + " operations");
			if (GUI.Button(new Rect(Screen.width / 2 - btnWidth, Screen.height / 2 - btnHeight, btnWidth, btnHeight), "Restart"))
			{
				bGameOver = false;
				bStarting = false;
			}
			return;
		}

		for (int i = 0; i < towerCount; i++)
		{
			if (selectedTower.HasValue && selectedTower.Value == i)
			{
				GUI.Box(new Rect(i * (towerWidth + 10) + 20, Screen.height * 0.8f - floorHeight * (floorCount + 2), floorWidth * 1.2f, towerHeight), "");
			}
			if (GUI.Button(new Rect(i * (towerWidth + 10) + 20, Screen.height * 0.8f - floorHeight * (floorCount + 2), floorWidth * 1.2f, towerHeight), ""))
			{
				if (bGameOver)
				{
					return;
				}
				if (selectedTower.HasValue)
				{
					if (selectedTower.Value == i)
					{
						selectedTower = null;
						return;
					}

					if (lTowers[i].Count == 0 || lTowers[selectedTower.Value][lTowers[selectedTower.Value].Count - 1] < lTowers[i][lTowers[i].Count - 1])
					{
						DoMoveFloor(i);
					}
				}
				else if (lTowers[i].Count > 0)
				{
					selectedTower = i;
				}
			}

			GUI.Box(new Rect(i * (towerWidth + 10) + 20, Screen.height * 0.8f - floorHeight * 0.5f, floorWidth * 1.2f, floorHeight * 0.5f), "|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
			GUI.Box(new Rect(i * (towerWidth + 10) + 20 + towerWidth / 2 - 10, Screen.height * 0.8f - towerHeight, 20, towerHeight), "=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n=\r\n");
			for (int j = 0; j < lTowers[i].Count; j++)
			{
				float curFloorWidth = lTowers[i][lTowers[i].Count - j - 1] * 0.15f * floorWidth;
				GUI.Box(new Rect(i * (towerWidth + 10) + 20 + towerWidth * 0.5f - curFloorWidth * 0.5f, Screen.height * 0.8f - floorHeight * 0.5f - floorHeight * (lTowers[i].Count - j), curFloorWidth, floorHeight), "");
			}
		}
	}
	bool bGameOver;
	void DoMoveFloor(int moveToTower)
	{
		int ifloor = lTowers[selectedTower.Value][lTowers[selectedTower.Value].Count - 1];
		lTowers[selectedTower.Value].RemoveAt(lTowers[selectedTower.Value].Count - 1);
		lTowers[moveToTower].Add(ifloor);
		ioperationCount++;
		selectedTower = null;

		if (moveToTower != 0 && lTowers[moveToTower].Count == floorCount)
		{
			bGameOver = true;
			CurLevel++;
		}
	}
}
