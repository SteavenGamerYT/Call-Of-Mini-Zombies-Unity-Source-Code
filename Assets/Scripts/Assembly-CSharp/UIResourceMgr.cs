using System;
using System.Collections;
using UnityEngine;

public class UIResourceMgr
{
	protected ArrayList materialList = new ArrayList();

	protected static UIResourceMgr instance = new UIResourceMgr();

	protected string m_ui_material_path;

	public static UIResourceMgr GetInstance()
	{
		return instance;
	}

	public Material GetMaterial(string name)
	{
		IEnumerator enumerator = materialList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MaterialInfo materialInfo = (MaterialInfo)enumerator.Current;
				if (materialInfo.name == name)
				{
					return materialInfo.material;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		Material material = LoadUIMaterial(name);
		MaterialInfo materialInfo2 = new MaterialInfo();
		materialInfo2.material = material;
		materialInfo2.name = name;
		materialList.Add(materialInfo2);
		return material;
	}

	protected Material LoadUIMaterial(string name)
	{
		string text = m_ui_material_path + name;
		if (Application.platform != RuntimePlatform.Android && ResolutionConstant.R == 0.5f)
		{
			text += "_3gs";
		}
		Material material = Resources.Load(text) as Material;
		if (material == null)
		{
			Debug.Log("load material error: " + text);
		}
		return material;
	}

	public void PrintExistingMaterials()
	{
		string text = "Print : ";
		IEnumerator enumerator = materialList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MaterialInfo materialInfo = (MaterialInfo)enumerator.Current;
				text = text + materialInfo.name + ", ";
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	protected void UnLoadMaterial(string name)
	{
		string text = m_ui_material_path + name;
		for (int num = materialList.Count - 1; num > 0; num--)
		{
			MaterialInfo materialInfo = materialList[num] as MaterialInfo;
			if (materialInfo.name == name)
			{
				materialInfo.material = null;
				materialList.Remove(materialInfo);
				Debug.Log("UnLoad UI Resource:" + name);
				return;
			}
		}
		Debug.Log("Unload material error: " + text);
	}

	public void LoadStartMenuUIMaterials()
	{
		GetInstance().GetMaterial("StartMenu");
		GetInstance().GetMaterial("Dialog");
		GetInstance().GetMaterial("Buttons");
		GetInstance().PrintExistingMaterials();
	}

	public void LoadAllUIMaterials()
	{
		GetInstance().GetMaterial("ArenaMenu");
		GetInstance().GetMaterial("Buttons");
		GetInstance().GetMaterial("ShopUI");
		GetInstance().GetMaterial("Weapons");
		GetInstance().GetMaterial("Weapons2");
		GetInstance().GetMaterial("Weapons3");
		GetInstance().GetMaterial("Avatar");
		GetInstance().GetMaterial("Dialog");
		GetInstance().PrintExistingMaterials();
	}

	public void LoadMapUIMaterials()
	{
		GetInstance().GetMaterial("ShopUI");
		GetInstance().GetMaterial("Buttons");
		GetInstance().GetMaterial("ArenaMenu");
	}

	public void LoadAllGameUIMaterials()
	{
		GetInstance().GetMaterial("GameUI");
		GetInstance().GetMaterial("Buttons");
		GetInstance().PrintExistingMaterials();
	}

	public void UnloadAllUIMaterials()
	{
		materialList.Clear();
		Resources.UnloadUnusedAssets();
		GetInstance().PrintExistingMaterials();
	}
}
