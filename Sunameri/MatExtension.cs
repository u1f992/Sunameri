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

        return mat.Clone(new Rect((int)rect.GetProperty("x"), (int)rect.GetProperty("y"), (int)rect.GetProperty("width"), (int)rect.GetProperty("height")));
    }
    /// <summary>
    /// ファイルに保存する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool save(this Mat mat, string fileName)
    {
        try
        {
            var path = Path.Join(AppContext.BaseDirectory, fileName);
            if (!mat.SaveImage(fileName))
                throw new Exception(string.Format("{0} was not saved.", fileName));
            return true;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return false;
        }
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
    /// 入力画像との類似度を算出する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="fileName"></param>
    /// <returns>0-1の範囲</returns>
    public static double getSimilarity(this Mat mat, Mat template)
    {
        // 大きい方の画像の中を小さい方でテンプレートマッチして、最も高いところの類似度を返す
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

    public static string getOCRResult(this Mat mat)
    {
        throw new NotImplementedException();
    }
}
