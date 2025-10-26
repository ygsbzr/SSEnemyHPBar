// ReSharper disable InconsistentNaming

namespace SSEnemyHPBar;

public class HPBar : MonoBehaviour {
	private GameObject bg_go;
	private GameObject mg_go;
	private GameObject fg_go;
	private GameObject ol_go;
	private CanvasRenderer bg_cr;
	private CanvasRenderer fg_cr;
	private CanvasRenderer mg_cr;
	private CanvasRenderer ol_cr;

	private readonly float bgScale = SSEnemyHPBarPlugin.ConfigSettings.bgScale;
	private readonly float fgScale = SSEnemyHPBarPlugin.ConfigSettings.fgScale;
	private readonly float olScale = SSEnemyHPBarPlugin.ConfigSettings.olScale;
	private readonly float mgScale = SSEnemyHPBarPlugin.ConfigSettings.mgScale;

	public Image health_bar;
	public Image hpbg;

	public float currHP;
	public float maxHP;
	public int oldHP;

	public HealthManager hm;

	public Vector2 objectPos;
	public Vector2 screenScale;

	public void Awake() {
		SSEnemyHPBarPlugin.Logger.LogDebug($@"Creating HP Bar for {name}");

		// On.CameraController.FadeOut += CameraController_FadeOut;

		Camera camera = GameCameras.instance.mainCamera;
		screenScale = new Vector2(camera.pixelWidth / 1280f * 0.025f, camera.pixelHeight / 720f * 0.025f);

		bg_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.canvas, SSEnemyHPBarPlugin.bg,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.bg.texture.width, SSEnemyHPBarPlugin.bg.texture
			.height), screenScale * bgScale), new Vector2(0,
			32)));
		mg_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.canvas, SSEnemyHPBarPlugin.mg,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.mg.texture.width, SSEnemyHPBarPlugin.mg.texture
				.height), screenScale * mgScale), new Vector2(0, 32)));
		fg_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.canvas, SSEnemyHPBarPlugin.fg,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.fg.texture.width, SSEnemyHPBarPlugin.fg.texture
				.height), screenScale * fgScale), new Vector2(0, 32)));
		ol_go = CanvasUtil.CreateImagePanel(SSEnemyHPBarPlugin.canvas, SSEnemyHPBarPlugin.ol,
			new CanvasUtil.RectData(Vector2.Scale(new Vector2(SSEnemyHPBarPlugin.ol.texture.width, SSEnemyHPBarPlugin.ol.texture
				.height), screenScale * olScale), new Vector2(0, 32)));

		bg_cr = bg_go.GetComponent<CanvasRenderer>();
		fg_cr = fg_go.GetComponent<CanvasRenderer>();
		mg_cr = mg_go.GetComponent<CanvasRenderer>();
		ol_cr = ol_go.GetComponent<CanvasRenderer>();
		hpbg = mg_go.GetComponent<Image>();
		hpbg.type = Image.Type.Filled;
		hpbg.fillMethod = Image.FillMethod.Horizontal;
		hpbg.preserveAspect = false;

		health_bar = fg_go.GetComponent<Image>();
		health_bar.type = Image.Type.Filled;
		health_bar.fillMethod = Image.FillMethod.Horizontal;
		health_bar.preserveAspect = false;
		bg_go.GetComponent<Image>().preserveAspect = false;
		ol_go.GetComponent<Image>().preserveAspect = false;

		hm = gameObject.GetComponent<HealthManager>();

		SetHPBarAlpha(0);
		maxHP = hm.hp;
		currHP = hm.hp;
		
	}

	private void SetHPBarAlpha(float alpha) {
		bg_cr.SetAlpha(alpha);
		fg_cr.SetAlpha(alpha);
		mg_cr.SetAlpha(alpha);
		ol_cr.SetAlpha(alpha);
	}

	private void DestroyHPBar() {
		Destroy(fg_go);
		Destroy(bg_go);
		Destroy(mg_go);
		Destroy(ol_go);
		Destroy(hpbg);
		Destroy(health_bar);
	}

	private void MoveHPBar(Vector2 position) {
		fg_go.transform.position = position;
		mg_go.transform.position = position;
		bg_go.transform.position = position;
		ol_go.transform.position = position;
	}

#pragma warning disable IDE0051

	private void OnDestroy() {
		SetHPBarAlpha(0);
		DestroyHPBar();
		SSEnemyHPBarPlugin.Logger.LogDebug($@"Destroyed enemy {name}");
	}

	private void OnDisable() {
		SetHPBarAlpha(0);
		SSEnemyHPBarPlugin.Logger.LogDebug($@"Disabled enemy {name}");
	}

	private void FixedUpdate() {
		if (currHP > hm.hp) {
			currHP -= 1f;
		} else {
			currHP = hm.hp;
		}

		SSEnemyHPBarPlugin.Logger.LogMessage($@"Enemy {name}: currHP {hm.hp}, maxHP {maxHP}");
		health_bar.fillAmount = hm.hp / maxHP;

		hpbg.fillAmount = currHP / maxHP;

		if (health_bar.fillAmount < 1f) {
			SetHPBarAlpha(1);
		}

		if (gameObject.name == "New Game Object" && currHP <= 0) {
			SSEnemyHPBarPlugin.Logger.LogDebug($@"Placeholder killed");
			Destroy(gameObject);
		}

		if (currHP <= 0f) {
			SetHPBarAlpha(0);
		}
		oldHP = hm.hp;
	}

	private void LateUpdate() {
		objectPos = transform.position + (Vector3.up * 1.5f);
		MoveHPBar(objectPos);
	}

#pragma warning restore IDE0051
}
