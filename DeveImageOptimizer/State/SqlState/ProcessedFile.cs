namespace DeveImageOptimizer.State.SqlState
{
    public class ProcessedFile
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public int ImageOptimizationLevel { get; set; }
        public string FileNames { get; set; }
    }
}
