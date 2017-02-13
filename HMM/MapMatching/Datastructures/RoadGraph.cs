using HMM.MapMatching.Models;
using HMM.MapMatching.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace HMM.MapMatching.Datastructures
{
    /// <summary>
    /// Encodes graph of a road network as an adjacency list : each node stores a list of the directed roads leaving that node.
    /// </summary>
    public class RoadGraph : Dictionary<string, List<DirectedRoad>>
    {
        /// <summary>
        /// Look up node position by id
        /// </summary>
        private Dictionary<string, Coord> _nodeLookup { get; set; }

        /// <summary>
        /// Look up road by id
        /// </summary>
        private Dictionary<string, DirectedRoad> _roadLookup { get; set; }

        /// <summary>
        ///  Look up number of incoming roads to given node
        /// </summary>
        private Dictionary<string, List<DirectedRoad>> _inverseGraph { get; set; }

        public RoadGraph() : base()
        {
            _nodeLookup = new Dictionary<string, Coord>();
            _roadLookup = new Dictionary<string, DirectedRoad>();
            _inverseGraph = new Dictionary<string, List<DirectedRoad>>();
        }

        public void AddRoad(DirectedRoad road)
        {
            _nodeLookup[road.Start] = road.Geometry.First();
            _roadLookup[road.Squid] = road;
            if (this.ContainsKey(road.Start))
            {
                //the beginning of the road ia alreasy a node in the graph
                this[road.Start].Add(road);
            }
            else
            {
                // add the start of the road as a node in the graph, plus the edge.
                this[road.Start] = new List<DirectedRoad>() { road };
            }
            if (_inverseGraph.ContainsKey(road.End))
            {
                _inverseGraph[road.End].Add(road);
            }
            else
            {
                _inverseGraph[road.End] = new List<DirectedRoad>() { road };
            }
        }

        public Dictionary<string, Coord> Nodes
        {
            get
            {
                return _nodeLookup;
            }
        }

        public List<DirectedRoad> Roads
        {
            get
            {
                return _roadLookup.Values.ToList();
            }
        }

        /// <summary>
        /// Stores INCOMING roads to a given node
        /// </summary>
        public Dictionary<string, List<DirectedRoad>> InverseGraph
        {
            get
            {
                return _inverseGraph;
            }
        }
    }
}