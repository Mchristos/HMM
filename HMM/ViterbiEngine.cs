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
        public ViterbiState<S, T> ViterbiState { get; set; }

        public IHiddenMarkovModel<S,T> Model { get; set; }

        public ViterbiEngine(IHiddenMarkovModel<S, T> model)
        {
            ViterbiState = ViterbiState<S, T>.InitialState();
            Model = model;
        }

        public bool TryUpdate(T observation)
        {
            //Get "nearby" states
            List<S> nearbyStates = Model.GetPossibleHiddenStates(observation);

            if(nearbyStates.Count == 0)
            {
                return false;
            }

            // Initialize new transition memory
            var transitionMemory = new Dictionary<S, S>();
            // Initialize new probability vector
            var newProbabilityVector = new ProbabilityVector<S>();
            if (ViterbiState.PrevObservation == null)
            {
                foreach (var state in nearbyStates)
                {
                    newProbabilityVector[state] = Model.GetEmissionProbability(StateObservationPair<S, T>.New(state, observation));
                    transitionMemory[state] = default(S);
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
                    transitionMemory[state] = maxCandidate.Key;
                }
            }

            if(newProbabilityVector.Sum() < double.Epsilon)
            {
                return false;
            }

            //Update state 
            ViterbiState.Probabilities = newProbabilityVector.Normalize();
            ViterbiState.PrevObservation = observation;
            ViterbiState.TransitionMemory.Add(transitionMemory);

            return true;
        }

        public List<S> GetMostLikelySequence()
        {
            return ViterbiState.GetMostLikelySequence();
        }

        public S GetMostLikelyState()
        {
            return ViterbiState.Probabilities.GetMostProbableItem();
        }

        public void Reset()
        {
            ViterbiState = ViterbiState<S, T>.InitialState();
        }
    }
}