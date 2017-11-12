namespace EHR.ServerEvent.Subscriber.Contract
{
    public interface ITransformer<in TSrc, out TDst>
    {
        TDst Transform(TSrc src);
    }
}
