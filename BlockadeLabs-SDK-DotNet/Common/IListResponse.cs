using System.Collections.Generic;

namespace BlockadeLabsSDK
{
    public interface IListResponse<out TObject>
        where TObject : BaseResponse
    {
        IReadOnlyList<TObject> Items { get; }
    }
}
