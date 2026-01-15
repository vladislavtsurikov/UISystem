# Code Commenting Rules

## Comments in Code

### Forbidden

Do not write comments in code. Code should be self-documenting.

**Bad:**
```csharp
// Check if user is active
if (user.IsActive && user.LastLoginDate > DateTime.Now.AddDays(-30))
{
    // Send notification
    SendNotification(user);
}
```

**Good:**
```csharp
if (IsUserActiveRecently(user))
{
    SendNotification(user);
}

private bool IsUserActiveRecently(User user)
{
    var thirtyDaysAgo = DateTime.Now.AddDays(-30);
    return user.IsActive && user.LastLoginDate > thirtyDaysAgo;
}
```

### Exceptions

Comments **are allowed** in the following cases:

1. **API classes** - classes that serve as a public API for working with other modules or external clients. Here comments serve as documentation.

2. **Workarounds and hacks** - when a solution is temporary or non-standard, and this needs to be explicitly indicated for future developers.

```csharp
// HACK: Unity doesn't call OnValidate when changing via SerializedProperty,
// so we force update the state here
ForceUpdateState();
```

## Code Structure

### Method Size

Methods should not exceed **200 lines of code**. If a method grows larger:
- Split it into several private methods
- Each method should perform one task
- Method names should describe what they do

### God Classes

Avoid creating classes with multiple responsibilities (God Classes).

**Signs of a God Class:**
- Class has too many fields
- Class has too many unrelated methods
- Class knows too much about other parts of the system
- Difficult to describe what the class does in one sentence

**Solution:**
- Apply the Single Responsibility Principle (SRP)
- Split the class into several classes, each with its own responsibility
- Use composition over inheritance

## Self-Documenting Code

### Naming

- Use clear names for variables, methods, and classes
- Names should answer "what is this?" or "what does this do?"
- Avoid abbreviations (except commonly accepted ones: id, url, etc.)

### Structure

- Group related code together
- Use regions only in extreme cases
- Order of class members: fields, properties, constructors, methods
