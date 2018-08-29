using System;
using System.Collections.Generic;
using Wrld.Space;

//Original Pre-Programmed locations for South Woodham Ferrers, Essex
public class OriginalLocations
{
    private Landmark Supermarket = new Landmark("Supermarket", "supermarket", 51.643622, 0.618206);
    private Landmark PostOffice = new Landmark("Post Office", "post_office", 51.645132, 0.619110);
    private Landmark PoliceStation = new Landmark("Police Station", "police", 51.644013, 0.620627);
    private Landmark Bar = new Landmark("Bar", "bar", 51.644308, 0.618818);
    private Landmark Park = new Landmark("Park", "park", 51.647504, 0.622620);
    private Landmark Library = new Landmark("Library", "library", 51.645631, 0.618705);
    private Landmark Gym = new Landmark("Gym", "gym", 51.645494, 0.618094);
    private Landmark BusStation = new Landmark("Bus Station", "bus_station", 51.643833, 0.619366);

    private LatLong OriginalStart = new LatLong(51.644128, 0.617604);

    public static OriginalLocations Instance { get; set; }

    public OriginalLocations()
    {
        Instance = this;
    }

    public LatLong getOriginalStart()
    {
        return OriginalStart;
    }

    public List<Landmark> getOriginals()
    {
        List<Landmark> list = new List<Landmark>();
        list.Add(Supermarket);
        list.Add(PostOffice);
        list.Add(PoliceStation);
        list.Add(Bar);
        list.Add(Park);
        list.Add(Library);
        list.Add(Gym);
        list.Add(BusStation);

        return list;
    }
}
