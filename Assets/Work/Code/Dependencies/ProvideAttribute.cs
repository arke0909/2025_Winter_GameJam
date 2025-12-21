using System;

namespace Lib.Dependencies
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ProvideAttribute : Attribute
    {
    }
}