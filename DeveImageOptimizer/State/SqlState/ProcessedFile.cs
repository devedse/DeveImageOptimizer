using System.Collections.Generic;

namespace DeveImageOptimizer.State.SqlState
{
    public class ProcessedFile
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string FileNames { get; set; }
    }
}
