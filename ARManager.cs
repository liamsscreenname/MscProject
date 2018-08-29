using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using Wrld.Space;

//Adapted from: https://www.youtube.com/watch?v=ehmBIP5sj0M, https://www.youtube.com/watch?v=gNwduUQrlJs, http://wirebeings.com/markerless-gps-ar.html, Tan et. Al (2015)
public class ARManager : MonoBehaviour {
    public static ARManager Instance { get; set; }
    public GameObject textBox;
    private GameObject camParent, groundPlane;
    public Text theText, t;
    public TextAsset[] textFile = new TextAsset[11];
    public Transform[] assets = new Transform[11];
    private string[] textLines;
    private bool textRunning = false;

    private void Awake()
    {
        Debug.Log("ARManager awake");
        Instance = this;
    }

    void Start()
    {
        Debug.Log(SystemInfo.supportsGyroscope);
        Input.gyro.enabled = true;
        //Scale ground plane according to field of view
        groundPlane = GameObject.Find("GroundPlane");
        float scale = (2.0f * Constants.VIEW_RANGE) / .75f;
        groundPlane.transform.localScale = new Vector3(scale, scale, scale);
        camParent = GameObject.Find("CamParent");
        //camParent.transform.Rotate(Vector3.right, 90);
        //Keep children (assets) from scaling up as well
        //foreach (Transform obj in groundPlane.transform)
        //    obj.localScale = new Vector3(obj.localScale.x / scale, obj.localScale.y / scale, obj.localScale.z / scale);
    }

    public void triggerAR(int asset)
    {
        StartCoroutine(displayText(asset));
    }

    //This function is caled once per frame while player is within view range of an asset
    public void displayAsset(GameLocation gl)
    {
        //Determine distance in longitude and latitude between player and object
        double latChangeDegrees = gl.GetPosition().GetLatitude() - GPS.Instance.latitude;
        double lonChangeDegrees = gl.GetPosition().GetLongitude() - GPS.Instance.longitude;
        //If object is in range
        if (latToMeters(latChangeDegrees) <= Constants.VIEW_RANGE / 2 && latToMeters(latChangeDegrees) >= -(Constants.VIEW_RANGE / 2)
            && lonToMeters(lonChangeDegrees) <= Constants.VIEW_RANGE / 2 && lonToMeters(lonChangeDegrees) >= -(Constants.VIEW_RANGE / 2))
        {
            //groundPlane.SetActive(true);
            Debug.Log("Visible");
            assets[gl.assetNum - 1].gameObject.SetActive(true);
            //Set asset position within ground plane
            assets[gl.assetNum - 1].transform.localPosition = new Vector3((float)lonToMeters(lonChangeDegrees) * 5 / Constants.VIEW_RANGE, assets[gl.assetNum - 1].transform.localPosition.y, (float)latToMeters(latChangeDegrees) * 5 / Constants.VIEW_RANGE);
        }
        else
            assets[gl.assetNum - 1].gameObject.SetActive(false);

        //float objAngle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs((float)latChange / (float)lonChange));
        //float azimuth;
        //if (latChange >= 0 && lonChange >= 0)
        //    azimuth = objAngle + 90.0f;
        //else if (latChange >= 0 && lonChange < 0)
        //    azimuth = 90.0f - objAngle;
        //else if (latChange < 0 && lonChange >= 0)
        //    azimuth = 270.0f + objAngle;
        //else
        //    azimuth = 270.0f - objAngle;
        //float angleDiff = Mathf.Abs(azimuth - GPS.Instance.getCompass());
        ////float angleDiff = Mathf.Abs((float)gl.GetPosition().BearingTo(GPS.Instance.latLong) - GPS.Instance.getCompass());
        ////TESTING ONLY
        //bool cameraAimed = angleDiff <= GPS.Instance.getCompass() + Constants.FIELD_OF_VIEW && angleDiff >= GPS.Instance.getCompass() - Constants.FIELD_OF_VIEW;
        //t.text = "Azimuth: " + gl.GetPosition().BearingTo(GPS.Instance.latLong) + "Angle Diff: : " + angleDiff + ", Compass: " + GPS.Instance.getCompass().ToString() + ", In Range: " + cameraAimed;   
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

     private IEnumerator displayText(int assetNumber)
     {
        Debug.Log("Text Displaying");
        GameObject.Find("ARCamera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        if (textFile[assetNumber-1] != null)
            textLines = textFile[assetNumber - 1].text.Split('\n');

        int currentLine = 0;
        textRunning = true;
        textBox.SetActive(true);
        while (currentLine < textLines.Length)
        {
            theText.text = textLines[currentLine];
            yield return new WaitForSeconds(5);
            currentLine++;
        }
        textBox.SetActive(false);
        textRunning = false;
        GameObject.Find("ARCamera").GetComponent<Camera>().rect = new Rect(0.62f, 0.05f, 0.35f, 0.4f);
        yield break;
    }

    private void Update()
    {
        //Keep ground plane fixed at world centre
        if (GPS.Instance.running)
        {
            groundPlane.transform.position = new Vector3(transform.position.x, transform.position.y + Constants.FLOOR_HEIGHT, transform.position.z);
        }
        ////Rotate camera with device
        if (SystemInfo.supportsGyroscope)
        {
            transform.rotation = Input.gyro.attitude;
            transform.Rotate(Vector3.right, 90);
        }
        else
            Application.Quit();
    }
}
