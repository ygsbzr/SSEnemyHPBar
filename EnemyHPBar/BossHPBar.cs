namespace SSEnemyHPBar;

public class BossHPBar : MonoBehaviour {
	private GameObject bg_go;
	private GameObject fg_go;
	private GameObject ol_go;
	private CanvasRenderer bg_cr;
	private CanvasRenderer fg_cr;
	private CanvasRenderer ol_cr;

	private readonly float bossbgScale = SSEnemyHPBarPlugin.ConfigSettings.bossbgScale;
	private readonly float bossfgScale = SSEnemyHPBarPlugin.ConfigSettings.bossfgScale;
	private readonly float bossolScale = SSEnemyHPBarPlugin.ConfigSettings.bossolScale;

	public Image health_bar;

	public float maxHP;

	public int position;
	public Vector2 screenScale;
	public HealthManager hm;
	public Vector2 objectPos;

	public void Awake() {
		SSEnemyHPBarPlugin.Logger.LogDebug($@"Creating Boss HP Bar for {name}");

		Camera camera = GameCameras.instance.mainCamera;
		screenScale = new Vector2(camera.pixelWidth / 1280f, camera.pixelHeight / 720f);

		bg_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.bossCanvas, SSEnemyHPBarPlugin.bossbg,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.bossbg.texture.width, SSEnemyHPBarPlugin.bossbg
			.texture.height), screenScale * bossbgScale), new Vector2(0f, 32f),
			new Vector2(0.5f, 0f),
				new Vector2(0.5f, 0f)));
		fg_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.bossCanvas, SSEnemyHPBarPlugin.bossfg,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.bossfg.texture.width, SSEnemyHPBarPlugin.bossfg
					.texture.height), screenScale * bossfgScale), new Vector2(0f, 32f), new Vector2(0.5f, 0f),
				new Vector2(0.5f, 0f)));
		ol_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.bossCanvas, SSEnemyHPBarPlugin.bossol,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.bossol.texture.width, SSEnemyHPBarPlugin.bossol
					.texture.height), screenScale * bossolScale), new Vector2(0f, 32f), new Vector2(0.5f, 0f),
				new Vector2(0.5f, 0f)));

		bg_cr = bg_go.GetComponent<CanvasRenderer>();
		fg_cr = fg_go.GetComponent<CanvasRenderer>();
		ol_cr = ol_go.GetComponent<CanvasRenderer>();

		objectPos = fg_go.transform.position;

		health_bar = fg_go.GetComponent<Image>();
		health_bar.type = Image.Type.Filled;
		health_bar.fillMethod = Image.FillMethod.Horizontal;
		health_bar.preserveAspect = false;

		bg_go.GetComponent<Image>().preserveAspect = false;
		ol_go.GetComponent<Image>().preserveAspect = false;

		hm = gameObject.GetComponent<HealthManager>();

		SetHPBarAlpha(0);

		maxHP = hm.hp;
	}

	private void SetHPBarAlpha(float alpha) {
		if (alpha <= 0f) {
			SSEnemyHPBarPlugin.ActiveBosses.Remove(gameObject);
		} else if (!SSEnemyHPBarPlugin.ActiveBosses.Contains(gameObject)) {
			SSEnemyHPBarPlugin.ActiveBosses.Add(gameObject);
		}

		bg_cr.SetAlpha(alpha);
		fg_cr.SetAlpha(alpha);
		ol_cr.SetAlpha(alpha);
	}

	private void DestroyHPBar() {
		Destroy(fg_go);
		Destroy(bg_go);
		Destroy(ol_go);
		Destroy(health_bar);
	}

	private void MoveHPBar(Vector2 position) {
		fg_go.transform.position = position;
		bg_go.transform.position = position;
		ol_go.transform.position = position;
	}

#pragma warning disable IDE0051

	private void OnDestroy() {
		SetHPBarAlpha(0);
		DestroyHPBar();
		SSEnemyHPBarPlugin.Logger.LogDebug($@"Destroyed enemy {gameObject.name}");
	}

	private void OnDisable() {
		SetHPBarAlpha(0);
		SSEnemyHPBarPlugin.Logger.LogDebug($@"Disabled enemy {gameObject.name}");
	}

	private void FixedUpdate() {
		position = SSEnemyHPBarPlugin.ActiveBosses.IndexOf(gameObject) + 1;

		SSEnemyHPBarPlugin.Logger.LogMessage($@"Enemy {name}: currHP {hm.hp}, maxHP {maxHP}");
		health_bar.fillAmount = hm.hp / maxHP;
		if (health_bar.fillAmount < 1f && health_bar.fillAmount > 0f) {
			float alpha = GameManager.instance.GameState == GameState.PAUSED ? 0.5f : 1;
			SetHPBarAlpha(alpha);
		}

		if (gameObject.name == "New Game Object" && hm.hp <= 0) {
			Destroy(gameObject);
			SSEnemyHPBarPlugin.Logger.LogDebug($@"Placeholder killed");
		}

		if (hm.hp <= 0f && !hm.hasSpecialDeath) {
			SetHPBarAlpha(0);
		}
	}

	private void LateUpdate() {
		position = SSEnemyHPBarPlugin.ActiveBosses.IndexOf(gameObject);
		MoveHPBar(new Vector2(objectPos.x, objectPos.y + (position * 30f)));
	}

#pragma warning restore IDE0051
}
