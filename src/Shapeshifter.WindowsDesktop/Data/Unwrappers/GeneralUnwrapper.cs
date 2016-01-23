﻿namespace Shapeshifter.WindowsDesktop.Data.Unwrappers
{
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    using Native;
    using Native.Interfaces;

    class GeneralUnwrapper: IMemoryUnwrapper
    {
        readonly IClipboardNativeApi clipboardNativeApi;

        readonly IEnumerable<int> excludedFormats;

        public GeneralUnwrapper(
            IClipboardNativeApi clipboardNativeApi)
        {
            this.clipboardNativeApi = clipboardNativeApi;
            excludedFormats = new[]
            {
                ClipboardNativeApi.CF_DSPBITMAP,
                ClipboardNativeApi.CF_DSPENHMETAFILE,
                ClipboardNativeApi.CF_ENHMETAFILE,
                ClipboardNativeApi.CF_METAFILEPICT,
                ClipboardNativeApi.CF_BITMAP
            };
        }

        public bool CanUnwrap(uint format)
        {
            return excludedFormats.All(x => x != format);
        }

        public byte[] UnwrapStructure(uint format)
        {
            return clipboardNativeApi.GetClipboardDataBytes(format);
        }
    }
}