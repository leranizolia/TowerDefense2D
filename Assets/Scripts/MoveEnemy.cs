using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    //[HideIninspector] ��� waypoints �����������, ��� �� �� ������ �������� �������� ��� ���� � Inspector,
    //�� ��-�������� ����� ����� ������ � ���� �� ������ ��������
    [HideInInspector]
    //waypoints � ������� �������� ����� ����� ��������
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
        //�� ������� ����� �������� �� �������� ��������� � �������� ������� �������� �������� ��������.
        Vector3 startPosition = waypoints[currentWaypoint].transform.position;
        Vector3 endPosition = waypoints[currentWaypoint + 1].transform.position;
        //��������� �����, ����������� ��� ����������� ����� ���������� � ������� ������� ����� = ���������� / ��������,
        //� ����� ���������� ������� ����� �� ��������.
        //� ������� Vector2.Lerp, �� ������������� ������� ������� ����� ����� ��������� � �������� ������ ��������.
        float pathLength = Vector3.Distance(startPosition, endPosition);
        float totalTimeForPath = pathLength / speed;
        float currentTimeOnPath = Time.time - lastWaypointSwitchTime;
        gameObject.transform.position = Vector2.Lerp(startPosition, endPosition, currentTimeOnPath / totalTimeForPath);

        //���������, ������ �� ���� endPosition.
        //���� ��, �� ������������ ��� ��������� ��������:
        if (gameObject.transform.position.Equals(endPosition))
        {
            //���� ���� �� ����� �� ��������� ����� ��������,
            //������� ����������� �������� currentWaypoint � ��������� lastWaypointSwitchTime.
            if (currentWaypoint < waypoints.Length - 2)
            {
                currentWaypoint++;
                lastWaypointSwitchTime = Time.time;
                RotateIntoMoveDirection();
            }
            //���� ������ ��������� ����� ��������, ����� �� ���������� ��� � ��������� �������� ������.
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
        //��������� ������� ����������� �������� ����,
        //������� ������� ������� ����� �������� �� ������� ��������� �����.
        Vector3 newStartPosition = waypoints[currentWaypoint].transform.position;
        Vector3 newEndPosition = waypoints[currentWaypoint + 1].transform.position;
        Vector3 newDirection = (newEndPosition - newStartPosition);
        //���������� Mathf.Atan2 ��� ����������� ���� � ��������,
        //� ������� ��������� newDirection (������� ����� ��������� ������).
        //�������� ��������� �� 180 / Mathf.PI, ���������� ���� � �������.
        float x = newDirection.x;
        float y = newDirection.y;
        float rotationAngle = Mathf.Atan2(y, x) * 180 / Mathf.PI;
        //�������� �������� ������ Sprite � ������������ �� rotationAngle �������� �� ���.
        //������������ ��������, � �� ������������ ������, ����� ������� �������, ������� �� ������� �����,
        //���������� ��������������
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
