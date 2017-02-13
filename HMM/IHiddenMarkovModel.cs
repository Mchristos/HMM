using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM
{
    /// <summary>
    /// 
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
        double GetTransition(StateObservationPair<S, T> from, StateObservationPair<S, T> to);

        /// <summary>
        /// Gets the probability of a hidden state emiting an observation 
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        double GetEmission(StateObservationPair<S, T> pair);

        /// <summary>
        /// Gets hidden state candidates for a given observation. Usually just states with emission greater than zero. This is useful if computing emissions for all states is either impossible or very expensive. 
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        List<S> GetHiddenStateCandidates(T observation);
    }
}
