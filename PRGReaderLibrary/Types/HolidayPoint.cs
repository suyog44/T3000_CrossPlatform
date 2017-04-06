namespace PRGReaderLibrary
{
    using System;
    using System.Collections.Generic;

    public class HolidayPoint : BasePoint, IBinaryObject
    {
        public Control Control { get; set; }
        public AutoManual AutoManual { get; set; }
        public int Unused { get; set; }

        public HolidayPoint(string description = "", string label = "",
            FileVersion version = FileVersion.Current)
            : base(description, label, version)
        { }

        #region Binary data

        public static int GetCount(FileVersion version = FileVersion.Current)
        {
            switch (version)
            {
                case FileVersion.Current:
                    return 4;

                default:
                    throw new FileVersionNotImplementedException(version);
            }
        }

        public static int GetSize(FileVersion version = FileVersion.Current)
        {
            switch (version)
            {
                case FileVersion.Current:
                    return 33;

                case FileVersion.Dos:
                    return 46;

                default:
                    throw new FileVersionNotImplementedException(version);
            }
        }

        /// <summary>
        /// FileVersion.Current - need 33 bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="version"></param>
        public HolidayPoint(byte[] bytes, int offset = 0,
            FileVersion version = FileVersion.Current)
            : base(bytes, offset, version)
        {
            switch (FileVersion)
            {
                case FileVersion.Current:
                    Control = (Control)bytes.ToByte(30 + offset);
                    AutoManual = (AutoManual)bytes.ToByte(31 + offset);
                    Unused = bytes.ToByte(32 + offset);
                    break;

                default:
                    throw new FileVersionNotImplementedException(FileVersion);
            }
        }

        /// <summary>
        /// FileVersion.Current - 33 bytes
        /// </summary>
        /// <returns></returns>
        public new byte[] ToBytes()
        {
            var bytes = new List<byte>();

            switch (FileVersion)
            {
                case FileVersion.Current:
                    bytes.AddRange(base.ToBytes());
                    bytes.Add((byte)Control);
                    bytes.Add((byte)AutoManual);
                    bytes.Add((byte)Unused);
                    break;

                default:
                    throw new FileVersionNotImplementedException(FileVersion);
            }

            return bytes.ToArray();
        }

        #endregion
    }
}