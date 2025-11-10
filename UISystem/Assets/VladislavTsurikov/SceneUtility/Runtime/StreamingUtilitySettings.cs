using System;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.SceneUtility.Runtime
{
    public enum CacheMemoryThreshold
    {
        Off,
        _1GB,
        _2GB,
        _3GB,
        _4GB,
        _5GB,
        _6GB,
        _7GB,
        _8GB,
        Custom
    }

    [LocationAsset("StreamingUtility/StreamingUtilitySettings")]
    public class StreamingUtilitySettings : SerializedScriptableObjectSingleton<StreamingUtilitySettings>
    {
        public CacheMemoryThreshold CacheMemoryThreshold = CacheMemoryThreshold._4GB;
        public float CustomCacheMemoryThreshold = 4000;

        public long GetCacheMemoryThresholdInBytes()
        {
            if (CacheMemoryThreshold == CacheMemoryThreshold.Custom)
            {
                return (long)((int)CustomCacheMemoryThreshold * Math.Pow(1024, 2));
            }

            return (long)((int)CacheMemoryThreshold * Math.Pow(1024, 3));
        }
    }
}
