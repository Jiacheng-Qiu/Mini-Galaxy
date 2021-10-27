using System;
using UnityEngine;
using UnityEngine.UI;

public class CraftAmount : MonoBehaviour
{
    private int curAmount;
    public void AddOne()
    {
        if (curAmount >= 999)
            return;
        curAmount++;
        gameObject.GetComponent<InputField>().text = curAmount.ToString();
    }

    public void DecreaseOne()
    {
        if (curAmount <= 1)
            return;
        curAmount--;
        gameObject.GetComponent<InputField>().text = curAmount.ToString();
    }

    public void UpdateAmount()
    {
        try
        {
            curAmount = int.Parse(gameObject.GetComponent<InputField>().text);
        }
        catch (Exception e)
        {
            gameObject.GetComponent<InputField>().text = curAmount.ToString();
        }
    }

    public void ResetAmount()
    {
        curAmount = 1;
        gameObject.GetComponent<InputField>().text = curAmount.ToString();
    }

    public int GetCurAmount()
    {
        return curAmount;
    }
}
