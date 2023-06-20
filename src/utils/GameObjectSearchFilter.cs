using UnityEngine;

namespace BetterBases
{
    public enum SearchResult
    {
        CONTINUE, SKIP_CHILDREN, STOP, INCLUDE_CONTINUE, INCLUDE_SKIP_CHILDREN, INCLUDE_STOP
    }

    public static class SearchResultMethods
    {
        public static bool IsContinue(this SearchResult searchResult)
        {
            return searchResult == SearchResult.CONTINUE || searchResult == SearchResult.INCLUDE_CONTINUE;
        }

        public static bool IsInclude(this SearchResult searchResult)
        {
            return searchResult == SearchResult.INCLUDE_CONTINUE || searchResult == SearchResult.INCLUDE_SKIP_CHILDREN || searchResult == SearchResult.INCLUDE_STOP;
        }

        public static bool IsStop(this SearchResult searchResult)
        {
            return searchResult == SearchResult.STOP || searchResult == SearchResult.INCLUDE_STOP;
        }
    }

    public abstract class GameObjectSearchFilter
    {
        public abstract SearchResult Filter(GameObject gameObject);
    }
}