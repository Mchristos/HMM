using HMM.Datastructures;
using HMM.MapMatching.Config;
using HMM.MapMatching.Functions;
using HMM.MapMatching.Models;
using HMM.MapMatching.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.MapMatching.Datastructures
{
    public class RoadSearchGrid : SearchGrid<DirectedRoad>
    {
        public RoadSearchGrid(double left, double bottom, double gridSizeX, double gridSizeY, int cellCountX, int cellCountY) : base(left, bottom, gridSizeX, gridSizeY, cellCountX, cellCountY)
        {
        }

        public List<DirectedRoad> GetNearbyValues(Coord query, double radiusInMeters)
        {
            List<DirectedRoad> searchResult = GetNearbyValues(query.Longitude, query.Latitude);
            //return searchResult;
            var result = new List<DirectedRoad>();
            foreach (var road in searchResult)
            {
                int pos;
                var projectedDistance = query.HaversineDistance(query.SnapToPolyline(road.Geometry, out pos));
                if (projectedDistance.DistanceInMeters < radiusInMeters)
                {
                    result.Add(road);
                }
            }
            if (result.Count == 0)
            {
                return searchResult;
            }
            return result;
        }

        public void Populate(IEnumerable<DirectedRoad> roads)
        {
            foreach (var road in roads)
            {
                AddRoad(road);
            }
        }

        public void AddRoad(DirectedRoad road)
        {
            for (int i = 1; i < road.Geometry.Count; i++)
            {
                int[] startCell = this.GetGridCellOfPoint(road.Geometry[i - 1].Longitude, road.Geometry[i - 1].Latitude);
                int[] endCell = this.GetGridCellOfPoint(road.Geometry[i].Longitude, road.Geometry[i].Latitude);
                List<int[]> intersectedCells = this.Bresenham(startCell[0], startCell[1], endCell[0], endCell[1]);
                foreach (var cell in intersectedCells)
                {
                    int x = cell[0];
                    int y = cell[1];
                    this[x][y].Add(road);
                }
            }
        }

        // This should be inplemented for the base class too :( 
        public RoadSearchGrid Clone()
        {
            var result = new RoadSearchGrid(Left, Bottom, GridSizeX, GridSizeY, CellCountX, CellCountY);
            foreach (var int1 in this.Keys)
            {
                var dict = new Dictionary<int, List<DirectedRoad>>();
                foreach (var int2 in this[int1].Keys)
                {
                    dict[int2] = this[int1][int2];
                }
                result[int1] = dict;
            }
            return result;
        }

        public static RoadSearchGrid ComputeSearchGrid(RoadGraph graph, double gridSizeInMeters, BoundingBox boundingBox = null)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            double minLat = double.NaN;
            double maxLat = double.NaN;
            double minLng = double.NaN;
            double maxLng = double.NaN;
            if (boundingBox != null)
            {
                minLat = boundingBox.LatLowerBound;
                maxLat = boundingBox.LatUpperBound;
                minLng = boundingBox.LngLowerBound;
                maxLng = boundingBox.LngUpperBound;
            }
            else
            {
                List<Coord> allCoords = graph.Roads.SelectMany(x => x.Geometry).ToList();
                var latitudes = allCoords.Select(x => x.Latitude);
                var longitudes = allCoords.Select(x => x.Longitude);
                minLat = latitudes.Min();
                maxLat = latitudes.Max();
                minLng = longitudes.Min();
                maxLng = longitudes.Max();
            }

            // Get rough lat/lng delta values corresponding to the grid size
            double refLatInRadians = (((minLat + maxLat) / 2) * (Math.PI / 180.0));
            double roughLngGridSize = ((gridSizeInMeters / (Constants.Earth_Radius_In_Meters * Math.Cos(refLatInRadians))) * (180.0 / Math.PI));
            double roughLatGridSize = ((gridSizeInMeters / Constants.Earth_Radius_In_Meters) * (180.0 / Math.PI));

            // Get total width and height of the grid
            double lngWidth = (maxLng + roughLngGridSize) - (minLng - roughLngGridSize);
            double latHeight = (maxLat + roughLatGridSize) - (minLat - roughLatGridSize);

            // Corners are the minimum values with an added margin
            double left = minLng - roughLngGridSize;
            double bottom = minLat - roughLatGridSize;

            // Dimensions are the total length over the grid size ( may not divide in perfectly)
            int cellCountX = (int)(lngWidth / roughLngGridSize);
            int cellCountY = (int)(latHeight / roughLatGridSize);

            // Actual grid sizes are obtained from the above dimensions and the length the sides
            double gridSizeX = lngWidth / cellCountX;
            double gridSizeY = latHeight / cellCountY;

            var result = new RoadSearchGrid(left, bottom, gridSizeX, gridSizeY, cellCountX, cellCountY);
            // TODO: correct roads input below??
            result.Populate(graph.Values.SelectMany(x => x));

            stopwatch.Stop();
            return result;
        }
    }
}
