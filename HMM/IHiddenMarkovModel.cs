using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM
{
    /// <summary>
    /// Encodes a Hidden Markov Model. To implement this interface, you must specify how to calculate transition and emission probabilities, 
    /// </summary>
    /// <typeparam name="S"> Hidden state type </typeparam>
    /// <typeparam name="T"> Observation type </typeparam>
    public interface IHiddenMarkovModel<S, T>
    {
        /// <summary>
        /// Gets the probability of transitioning between two hidden states, given observations 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        double GetTransitionProbability(StateObservationPair<S, T> from, StateObservationPair<S, T> to);

        /// <summary>
        /// Gets the probability of a hidden state emiting an observation 
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        double GetEmissionProbability(StateObservationPair<S, T> pair);

        /// <summary>
        /// Gets possible hidden state candidates for a given observation. Usually just states with emission greater than zero. This is useful if computing emissions for all states is either impossible or very expensive.
        /// If you're not sure, let this function return all possible hidden states (ignoring the method argument).
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        List<S> GetPossibleHiddenStates(T observation);
    }
}
