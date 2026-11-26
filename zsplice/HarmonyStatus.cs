namespace zsplice
{
    public enum HarmonyStatus : int
    {
        HARMONY_OK = 0,

        HARMONY_TYPE_NOT_FOUND,
        HARMONY_METHOD_NOT_FOUND,
        HARMONY_AMBIGUOUS_METHOD,
        HARMONY_INVALID_ARGUMENT,

        HARMONY_PATCH_FAILED,
        HARMONY_INTERNAL_ERROR
    }
}
