using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_4 : Enemy
{
    private Vector3 p0, p1; //Две точки для интерполяции
    private float timeStart; //Время создания этого корабля
    private float duration = 4; //Продолжительность перемещения

    // Start is called before the first frame update
    void Start()
    {
        //Начальная позиция уже выбрана в Main.SpawnEnemy(),
        //Поэтому запишем её как начальные значения в p0 и p1
        p0 = p1 = pos;
        InitMovement();
    }

    void InitMovement()
    {
        p0 = p1; //Перезаписать p1 в p0
        //Выбрать новую точку p1 на экране
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        //Cбросить время
        timeStart = Time.time;
    }

    public override void Move()
    {
        //Этот метод переопределяет Enemy.Move() и реализует линейную интерполяцию
        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement(); 
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); //Применить плавное замедление
        pos = (1 - u) * p0 + u * p1; //Простая линейная интерполяция
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
