# UnityLogger

Lightweight, color-coded, level-based logger for Unity projects — drop-in replacement for Debug.Log with production toggles, rich-text colours, and ScriptableObject configuration.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage](#usage)
- [Log Levels](#log-levels)
- [Color Configuration](#color-configuration)
- [Editor Tools](#editor-tools)
- [Performance](#performance)
- [API Reference](#api-reference)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

UnityLogger is a high-performance logging system designed specifically for Unity projects. It provides a drop-in replacement for `UnityEngine.Debug` with enhanced features like:

- **Runtime log level filtering** - Control what gets logged at runtime
- **Per-class color coding** - Each class can have its own color in the console
- **Zero runtime overhead in builds** - Logging calls are stripped from production builds
- **Lazy evaluation support** - Avoid string allocations when logs are filtered out
- **Rich editor integration** - Visual tools for configuring colors and settings

## ✨ Features

### Core Features
- **Drop-in Debug replacement** - Use `using Debug = GameLogger;` for seamless integration
- **Runtime log level control** - Change logging verbosity without rebuilding
- **Class-based color coding** - Each class gets its own color in the Unity Console
- **Production-safe** - Zero overhead in player builds
- **Lazy evaluation** - Avoid allocations when logs are filtered out

### Editor Features
- **Visual color configuration** - Easy-to-use editor window for setting class colors
- **Class picker** - Browse and select classes to assign colors
- **Search functionality** - Quickly find classes in large projects
- **ScriptableObject settings** - Persistent configuration that survives project restarts

### Performance Features
- **Aggressive inlining** - Minimal call stack overhead
- **Build-time stripping** - Logging calls removed from production builds
- **Efficient filtering** - Early exit when logs are disabled
- **Memory efficient** - No allocations when logs are filtered

## 📦 Installation

1. **Download the asset** from the Unity Asset Store or import the package
2. **Import into your project** - Unity will automatically import all necessary files
3. **Verify installation** - Check that the following folders exist:
   - `Assets/Runtime/` - Contains the main logging system
   - `Assets/Editor/` - Contains editor tools and configuration

## 🚀 Quick Start

### Basic Usage

```csharp
using Debug = GameLogger; // Drop-in replacement

public class PlayerController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Player initialized"); // Colored by class name
        Debug.LogWarning("Health is low");
        Debug.LogError("Player died");
    }
}
```

### Setting Log Levels

```csharp
// Control what gets logged at runtime
GameLogger.SetLogLevel(LogLevel.Warning); // Only warnings and errors
GameLogger.SetLogLevel(LogLevel.All);     // Log everything
GameLogger.SetLogLevel(LogLevel.None);    // Disable all logging
```

### Lazy Evaluation

```csharp
// Avoid string allocation when logs are filtered out
Debug.Log(() => $"Expensive calculation: {CalculateComplexValue()}");
```

## 📖 Usage

### Basic Logging

UnityLogger provides the same API as `UnityEngine.Debug`:

```csharp
using Debug = GameLogger;

// Standard logging
Debug.Log("Information message");
Debug.LogWarning("Warning message");
Debug.LogError("Error message");
Debug.LogException(exception);

// With context object
Debug.Log("Message", gameObject);
Debug.LogWarning("Warning", this);
Debug.LogError("Error", transform);
```

### Log Level Control

```csharp
// Set the current log level
GameLogger.SetLogLevel(LogLevel.Information);

// Check current level
if (GameLogger.CurrentLevel >= LogLevel.Warning)
{
    Debug.LogWarning("This will be logged");
}
```

## 📊 Log Levels

UnityLogger supports multiple log levels that can be controlled at runtime:

| Level | Description | Example Use |
|-------|-------------|-------------|
| `All` | Log everything | Development and debugging |
| `Information` | Info and above | Normal development |
| `Warning` | Warnings and errors | Testing and validation |
| `Error` | Errors only | Production monitoring |
| `Exception` | Exceptions only | Critical error tracking |
| `None` | No logging | Performance-critical scenarios |

### Level Hierarchy

```
All > Information > Warning > Error > Exception > None
```

When you set a level, only that level and above will be logged.

## 🎨 Color Configuration

### Setting Up Colors

1. **Open the Color Configurator**:
   - Go to `Tools > Logging > Log Colour Configurator`
   - Or use the menu item in the Unity Editor

2. **Add a Class**:
   - Click "Add Class..." button
   - Select the class you want to color
   - Choose a color for that class

3. **Customize Colors**:
   - Use the color picker to set the desired color
   - Remove entries with the "–" button
   - Search for classes using the search field

### Color Best Practices

- **Use distinct colors** for different systems (Player, UI, Network, etc.)
- **Avoid similar colors** that might be hard to distinguish
- **Use bright colors** for important systems
- **Use darker colors** for background/utility systems

### Example Color Scheme

```csharp
// Player-related classes - Blue
PlayerController: #4A90E2
PlayerHealth: #4A90E2
PlayerMovement: #4A90E2

// UI-related classes - Green
UIManager: #7ED321
HUDController: #7ED321
MenuSystem: #7ED321

// Network-related classes - Orange
NetworkManager: #F5A623
NetworkPlayer: #F5A623
NetworkSync: #F5A623

// System classes - Gray
GameManager: #9B9B9B
AudioManager: #9B9B9B
```

## 🛠️ Editor Tools

### Log Colour Configurator

**Location**: `Tools > Logging > Log Colour Configurator`

**Features**:
- Add/remove class color mappings
- Live search through classes
- Color picker for each class
- Undo/redo support
- Automatic settings persistence

### Class Picker

**Features**:
- Browse all project classes
- Filter by search term
- Excludes already configured classes
- Alphabetical sorting

## ⚡ Performance

### Runtime Performance

- **Zero overhead in builds** - All logging calls are stripped from production builds
- **Efficient filtering** - Early exit when logs are disabled
- **Minimal call stack** - Uses `[HideInCallstack]` to keep Unity's stack trace clean
- **Aggressive inlining** - Reduces method call overhead

### Memory Performance

- **Lazy evaluation** - Avoid string allocations when logs are filtered
- **No allocations** when logging is disabled
- **Efficient color lookup** - Cached HTML color values

### Build Performance

- **Conditional compilation** - Editor-only code is stripped from builds
- **Build-time optimization** - Logging infrastructure removed in production

## 📚 API Reference

### GameLogger Class

#### Properties
```csharp
public static LogLevel CurrentLevel { get; private set; }
```

#### Methods
```csharp
// Log level control
public static void SetLogLevel(LogLevel level)

// Standard logging
public static void Log(object message)
public static void Log(object message, UnityEngine.Object context)
public static void LogWarning(object message)
public static void LogWarning(object message, UnityEngine.Object context)
public static void LogError(object message)
public static void LogError(object message, UnityEngine.Object context)
public static void LogException(Exception exception, UnityEngine.Object context = null)

// Lazy evaluation
public static void Log(Func<string> supplier)
public static void LogWarning(Func<string> supplier)
public static void LogError(Func<string> supplier)
```

### LogLevel Enum

```csharp
public enum LogLevel : int
{
    All,           // Log everything
    Information,   // Info and above
    Warning,       // Warnings and errors
    Error,         // Errors only
    Exception,     // Exceptions only
    None           // No logging
}
```

## 💡 Examples

### Basic Game System

```csharp
using Debug = GameLogger;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Game starting...");
        GameLogger.SetLogLevel(LogLevel.Information);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
        }
    }
}
```

### Performance-Critical Code

```csharp
public class PerformanceSystem : MonoBehaviour
{
    void Update()
    {
        // Use lazy evaluation to avoid allocations
        Debug.Log(() => $"Frame time: {Time.deltaTime:F3}s");
        
        // Or check level before expensive operations
        if (GameLogger.CurrentLevel >= LogLevel.Warning)
        {
            Debug.LogWarning("Performance warning");
        }
    }
}
```

### Runtime Configuration

```csharp
public class LoggingController : MonoBehaviour
{
    void Start()
    {
        // Set initial log level based on build type
        #if UNITY_EDITOR
            GameLogger.SetLogLevel(LogLevel.All);
        #else
            GameLogger.SetLogLevel(LogLevel.Warning);
        #endif
    }

    // Allow runtime configuration
    public void SetLoggingLevel(int level)
    {
        GameLogger.SetLogLevel((LogLevel)level);
    }
}
```

## 🔧 Troubleshooting

### Common Issues

**Q: Logs aren't appearing in the console**
A: Check the current log level with `GameLogger.CurrentLevel`. Make sure it's set to a level that includes your log type.

**Q: Colors aren't showing up**
A: Colors only work in the Unity Editor. In builds, logs will appear without colors but with class name prefixes.

**Q: Performance issues**
A: Use lazy evaluation (`Debug.Log(() => "message")`) for expensive string operations, or check the log level before expensive operations.

**Q: Settings not saving**
A: Make sure the LogColorSettings asset is in the correct location: `Assets/Editor/Resources/Settings/GameLogger/LogColorSettings.asset`

### Debug Tips

1. **Check log levels**: Use `Debug.Log($"Current level: {GameLogger.CurrentLevel}")`
2. **Verify class names**: Make sure your class names match exactly in the color settings
3. **Test in editor**: Colors only work in the Unity Editor, not in builds
4. **Use lazy evaluation**: For expensive logging, use the `Func<string>` overloads

### Build Considerations

- Logging calls are stripped from production builds for performance
- Colors are only available in the Unity Editor
- Class name prefixes are preserved in builds

## 📄 License

This asset is provided as-is for use in Unity projects. Please refer to the license terms included with the asset package.

## 🤝 Support

For issues, questions, or feature requests:
- Check the troubleshooting section above
- Review the examples for common use cases
- Ensure you're using the latest version of the asset

---

**UnityLogger** - Making Unity development more colorful and efficient! 🎨✨