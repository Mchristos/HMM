using HMM.Datastructures;
using System.Collections.Generic;
using HMM.Functions;

namespace HMM
{
    /// <summary>
    /// Records the state of the HMM.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public class ViterbiState<S, T>
    {
        public T PrevObservation { get; set; }
        public ProbabilityVector<S> Probabilities { get; set; }
        public List<Dictionary<S,S>> TransitionMemory { get; set; }

        public ViterbiState()
        {
        }

        public static ViterbiState<S, T> InitialState()
        {
            var result = new ViterbiState<S, T>();
            result.Probabilities = new ProbabilityVector<S>();
            result.TransitionMemory = new List<Dictionary<S, S>>();
            return result;
        }

        public List<S> GetMostLikelySequence()
        {
            if (Probabilities != null && Probabilities.Count > 0)
            {
                // get most probable road
                S mostLikelyState = Probabilities.GetMostProbableItem();
                // trace back most likely sequence
                List<S> result = TransitionMemory.TraceBackSequence(mostLikelyState);
                return result;
            }
            else
            {
                return new List<S>();
            }
        }

    }
}