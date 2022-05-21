public class Timer
{
    /// <summary>
    /// 指定された時間待機する。
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    public void sleep(int millisecondsTimeout)
    {
        start(millisecondsTimeout);
        wait();
    }

    private bool _running = false;
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
        start();
        stop();
    }
    
    /// <summary>
    /// タイマーを開始する
    /// </summary>
    /// <returns></returns>
    public void start()
    {
        start(long.MaxValue);
    }
    /// <summary>
    /// タイマーを開始する
    /// </summary>
    /// <returns></returns>
    public void start(long milliseconds)
    {
        _running = true;
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

            _running = false;
        });
    }

    /// <summary>
    /// タイマーの終了時間を設定する
    /// </summary>
    /// <param name="milliseconds"></param>
    public void submit(long milliseconds)
    {
        if (!_running) return;
        _submittedMilliseconds = milliseconds;
    }

    public void wait()
    {
        if (!_running) return;
        _task?.Wait();
    }

    /// <summary>
    /// タイマーを強制停止しリセットする
    /// </summary>
    public void stop()
    {
        if (!_running) return;

        _elapsedMilliseconds = long.MaxValue;
        _task?.Wait();

        _task = null;
        _elapsedMilliseconds = 0;
        _submittedMilliseconds = long.MaxValue;
    }
}
