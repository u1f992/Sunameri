public class Timer
{
    /// <summary>
    /// 指定された時間待機する。
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    public void sleep(int millisecondsTimeout)
    {
        var task = start(millisecondsTimeout);
        task.Wait();
    }

    private Task? _task;
    private long _elapsedMilliseconds = 0;
    private long _submittedMilliseconds = long.MaxValue;

    /// <summary>
    /// 開始後に終了時間を設定することができるタイマー
    /// </summary>
    public Timer()
    {
        // 初回のみ遅延があるため、コンストラクタで捨てておく
        // メモリに展開する際の何らかな気がする
        var task = start();
        stop();
    }
    
    /// <summary>
    /// タイマーを開始する
    /// </summary>
    /// <returns></returns>
    public async Task start()
    {
        await start(long.MaxValue);
    }
    /// <summary>
    /// タイマーを開始する
    /// </summary>
    /// <returns></returns>
    public async Task start(long milliseconds)
    {
        submit(milliseconds);

        _elapsedMilliseconds = 0;

        _task = Task.Run(() =>
        {
            var interval = 10000000 / 1000;
            var next = DateTime.Now.Ticks + interval;

            while (_elapsedMilliseconds < _submittedMilliseconds)
            {
                if (next <= DateTime.Now.Ticks)
                {
                    _elapsedMilliseconds++;
                    next += interval;
                }
            }
        });
        await _task.ConfigureAwait(false);
    }

    /// <summary>
    /// タイマーの終了時間を設定する
    /// </summary>
    /// <param name="milliseconds"></param>
    public void submit(long milliseconds)
    {
        _submittedMilliseconds = milliseconds;
    }

    /// <summary>
    /// タイマーを強制停止しリセットする
    /// </summary>
    public void stop()
    {
        _elapsedMilliseconds = long.MaxValue;
        _task?.Wait();

        _task = null;
        _elapsedMilliseconds = 0;
        _submittedMilliseconds = long.MaxValue;
    }
}
