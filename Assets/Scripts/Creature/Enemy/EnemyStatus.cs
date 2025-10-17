using UnityEngine;

public class EnemyStatus : Status
{
    public EnemyType Type { get; set; } = EnemyType.None;

    [Header("HP¹Ù")]
    public RectTransform hpBar;
    public GameObject barPrefab;
    protected GameObject hpBarInstance;
    public GameObject canvas;
    public float barHeight;

    public int expAmount = 0;

    protected override void Awake()
    {
        base.Awake();

        if (barPrefab != null)
        {
            canvas = GameObject.Find("GameHUD");

            hpBarInstance = Instantiate(barPrefab, canvas.transform);
            hpBar = hpBarInstance.GetComponent<RectTransform>();
        }
        CurrentHealth = BaseMaxHealth;
    }

    protected override void Update()
    {
        base.Update();

        if (barPrefab != null)
        {
            HPBar bar = hpBarInstance.GetComponent<HPBar>();
            bar.curHp = CurrentHealth;
            bar.maxHp = BaseMaxHealth;
        }
    }

    private void FixedUpdate()
    {
        if (barPrefab != null)
        {
            Vector3 barPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + barHeight, 0));
            hpBar.position = barPos;
        }
    }

    public override void Respawn()
    {
        base.Respawn();
        transform.root.gameObject.SetActive(true);
        hpBarInstance?.SetActive(true);
    }

    protected override void OnDied()
    {
        base.OnDied();
        if (hpBarInstance != null)
            hpBarInstance.SetActive(false);
        GameManager.instance.CurrentScene.AddExp(expAmount);
    }
}
