using Unity.Cinemachine;
using UnityEngine;

public class BossScene : SceneBase
{
    [SerializeField] Transform boss;
    [SerializeField] Transform hpBarUI;

    [SerializeField] UnityEngine.UI.Image hpBarImage;
    BossStatus bossStatus;

    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Town;

        if (GameObject.Find("CMCam").TryGetComponent<CinemachineCamera>(out cam) && Player.instance is not null)
        {
            cam.Target.TrackingTarget = Player.instance.transform;
        }
    }

    protected override void Start()
    {
        base.Start();

        InitScene();

        bossStatus = boss.GetComponent<BossStatus>();

    }

    private void Update()
    {
        if(hpBarUI.gameObject.activeSelf is true)
        {
            hpBarImage.fillAmount = CalculateRatio(bossStatus.CurrentHealth, bossStatus.BaseMaxHealth);
        }

    }

    public void OpenHPBarUI()
    {
        if (boss is not null && hpBarUI is not null)
            hpBarUI.gameObject.SetActive(true);
    }

    float CalculateRatio(float cur, float max)
    {
        if (max is not 0)
            return cur / max;
        else return 0;
    }

    public void InitScene()
    {
        Player.instance?.InventoryController.Init();
        UIManager.instance?.InitUI();

        GameManager.instance?.InjectData();
    }
}
