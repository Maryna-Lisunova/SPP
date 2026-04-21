using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGenerator.Core.Models
{
    public record ClassInfo(string Name, List<MethodInfo> Methods, List<string> ConstructorDependencies);
}
