using Microsoft.ClearScript;
using OpenCvSharp;
using OpenCvSharp.Text;

public static class MatExtension
{
    /// <summary>
    /// Matの一部を切り取る。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Mat Clone(this Mat mat, ScriptObject rect)
    {
        var propertyNames = rect.PropertyNames;
        if (!propertyNames.Contains("x") || !propertyNames.Contains("y") || !propertyNames.Contains("width") || !propertyNames.Contains("height"))
            throw new Exception("Object must contain the properties x, y, width and height.");

        var x      = (int)rect.GetProperty("x");
        var y      = (int)rect.GetProperty("y");
        var width  = (int)rect.GetProperty("width");
        var height = (int)rect.GetProperty("height");

        return mat.Clone(new Rect(x, y, width, height));
    }

    /// <summary>
    /// Matを縦横の比で変形させる。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public static Mat Resize(this Mat mat, ScriptObject ratio)
    {
        var propertyNames = ratio.PropertyNames;
        if (!propertyNames.Contains("fx") || !propertyNames.Contains("fy"))
            throw new Exception("Object must contain the properties fx and fy.");
        
        var fx = (double)ratio.GetProperty("fx");
        var fy = (double)ratio.GetProperty("fy");

        return mat.Resize(new Size(), fx, fy);
    }

    /// <summary>
    /// 大きい方の画像に小さい方の画像が含まれているか調べる。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="source"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static bool Contains(this Mat mat, Mat source, double threshold = 0.5)
    {
        double result;
        if (mat.Width >= source.Width && mat.Height >= source.Height)
            // matの各辺の長さがそれぞれsource以上の場合
            result = MatchTemplate(mat, source);
        else if (mat.Width <= source.Width && mat.Height <= source.Height)
            // sourceの各辺の長さがそれぞれmat以上の場合
            result = MatchTemplate(source, mat);
        else
            // 一方をもう一方に収めることができない場合
            throw new Exception("It doesn't fit either.");
        
        return result >= threshold;

        double MatchTemplate(Mat larger, Mat smaller)
        {
            using (var result = larger.MatchTemplate(smaller, TemplateMatchModes.CCoeffNormed))
            {
                result.MinMaxLoc(out double minVal, out double maxVal);
                return maxVal;
            }
        }
    }

    /// <summary>
    /// Streamに変換する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="fileName">生成した中間ファイル名<br/>必要に応じて削除する</param>
    /// <returns></returns>
    public static Stream ToStream(this Mat mat, out string fileName)
    {
        var tmpPath = Path.GetTempFileName();
        fileName = Path.Join(Path.GetDirectoryName(tmpPath), Path.GetFileNameWithoutExtension(tmpPath) + ".png");
        mat.SaveImage(fileName);

        return File.OpenRead(fileName);
    }

    /// <summary>
    /// OCR結果を取得する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="tessConfig">datapath, language, charWhitelist, oem, psmodeを設定可能</param>
    /// <returns></returns>
    public static string GetOCRResult(this Mat mat, ScriptObject tessConfig)
    {
        var propertyNames = tessConfig.PropertyNames;

        var datapath      = propertyNames.Contains("datapath")      ? (string)tessConfig.GetProperty("datapath")      : null;
        var language      = propertyNames.Contains("language")      ? (string)tessConfig.GetProperty("language")      : null;
        var charWhitelist = propertyNames.Contains("charWhitelist") ? (string)tessConfig.GetProperty("charWhitelist") : null;
        var oem           = propertyNames.Contains("oem")           ? (int)tessConfig.GetProperty("oem")              : 3;
        var psmode        = propertyNames.Contains("psmode")        ? (int)tessConfig.GetProperty("psmode")           : 3;
        
        using (var tesseract = OCRTesseract.Create(datapath, language, charWhitelist, oem, psmode))
        {
            tesseract.Run(mat, out var outputText, out var componentRects, out var componentTexts, out var componentConfidences);
            return outputText;
        }
    }
}
