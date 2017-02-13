using HMM.Datastructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HMM
{
    /// <summary>
    /// Uses the viterbi algorithm to track the state of a Hidden Markov Model 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ViterbiEngine<S, T>
    {
        public ViterbiState<S, T> ViterbiState { get; set; }

        private Func<StateObservationPair<S, T>, StateObservationPair<S, T>, double> TransitionFunction { get; set; }

        private Func<StateObservationPair<S, T>, double> EmissionFunction { get; set; }

        private Func<T, List<S>> NearbyStates { get; set; }

        public ViterbiEngine(IHiddenMarkovModel<S, T> model)
        {
            TransitionFunction = (x, y) => model.GetTransition(x, y);
            EmissionFunction = x => model.GetEmission(x);
            NearbyStates = x => model.GetHiddenStateCandidates(x);
            ViterbiState = ViterbiState<S, T>.InitialState();
        }

        public bool TryUpdate(T observation)
        {
            //Get "nearby" states
            List<S> nearbyStates = NearbyStates(observation);

            // Initialize new transition memory
            var transitionMemory = new Dictionary<S, S>();
            // Initialize new probability vector
            var newProbabilityVector = new ProbabilityVector<S>();
            if (ViterbiState.PrevObservation == null)
            {
                foreach (var state in nearbyStates)
                {
                    newProbabilityVector[state] = EmissionFunction(StateObservationPair<S, T>.New(state, observation));
                    transitionMemory[state] = default(S);
                }
            }
            else
            {
                foreach (var state in nearbyStates)
                {
                    var stateObservationPair = StateObservationPair<S, T>.New(state, observation);
                    // Calculate emission probability
                    double emission = EmissionFunction(stateObservationPair);

                    var prevStateCandidates = new Dictionary<S, double>();
                    //Calculate most likely transition probability 
                    foreach (var prevCandidate in ViterbiState.Probabilities.Keys)
                    {
                        var candidateStateObservationPair = StateObservationPair<S, T>.New(prevCandidate, ViterbiState.PrevObservation);
                        double transition = TransitionFunction(candidateStateObservationPair, stateObservationPair);
                        prevStateCandidates[prevCandidate] = ViterbiState.Probabilities[prevCandidate] * transition;
                    }
                    var maxCandidate = prevStateCandidates.Aggregate((x, y) => x.Value > y.Value ? x : y);

                    // Update probability and transition memory
                    newProbabilityVector[state] = maxCandidate.Value * emission;
                    transitionMemory[state] = maxCandidate.Key;
                }
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
    }
}