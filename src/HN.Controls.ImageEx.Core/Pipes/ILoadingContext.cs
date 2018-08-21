namespace HN.Pipes
{
    public interface ILoadingContext<TResult> where TResult : class
    {
        object Current { get; set; }

        double? DesiredHeight { get; }

        double? DesiredWidth { get; }

        byte[] HttpResponseBytes { get; set; }

        object OriginSource { get; }

        TResult Result { get; set; }
    }
}