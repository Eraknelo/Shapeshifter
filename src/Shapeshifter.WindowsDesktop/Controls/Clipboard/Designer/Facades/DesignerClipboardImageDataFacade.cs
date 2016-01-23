﻿namespace Shapeshifter.WindowsDesktop.Controls.Clipboard.Designer.Facades
{
    using Data;

    using Properties;

    using Services;
    using Services.Interfaces;

    class DesignerClipboardImageDataFacade: ClipboardImageData
    {
        public DesignerClipboardImageDataFacade(
            IDesignerImageConverterService designerImageConverterService)
            :
                base(new DesignerFileDataSourceService(designerImageConverterService))
        {
            RawData =
                designerImageConverterService.GenerateDesignerImageBytesFromFileBytes(
                    Resources
                        .FileImageSample);
        }
    }
}