using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Wrld;
using Wrld.Space;

//Apadted from https://www.youtube.com/watch?v=g04jaC-Tpn0
public class GPS : MonoBehaviour {

    public static GPS Instance { get; set; }

    public double longitude, latitude;
    public LatLong latLong;

    private LatLong testLocation = new LatLong(51.547226, 0.707782);
    private LatLong testLocation2 = new LatLong(51.478484, 0.327042);
    private LatLong testLocation3 = new LatLong(51.536683, 0.713102);
    private LatLong testLocation4 = new LatLong(6.535225, 3.351227);
    public Text statusText;
    public bool running = false;

	// Use this for initialization
	void Awake () {
        Debug.Log("GPS Awake Start");
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
        statusText.text = "Starting...";
	}

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS Not Enabled");
            statusText.text = "Not Enabled";
            //CHANGE TEST LOCATION HERE
            latLong = testLocation3;
            latitude = latLong.GetLatitude();
            longitude = latLong.GetLongitude();
            Api.Instance.CameraApi.MoveTo(latLong, distanceFromInterest: 300, headingDegrees: 0, tiltDegrees: 50);
            new Translation(latLong);
            Debug.Log("GPS Awake End");
            yield break;
        }

        Input.location.Start();
        int gpsWaitTime = 0;
        while(Input.location.status == LocationServiceStatus.Initializing && gpsWaitTime <= 30)
        {
            yield return new WaitForSeconds(1);
            gpsWaitTime++;
        }

        if(gpsWaitTime >= 30)
        {
            Debug.Log("GPS Initialization timed out");
            statusText.text = "Timed out.";
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS Initialization Failed");
            statusText.text = "Failed";
            yield break;
        }

        Input.compass.enabled = true;
        while (!Input.compass.enabled)
            yield return new WaitForSeconds(1);
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        latLong = new LatLong(latitude, longitude);
        running = true;
        Debug.Log("Starting: " + latLong.GetLatitude());
        //Camera snap to location
        Api.Instance.CameraApi.MoveTo(latLong, distanceFromInterest: 300, headingDegrees: 0, tiltDegrees: 50);
        new Translation(latLong);
        Debug.Log("GPS Awake End");
        yield break;
    }

    public float getCompass()
    {
        return Input.compass.trueHeading;
    }

    // Update is called once per frame
    void Update () {
		if(Input.location.status == LocationServiceStatus.Running)
        {
            statusText.text = "Running";
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            latLong.SetLatitude(latitude);
            latLong.SetLongitude(longitude);
        }
        else
            statusText.text = "Starting...";
    }
}
