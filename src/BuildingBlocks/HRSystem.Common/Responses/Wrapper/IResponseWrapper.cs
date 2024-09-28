namespace HRSystem.Common.Responses.Wrapper
{
    public interface IResponseWrapper
    {
        public List<string> Messages { get; set; }
        public bool IsSuccessful{ get; set; }
    }

    public interface IResponseWrapper<out T> : IResponseWrapper
    {
        T ResponseData { get;}
    }
}
