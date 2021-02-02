using System;
using BenchmarkDotNet.Running;

namespace zero_allocation_app
{
    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
