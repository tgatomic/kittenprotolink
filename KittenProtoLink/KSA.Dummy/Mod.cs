using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSA
{
    public class Mod
    {
        public required string DirectoryPath { get; init; }
        public string Name = string.Empty;

        public Mod() { }
        public void PrepareSystems()
        {
        }
    }
}
