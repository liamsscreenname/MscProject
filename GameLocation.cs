using UnityEngine;
using Wrld.Space;

public class GameLocation : GeographicTransform {

    private Landmark closest;
    public int assetNum;
    public GameLocation[] dependencies = new GameLocation[2];
    private bool isCollected;
    public GameSpace space;

    public override string ToString()
    {
        return name;
    }

    public override bool Equals(object other)
    {
        return other is GameLocation && (GameLocation)other == this;
    }

    public static bool operator == (GameLocation x, GameLocation y)
    {
        return x.name == y.name;
    }

    public static bool operator != (GameLocation x, GameLocation y)
    {
        return !(x == y);
    }

    new private void Awake()
    {
        base.Awake();
        RegisterSelf();
        SetPosition(new LatLong(InitialLatitude, InitialLongitude));
        closest = findClosest();
        GameController.Instance.addAsset(this);
        Debug.Log(name + " Awake");
    }

    public bool getCollected()
    {
        return isCollected;
    }

    public void setCollected()
    {
        isCollected = true;
    }

    public Landmark getClosest()
    {
        return closest;
    }

    public void setClosest(Landmark l)
    {
        closest = l;
    }

    private Landmark findClosest()
    {
        double minDistance = double.MaxValue;
        Landmark closest = null;
        foreach (Landmark l in GameController.Instance.originalLandmarks)
        {
            double distance = LatLong.EstimateGreatCircleDistance(GetPosition(), l.getPosition());
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = l;
            }
        }
        return closest;
    }

    private bool allDeps()
    {
        if (dependencies.Length == 0)
            return true;

        for(int i = 0; i < dependencies.Length; i++)
        {
            if (!dependencies[i].getCollected())
                return false;
        }
        return true;
    }

    public void setSpace()
    {
        space = new GameSpace(Constants.ASSET_SIZE_MTRS, GetPosition());
    }

    private void collected()
    {
        setCollected();
        GameController.Instance.removeActive(this);
        ARManager.Instance.triggerAR(assetNum);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        //Set object as active when all dependencies are collected
        if (allDeps() && !isCollected && !GameController.Instance.listContains(this))
            GameController.Instance.setActive(this);

        //Set object as collected when player enters gamespace
        if (Translation.Instance != null && Input.location.status == LocationServiceStatus.Running && space.inSpace(GPS.Instance.latLong) && GameController.Instance.listContains(this))
        {
            collected();
        }

        //Control assets display on AR camera
        ARManager.Instance.displayAsset(this);
    }
}