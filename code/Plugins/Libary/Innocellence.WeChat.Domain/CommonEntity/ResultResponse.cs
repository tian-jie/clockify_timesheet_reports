namespace Innocellence.WeChat.Domain.Contracts.CommonEntity
{
    public class ResultResponse<TEntity, TStatus>
    {
        public TEntity Entity { get; set; }

        public TStatus Status { get; set; }
    }
}
