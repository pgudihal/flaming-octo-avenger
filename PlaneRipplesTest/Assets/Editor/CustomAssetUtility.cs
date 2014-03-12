using UnityEngine;
using UnityEditor;
using System.IO;

public static class CustomAssetUtility
{
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension (path) != "")
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	//all paths are relative to the assets folder. no need to specify that its in the assets folder
	public static void CreateAssetAtPath<T> (string path, string AssetName,T asset) where T : ScriptableObject
	{
		//T asset = ScriptableObject.CreateInstance<T>();

		path = "Assets" + "/" + path;
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(T).ToString() + "-" + AssetName + ".asset");

		AssetDatabase.CreateAsset(asset,assetPathAndName);
		AssetDatabase.SaveAssets();
	}
}