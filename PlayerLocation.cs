using UnityEngine;
using Wrld.Space;

public class PlayerLocation : MonoBehaviour {

    private GeographicTransform player;
    LatLong position = new LatLong(0, 0);

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<GeographicTransform>();   
    }

    // Update is called once per frame
    void Update()
    {
        position.SetLatitude(GPS.Instance.latitude);
        position.SetLongitude(GPS.Instance.longitude);
        player.SetPosition(position);
    }
}

	