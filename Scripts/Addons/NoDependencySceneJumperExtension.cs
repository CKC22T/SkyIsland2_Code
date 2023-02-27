#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class SceneJumperExtension : EditorWindow
{


	[MenuItem("Jumper/Olympus/Scenes/AITestScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_AITestScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/AITestScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/AnimatedUIScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_AnimatedUIScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/AnimatedUIScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/BossTestScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_BossTestScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/BossTestScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/Empty", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_Empty()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/Empty.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InGameBossScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InGameBossScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InGameBossScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InGameEndingScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InGameEndingScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InGameEndingScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InGameFallScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InGameFallScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InGameFallScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InGameSpringScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InGameSpringScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InGameSpringScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InGameSummerScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InGameSummerScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InGameSummerScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InGameWinterScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InGameWinterScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InGameWinterScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/InteractiveGrassSample", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_InteractiveGrassSample()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/InteractiveGrassSample.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/LevelTestScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_LevelTestScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/LevelTestScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/Main", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_Main()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/Main.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/MufflerSampleScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_MufflerSampleScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/MufflerSampleScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/PhotoModeSample", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_PhotoModeSample()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/PhotoModeSample.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/SplashScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_SplashScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/SplashScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/Startup", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_Startup()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/Startup.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/TestScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_TestScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/TestScene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/TitleScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_TitleScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/TitleScene.unity");
	}

	[MenuItem("Jumper/Olympus/Art/Scenes/AD_Scene01", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Art_Scenes_AD_Scene01()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Art/Scenes/AD_Scene01.unity");
	}

	[MenuItem("Jumper/Olympus/Art/Scenes/AD_Scene02", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Art_Scenes_AD_Scene02()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Art/Scenes/AD_Scene02.unity");
	}

	[MenuItem("Jumper/Olympus/Art/Scenes/FX_Scene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Art_Scenes_FX_Scene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Art/Scenes/FX_Scene.unity");
	}

	[MenuItem("Jumper/Olympus/Art/Scenes/V.0.1_Playtest_VFX_Test", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Art_Scenes_V_0_1_Playtest_VFX_Test()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Art/Scenes/V.0.1_Playtest_VFX_Test.unity");
	}

	[MenuItem("Jumper/Olympus/Art/UI/UI_Scene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Art_UI_UI_Scene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Art/UI/UI_Scene.unity");
	}

	[MenuItem("Jumper/Olympus/Scenes/GraphicsScenes/InteractiveSystemTestScene", priority = 200)]
	private static void ChangeTo_Jumper_Olympus_Scenes_GraphicsScenes_InteractiveSystemTestScene()
	{
		SceneJumper.ChangeScene("Assets/Olympus/Scenes/GraphicsScenes/InteractiveSystemTestScene.unity");
	}

	[MenuItem("Jumper/Tools/TerrainRotator/DEMO/Scenes/scene_RotationDemo", priority = 200)]
	private static void ChangeTo_Jumper_Tools_TerrainRotator_DEMO_Scenes_scene_RotationDemo()
	{
		SceneJumper.ChangeScene("Assets/Tools/TerrainRotator/DEMO/Scenes/scene_RotationDemo.unity");
	}

}

#endif