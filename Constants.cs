using System;

public static class Constants
{
    //Number of candidates to find in "Similar Landmarks" search
    public const int NUM_CANDIDATES = 20;
    //Size of the area around each game asset in Meters
    public const double ASSET_SIZE_MTRS = 40.0;
    //Size of the gamespace in Meters
    public const double GAMESPACE_SIZE_MTRS = 2500.0;
    public const double PI = 3.14159265;
    //Radius of Earth in metres [Reference to stackexchange page]
    public const double RADIUS = 6378137;
    //Min and Max displacements for generating Game Locations
    public const float MAX_DISPLACEMENT = 40.0f;
    public const float MIN_DISPLACEMENT = -40.0f;
    //Range within which objects come into view 
    public const float VIEW_RANGE = 30.0f;
    //Height of ground plane
    public const float FLOOR_HEIGHT = -2500.0f;
}
