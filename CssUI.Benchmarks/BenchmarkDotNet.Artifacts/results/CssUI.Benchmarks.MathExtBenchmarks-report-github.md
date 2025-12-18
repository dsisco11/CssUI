```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7171/24H2/2024Update/HudsonValley)
12th Gen Intel Core i9-12900K 3.20GHz, 1 CPU, 24 logical and 16 physical cores
.NET SDK 9.0.308
  [Host]   : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v3
  .NET 8.0 : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v3

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                     | Mean       | Error     | StdDev    | Median     | Allocated |
|--------------------------- |-----------:|----------:|----------:|-----------:|----------:|
| MathExt_Min_Int            |  0.0000 ns | 0.0000 ns | 0.0000 ns |  0.0000 ns |         - |
| SystemMath_Min_Int         |  0.0014 ns | 0.0042 ns | 0.0035 ns |  0.0000 ns |         - |
| MathExt_Min_Double         |  0.2761 ns | 0.0378 ns | 0.0316 ns |  0.2712 ns |         - |
| SystemMath_Min_Double      |  0.2967 ns | 0.0382 ns | 0.0572 ns |  0.2871 ns |         - |
| MathExt_Max_Int            |  0.0531 ns | 0.0337 ns | 0.0388 ns |  0.0477 ns |         - |
| SystemMath_Max_Int         |  0.0053 ns | 0.0099 ns | 0.0111 ns |  0.0000 ns |         - |
| MathExt_Max_Double         |  0.2657 ns | 0.0384 ns | 0.0526 ns |  0.2614 ns |         - |
| SystemMath_Max_Double      |  0.3251 ns | 0.0397 ns | 0.0819 ns |  0.3101 ns |         - |
| MathExt_Clamp_Int          |  0.3377 ns | 0.0414 ns | 0.0668 ns |  0.3409 ns |         - |
| SystemMath_Clamp_Int       |         NA |        NA |        NA |         NA |        NA |
| MathExt_Clamp_Double       |  0.3848 ns | 0.0399 ns | 0.0730 ns |  0.3924 ns |         - |
| SystemMath_Clamp_Double    |         NA |        NA |        NA |         NA |        NA |
| MathExt_Min_Bulk           | 10.3679 ns | 0.2392 ns | 0.2349 ns | 10.4192 ns |         - |
| SystemMath_Min_Bulk        |  9.7467 ns | 0.2345 ns | 0.2606 ns |  9.8522 ns |         - |
| MathExt_Min_Bulk_Double    | 10.4703 ns | 0.2358 ns | 0.3529 ns | 10.5397 ns |         - |
| SystemMath_Min_Bulk_Double | 12.6223 ns | 0.1659 ns | 0.1552 ns | 12.6317 ns |         - |

Benchmarks with issues:
  MathExtBenchmarks.SystemMath_Clamp_Int: .NET 8.0(Runtime=.NET 8.0)
  MathExtBenchmarks.SystemMath_Clamp_Double: .NET 8.0(Runtime=.NET 8.0)
