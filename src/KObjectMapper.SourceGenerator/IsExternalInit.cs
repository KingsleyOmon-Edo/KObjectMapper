// Polyfill required for C# record types targeting netstandard2.0
using System.ComponentModel;

namespace System.Runtime.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit { }
