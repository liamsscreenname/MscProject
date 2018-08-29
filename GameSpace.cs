using Wrld.Space;

public class GameSpace
{
    //Four corners of a square
    private LatLong tl, tr, bl, br;

	public GameSpace(double size, LatLong centre)
	{
        double lat = centre.GetLatitude();
        double lon = centre.GetLongitude();
        double offset = size / 2.0;

        tl = new LatLong(Translation.Instance.offsetLat(lat, offset), Translation.Instance.offsetLon(lon, lat, -offset));
        tr = new LatLong(Translation.Instance.offsetLat(lat, offset), Translation.Instance.offsetLon(lon, lat, offset));
        bl = new LatLong(Translation.Instance.offsetLat(lat, -offset), Translation.Instance.offsetLon(lon, lat, -offset));
        br = new LatLong(Translation.Instance.offsetLat(lat, -offset), Translation.Instance.offsetLon(lon, lat, offset));
    }

    //Check whether a given position is within the gamespace
    public bool inSpace(LatLong pos)
    {
        double lat = pos.GetLatitude();
        double lon = pos.GetLongitude();

        return lat < tl.GetLatitude() && lat > bl.GetLatitude() && lon < tr.GetLongitude() && lon > tl.GetLongitude();
    }
}
