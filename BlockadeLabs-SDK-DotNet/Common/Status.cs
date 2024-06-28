using System.Runtime.Serialization;

namespace BlockadeLabsSDK
{
    public enum Status
    {
        All = 1,
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "dispatched")]
        Dispatched,
        [EnumMember(Value = "processing")]
        Processing,
        [EnumMember(Value = "complete")]
        Complete,
        [EnumMember(Value = "abort")]
        Abort,
        [EnumMember(Value = "error")]
        Error
    }
}
