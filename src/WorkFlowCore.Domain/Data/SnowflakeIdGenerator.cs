namespace WorkFlowCore.Domain.Data;

/// <summary>
/// 雪花算法主键生成器
/// Twitter's Snowflake algorithm implementation
/// </summary>
public static class SnowflakeIdGenerator
{
    private static long _workerId = 1;
    private static long _datacenterId = 1;
    private static long _sequence = 0L;
    private static long _lastTimestamp = -1L;
    private static readonly object _lock = new();

    // 起始时间戳 (2020-01-01)
    private const long Twepoch = 1577808000000L;

    // 各部分占用位数
    private const int WorkerIdBits = 5;
    private const int DatacenterIdBits = 5;
    private const int SequenceBits = 12;

    // 最大值
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    // 位移量
    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

    /// <summary>
    /// 初始化雪花算法
    /// </summary>
    /// <param name="workerId">工作机器ID (0-31)</param>
    /// <param name="datacenterId">数据中心ID (0-31)</param>
    public static void Initialize(long workerId, long datacenterId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
        {
            throw new ArgumentException($"WorkerId 必须在 0 到 {MaxWorkerId} 之间");
        }

        if (datacenterId > MaxDatacenterId || datacenterId < 0)
        {
            throw new ArgumentException($"DatacenterId 必须在 0 到 {MaxDatacenterId} 之间");
        }

        _workerId = workerId;
        _datacenterId = datacenterId;
    }

    /// <summary>
    /// 生成下一个ID
    /// </summary>
    /// <returns></returns>
    public static long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();

            // 时钟回拨检测
            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException(
                    $"时钟回拨异常。拒绝生成 ID，当前时间戳 {timestamp} 小于上次时间戳 {_lastTimestamp}");
            }

            // 同一毫秒内
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    // 序列号用尽，等待下一毫秒
                    timestamp = WaitNextMillis(_lastTimestamp);
                }
            }
            else
            {
                // 不同毫秒，序列号重置
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            // 组装ID
            return ((timestamp - Twepoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }

    /// <summary>
    /// 获取当前时间戳（毫秒）
    /// </summary>
    private static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 等待下一毫秒
    /// </summary>
    private static long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}

