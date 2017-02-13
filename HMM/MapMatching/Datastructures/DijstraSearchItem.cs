using HMM.MapMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.MapMatching.Datastructures
{
    public class DijstraSearchItem : IComparable<DijstraSearchItem>
    {
        public string Id { get; set; }

        //remembers the road leading to this item (since different roads can come from the same previous node id)
        public DirectedRoad PrevRoad { get; set; }

        public DijstraSearchItem Prev { get; set; }

        public double Distance { get; set; }

        public DijstraSearchItem(string id, DirectedRoad prevRoad, DijstraSearchItem prev, double dist)
        {
            Id = id;
            PrevRoad = prevRoad;
            Prev = prev;
            Distance = dist;
        }

        public int CompareTo(DijstraSearchItem other)
        {
            return Distance.CompareTo(other.Distance);
        }
    }
}
