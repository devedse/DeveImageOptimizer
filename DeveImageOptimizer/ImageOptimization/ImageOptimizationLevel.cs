namespace DeveImageOptimizer.ImageOptimization
{
    public enum ImageOptimizationLevel
    {
        //The fastest possible optimzation mode
        SuperFast = 0,

        //A fast optimization mode that tries to optimize quickly but still achieve quite good compression
        Fast = 1,

        //Average
        Normal = 2,

        //Basically the best mode that makes sense
        Maximum = 3,

        //This mode takes incredibly long (ECT) and sometimes achieves a few bits more compression then Maximum
        Placebo = 4
    }
}
