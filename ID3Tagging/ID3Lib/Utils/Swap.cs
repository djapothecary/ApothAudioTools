
namespace ID3Tagging.ID3Lib.Utils
{
    /// <summary>
    /// Performs byte swapping.
    /// </summary>
    internal static class Swap
    {
        #region Methods

        /// <summary>
        /// The int 32.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Int32(int val)
        {
            return (int)UInt32((uint)val);
        }

        /// <summary>
        /// The u int 32.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint UInt32(uint val)
        {
            uint retval = (val & 0xff) << 24;
            retval |= (val & 0xff00) << 8;
            retval |= (val & 0xff0000) >> 8;
            retval |= (val & 0xff000000) >> 24;
            return retval;
        }

        /// <summary>
        /// The int 16.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        public static short Int16(short val)
        {
            return (short)UInt16((ushort)val);
        }

        /// <summary>
        /// The u int 16.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="ushort"/>.
        /// </returns>
        public static ushort UInt16(ushort val)
        {
            uint retval = ((uint)val & 0xff) << 8;
            retval |= ((uint)val & 0xff00) >> 8;
            return (ushort)retval;
        }

        #endregion
    }
}