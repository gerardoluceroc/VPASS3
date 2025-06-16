namespace VPASS3_backend.Enums
{
    [Flags]
    public enum CommonAreaMode
    {
        None = 0,
        Usable = 1 << 0,
        Reservable = 1 << 1,
    }

}
