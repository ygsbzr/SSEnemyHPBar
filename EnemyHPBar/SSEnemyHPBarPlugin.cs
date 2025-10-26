using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EnemyHPBar;
using HarmonyLib;

namespace SSEnemyHPBar;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.ygsbzr.ssenemyhpbar")]
public partial class SSEnemyHPBarPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger
    {
        get => field ?? throw new InvalidOperationException("instance not present");
        private set;
    }
    private void Awake()
    {
        if (!Directory.Exists(DATA_DIR))
        {
            Directory.CreateDirectory(DATA_DIR);
        }
        Logger = base.Logger;
        // Put your initialization logic here

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
        Harmony.CreateAndPatchAll(typeof(Patches));
    }
    private void Start()
    {
        DefineConfig();
        CompleteImage(DATA_DIR);

        LoadLoader();
        SceneManager.sceneLoaded += ChangeScene;
        canvas = CanvasUtil.CreateCanvas(RenderMode.WorldSpace, new Vector2(1920f, 1080f));
        bossCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920f, 1080f));
        canvas.GetComponent<Canvas>().sortingOrder = 1;
        bossCanvas.GetComponent<Canvas>().sortingOrder = 1;


        bossol = HPBarCreateSprite(ResourceLoader.GetBossOutlineImage());
        bossbg = HPBarCreateSprite(ResourceLoader.GetBossBackgroundImage());
        bossfg = HPBarCreateSprite(ResourceLoader.GetBossForegroundImage());
        ol = HPBarCreateSprite(ResourceLoader.GetOutlineImage());
        fg = HPBarCreateSprite(ResourceLoader.GetForegroundImage());
        mg = HPBarCreateSprite(ResourceLoader.GetMiddlegroundImage());
        bg = HPBarCreateSprite(ResourceLoader.GetBackgroundImage());
        UObject.DontDestroyOnLoad(canvas);
        UObject.DontDestroyOnLoad(bossCanvas);
    }

    private void ChangeScene(Scene arg0, LoadSceneMode arg1)
    {
        ActiveBosses = new();
    }

    public static void CompleteImage(string skinpath)
    {
        foreach (string res in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(t => t.EndsWith("png")))
        {
            string properRes = res.Replace("SSEnemyHPBar.Resources.", "");
            string resPath = Path.Combine(skinpath, properRes);
            if (File.Exists(resPath))
            {
                continue;
            }

            using FileStream file = File.Create(resPath);
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
            stream.CopyTo(file);
        }
    }

    public void LoadLoader()
    {
        if (spriteLoader == null)
        {
            spriteLoader = new GameObject();
            spriteLoader.AddComponent<ResourceLoader>();
        }
    }

    public static Sprite HPBarCreateSprite(byte[] data)
    {
        var texture2D = new Texture2D(1, 1);
        texture2D.LoadImage(data);
        texture2D.anisoLevel = 0;
        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
    }

    private void DefineConfig()
    {
        ConfigEntry<float> cfgfgScale = Config.Bind(
            configDefinition: new ConfigDefinition("hpbar", "fgscale"),
            defaultValue: 1.0f,
            configDescription: new ConfigDescription("test")
            );
        cfgfgScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.fgScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.fgScale = cfgfgScale.Value;

        ConfigEntry<float> cfgbgScale = Config.Bind(
            configDefinition: new ConfigDefinition("hpbar", "bgscale"),
            defaultValue: 1.0f,
            configDescription: new ConfigDescription("test")
            );
        cfgbgScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.bgScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.bgScale = cfgbgScale.Value;

        ConfigEntry<float> cfgmgScale = Config.Bind(
            configDefinition: new ConfigDefinition("hpbar", "mgscale"),
            defaultValue: 1.0f,
            configDescription: new ConfigDescription("test")
            );
        cfgmgScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.mgScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.mgScale = cfgmgScale.Value;
        ConfigEntry<float> cfgolScale = Config.Bind(
            configDefinition: new ConfigDefinition("hpbar", "olscale"),
            defaultValue: 1.0f,
            configDescription: new ConfigDescription("test")
            );
        cfgolScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.olScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.olScale = cfgolScale.Value;

        ConfigEntry<float> cfgBossFgScale = Config.Bind(
        configDefinition: new ConfigDefinition("bossbar", "bossfgscale"),
        defaultValue: 1.0f,
        configDescription: new ConfigDescription("BossÑªÌõÇ°¾°Ëõ·Å")
    );
        cfgBossFgScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.bossfgScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.bossfgScale = cfgBossFgScale.Value;

        // Boss ±³¾°Ëõ·Å
        ConfigEntry<float> cfgBossBgScale = Config.Bind(
            configDefinition: new ConfigDefinition("bossbar", "bossbgscale"),
            defaultValue: 1.0f,
            configDescription: new ConfigDescription("BossÑªÌõ±³¾°Ëõ·Å")
        );
        cfgBossBgScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.bossbgScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.bossbgScale = cfgBossBgScale.Value;

        // Boss ÂÖÀªËõ·Å
        ConfigEntry<float> cfgBossOlScale = Config.Bind(
            configDefinition: new ConfigDefinition("bossbar", "bossolscale"),
            defaultValue: 1.0f,
            configDescription: new ConfigDescription("BossÑªÌõÂÖÀªËõ·Å")
        );
        cfgBossOlScale.SettingChanged += (o, e) =>
        {
            SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            ConfigSettings.bossolScale = (float)args.ChangedSetting.BoxedValue;
        };
        ConfigSettings.bossolScale = cfgBossOlScale.Value;

    }
    public static GameObject canvas;
    public static GameObject bossCanvas;
    private static GameObject spriteLoader;
    public static List<GameObject> ActiveBosses;

    public const string HPBAR_BG = "bg.png";
    public const string HPBAR_FG = "fg.png";
    public const string HPBAR_MG = "mg.png";
    public const string HPBAR_OL = "ol.png";
    public const string HPBAR_BOSSOL = "bossol.png";
    public const string HPBAR_BOSSFG = "bossfg.png";
    public const string HPBAR_BOSSBG = "bossbg.png";
    public const string SPRITE_FOLDER = "CustomHPBar";

    public static Sprite bg;
    public static Sprite mg;
    public static Sprite fg;
    public static Sprite ol;
    public static Sprite bossbg;
    public static Sprite bossfg;
    public static Sprite bossol;

    public static Settings ConfigSettings { get; } = new();

    public static readonly string DATA_DIR = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SPRITE_FOLDER);

}