using HMM.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HMM.Test
{
    public class Tests
    {
        [Fact]
        public void SequenceTest()
        {
            List<string> states = new List<string>() { "s1", "s2" };
            List<string> observations = new List<string>() { "o1", "o2" };

            var transitionMatrix = new Dictionary<string, Dictionary<string, double>>();
            transitionMatrix["s1"] = new Dictionary<string, double>();
            transitionMatrix["s1"]["s1"] = 0.8;
            transitionMatrix["s1"]["s2"] = 0.2;
            transitionMatrix["s2"] = new Dictionary<string, double>();
            transitionMatrix["s2"]["s2"] = 0.5;
            transitionMatrix["s2"]["s1"] = 0.5;

            var emissionMatrix = new Dictionary<string, Dictionary<string, double>>();
            emissionMatrix["s1"] = new Dictionary<string, double>();
            emissionMatrix["s1"]["o1"] = 0.8;
            emissionMatrix["s1"]["o2"] = 0.2;
            emissionMatrix["s2"] = new Dictionary<string, double>();
            emissionMatrix["s2"]["o1"] = 0.2;
            emissionMatrix["s2"]["o2"] = 0.8;


            var hmmModel = new HMM.ExampleModels.SimpleHMM<string,string>(states, observations, transitionMatrix, emissionMatrix);

            var sequenceOfObservations = new List<string>() { "o1", "o2","o1" };
            var expectedResult = new List<string>() { "s1", "s1", "s1"};

            ViterbiEngine<string, string> engine = new ViterbiEngine<string, string>(hmmModel);
            foreach (var obs in sequenceOfObservations)
            {
                engine.TryUpdate(obs);
            }

            var result = engine.GetMostLikelySequence();

            Assert.Equal(expectedResult, result);
        }


        [Fact]
        public void TraceBackSequence_Expected()
        {
            var input = new List<Dictionary<int, int>>();
            input.Add(
                new Dictionary<int, int>()
                {
                    [1] = 1,
                    [2] = 2,
                    [3] = 3
                });

            int seedValue = 1;
            var actualResult = input.TraceBackSequence(seedValue);
            // expect traced back sequnce to be one longer than the input 'memory' list 
            Assert.Equal(input.Count + 1, actualResult.Count);

            var expectedResult = new List<int>() { 1, 1 };

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void TraceBackSequence_Exception()
        {
            var input = new List<Dictionary<int, int>>();
            input.Add(
                new Dictionary<int, int>()
                {
                    [1] = 1,
                    [2] = 2,
                    [3] = 3
                });

            // seed value to start tracing back sequence not valid
            int seedValue = 99;
            Assert.Throws<ArgumentException>(() => input.TraceBackSequence(seedValue));
        }
    }
}
