using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//��� �� ������ MonsterLevel.
//� �� ������������ ���� (� ������, ������� �� ����� ������������ ����)
//� ���������� ������������� ������ �������.

//�� ��������� ������ [System.Serializable], ����� ���������� ������ ����� ���� �������� � ����������.
//��� ��������� ��� ������ ������ ��� �������� ������ Level, ���� ����� ���� ��������.
//��� ���������� ������� ��� ������������ ����.
[System.Serializable]
public class MonsterLevel
{
    public int cost;
    public GameObject visualization;

    // ������� ������ ������� � ������� �������� ��� ������� ������ ��������
    public GameObject bullet;
    public float fireRate;
}

public class MonsterData : MonoBehaviour
{
    public List<MonsterLevel> levels;
    private MonsterLevel currentLevel;

    //����� �������� ������� ���������� currentLevel.
    //����� ��������, �� ������ �������� � ��� ����� ������ ����������: ��� ��� CurrentLevel (������ ������)
    //��� ��� monster.CurrentLevel (�� ��� ���������).
    //�� ����� ���������� � ������ ������� ��� ������� �������� ����� ���������,
    //� �������� ������ ������, ������ ��� �� �����, ����� ��������� ���������������� ��������: ������ ��� ������, ������ ��� ������ � ��� ������/������.
    public MonsterLevel CurrentLevel
    {
        //� ������� �� ���������� �������� currentLevel
        get
        {
            return currentLevel;
        }
        //� ������� �� ����������� currentLevel ����� ��������.
        //����� �� �������� ������ �������� ������.
        //�������, �� �������� � ����� �� ���� ������� � ��������/��������� ���������� ����������� � ����������� �� currentLevelIndex.
        //��� �������, ������ ��� ��� ��������� currentLevel ������ ����������� �������������.
        set
        {
            currentLevel = value;
            int currentLevelIndex = levels.IndexOf(currentLevel);

            GameObject levelVisualization = levels[currentLevelIndex].visualization;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levelVisualization != null)
                {
                    if (i == currentLevelIndex)
                    {
                        levels[i].visualization.SetActive(true);
                    }
                    else
                    {
                        levels[i].visualization.SetActive(false);
                    }
                }
            }
        }
    }

    //����� �� ��� ���������� ����� CurrentLevel.
    //��� �����������, ��� ����� ������� ������ ������ ������.
    private void OnEnable()
    {
        CurrentLevel = levels[0];
    }

    //����������: ����� ���������������� �������� � OnEnable, � �� � OnStart, ������ ��� �� �������� ���������� ������ ��� �������� ����������� ��������.
    //OnEnable ����� ������ ����� �� ��� �������� �������(���� ������ ��� ������� � ��������� enabled), �� OnStart �� ����������, ���� ������ �� �������� ����������� ��� ����� �����.
    //��� ���������� ��������� ��� ������ �� ���������� �������, ������� �� �������������� �� � OnEnable


    //�����, ����� ������, �������� �� ������� �������
    public MonsterLevel GetNextLevel()
    {
        int currentLevelIndex = levels.IndexOf(currentLevel);
        int maxLevelIndex = levels.Count - 1;
        if (currentLevelIndex < maxLevelIndex)
        {
            return levels[currentLevelIndex];
        }
        else
        {
            return null;
        }
    }

    public void IncreaseLevel()
    {
        int currentLevelIndex = levels.IndexOf(CurrentLevel);
        if (currentLevelIndex < levels.Count - 1)
        {
            CurrentLevel = levels[currentLevelIndex + 1];
        }
    }
}
