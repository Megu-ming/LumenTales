using Unity.Cinemachine;
using UnityEngine;

public class BossScene : SceneBase
{
    [SerializeField] Transform boss;
    [SerializeField] Transform hpBarUI;
    [SerializeField] UIRoot uiRoot;

    [SerializeField] UnityEngine.UI.Image hpBarImage;
    BossStatus bossStatus;

    protected void Start()
    {
        var bc = boss.GetComponent<BossController>();
        bossStatus = boss.GetComponent<BossStatus>();

        player.Init();
        bc.Init(player);
        uiRoot.Init(player);

        GameManager.instance?.SceneStart();
    }

    private void Update()
    {
        //if(hpBarUI.gameObject.activeSelf is true)
        {
            hpBarImage.fillAmount = CalculateRatio(bossStatus.CurrentHealth, bossStatus.BaseMaxHealth);
        }
    }

    public void OpenHPBarUI()
    {
        if (boss != null && hpBarUI is not null)
            hpBarUI.gameObject.SetActive(true);
    }

    float CalculateRatio(float cur, float max)
    {
        if (max is not 0)
            return cur / max;
        else return 0;
    }

    public override void Init()
    {
        base.Init();
        sceneType = SceneType.Boss;

        SpawnAndTrackingPlayer();

        GameManager.instance?.LoadData();
    }
}
