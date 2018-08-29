using System;
using System.Net;
using LitJson;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using UnityEngine;

public class GoogleAPI 
{
    public static GoogleAPI Instance {get; set;}
    private static WebClient client = new WebClient();

    public Landmark mark;

    private string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?key=AIzaSyDulvzdsfygplaUcl2QTs0WLCHHimWVFxQ&radius=" + Constants.GAMESPACE_SIZE_MTRS + "&location=";

    public GoogleAPI()
    {
        Instance = this;
        Debug.Log("Google API Construct");
    }

    //From "ludo6577" - https://answers.unity.com/questions/50013/httpwebrequestgetrequeststream-https-certificate-e.html
    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
    
    //Parse a JSONData object to a Landmark
    public Landmark jsonParse(JsonData result, string type)
    {
        JsonData location = result["geometry"]["location"];
        return new Landmark(result["name"].ToString(), type, (double)location["lat"], (double)location["lng"]);
    }

    //Retrieve 10 similar landmarks in a given location
    public List<Landmark> getSimilar (Landmark l)
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        string response = client.DownloadString(url + Translation.Instance.getStartLat() + ',' + Translation.Instance.getStartLon() + "&type=" + l.getType());
        JsonData jsonResponse = JsonMapper.ToObject(response);
        List<Landmark> results = new List<Landmark>();
        for (int i = 0; i < Constants.NUM_CANDIDATES; i++)
        {
            JsonData res = jsonResponse["results"];
            //If there are enough valid results
            if (res.Count > i)
            {
                results.Add(jsonParse(res[i], l.getType()));
            }
        }
        return results;
    }
}
