# Dependency Analyzer

A Unity Editor tool for analyzing and removing unused assembly definition dependencies, with advanced dependency graph visualization.

## Features

- **Automatic Scanning**: Scans all assembly definition files (.asmdef) in your project
- **Smart Analysis**: Analyzes C# source code to detect unused namespace references
- **Visual Interface**: Clean editor window with filtering and search capabilities
- **Safe Removal**: Safely removes unused dependencies with confirmation dialogs
- **Real-time Statistics**: Shows counts of total assemblies, assemblies with unused dependencies, and total unused references
- **Dependency Graph Visualization**: Export and visualize dependency graphs with centrality-based layouts
  - **DOT Format**: For Graphviz and other graph tools
  - **JSON Format**: For custom visualization with D3.js, vis.js, etc.
  - **Interactive HTML**: Standalone visualization with force-directed layout

## How to Use

### Opening the Tool

1. In Unity Editor, go to **Tools → Vladislav Tsurikov → Analyze Dependencies**
2. The Dependency Analyzer window will open

### Scanning and Analyzing

1. Click **Scan Assemblies** to scan all .asmdef files in your project
2. Click **Analyze Dependencies** to check which dependencies are actually used
3. The tool will analyze C# files and detect unused namespace references

### Viewing Results

- Assemblies with unused dependencies are highlighted in orange
- Assemblies with all dependencies used show a green "All used" badge
- Each assembly shows:
  - Total number of dependencies
  - Number of unused dependencies
  - List of unused dependency names
  - File path

### Removing Unused Dependencies

#### Remove All
Click **Remove All Unused** to remove all unused dependencies from all assemblies at once

#### Remove Per Assembly
Click the **Remove** button next to a specific assembly to remove only its unused dependencies

### Filtering Results

- **Show only assemblies with unused dependencies**: Toggle to hide assemblies that have no unused dependencies
- **Search**: Filter assemblies by name using the search box

### Exporting Dependency Graphs

Visualize your project's dependency structure with automatic centrality-based layouts:

#### Export Graph (DOT)
- Click **Export Graph (DOT)** to export in Graphviz DOT format
- Open in tools like Graphviz, or online viewers:
  - [Graphviz Online](https://dreampuf.github.io/GraphvizOnline/)
  - [WebGraphviz](https://www.webgraphviz.com/)
- Nodes are grouped by centrality (core/mid-tier/edge)
- Unused dependencies shown as dashed red lines

#### Export Graph (JSON)
- Click **Export Graph (JSON)** to export structured data
- Use with D3.js, vis.js, Cytoscape.js, or other graph libraries
- Contains:
  - Node data with centrality metrics
  - Edge data with usage status
  - Statistics summary

#### Export Visualization (HTML)
- Click **Export Visualization (HTML)** for interactive visualization
- Creates standalone HTML file with D3.js force-directed graph
- Features:
  - **Core dependencies** (high centrality): Center position, green color
  - **Mid-tier dependencies**: Middle position, blue color
  - **Edge dependencies** (low centrality): Periphery position, gray color
  - Node size based on importance
  - Zoom, pan, and drag interactions
  - Hover tooltips with detailed metrics

#### Centrality Calculation

The tool calculates **centrality** for each assembly based on:
- **Usage count** (70% weight): How many assemblies depend on it
- **Dependency penalty** (30% weight): Assemblies with fewer dependencies are more core

High centrality (>0.7) = Core dependency (center)
Medium centrality (0.3-0.7) = Mid-tier dependency
Low centrality (<0.3) = Edge dependency (periphery)

### Detecting Cyclic Dependencies

Find circular dependency chains that can cause compilation and maintenance issues:

#### Running Detection
- Click **🔄 Detect Cyclic Dependencies** to analyze the dependency graph
- The tool uses Depth-First Search (DFS) algorithm to detect cycles
- Results show each cycle as a chain: A → B → C → A

#### Understanding Results
- **No cycles**: Your dependency graph is acyclic (DAG) ✓
- **Cycles found**: Shows warning with cycle count and details
- Toggle between "Show Dependencies" and "Show Cycles" views

#### Cycle Display
Each cycle shows:
- **Chain visualization**: Assembly1 → Assembly2 → Assembly3 → Assembly1
- **Assembly count**: Number of assemblies involved
- **Refactoring suggestion**: Hint to break the cycle

#### Why Cycles Are Bad
Cyclic dependencies cause:
- **Increased compilation times**: Unity must recompile all assemblies in the cycle
- **Code maintenance difficulty**: Hard to understand and modify
- **Potential runtime issues**: Initialization order problems

#### How to Fix Cycles
Common strategies:
1. **Extract common code**: Move shared functionality to a new assembly
2. **Invert dependency**: Use interfaces or events to break the cycle
3. **Merge assemblies**: If tightly coupled, consider combining them
4. **Use dependency injection**: Remove hard references

## How It Works

The analyzer works in several steps:

1. **Scanning**: Finds all .asmdef files and parses their JSON content
2. **Mapping**: Creates a mapping between GUIDs and assembly names
3. **Analysis**: For each assembly:
   - Gets all C# files in the assembly's directory
   - For each dependency, extracts potential namespaces
   - Checks if any namespace is used in the C# files via:
     - `using Namespace;` statements
     - `using Namespace.Something;` statements
     - Direct qualified name usage: `Namespace.Class`
4. **Removal**: Removes unused dependency GUIDs from the .asmdef files

## Benefits

- **Faster Build Times**: Fewer dependencies mean faster compilation
- **Reduced Coupling**: Assemblies become more independent
- **Cleaner Architecture**: Only necessary dependencies remain
- **Better Maintenance**: Easier to understand assembly relationships

## Notes

- The tool only analyzes Editor assemblies (runs in Unity Editor only)
- Changes to .asmdef files trigger Unity to recompile affected assemblies
- It's recommended to commit your changes before removing dependencies (so you can revert if needed)
- The tool uses regex pattern matching to detect namespace usage

## Example Output

```
Total Assemblies: 45
With Unused: 9
Total Unused: 9

VladislavTsurikov.BVH [1 unused]
  • OdinSerializer

VladislavTsurikov.MegaWorld [1 unused]
  • VladislavTrurikov.GameObjectCollider
```
