using UnityEngine;
#if UNITY_EDITOR || WINDOWS || LINUX
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerKeysSO", menuName = "Megaman/PlayerKeysSO", order = 1)]
public class PlayerKeysSO : ScriptableObject {
    public KeyCode jumpKey, leftKey, rightKey, fire;
    public string otherPlayerLayer;

#if UNITY_EDITOR || WINDOWS || LINUX
    [MenuItem("Megaman/Create/Player Keys")]
    public static void CreateMyAsset()
    {
        PlayerKeysSO asset = CreateInstance<PlayerKeysSO>();

        AssetDatabase.CreateAsset(asset, "Assets/NewPlayerKeysSO.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
#endif
}
