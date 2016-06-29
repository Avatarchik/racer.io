using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardController : MonoBehaviour
{
    static LeaderboardController _instance;

    public static LeaderboardController Instance { get { return _instance; } }

    public Color PlayerContainerTextColor;
    public LeaderboardContainer PlayerContainer;
    public List<LeaderboardContainer> LeaderboardContainerList;

    List<CarScript> _leaderboardList;

    void Awake()
    {
        _instance = this;

        _leaderboardList = new List<CarScript>();

        PlayerContainer.gameObject.SetActive(false);
    }

    public void FixedUpdateFrame()
    {
        List<CarScript> activeCarList = CarManagerBase.BaseInstance.ActiveCarDict.Values.ToList();
        
        _leaderboardList = activeCarList.OrderByDescending(p => p.Score).ToList();
        CarScript player = _leaderboardList.Find(p => p.IsPlayerCar);

        UpdateLeaderboard();

        if (player != null)
        {
            if (_leaderboardList.IndexOf(player) < 10)
                PlayerContainer.gameObject.SetActive(false);
            else
                PlayerContainer.gameObject.SetActive(true);

            PlayerContainer.Set(player, _leaderboardList.IndexOf(player) + 1);
        }
    }

    void UpdateLeaderboard()
    {
        for (int i = 0; i < LeaderboardContainerList.Count; i++)
        {
            CarScript targetCar = null;

            if (i < _leaderboardList.Count)
                targetCar = _leaderboardList[i];

            LeaderboardContainerList[i].Set(targetCar);
        }
    }

    public int GetLeaderboardRank(CarScript car)
    {
        if (!_leaderboardList.Contains(car))
            return -1;

        return _leaderboardList.IndexOf(car) + 1;
    }
}
