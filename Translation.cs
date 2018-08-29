using System.Collections.Generic;
using UnityEngine;
using Wrld.Space;

//Script in charge of the translation procedure
public class Translation
{
    public static Translation Instance { get; set; }

    public GameSpace gameSpace;
    private LatLong startPosition;

	public Translation(LatLong start)
	{
        Debug.Log("Translation Construct");
        Instance = this;
        startPosition = start;
        gameSpace = new GameSpace(Constants.GAMESPACE_SIZE_MTRS, start);
    }

    public double getStartLat()
    {
        return startPosition.GetLatitude();
    }

    public double getStartLon()
    {
        return startPosition.GetLongitude();
    }

    public double offsetLat(double lat, double off)
    {
        return lat + (off / Constants.RADIUS) * (180.0 / Constants.PI);
    }

    public double offsetLon(double lon, double lat, double off)
    {
        return lon + (off / (Constants.RADIUS * System.Math.Cos(Constants.PI * lat / 180.0))) * (180.0 / Constants.PI);
    }

    public List<Landmark> translateLandmark()
    {
        int numLocations = GameController.Instance.originalLandmarks.Count;

        List<Landmark> results = new List<Landmark>();
        //Results starts as originals
        foreach (Landmark l in GameController.Instance.originalLandmarks)
            results.Add(l);

        List<List<Landmark>> candidates = new List<List<Landmark>>();

        float[] costs = new float[numLocations];
        for (int i = 0; i < numLocations; i++)
            costs[i] = float.MaxValue;

        for (int i = 0; i < numLocations; i++)
        {
            //Assign 10 similar landmarks for each original
            candidates.Add(GoogleAPI.Instance.getSimilar(GameController.Instance.originalLandmarks[i]));

            //Algorithm C - Average Distance between original landmark and previous landmark
            double prevDistanceOriginal;
            if (i == 0)
                prevDistanceOriginal = LatLong.EstimateGreatCircleDistance(OriginalLocations.Instance.getOriginalStart(), GameController.Instance.originalLandmarks[i].getPosition());
            else
                prevDistanceOriginal = LatLong.EstimateGreatCircleDistance(GameController.Instance.originalLandmarks[i].getPosition(), GameController.Instance.originalLandmarks[i - 1].getPosition());

            //Algorithm A - Determine average distance between original landmark and all other original landmarks
            //double avDistanceOriginal = 0.0;
            //for (int j = 0; j < numLocations; j++)
            //{
            //    avDistanceOriginal += LatLong.EstimateGreatCircleDistance(GameController.Instance.originalLandmarks[i].getPosition(), GameController.Instance.originalLandmarks[j].getPosition());
            //}
            //avDistanceOriginal /= numLocations;

            for (int j = 0; j < candidates[i].Count; j++)
            {
                //Algorithm B -Distance between candidate and starting position compared with distance between original landmark and original starting position
                //double costB = LatLong.EstimateGreatCircleDistance(candidates[i][j].getPosition(), startPosition) -
                //    LatLong.EstimateGreatCircleDistance(GameController.Instance.originalLandmarks[i].getPosition(), OriginalLocations.Instance.getOriginalStart());

                //double avDistanceTranslate = 0.0;
                //Algorithm A -Determine average distance between candidate and the other locations that have already been set
                //for (int l = 0; l < i; l++)
                //{
                //    avDistanceTranslate += LatLong.EstimateGreatCircleDistance(candidates[i][j].getPosition(), results[l].getPosition());
                //}
                //if (i > 0)
                //    avDistanceTranslate /= i;

                //Algorithm C - Distance between candidate and previous set location
                double prevDistanceTranslate;
                if (i == 0)
                    prevDistanceTranslate = LatLong.EstimateGreatCircleDistance(startPosition, candidates[i][j].getPosition());
                else
                    prevDistanceTranslate = LatLong.EstimateGreatCircleDistance(candidates[i][j].getPosition(), results[i - 1].getPosition());

                double costC = prevDistanceTranslate - prevDistanceOriginal;
                //double costA = avDistanceTranslate - avDistanceOriginal;
                float cost =  Mathf.Abs((float)costC);
                if (cost < costs[i] && gameSpace.inSpace(candidates[i][j].getPosition()) && !duplicateChecker(candidates[i][j], results))
                {
                    results.RemoveAt(i);
                    results.Insert(i, candidates[i][j]);
                    costs[i] = Mathf.Abs((float)cost);
                }
            }
        }
        return results;
    }

    //Check for duplicate landmarks, e.g. when one landmark is both a gym and a bar. Returns true if given landmark has the same name as one already in the list, false otherwise
    private bool duplicateChecker(Landmark lmrk, List<Landmark> list)
    {
        foreach(Landmark l in list)
        {
            if (l.getName() == lmrk.getName())
                return true;
        }
        return false;
    }
}
