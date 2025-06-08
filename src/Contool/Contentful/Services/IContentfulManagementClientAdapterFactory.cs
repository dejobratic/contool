namespace Contool.Contentful.Services;

internal interface IContentfulManagementClientAdapterFactory
{
    IContentfulManagementClientAdapter Create(string spaceId, string environmentId, bool usePreviewApi);
}