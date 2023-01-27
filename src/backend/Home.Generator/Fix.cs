// Workaround to use C# 9.0 init while targeting netstandard2.0
// https://stackoverflow.com/questions/62648189/

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices {

    public class IsExternalInit { }

}