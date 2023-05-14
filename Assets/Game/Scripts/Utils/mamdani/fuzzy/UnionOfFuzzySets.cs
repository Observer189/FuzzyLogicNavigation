using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class UnionOfFuzzySets: FuzzySetInterface
    {
        private List<FuzzySetInterface> fuzzySets = new List<FuzzySetInterface>();

        public UnionOfFuzzySets() { }

        public void AddFuzzySet(FuzzySetInterface fuzzySet)
        {
            fuzzySets.Add(fuzzySet);
        }

        private double getMaxValue(double x)
        {
            double result = 0.0;

            foreach (FuzzySetInterface fuzzySet in fuzzySets)
            {
                result = Math.Max(result, fuzzySet.GetValue(x));
            }

            return result;

           
        }

        public double GetValue(double value)
        {
            return getMaxValue(value);
        }
        }
}
