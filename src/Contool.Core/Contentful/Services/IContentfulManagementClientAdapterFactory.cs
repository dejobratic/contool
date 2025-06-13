namespace Contool.Core.Contentful.Services;

public interface IContentfulManagementClientAdapterFactory
{
    IContentfulManagementClientAdapter Create(string spaceId, string environmentId, bool usePreviewApi);
}