using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGenerator.Core.Models
{
    public record MethodInfo(string Name, List<string> Parameters, string ReturnType);
}
