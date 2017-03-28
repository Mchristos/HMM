using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.ExampleModels
{
    public class SimpleHMM<S,T> : IHiddenMarkovModel<S, T>
    {

        public IEnumerable<S> PossibleStates { get; set; }

        public IEnumerable<T> PossibleObservations { get; set; }

        public Dictionary<S, Dictionary<S, double>> TransitionMatrix { get; set; }

        public Dictionary<S, Dictionary<T, double>> EmissionMatrix { get; set; }

        public SimpleHMM(IEnumerable<S> possibleStates, IEnumerable<T> possibleObservations, Dictionary<S, Dictionary<S, double>> transitionMatrix, Dictionary<S, Dictionary<T, double>> emissionMatrix)
        {
            PossibleStates = possibleStates;
            PossibleObservations = possibleObservations;
            TransitionMatrix = transitionMatrix;
            EmissionMatrix = emissionMatrix;
        }

        public double GetEmissionProbability(StateObservationPair<S, T> pair)
        {
            return EmissionMatrix[pair.State][pair.Observation];
        }

        public List<S> GetPossibleHiddenStates(T observation)
        {
            return PossibleStates.Where(s => EmissionMatrix[s][observation] > 0).ToList();
        }

        public double GetTransitionProbability(StateObservationPair<S, T> from, StateObservationPair<S, T> to)
        {
            return TransitionMatrix[from.State][to.State];
        }
    }
}
