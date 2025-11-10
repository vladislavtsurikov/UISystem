namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class LayerEx
    {
        public static bool IsLayerBitSet(int layerBits, int layerNumber) => (layerBits & (1 << layerNumber)) != 0;
    }
}
