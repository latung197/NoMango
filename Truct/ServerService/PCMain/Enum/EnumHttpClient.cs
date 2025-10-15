
namespace PCMain.BaseHttp.Interface
{
    public enum HttpClientMethod
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        HEAD = 4,
        OPTIONS = 5,
        PATCH = 6,
        MERGE = 7,
        COPY = 8
    }

    public enum AccessTokenType
    {
        None = 0,
        Bearer = 1,
        Basic = 2
    }
}
