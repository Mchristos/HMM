# HMM

An abstract formulation of Hidden Markov models written in C#. 

This library allows you to formulate a problem as a [Hidden Markov model](https://en.wikipedia.org/wiki/Hidden_Markov_model). Once you've encoded your problem as an HMM and have a sequence of observations, you can solve for the most likely sequence of states using the [Viterbi algorithm](https://en.wikipedia.org/wiki/Viterbi_algorithm).

## Example usage 

            // define states and observations 
            List<string> states = new List<string>() { "s1", "s2" };
            List<string> observations = new List<string>() { "o1", "o2" };
            
            // define transition probabilities between states 
            var transitionMatrix = new Dictionary<string, Dictionary<string, double>>();
            transitionMatrix["s1"] = new Dictionary<string, double>();
            transitionMatrix["s1"]["s1"] = 0.8;
            transitionMatrix["s1"]["s2"] = 0.2;
            transitionMatrix["s2"] = new Dictionary<string, double>();
            transitionMatrix["s2"]["s2"] = 0.5;
            transitionMatrix["s2"]["s1"] = 0.5;

            // define emission probability = P(observation|state) i.e. probability of observation given state. 
            var emissionMatrix = new Dictionary<string, Dictionary<string, double>>();
            emissionMatrix["s1"] = new Dictionary<string, double>();
            emissionMatrix["s1"]["o1"] = 0.8;
            emissionMatrix["s1"]["o2"] = 0.2;
            emissionMatrix["s2"] = new Dictionary<string, double>();
            emissionMatrix["s2"]["o1"] = 0.2;
            emissionMatrix["s2"]["o2"] = 0.8;

            // instantiate and the HMM model 
            var hmmModel = new HMM.ExampleModels.SimpleHMM<string,string>(states, observations, transitionMatrix, emissionMatrix);

            // we have a sequence of observations, and wish to find the most likely sequence of states 
            var sequenceOfObservations = new List<string>() { "o1", "o2","o1" };

            // The viterbi algorithm soves this problem, and we use the ViterbiEngine class to solve the problem
            ViterbiEngine<string, string> engine = new ViterbiEngine<string, string>(hmmModel);
            // feed each observation into the engine
            foreach (var obs in sequenceOfObservations)
            {
                engine.TryUpdate(obs);
            }
            
            // get out the most likely sequence of states given { "o1", "o2","o1" }
            var result = engine.GetMostLikelySequence();
