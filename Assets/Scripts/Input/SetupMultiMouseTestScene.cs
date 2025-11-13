/*
 * Unity6_MultiMouse Project - Scene Setup Script
 * このスクリプトは Unity Editor のコンテキストメニューから実行し、
 * テストシーンを自動作成します。
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity6MultiMouse.Input;

public class SetupMultiMouseTestScene
{
    [MenuItem("Tools/Unity6_MultiMouse/Setup Test Scene")]
    public static void CreateTestScene()
    {
        // 新規シーン作成
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // InputManager GameObject 作成
        GameObject inputManagerGO = new GameObject("InputManager");
        
        // MultiMouseInput コンポーネント追加
        inputManagerGO.AddComponent<MultiMouseInput>();
        
        // テスター コンポーネント追加
        inputManagerGO.AddComponent<MultiMouseInputInitTester>();
        
        // シーンを保存
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MultiMouseTest.unity");
        
        Debug.Log("✅ テストシーン作成完了: Assets/Scenes/MultiMouseTest.unity");
        Debug.Log("Play モードで実行して、コンソールでマウス数を確認してください。");
    }
}
#endif
