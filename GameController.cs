using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Wrld.Space;

//Controls Game events/rules
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; set; }
    private List<GameLocation> active = new List<GameLocation>();
    public List<Landmark> originalLandmarks;
    public List<Landmark> translated;
    public List<GameLocation> assets = new List<GameLocation>();

    public Text t;

    private void Awake()
    {
        Debug.Log("G.C. Awake Start");
        Instance = this;
        new OriginalLocations();
        originalLandmarks = OriginalLocations.Instance.getOriginals();
        new GoogleAPI();
        Debug.Log("G.C. Awake End");
    }

    private void Start()
    {
        GameObject arCam = GameObject.Find("ARCamera");
        foreach (Transform trans in arCam.GetComponentInChildren<Transform>(true))
            trans.gameObject.layer = 9;
        assets.Sort((x, y) => x.assetNum.CompareTo(y.assetNum));
        StartCoroutine(StartUp());
    }

    private IEnumerator StartUp()
    {
        Debug.Log("G.C. Start Start");
        //Wait for GPS
        while (Translation.Instance == null)
            yield return new WaitForSeconds(1);
        //Generate list of translated landmarks
        translated = Translation.Instance.translateLandmark();

        foreach (Landmark l in translated)
            Debug.Log(l.getName() + ": " + l.getPosition().GetLatitude().ToString() + ", " + l.getPosition().GetLongitude().ToString());

        for (int i = 0; i < assets.Count; i++)
        {
            for (int j = 0; j < translated.Count; j++)
            {
                if (assets[i].getClosest().getType() == translated[j].getType())
                {
                    //Change closest Landmark from original to translated
                    assets[i].setClosest(translated[j]);
                }
            }

            //Set Game Location
            setGameLocations(assets[i]);
            assets[i].setSpace();

            //Check for overlapping spaces and re-set if overlapping
            for (int l = 0; l < i; l++)
            {
                while (assets[l].space.inSpace(assets[i].GetPosition()))
                {
                    setGameLocations(assets[i]);
                    assets[i].setSpace();
                }
            }
        }
        yield break;
    }

    //Set Game Location to random distance (within range) away from closest Landmark
    private void setGameLocations(GameLocation gl)
    {
        //Generate two random offsets for LAtitude and Longitude
        double latOffset = Random.Range(Constants.MIN_DISPLACEMENT, Constants.MAX_DISPLACEMENT);
        double lonOffset = Random.Range(Constants.MIN_DISPLACEMENT, Constants.MAX_DISPLACEMENT);

        double lat = gl.getClosest().getPosition().GetLatitude();
        double lon = gl.getClosest().getPosition().GetLongitude();

        gl.SetPosition(new LatLong(Translation.Instance.offsetLat(lat, latOffset), Translation.Instance.offsetLon(lon, lat, lonOffset)));
    }

    public void setActive(GameLocation l)
    {
        l.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        active.Add(l);
    }


    public List<GameLocation> getActive()
    {
        return active;
    }

    public void removeActive(GameLocation l)
    {
        active.Remove(l);
    }

    public bool listContains(GameLocation l)
    {
        return active.Contains(l);
    }


    public void addAsset(GameLocation l)
    {
        assets.Add(l);
    }

    void Update()
    {
        if (active.Count > 0)
        {
            t.text = active[0].name + ' ' + LatLong.EstimateGreatCircleDistance(active[0].GetPosition(), GPS.Instance.latLong);
        }
        else
            t.text = "empty list.";
    }
}
