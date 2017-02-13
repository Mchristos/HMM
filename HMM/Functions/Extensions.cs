using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM.Functions
{
    public static class Extensions
    {
        public static List<T> TraceBackSequence<T>(this List<Dictionary<T, T>> prevMemory, T seedValue)
        {
            List<T> backwardsResult = new List<T>() { seedValue };

            T currentValue = seedValue;
            for (int i = prevMemory.Count - 1; i >= 0; i--)
            {
                Dictionary<T, T> memory = prevMemory[i];
                if (memory.ContainsKey(currentValue))
                {
                    T prevValue = memory[currentValue];
                    if (prevValue == null) break;
                    backwardsResult.Add(prevValue);
                    currentValue = prevValue;
                }
                else
                {
                    throw new ArgumentException("Error in the input 'prevMemory' ");
                }
            }
            return Enumerable.Reverse(backwardsResult).ToList();
        }

        /// <summary>
        /// Eradicates duplicated neighboring values
        /// e.g. [0,1,2,2,2,3] --> [0,1,2,3]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<T> RemoveDuplicateNeighbors<T>(this List<T> input) where T : IEquatable<T>
        {
            if (input.Count < 2) return input;
            T currentValue = input.First();
            var result = new List<T>() { currentValue };
            for (int i = 1; i < input.Count; i++)
            {
                var value = input[i];
                if (!value.Equals(currentValue))
                {
                    result.Add(value);
                    currentValue = value;
                }
            }
            return result;
        }
    }
}
