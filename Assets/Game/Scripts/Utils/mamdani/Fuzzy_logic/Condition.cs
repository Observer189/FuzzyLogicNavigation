using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy_logic
{
    public class Condition: Statement
    {
        public Condition(Variable variable, FuzzySet term) : base(term, variable)
        {
            
        }
    }
}
