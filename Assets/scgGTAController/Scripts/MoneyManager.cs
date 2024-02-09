using scgGTAController;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public float changedMoneyHideTime;
    public int totalMoney = 0;

    public static MoneyManager instance;
    private HudController hc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        hc = HudController.instance;
        hc.changedMoney.text = "";
        hc.totalMoney.text = "$" + totalMoney;
    }

    public void ChangeMoney(int amount)
    {

        totalMoney += amount;

        if (amount > 0)
        {
            hc.changedMoney.color = Color.white;
            hc.changedMoney.text = "+$" + amount;

            CancelInvoke("ChangedMoneyHide");
            Invoke("ChangedMoneyHide", changedMoneyHideTime);
        }
        else
        {
            hc.changedMoney.color = new Color(.9f, .31f, .32f);
            hc.changedMoney.text = "-$" + Mathf.Abs(amount);

            CancelInvoke("ChangedMoneyHide");
            Invoke("ChangedMoneyHide", changedMoneyHideTime);
        }

        hc.totalMoney.text = "$" + totalMoney.ToString(); 
    }

    private void ChangedMoneyHide()
    {
        hc.changedMoney.text = "";
    }
}