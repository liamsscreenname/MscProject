using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrld.Space;

public class Tester : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        List<LatLong> list = new List<LatLong>();
        list.Add(new LatLong(Translation.Instance.getStartLat(), Translation.Instance.getStartLon()));
        foreach (GameLocation l in GameController.Instance.assets)
            list.Add(l.getClosest().getPosition());
        double distance = LatLong.EstimateGreatCircleDistance(list[0], list[1]);
        Debug.Log("Start - " + GameController.Instance.assets[0].getClosest().getName() + ": " + LatLong.EstimateGreatCircleDistance(list[0], list[1]));
        for (int i = 1; i < list.Count - 1; i++)
        {
            distance += LatLong.EstimateGreatCircleDistance(list[i], list[i + 1]);
            Debug.Log(GameController.Instance.assets[i-1].getClosest().getName() + " - " + GameController.Instance.assets[i].getClosest().getName() + ": " + LatLong.EstimateGreatCircleDistance(list[i], list[i + 1]));
        }
        Debug.Log("Total Distance: " + distance);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
