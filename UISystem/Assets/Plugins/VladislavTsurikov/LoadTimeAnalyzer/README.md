# LoadTimeAnalyzer Framework

## Purpose

`LoadTimeAnalyzer` is a lightweight performance profiling framework for Unity that measures the duration of each step in your game’s loading pipeline.  
It provides a hierarchical breakdown of execution time, allowing developers to identify bottlenecks and optimize startup performance.

The framework captures:
- The total duration of the entire bootstrap sequence  
- Execution times of individual asynchronous operations  
- Nested step hierarchies with relative percentages and milliseconds

---

## Core Concept

`TimeLogger` operates as a **stack-based timer system**.  
Each `BeginSample("Name")` call starts a new timing block, and `EndSample("Name")` finalizes it.  
Nested samples are automatically grouped, forming a structured performance tree.

When the root sample (for example, `"Bootstrap Load"`) completes, the logger outputs a fully formatted breakdown:

```
[TimeLogger] Load timing breakdown:
Bootstrap Load: 28042 ms (28.042s) (100,00%)
  InitSettings: 2 ms (2ms) (0,01%)
  TryConnectToServer: 5065 ms (5.065s) (18,06%)
  InitializeServices: 5682 ms (5.682s) (20,26%)
  LoadSceneAsync: 1858 ms (1.858s) (6,63%)
  ApplySceneTransition: 5782 ms (5.782s) (20,62%)
```

Each sub-sample shows its duration and percentage relative to its parent sample.

---

## Integration

Add `TimeLogger` calls into your main loading pipeline:

```csharp
public async void Initialize()
{
    TimeLogger.Clear();
    TimeLogger.BeginSample("Bootstrap Load");

    TimeLogger.BeginSample("InitSettings");
    await InitSettings();
    TimeLogger.EndSample("InitSettings");

    TimeLogger.BeginSample("TryConnectToServer");
    if (!await TryConnectToServer())
        return;
    TimeLogger.EndSample("TryConnectToServer");

    TimeLogger.BeginSample("InitializeServices");
    await InitializeServices();
    TimeLogger.EndSample("InitializeServices");

    TimeLogger.BeginSample("LoadSceneAsync");
    var handle = Addressables.LoadSceneAsync("MainScene");
    await handle.ToUniTask();
    TimeLogger.EndSample("LoadSceneAsync");

    TimeLogger.BeginSample("ApplySceneTransition");
    await TransitionEffect.ApplyAsync();
    TimeLogger.EndSample("ApplySceneTransition");

    TimeLogger.EndSample("Bootstrap Load");
    TimeLogger.LogResults();
}
```

When `LogResults()` is called, a detailed breakdown is printed in the Unity Console.

---

## API Reference

| Method | Description |
|---------|--------------|
| `Clear()` | Clears all previous samples. |
| `BeginSample(string name)` | Starts a timing block with the specified name. |
| `EndSample(string name)` | Ends the timing block. |
| `EnableSorting(bool enabled)` | Sorts child samples by duration in the output. |
| `LogResults()` | Outputs the complete hierarchical timing report. |

---

## Output Example

```
[TimeLogger] Load timing breakdown:
Bootstrap Load: 28042 ms (28.042s) (100%)
  InitSettings: 2 ms (0.01%)
  TryConnectToServer: 5065 ms (18.06%)
  InitializeData: 4030 ms (14.37%)
  InitializeServices: 5682 ms (20.26%)
  LoadAdditionalPackages: 1238 ms (4.41%)
  LoadSceneAsync: 1858 ms (6.63%)
  ApplySceneTransition: 5782 ms (20.62%)
  StartServices: 2 ms (0.01%)
```
