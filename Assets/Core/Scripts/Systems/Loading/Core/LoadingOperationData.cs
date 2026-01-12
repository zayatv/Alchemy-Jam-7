namespace Core.Systems.Loading
{
    public class LoadingOperationData
    {
        public LoadingPhase Phase { get; set; }
        public float Progress { get; set; }
        public string Description { get; set; }
        public ILoadingScreen LoadingScreen { get; set; }
    }
    
    public enum LoadingPhase
    {
        Idle,
        Loading
    }
}