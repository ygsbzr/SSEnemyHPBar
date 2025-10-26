namespace SSEnemyHPBar;

using System.Text.RegularExpressions;
internal sealed class ResourceLoader : MonoBehaviour {
	private static byte[] GetImage(string name)
    {
		SSEnemyHPBarPlugin.CompleteImage(Path.Combine(SSEnemyHPBarPlugin.DATA_DIR));
		return File.ReadAllBytes(Path.Combine(SSEnemyHPBarPlugin.DATA_DIR, name));

	}
	public static byte[] GetBackgroundImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_BG);

	public static byte[] GetForegroundImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_FG);

	public static byte[] GetMiddlegroundImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_MG);

	public static byte[] GetOutlineImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_OL);

	public static byte[] GetBossBackgroundImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_BOSSBG);

	public static byte[] GetBossForegroundImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_BOSSFG);

	public static byte[] GetBossOutlineImage() => GetImage(SSEnemyHPBarPlugin.HPBAR_BOSSOL);
}
