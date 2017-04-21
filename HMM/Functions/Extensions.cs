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
    }
}
