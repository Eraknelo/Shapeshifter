﻿namespace Shapeshifter.WindowsDesktop.Data.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    using Data.Interfaces;

    using Infrastructure.Threading.Interfaces;

    using Interfaces;

    using Services.Clipboard.Interfaces;
    using Services.Images.Interfaces;
    using Services.Web;
    using Services.Web.Interfaces;

    class CopyImageLinkAction: ICopyImageLinkAction
    {
        readonly IClipboardInjectionService clipboardInjectionService;

        readonly IDownloader downloader;

        readonly IImageFileInterpreter imageFileInterpreter;

        readonly ILinkParser linkParser;

        readonly IAsyncFilter asyncFilter;

        public CopyImageLinkAction(
            ILinkParser linkParser,
            IImageFileInterpreter imageFileInterpreter,
            IDownloader downloader,
            IClipboardInjectionService clipboardInjectionService,
            IAsyncFilter asyncFilter)
        {
            this.asyncFilter = asyncFilter;
            this.linkParser = linkParser;
            this.downloader = downloader;
            this.imageFileInterpreter = imageFileInterpreter;
            this.clipboardInjectionService = clipboardInjectionService;
        }

        public string Description
            => "Copies the image that the link in the clipboard text points to.";

        public string Title => "Copy image from link";

        public byte Order => 100;

        public async Task<bool> CanPerformAsync(IClipboardDataPackage package)
        {
            return await GetFirstSupportedDataAsync(package)
                             .ConfigureAwait(false) != null;
        }

        async Task<IClipboardData> GetFirstSupportedDataAsync(IClipboardDataPackage package)
        {
            var validItems = await asyncFilter.FilterAsync(package.Contents, CanPerformAsync)
                                              .ConfigureAwait(false);
            return validItems.FirstOrDefault();
        }

        async Task<bool> CanPerformAsync(IClipboardData data)
        {
            var textData = data as IClipboardTextData;
            return (textData != null) &&
                   await linkParser.HasLinkOfTypeAsync(textData.Text, LinkType.ImageFile)
                                   .ConfigureAwait(false);
        }

        public async Task PerformAsync(IClipboardDataPackage package)
        {
            var textData = (IClipboardTextData) await GetFirstSupportedDataAsync(package)
                                                          .ConfigureAwait(false);
            var links = await linkParser.ExtractLinksFromTextAsync(textData.Text)
                                        .ConfigureAwait(false);

            var imagesBytes = await DownloadLinksAsync(links)
                                        .ConfigureAwait(false);

            var images = InterpretImages(imagesBytes);
            InjectImages(images);
        }

        IEnumerable<BitmapSource> InterpretImages(IEnumerable<byte[]> imagesBytes)
        {
            return imagesBytes.Select(imageFileInterpreter.Interpret);
        }

        void InjectImages(IEnumerable<BitmapSource> images)
        {
            foreach (var image in images)
            {
                clipboardInjectionService.InjectImage(image);
            }
        }

        async Task<IEnumerable<byte[]>> DownloadLinksAsync(IEnumerable<string> links)
        {
            var downloadTasks = new List<Task<byte[]>>();
            foreach (var link in links)
            {
                downloadTasks.Add(downloader.DownloadBytesAsync(link));
            }

            await Task.WhenAll(downloadTasks)
                      .ConfigureAwait(false);

            return downloadTasks.Select(x => x.Result);
        }
    }
}