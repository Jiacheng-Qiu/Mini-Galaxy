using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderCaller : MonoBehaviour
{
    private Slider slider;
    public Map map;
    private void Start()
    {
        slider = gameObject.GetComponent<Slider>();
    }
    public void ModifyAmount()
    {
        map.ChangeSize(slider.value);
    }
}
