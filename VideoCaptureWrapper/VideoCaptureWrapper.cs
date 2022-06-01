namespace Sunameri;

using System.Diagnostics;
using Microsoft.ClearScript;
using OpenCvSharp;
using NLog;

public class VideoCaptureWrapper : IDisposable
{
    static Logger _logger = LogManager.GetCurrentClassLogger();

    Mat _mat = new Mat();
    Size _size;
    Size _sizeToShow;
    Task _task;
    CancellationTokenSource _cancellationTokenSource;
    CancellationToken _cancellationToken;

    public VideoCaptureWrapper(ScriptObject config) : this((int)config.GetProperty("index"), (int)config.GetProperty("width"), (int)config.GetProperty("height"), (bool)config.GetProperty("visible")) { }
    public VideoCaptureWrapper(int index, int width, int height, bool visible)
    {
        _size = new Size(width, height);

        // 表示サイズのデフォルト値は縦480
        const int heightToShow = 480;
        int widthToShow = (int)((1.0 * heightToShow / height) * width);
        setSizeToShow(widthToShow, heightToShow);

        // 接続からtimeoutミリ秒で初回Matを取得できなかった場合throw
        var ready = false;
        var timeout = 5000;
        var stopwatch = new Stopwatch();

        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _task = Task.WhenAll
        (
            Task.Run(() =>
            {
                // _matを更新するTask
                using (var videoCapture = new VideoCapture(index)
                {
                    FrameWidth = width,
                    FrameHeight = height
                })
                {
                    stopwatch.Start();
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        lock (_mat)
                            videoCapture.Read(_mat);

                        if (!_mat.Empty() && !ready) ready = true;
                    }
                }
            }, _cancellationToken),
            Task.Run(() =>
            {
                // visible = trueの場合_matを表示するTask
                if (!visible) return;
                while (!ready) Thread.Sleep(1);

                using (var window = new Window())
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            using (var raw = getFrame())
                            using (var resized = raw.Resize(_sizeToShow))
                                window.ShowImage(resized);
                        }
                        catch (Exception e) { _logger.Error(e); }

                        if (Cv2.WaitKey(1) == (int)'s')
                            using (var mat = getFrame())
                                mat.SaveImage(DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
                    }
            }, _cancellationToken)
        );
        while (!ready && stopwatch.ElapsedMilliseconds < timeout) Thread.Sleep(1);
        if (!ready) throw new Exception("VideoCapture seems not to open.");
    }

    public Mat getFrame()
    {
        try
        {
            return _mat.Clone();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            return new Mat(_size, MatType.CV_8UC3);
        }
    }

    public void setSizeToShow(int width, int height)
    {
        _sizeToShow = new Size(width, height);
    }

    #region IDisposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _cancellationTokenSource.Cancel();
                _task.Wait();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Hoge()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
