using Wrld.Space;

public class Landmark {

    private string name, type;
    private LatLong position;

    public Landmark(string n, string t, double lat, double lon)
    {
        name = n;
        type = t;
        position = new LatLong(lat, lon);
    }

    public string getName()
    {
        return name;
    }

    public string getType()
    {
        return type;
    }

    public LatLong getPosition()
    {
        return position;
    }	
}
