namespace jfYu.Core.Common.SnowFlake
{
    public interface ISnowFlake
    {
        long Sequence { get; }

        long NextId();
    }
}