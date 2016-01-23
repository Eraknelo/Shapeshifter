﻿namespace Shapeshifter.WindowsDesktop.Controls.Clipboard.ViewModels
{
    using Autofac;

    using Data.Interfaces;

    using Interfaces;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    [TestClass]
    public class ClipboardTextDataViewModelTest: TestBase
    {
        [TestMethod]
        public void FriendlyTextRemovesDuplicateWhitespaces()
        {
            var container = CreateContainer();

            var fakeTextData = Substitute.For<IClipboardTextData>();
            fakeTextData.Text.Returns("hello  \t  world");

            var viewModel = container.Resolve<IClipboardTextDataViewModel>();
            viewModel.Data = fakeTextData;

            Assert.AreEqual("hello world", viewModel.FriendlyText);
        }

        [TestMethod]
        public void FriendlyTextDoesNotRemoveSingleWhitespace()
        {
            var container = CreateContainer();

            var fakeTextData = Substitute.For<IClipboardTextData>();
            fakeTextData.Text.Returns("hello world");

            var viewModel = container.Resolve<IClipboardTextDataViewModel>();
            viewModel.Data = fakeTextData;

            Assert.AreEqual("hello world", viewModel.FriendlyText);
        }

        [TestMethod]
        public void FriendlyTextSubstitutesTabCharacters()
        {
            var container = CreateContainer();

            var fakeTextData = Substitute.For<IClipboardTextData>();
            fakeTextData.Text.Returns("hello\tworld");

            var viewModel = container.Resolve<IClipboardTextDataViewModel>();
            viewModel.Data = fakeTextData;

            Assert.AreEqual("hello world", viewModel.FriendlyText);
        }

        [TestMethod]
        public void FriendlyTextIsNotLongerThanHalfKilobyte()
        {
            var container = CreateContainer();

            var fakeTextData = Substitute.For<IClipboardTextData>();

            var repeatText = "hello world";
            while (repeatText.Length < 512)
            {
                repeatText += repeatText;
            }

            fakeTextData.Text.Returns(repeatText);

            var viewModel = container.Resolve<IClipboardTextDataViewModel>();
            viewModel.Data = fakeTextData;

            Assert.AreEqual(512, viewModel.FriendlyText.Length);
        }

        [TestMethod]
        public void FriendlyTextTrimsLeadingWhitespaces()
        {
            var container = CreateContainer();

            var fakeTextData = Substitute.For<IClipboardTextData>();
            fakeTextData.Text.Returns("   hello world   ");

            var viewModel = container.Resolve<IClipboardTextDataViewModel>();
            viewModel.Data = fakeTextData;

            Assert.AreEqual("hello world", viewModel.FriendlyText);
        }
    }
}