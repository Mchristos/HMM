using HMM.Datastructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HMM
{
    /// <summary>
    /// Uses the viterbi algorithm to track the most likely state of a Hidden Markov Model, given observations.
    /// To use this class, define your own Hidden Markov Model by implementing IHiddenMarkovModel, and use the TryUpdate function to update the state of the engine. 
    /// Then you can get the most likely sequence of states using GetMostLikelySequence(). 
    /// </summary>
    /// <typeparam name="S"> State type </typeparam>
    /// <typeparam name="T"> Observation type </typeparam>
    public class ViterbiEngine<S, T>
    {
        private int _stateHistoryDepth = 5;

        /// <summary>
        /// Records the latest viterbi states (from the last x updates, where x = _stateHistoryDepth) 
        /// </summary>
        public List<ViterbiState<S, T>> LatestViterbiStates { get; set; }

        /// <summary>
        /// The state of the Viterbi Engine (not to be confused with the states of type S)  
        /// </summary>
        public ViterbiState<S, T> ViterbiState
        {
            get
            {
                return LatestViterbiStates.Last();
            }
        }

        /// <summary>
        /// The hidden markov model used to calculate emissions and transitions. 
        /// </summary>
        public IHiddenMarkovModel<S,T> Model { get; set; }

        public ViterbiEngine(IHiddenMarkovModel<S, T> model)
        {
            LatestViterbiStates = new List<ViterbiState<S, T>>() { ViterbiState<S, T>.InitialState() };
            Model = model;
        }

        /// <summary>
        /// Updates the state of the Viterbi engine given an observation. 
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        public bool TryUpdate(T observation)
        {
            //Get "nearby" states
            List<S> nearbyStates = Model.GetPossibleHiddenStates(observation);

            if(nearbyStates.Count == 0)
            {
                return false;
            }

            // Initialize new transition memory
            var transitions = new Dictionary<S, S>();
            // Initialize new probability vector
            var newProbabilityVector = new ProbabilityVector<S>();
            if (ViterbiState.PrevObservation == null)
            {
                foreach (var state in nearbyStates)
                {
                    newProbabilityVector[state] = Model.GetEmissionProbability(StateObservationPair<S, T>.New(state, observation));
                    transitions[state] = default(S);
                }
            }
            else
            {
                foreach (var state in nearbyStates)
                {
                    var stateObservationPair = StateObservationPair<S, T>.New(state, observation);
                    // Calculate emission probability
                    double emission = Model.GetEmissionProbability(stateObservationPair);

                    var prevStateCandidates = new Dictionary<S, double>();
                    //Calculate most likely transition probability 
                    foreach (var prevCandidate in ViterbiState.Probabilities.Keys)
                    {
                        var candidateStateObservationPair = StateObservationPair<S, T>.New(prevCandidate, ViterbiState.PrevObservation);
                        double transition = Model.GetTransitionProbability(candidateStateObservationPair, stateObservationPair);
                        prevStateCandidates[prevCandidate] = ViterbiState.Probabilities[prevCandidate] * transition;
                    }
                    var maxCandidate = prevStateCandidates.Aggregate((x, y) => x.Value > y.Value ? x : y);

                    // Update probability and transition memory
                    newProbabilityVector[state] = maxCandidate.Value * emission;
                    transitions[state] = maxCandidate.Key;
                }
            }

            if(newProbabilityVector.Sum() < double.Epsilon)
            {
                return false;
            }

            //Update state 
            UpdateViterbiState(newProbabilityVector.Normalize(), observation, transitions);
            return true;
        }

        /// <summary>
        /// Gets the most likely sequence of states given the current state of the engine. 
        /// </summary>
        /// <returns></returns>
        public List<S> GetMostLikelySequence()
        {
            return ViterbiState.GetMostLikelySequence();
        }

        /// <summary>
        /// Gets the most likely state (technically, the last state of the most likely sequence) given the current state of the engine. 
        /// </summary>
        /// <returns></returns>
        public S GetMostLikelyState()
        {
            return ViterbiState.Probabilities.GetMostProbableItem();
        }

        /// <summary>
        /// Resets the state of the engine. 
        /// </summary>
        public void Reset()
        {
            LatestViterbiStates = new List<ViterbiState<S, T>>() { ViterbiState<S, T>.InitialState() };
        }

        private void UpdateViterbiState(ProbabilityVector<S> probabilities, T prevObservation, Dictionary<S, S> transitions)
        {
            var newTransitionMemory = ViterbiState.TransitionMemory.ToList();
            newTransitionMemory.Add(transitions);
            var newViterbiState = new ViterbiState<S, T>(probabilities, prevObservation, newTransitionMemory);
            LatestViterbiStates.Add(newViterbiState);
            if (LatestViterbiStates.Count > _stateHistoryDepth)
            {
                LatestViterbiStates = LatestViterbiStates.Skip(1).ToList();
            }
        }
    }
}