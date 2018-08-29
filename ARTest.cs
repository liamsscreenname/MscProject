using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Wrld.Space;

//Adapted from: https://www.youtube.com/watch?v=ehmBIP5sj0M
public class ARTest : MonoBehaviour {

    public static ARTest Instance { get; set; }
    public GameObject textBox;
    private GameObject camParent, groundPlane;
    public Text theText, t;
    public TextAsset[] textFile = new TextAsset[11];
    public Transform[] assets = new Transform[11];
    private string[] textLines;
    private bool textRunning = false;
    private float floorHeight;
    private LatLong loc1, loc2;

    void Start()
    {
        Instance = this;
        Debug.Log(SystemInfo.supportsGyroscope);
        Input.gyro.enabled = true;
        //Scale ground plane according to field of view
        groundPlane = GameObject.Find("GroundPlane");
        floorHeight = groundPlane.transform.position.y;
        float scale = (2.0f * Constants.VIEW_RANGE) / .0075f;
        //groundPlane.transform.localScale = new Vector3(scale, scale, scale);
        camParent = GameObject.Find("CamParent");
        //camParent.transform.Rotate(Vector3.right, 90);
        //Keep children (assets) from scaling up as well
        //foreach (Transform obj in groundPlane.transform)
        //    obj.localScale = new Vector3(obj.localScale.x / scale, obj.localScale.y / scale, obj.localScale.z / scale);
        loc1 = new LatLong(51.644247, 0.618766);
        loc2 = new LatLong(51.641737, 0.603801);
    }

    public void displayAsset(LatLong gl, int i)
    {
        //Determine distance in longitude and latitude between player and object
        double latChangeDegrees = gl.GetLatitude() - GPS.Instance.latitude;
        double lonChangeDegrees = gl.GetLongitude() - GPS.Instance.longitude;
        Debug.Log("Distance: " + latToMeters(latChangeDegrees) + ", " + lonToMeters(lonChangeDegrees));
        //If object is in range
        if (latToMeters(latChangeDegrees) <= Constants.VIEW_RANGE / 2 && latToMeters(latChangeDegrees) >= -(Constants.VIEW_RANGE / 2)
            && lonToMeters(lonChangeDegrees) <= Constants.VIEW_RANGE / 2 && lonToMeters(lonChangeDegrees) >= -(Constants.VIEW_RANGE / 2))
        {
            //groundPlane.SetActive(true);
            Debug.Log("Visible");
            assets[i - 1].gameObject.SetActive(true);
            //Set asset position within ground plane
            assets[i - 1].transform.localPosition = new Vector3((float)lonToMeters(lonChangeDegrees) / 20, assets[i - 1].transform.localPosition.y, (float)latToMeters(latChangeDegrees) / 20);
        }
        else
            assets[i - 1].gameObject.SetActive(false);
    }

    private double latToMeters(double lat)
    {
        double metersPerDegree = 111132.92 - 559.82 * Mathf.Cos(2.0f * (float)lat) + 1.175 * Mathf.Cos(4.0f * (float)lat) - 0.0023 * Mathf.Cos(6.0f * (float)lat);
        return lat * metersPerDegree;
    }

    private double lonToMeters(double lon)
    {
        double metersPerDegree = 111412.84 * Mathf.Cos((float)lon) - 93.5 * Mathf.Cos(3.0f * (float)lon) + 0.118 * Mathf.Cos(5.0f * (float)lon);
        return lon * metersPerDegree;
    }

    private IEnumerator WaitForGPS()
    {
        while (!GPS.Instance.running)
            yield return new WaitForSeconds(1);
    }

    private void Update()
    {      
        //Keep ground plane fixed at world centre
        StartCoroutine(WaitForGPS());
        if (GPS.Instance.running)
        {
            groundPlane.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + floorHeight, this.transform.position.z);
            Debug.Log(groundPlane.transform.position.ToString());
            //displayAsset(loc1, 1);
            //displayAsset(GPS.Instance.latLong, 2);
        }
        ////Rotate camera with device
        Debug.Log(SystemInfo.supportsGyroscope);
        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log(Input.gyro.attitude.ToString());
            this.transform.rotation = Input.gyro.attitude;
            this.transform.Rotate(Vector3.right, 90);            
        }
        else
            Application.Quit();
    }
}
