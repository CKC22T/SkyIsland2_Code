using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class Constant
    {

    }

    public enum SceneType
    {
        // << Scene
    TestScene,
    Startup,
    PhotoModeSample,
    MufflerSampleScene,
    None,
    InteractiveGrassSample,
    SnowTessellation_Test,
    Empty,
    BossTestScene,
    AITestScene,
    TitleScene,
    InGameSpringScene,
    InGameSummerScene,
    InGameFallScene,
    InGameWinterScene,
    InGameBossScene,
    InGameEndingScene,
    SplashScene,// >> Scene
    }

    public enum EntityType
    {
        None = 0,

        TestPlayer,
        TestEnemy,

        Player,
        MagicBoar,
        Puri,
        Boss,

        Wisp,
        Spirit,
        FlowerTrap,

        MagicStone,
        TreasureBox,
        CheckPoint,
        JumpFlower,
        Wall,

        OldPuri,
        MiniPuri,
        MayourPuri,
        KeiraPuri,
        RhachiumPuri,
        KidPuri,
        MerchantPuri,
        GangsterPuri,
        FlowerPuri,
        DeveloperPuri_1,
        DeveloperPuri_2,
        BluePuri,
        BlueGreenPuri,
        GreenPuri,
        LightYellowPuri,
        LightGreenPuri,
        SpeakDoar,
        Snowball,

        BoxPuri,

        End,
    }

    public enum ActionType
    {
        None = 0,

        Idle,

        Move,
        Dash,
        Chase,
        Jump,

        Attack,
        SecondaryAttack,
        Skill,
        SecondarySkill,

        Dead,

        Walk,
        Run,
        Emotion,
        Fly,
        GroundCharging,
        MountedFly,
        Alert,

        Roar,
        Teleport,
        Ultimate,
        Hit,

        Spawn,

        End,
    }

    public enum ItemType
    { 
        None = 0,

        HP,
        Star,


        End,
    }

    public enum IslandType
    {
        None = 0,

        Spring,
        Summer,
        Fall,
        Winter,

        Boss,
        Ending,

        End,
    }

    public enum TutorialType
    {
        Move,
        Jump,
        Mouse,
    }

    public static class IslandNameInfo
    {
        public static readonly Dictionary<IslandType, string> ISLAND_NAME = new()
        {
            { IslandType.Spring, "Text_Island_Spring" },
            { IslandType.Summer, "Text_Island_Summer" },
            { IslandType.Fall, "Text_Island_Fall" },
            { IslandType.Winter, "Text_Island_Winter" },
            { IslandType.Boss, "Text_Island_Boss" },
        };

        public static readonly Dictionary<IslandType, string> OLYMPUS_NAME = new()
        {
            { IslandType.Spring, "EPIRUS" },
            { IslandType.Summer, "IO" },
            { IslandType.Fall, "THESSALY" },
            { IslandType.Winter, "CRETA" },
            { IslandType.Boss, "HOMEROS" },
        };
    }

    public static class TutorialInfo
    {
        public static readonly Dictionary<int, TutorialType> TUTORIAL_TYPE = new()
        {
            { 10109, TutorialType.Move },
            { 10201, TutorialType.Jump },
            { 10601, TutorialType.Mouse },
        };
    }

    public static class GameData
    {
        public static readonly int[] UPGRADE_STAR = { 10, 20, 30 };

        public static readonly Dictionary<IslandType, int> OLYMPUS_ACHIEVEMNET = new()
        {
            { IslandType.Spring, 10000 },
            { IslandType.Summer, 20000 },
            { IslandType.Fall, 20000 },
            { IslandType.Winter, 10000 },
        };

        public static IslandType SceneTypeToIslaneType(SceneType sceneType)
        {
            IslandType islandType = IslandType.None;

            switch(sceneType)
            {
                case SceneType.InGameSpringScene:
                    islandType = IslandType.Spring;
                    break;
                case SceneType.InGameSummerScene:
                    islandType = IslandType.Summer;
                    break;
                case SceneType.InGameFallScene:
                    islandType = IslandType.Fall;
                    break;
                case SceneType.InGameWinterScene:
                    islandType = IslandType.Winter;
                    break;
                case SceneType.InGameBossScene:
                    islandType = IslandType.Boss;
                    break;
                case SceneType.InGameEndingScene:
                    islandType = IslandType.Ending;
                    break;
            }

            return islandType;
        }

        public static SceneType IslandTypeToSceneType(IslandType islandType)
        {
            SceneType sceneType = SceneType.None;

            switch (islandType)
            {
                case IslandType.Spring:
                    sceneType = SceneType.InGameSpringScene;
                    break;
                case IslandType.Summer:
                    sceneType = SceneType.InGameSummerScene;
                    break;
                case IslandType.Fall:
                    sceneType = SceneType.InGameFallScene;
                    break;
                case IslandType.Winter:
                    sceneType = SceneType.InGameWinterScene;
                    break;
                case IslandType.Boss:
                    sceneType = SceneType.InGameBossScene;
                    break;
                case IslandType.Ending:
                    sceneType = SceneType.InGameEndingScene;
                    break;
            }

            return sceneType;
        }
    }

    public static class LayerData
    {
        public static readonly int LAYER_DEFAULT = LayerMask.NameToLayer("Default");
        public static readonly int LAYER_PLAYER = LayerMask.NameToLayer("Player");
        public static readonly int LAYER_PURI = LayerMask.NameToLayer("Puri");
        public static readonly int LAYER_ENEMY = LayerMask.NameToLayer("Enemy");
        public static readonly int LAYER_BOSS = LayerMask.NameToLayer("Boss");
        public static readonly int LAYER_ENVIRONMENT = LayerMask.NameToLayer("Environment");

        public static readonly int LAYER_MASK_DEFAULT = 1 << LAYER_DEFAULT;
        public static readonly int LAYER_MASK_PLAYER = 1 << LAYER_PLAYER;
        public static readonly int LAYER_MASK_PURI = 1 << LAYER_PURI;
        public static readonly int LAYER_MASK_ENEMY = 1 << LAYER_ENEMY;
        public static readonly int LAYER_MASK_BOSS = 1 << LAYER_BOSS;
        public static readonly int LAYER_MASK_PLAYER_COLLISION = (1 << LAYER_DEFAULT)
                                                                | (1 << LAYER_ENEMY)
                                                                | (1 << LAYER_BOSS)
                                                                | (1 << LAYER_PLAYER)
                                                                | (1 << LAYER_ENVIRONMENT);
    }
}