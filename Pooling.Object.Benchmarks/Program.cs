using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using BenchmarkDotNet.Validators;
using Depra.Pooling.Object.Benchmarks;

var benchmark = BenchmarkSwitcher.FromTypes([
	typeof(ObjectPoolBenchmarks),
]);

IConfig configuration = DefaultConfig.Instance
	.AddDiagnoser(MemoryDiagnoser.Default)
	.AddValidator(JitOptimizationsValidator.FailOnError)
	.AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance))
	.WithOptions(ConfigOptions.DisableOptimizationsValidator)
	.WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));

if (args.Length > 0)
{
	benchmark.Run(args, configuration);
}
else
{
	benchmark.RunAll(configuration);
}