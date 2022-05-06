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
    public static Mat trim(this Mat mat, ScriptObject rect)
    {
        var propertyNames = rect.PropertyNames;
        if (!propertyNames.Contains("x") || !propertyNames.Contains("y") || !propertyNames.Contains("width") || !propertyNames.Contains("height"))
            throw new Exception("Object must contain the properties x, y, width and height.");

        var x = (int)rect.GetProperty("x");
        var y = (int)rect.GetProperty("y");
        var width = (int)rect.GetProperty("width");
        var height = (int)rect.GetProperty("height");

        return mat.Clone(new Rect(x, y, width, height));
    }

    /// <summary>
    /// 入力画像との類似度を算出する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="fileName"></param>
    /// <returns>0-1の範囲</returns>
    public static double getSimilarity(this Mat mat, string fileName)
    {
        using (var template = new Mat(fileName))
            return mat.getSimilarity(template);
    }
    /// <summary>
    /// 大きい方の画像の中を小さい方でテンプレートマッチして、類似度が最も高いところの値を返す。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="template"></param>
    /// <returns>0-1の範囲</returns>
    public static double getSimilarity(this Mat mat, Mat template)
    {
        if (mat.Width >= template.Width && mat.Height >= template.Height)
            return MatchTemplate(mat, template);
        else if (mat.Width <= template.Width && mat.Height <= template.Height)
            return MatchTemplate(template, mat);
        else
            throw new Exception("It doesn't fit either.");
        
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
    public static Stream toStream(this Mat mat, out string fileName)
    {
        var tmpPath = Path.GetTempFileName();
        fileName = Path.Join(Path.GetDirectoryName(tmpPath), Path.GetFileNameWithoutExtension(tmpPath) + ".png");
        mat.SaveImage(fileName);

        return File.OpenRead(fileName);
    }

    public static string getOCRResult(this Mat mat, ScriptObject tessConfig)
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
