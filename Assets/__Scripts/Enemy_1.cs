using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_1 : Enemy //Enemy_1 расширяет класс Enemy
{
    //Число секунд полного цикла синусоиды
    public float waveFrequency = 2;
    //Ширина синусоиды в метрах
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; //Начальное значение координаты x
    private float birthTime;

    //Метод start хорошо подходит для наших целей, так как не используется суперклассом Enemy
    void Start()
    {
        //Установить начальную координату x объекта Enemy_1
        x0 = pos.x;
        birthTime = Time.time;
    }

    //Переопределить функцию Move суперкласса Enemy
    public override void Move()
    {
        //Так как pos - это свойство, то нельзя напрямую изменить pos.x
        //Поэтому получим pos в виде вектора Vector3, доступного для изменения
        Vector3 tempPos = pos;
        //Значение theta изменяется с течением времени
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //Повернуть ннемного относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //base.Move() обрабатывает движение вниз, вдоль оси Y
        base.Move(); 
    }
}