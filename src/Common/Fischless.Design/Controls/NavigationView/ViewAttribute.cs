namespace Fischless.Design.Controls;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ViewAttribute(Type type) : Attribute
{
    public Type ViewType { get; init; } = type;
}
