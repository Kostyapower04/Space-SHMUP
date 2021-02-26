﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    static public Shield S;
    [Header("Set in Inspector")]
    public float rotationsPerSecond = 0.1f;
    public float gameRestartDelay = 0f;

    [Header("Set Dynamically")]
    public int levelShown = 0;

    //Скрытые переменные, не появляющиеся в инспекторе
    Material mat;

    void Awake()
    {
        S = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Прочитать текущую мощность защитного поля из объекта-одиночки Hero
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        //Если она отличается от levelShown...
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            //Скорректировать смещение в текстуре, чтобы отобразить поле с другой мощностью
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }
        if (levelShown < 0)
        {
            Destroy(this.gameObject);
            //Сообщить объекту Main.S о необходимости перезапустить игру
            Main.S.DelayedRestart(gameRestartDelay);
        }
        //Поворачивать поле в каждом кадре с постоянной скоростью
        float rZ = -(rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
