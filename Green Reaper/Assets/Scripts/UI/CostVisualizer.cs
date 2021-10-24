using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CostVisualizer : MonoBehaviour
{
    private Button button;
    private Text text;

    [SerializeField]
    private UpgradeHolder.UpgradeType type;
    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(BuyUpgrade);

        if (text == null)
            text = GetComponentInChildren<Text>();
    }

    public void CheckForNewCost()
    {
        if (GameManager.instance.upgrades.IsLastLevel(type))
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (text == null)
                text = GetComponentInChildren<Text>();
            text.text = UpgradeManager.instance.NextCost(type).ToString();
        }
    }

    private void BuyUpgrade()
    {
        if (!GameManager.instance.upgrades.IsLastLevel(type))
            UpgradeManager.instance.TryPurchaseUpgrade(type);
    }
    
}
