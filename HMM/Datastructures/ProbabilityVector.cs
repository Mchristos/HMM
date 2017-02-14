using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.Datastructures
{
    public class ProbabilityVector<S> : Dictionary<S, double>
    {
        public ProbabilityVector() : base()
        {
        }

        public ProbabilityVector(IEnumerable<S> collection) : base()
        {
            if (collection == null) { throw new NullReferenceException("Collection is null"); }
            int count = collection.Count();
            if (count == 0) { throw new ArgumentException("Collection is empty"); }
            foreach (var item in collection)
            {
                this[item] = 1 / count;
            }
        }

        public ProbabilityVector<S> Normalize()
        {
            double sum = GetSum();
            if (sum > 0)
            {
                var result = new ProbabilityVector<S>();
                foreach (var key in this.Keys)
                {
                    result[key] = this[key] / sum;
                }
                return result;
            }
            else
            {
                return this;
            }
        }

        public double GetSum()
        {
            return this.Values.Sum();
        }

        public S GetMostProbableItem()
        {
            if (this.Count == 0) { throw new Exception("Probability vector is empty"); }
            return this.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        }
    }
}
