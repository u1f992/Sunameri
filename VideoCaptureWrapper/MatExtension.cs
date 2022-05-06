using Microsoft.ClearScript;
using OpenCvSharp;

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
        try
        {
            using (var template = new Mat(fileName))
                return mat.getSimilarity(template);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 0;
        }
    }
    /// <summary>
    /// 大きい方の画像の中を小さい方でテンプレートマッチして、最も高いところの類似度を返す。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="template"></param>
    /// <returns>0-1の範囲</returns>
    public static double getSimilarity(this Mat mat, Mat template)
    {
        try
        {
            if (mat.Width >= template.Width && mat.Height >= template.Height)
                return MatchTemplate(mat, template);
            else if (mat.Width <= template.Width && mat.Height <= template.Height)
                return MatchTemplate(template, mat);
            else
                throw new Exception("It doesn't fit either.");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 0;
        }
        
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
    /// <param name="fileName">中間ファイル名<br/>必要に応じて削除する</param>
    /// <returns></returns>
    public static Stream toStream(this Mat mat, out string fileName)
    {
        var tmpPath = Path.GetTempFileName();
        fileName = Path.Join(Path.GetDirectoryName(tmpPath), Path.GetFileNameWithoutExtension(tmpPath) + ".png");
        mat.SaveImage(fileName);

        return File.OpenRead(fileName);
    }

    public static string getOCRResult(this Mat mat)
    {
        throw new NotImplementedException();
    }
}
