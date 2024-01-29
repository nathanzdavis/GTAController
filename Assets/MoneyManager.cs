using scgGTAController;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private int totalMoney = 0;
    private HudController hc;
    public float changedMoneyHideTime;

    private void Start()
    {
        hc = HudController.instance;

        hc.changedMoney.text = "";
        hc.totalMoney.text = "0";
    }

    public void ChangeMoney(int amount)
    {

        totalMoney += amount;

        if (amount > 0)
        {
            hc.changedMoney.color = Color.white;
            hc.changedMoney.text = "+" + amount;

            CancelInvoke("ChangedMoneyHide");
            Invoke("ChangedMoneyHide", changedMoneyHideTime);
        }
        else
        {
            hc.changedMoney.color = Color.red;
            hc.changedMoney.text = "-" + amount;
        }

        hc.totalMoney.text = "$" + totalMoney.ToString();
       
    }

    private void ChangedMoneyHide()
    {
        hc.changedMoney.text = "";
    }
}