﻿namespace Shapeshifter.WindowsDesktop.Controls.Clipboard.Designer.Facades
{
    using Data.Interfaces;

    using Services.Interfaces;

    class DesignerDataSourceFacade: IDataSource
    {
        byte[] icon;

        readonly IDesignerImageConverterService designerImageConverterService;

        public DesignerDataSourceFacade(
            IDesignerImageConverterService designerImageConverterService)
        {
            this.designerImageConverterService = designerImageConverterService;
        }

        byte[] DecorateIcon(byte[] iconBytes)
        {
            return designerImageConverterService.GenerateDesignerImageBytesFromFileBytes(iconBytes);
        }

        public byte[] Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = DecorateIcon(value);
            }
        }

        public string Text { get; set; }
    }
}