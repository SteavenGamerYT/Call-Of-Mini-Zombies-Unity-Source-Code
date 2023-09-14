using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Zombie3D;

public class GameParametersXML
{
	public SpawnConfig Load(string path, int levelNum, bool isEndless)
	{
		SpawnConfig spawnConfig = new SpawnConfig();
		spawnConfig.Waves = new List<Wave>();
		Stream stream = null;
		XmlDocument xmlDocument = new XmlDocument();
		if (path != null)
		{
			Debug.Log("path not null");
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlDocument.Load(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			xmlDocument.LoadXml(configXml.text);
		}
		Wave wave = null;
		Round round = null;
		XmlNodeList xmlNodeList = null;
		if (isEndless)
		{
			XmlNodeList xmlNodeList2 = xmlDocument.SelectNodes("Config/EnemySpawns/Endless");
			xmlNodeList = xmlNodeList2[levelNum].SelectNodes("Wave");
		}
		else
		{
			XmlNodeList xmlNodeList3 = xmlDocument.SelectNodes("Config/EnemySpawns/Level");
			if (levelNum <= xmlNodeList3.Count)
			{
				levelNum--;
			}
			else
			{
				int num = UnityEngine.Random.Range(xmlNodeList3.Count - 10, xmlNodeList3.Count);
				levelNum = num;
			}
			Debug.Log("levelNum" + levelNum);
			xmlNodeList = xmlNodeList3[levelNum].SelectNodes("Wave");
		}
		IEnumerator enumerator = xmlNodeList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				XmlNode xmlNode = (XmlNode)enumerator.Current;
				wave = new Wave();
				wave.Rounds = new List<Round>();
				spawnConfig.Waves.Add(wave);
				wave.intermission = int.Parse(xmlNode.Attributes["intermission"].Value);
				XmlNodeList xmlNodeList4 = xmlNode.SelectNodes("Round");
				IEnumerator enumerator2 = xmlNodeList4.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						XmlNode xmlNode2 = (XmlNode)enumerator2.Current;
						round = new Round();
						round.EnemyInfos = new List<EnemyInfo>();
						wave.Rounds.Add(round);
						round.intermission = int.Parse(xmlNode2.Attributes["intermission"].Value);
						XmlNodeList xmlNodeList5 = xmlNode2.SelectNodes("Enemy");
						IEnumerator enumerator3 = xmlNodeList5.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								XmlNode xmlNode3 = (XmlNode)enumerator3.Current;
								EnemyInfo enemyInfo = new EnemyInfo();
								round.EnemyInfos.Add(enemyInfo);
								switch (xmlNode3.Attributes["id"].Value)
								{
								case "zombie":
									enemyInfo.EType = EnemyType.E_ZOMBIE;
									break;
								case "nurse":
									enemyInfo.EType = EnemyType.E_NURSE;
									break;
								case "tank":
									enemyInfo.EType = EnemyType.E_TANK;
									break;
								case "hunter":
									enemyInfo.EType = EnemyType.E_HUNTER;
									break;
								case "boomer":
									enemyInfo.EType = EnemyType.E_BOOMER;
									break;
								case "swat":
									enemyInfo.EType = EnemyType.E_SWAT;
									break;
								case "dog":
									enemyInfo.EType = EnemyType.E_DOG;
									break;
								case "police":
									enemyInfo.EType = EnemyType.E_POLICE;
									break;
								}
								enemyInfo.Count = int.Parse(xmlNode3.Attributes["count"].Value);
								string value = xmlNode3.Attributes["from"].Value;
								if (value == "grave")
								{
									enemyInfo.From = SpawnFromType.Grave;
								}
								else if (value == "door")
								{
									enemyInfo.From = SpawnFromType.Door;
								}
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = enumerator3 as IDisposable) != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = enumerator2 as IDisposable) != null)
					{
						disposable2.Dispose();
					}
				}
			}
		}
		finally
		{
			IDisposable disposable3;
			if ((disposable3 = enumerator as IDisposable) != null)
			{
				disposable3.Dispose();
			}
		}
		if (stream != null)
		{
			stream.Close();
		}
		return spawnConfig;
	}
}
