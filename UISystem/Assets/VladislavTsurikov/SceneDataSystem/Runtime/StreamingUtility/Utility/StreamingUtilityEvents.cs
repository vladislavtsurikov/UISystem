namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    public static class StreamingUtilityEvents
    {
        public delegate void CreateSceneAfter();

        public delegate void DeleteAllAdditiveScenesBefore();

        public static DeleteAllAdditiveScenesBefore BeforeDeleteAllAdditiveScenesEvent;
        public static CreateSceneAfter CreateSceneAfterEvent;
    }
}
