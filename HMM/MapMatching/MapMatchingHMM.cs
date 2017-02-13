using HMM.MapMatching.Config;
using HMM.MapMatching.Datastructures;
using HMM.MapMatching.Functions;
using HMM.MapMatching.Models;
using HMM.MapMatching.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.MapMatching
{
    class MapMatchingHMM<T> : IHiddenMarkovModel<DirectedRoad, Coord>
    {

        public MapMatcherParameters Parameters { get; set; }
        public RoadGraph Graph { get; set; }
        public RoadSearchGrid SearchGrid { get; set; }


        private Dictionary<string, T> _dataByRoadId { get; set; }

        public MapMatchingHMM(List<T> data, Func<T, DirectedRoad> dataToRoad, MapMatcherParameters parameters, BoundingBox boundingBox = null)
        {
            Parameters = parameters;

            //Build graph
            _dataByRoadId = new Dictionary<string, T>();
            var graph = new RoadGraph();
            foreach (var datum in data)
            {
                var road = dataToRoad(datum);
                graph.AddRoad(road);
                _dataByRoadId[road.Squid] = datum;
            }
            Graph = graph;

            // Compute search grid (for accessing nearby roads)
            SearchGrid = RoadSearchGrid.ComputeSearchGrid(graph, parameters.NearbyRoadsThreshold, boundingBox);
        }

        public double GetEmission(StateObservationPair<DirectedRoad, Coord> pair)
        {
            var road = pair.State;
            var coord = pair.Observation;
            int position;
            Coord projection = coord.SnapToPolyline(road.Geometry, out position);
            Distance dist = coord.HaversineDistance(projection);
            return ProbabilityFunctions.HalfGaussian(dist.DistanceInMeters, Parameters.Sigma);            
        }

        private Dictionary<Coord, RoadProjection> _projections = new Dictionary<Coord, RoadProjection>();

        public List<DirectedRoad> GetHiddenStateCandidates(Coord observation)
        {
            return SearchGrid.GetNearbyValues(observation, DefaultValues.Nearby_Road_Radius_In_Meters);
        }

        public double GetTransition(StateObservationPair<DirectedRoad, Coord> from, StateObservationPair<DirectedRoad, Coord> to)
        {
            DirectedRoad road1 = from.State;
            DirectedRoad road2 = to.State;

            RoadProjection projection1;
            if (! _projections.TryGetValue(from.Observation, out projection1))
            {
                projection1 = new RoadProjection(from.Observation, from.State);
                _projections[from.Observation] = projection1;
            }
            RoadProjection projection2;
            if (!_projections.TryGetValue(to.Observation, out projection2))
            {
                projection2 = new RoadProjection(to.Observation, to.State);
                _projections[to.Observation] = projection2;
            }

            //calculate on road distance
            double onRoadDistanceInMeters;
            Distance startingDist = projection1.DistanceToEnd;
            Distance endDist = projection2.DistanceFromStart;

            // Roads are the same:
            if (road1.Equals(road2))
            {
                //negative if going backwards along road
                onRoadDistanceInMeters = projection2.DistanceFromStart.DistanceInMeters - projection1.DistanceFromStart.DistanceInMeters;
            }
            // Road start or end on the same node
            else if (road1.End == road2.End || road1.Start == road2.Start)
            {
                //make this transition impossible
                return 0;
            }

            // Roads are connected (can be same road in opposite direction)
            else if (road1.End == road2.Start)
            {
                onRoadDistanceInMeters = startingDist.DistanceInMeters + endDist.DistanceInMeters;
            }

            // Try connect roads using Dijstra
            else
            {
                List<DirectedRoad> path;
                if (PathFinding.DijstraTryFindPath(Graph, road1.End, road2.Start, Parameters.DijstraUpperSearchLimit, out path))
                {
                    Distance connectingDist = Distance.Zero;
                    foreach (var road in path)
                    {
                        connectingDist += road.Length;
                    }
                    onRoadDistanceInMeters = startingDist.DistanceInMeters + connectingDist.DistanceInMeters + endDist.DistanceInMeters;
                }
                else
                {
                    //cannot connect up roads. transition probability is zero
                    return 0;
                }
            }
            Distance haversineDistance = from.Observation.HaversineDistance(to.Observation);
            double diffInMeters = Math.Abs(haversineDistance.DistanceInMeters - onRoadDistanceInMeters);
            if (diffInMeters > Parameters.TransitionDiffThreshold)
            {
                return 0;
            }
            else
            {
                return ProbabilityFunctions.ExponentialDistribution(diffInMeters, Parameters.Beta);
            }
        }
    }

    internal class RoadProjection
    {
        // Distance from the co-ordinate to its projection on the road
        public Distance ProjectedDistance { get; set; }
        public Coord Projection { get; set; }
        public DirectedRoad Road { get; set; }
        public int IndexInRoad { get; set; }
        public Distance DistanceFromStart { get; set; }
        public Distance DistanceToEnd { get; set; }
        public RoadProjection(Coord coord, DirectedRoad road)
        {
            Road = road;
            int position = -1;
            Coord projection = coord.SnapToPolyline(road.Geometry, out position);
            IndexInRoad = position;
            Projection = projection;
            ProjectedDistance = coord.HaversineDistance(projection);
            DistanceFromStart = computeDistanceFromStart();
            DistanceToEnd = computeDistanceToEnd();
        }
        private Distance computeDistanceFromStart()
        {
            return DistanceFunctions.ComputeCumulativeDistanceFromStart(Road.Geometry, IndexInRoad, Projection);
        }
        private Distance computeDistanceToEnd()
        {
            return DistanceFunctions.ComputeDistanceToEnd(Road.Geometry, IndexInRoad, Projection);
        }
    }
}
