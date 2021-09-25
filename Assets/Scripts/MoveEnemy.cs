using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    //[HideIninspector] над waypoints гарантирует, что мы не сможем случайно изменить это поле в Inspector,
    //но по-прежнему будем иметь доступ к нему из других скриптов
    [HideInInspector]
    //waypoints в массиве хранится копия точек маршрута
    public GameObject[] waypoints;
    private int currentWaypoint = 0;
    private float lastWaypointSwitchTime;
    public float speed = 1.0f;

    private void Start()
    {
        lastWaypointSwitchTime = Time.time;
    }

    private void Update()
    {
        //Из массива точек маршрута мы получаем начальную и конечную позиции текущего сегмента маршрута.
        Vector3 startPosition = waypoints[currentWaypoint].transform.position;
        Vector3 endPosition = waypoints[currentWaypoint + 1].transform.position;
        //Вычисляем время, необходимое для прохождения всего расстояния с помощью формулы время = расстояние / скорость,
        //а затем определяем текущее время на маршруте.
        //С помощью Vector2.Lerp, мы интерполируем текущую позицию врага между начальной и конечной точной сегмента.
        float pathLength = Vector3.Distance(startPosition, endPosition);
        float totalTimeForPath = pathLength / speed;
        float currentTimeOnPath = Time.time - lastWaypointSwitchTime;
        gameObject.transform.position = Vector2.Lerp(startPosition, endPosition, currentTimeOnPath / totalTimeForPath);

        //Проверяем, достиг ли враг endPosition.
        //Если да, то обрабатываем два возможных сценария:
        if (gameObject.transform.position.Equals(endPosition))
        {
            //Враг пока не дошёл до последней точки маршрута,
            //поэтому увеличиваем значение currentWaypoint и обновляем lastWaypointSwitchTime.
            if (currentWaypoint < waypoints.Length - 2)
            {
                currentWaypoint++;
                lastWaypointSwitchTime = Time.time;
                RotateIntoMoveDirection();
            }
            //Враг достиг последней точки маршрута, тогда мы уничтожаем его и запускаем звуковой эффект.
            else
            {
                Destroy(gameObject);

                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
                GameManagerBehavior gameManager =
    GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
                gameManager.Health -= 1;
            }
        }
    }

    private void RotateIntoMoveDirection()
    {
        //Вычисляет текущее направление движения жука,
        //вычитая позицию текущей точки маршрута из позиции следующей точки.
        Vector3 newStartPosition = waypoints[currentWaypoint].transform.position;
        Vector3 newEndPosition = waypoints[currentWaypoint + 1].transform.position;
        Vector3 newDirection = (newEndPosition - newStartPosition);
        //Использует Mathf.Atan2 для определения угла в радианах,
        //в котором направлен newDirection (нулевая точка находится справа).
        //Умножает результат на 180 / Mathf.PI, преобразуя угол в градусы.
        float x = newDirection.x;
        float y = newDirection.y;
        float rotationAngle = Mathf.Atan2(y, x) * 180 / Mathf.PI;
        //получает дочерний объект Sprite и поворачивает на rotationAngle градусов по оси.
        //поворачиваем дочерний, а не родительский объект, чтобы полоска энергии, которую мы добавим позже,
        //оставались горизонтальной
        GameObject sprite = gameObject.transform.Find("Sprite").gameObject;
        sprite.transform.rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
    }

    public float DistanceToGoal()
    {
        float distance = 0;
        distance += Vector2.Distance(
            gameObject.transform.position,
            waypoints[currentWaypoint + 1].transform.position);
        for (int i = currentWaypoint + 1; i < waypoints.Length - 1; i++)
        {
            Vector3 startPosition = waypoints[i].transform.position;
            Vector3 endPosition = waypoints[i + 1].transform.position;
            distance += Vector2.Distance(startPosition, endPosition);
        }
        return distance;
    }

}
