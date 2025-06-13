using Contentful.Core.Models.Management;

namespace Contool.Contentful.Extensions;

internal static class ContentTypeFieldValidatorExtensions
{
    public static IFieldValidator? CloneSafely(this IFieldValidator validator)
    {
        if (!IsSafeValidator(validator))
            return null;

        // Special handling for NodesValidator → perform safe clone if needed
        if (validator is NodesValidator nodesValidator)
            return SafeClone(nodesValidator);

        // Default → safe validator → return as is
        return validator;
    }

    private static bool IsSafeValidator(IFieldValidator validator)
    {
        if (validator is null)
            return false;

        var type = validator.GetType();

        // Skip known problematic dynamic types
        if (type.FullName != null &&
            (type.FullName.Contains("JObject") || type.FullName.Contains("JsonObject")))
            return false;

        // Skip anonymous types (compiler generated)
        if (type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false))
            return false;

        // Skip non-public types
        if (!type.IsPublic)
            return false;

        // Special handling for NodesValidator → defer to CloneValidatorSafely which already does the deep check
        // So here we simply say it's safe → CloneValidatorSafely will do the deeper decision.

        return true;
    }

    private static NodesValidator? SafeClone(NodesValidator nodesValidator)
    {
        static bool IsValidSubList(IEnumerable<IFieldValidator> list)
            => list != null && list.Any(IsSafeValidator);

        // If all sublists are empty/null → skip NodesValidator (prevents "Expected validations to be an Array" error)
        if (!IsValidSubList(nodesValidator.EntryHyperlink)
            && !IsValidSubList(nodesValidator.EmbeddedEntryBlock)
            && !IsValidSubList(nodesValidator.EmbeddedEntryInline))
        {
            return null;
        }

        // Safe → clone NodesValidator
        return new NodesValidator
        {
            EntryHyperlink = nodesValidator.EntryHyperlink?.Where(IsSafeValidator).ToList(),
            EmbeddedEntryBlock = nodesValidator.EmbeddedEntryBlock?.Where(IsSafeValidator).ToList(),
            EmbeddedEntryInline = nodesValidator.EmbeddedEntryInline?.Where(IsSafeValidator).ToList()
        };
    }
}
