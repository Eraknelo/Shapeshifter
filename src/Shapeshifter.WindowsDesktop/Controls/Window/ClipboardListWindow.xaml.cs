﻿namespace Shapeshifter.WindowsDesktop.Controls.Window
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;

    using Infrastructure.Events;

    using Interfaces;

    using Services.Interfaces;
    using Services.Messages.Interceptors.Hotkeys.Interfaces;
    using Services.Messages.Interfaces;

    using ViewModels.Interfaces;

    /// <summary>
    ///     Interaction logic for ClipboardListWindow.xaml
    /// </summary>
    public partial class ClipboardListWindow
        : Window,
          IClipboardListWindow
    {
        readonly IMainWindowHandleContainer handleContainer;
        readonly IKeyInterceptor keyInterceptor;
        readonly IClipboardListViewModel viewModel;
        readonly IWindowMessageHook windowMessageHook;
        readonly IMouseWheelHook mouseWheelHook;

        public ClipboardListWindow(
            IClipboardListViewModel viewModel,
            IKeyInterceptor keyInterceptor,
            IWindowMessageHook windowMessageHook,
            IMouseWheelHook mouseWheelHook,
            IMainWindowHandleContainer handleContainer)
        {
            this.handleContainer = handleContainer;
            this.keyInterceptor = keyInterceptor;
            this.viewModel = viewModel;
            this.windowMessageHook = windowMessageHook;
            this.mouseWheelHook = mouseWheelHook;

            SourceInitialized += ClipboardListWindow_SourceInitialized;
            Activated += ClipboardListWindow_Activated;

            InitializeComponent();
            SetupMouseHook();

            SetupViewModel();
        }

        void SetupMouseHook()
        {
            mouseWheelHook.WheelScrolledDown += MouseWheelHookOnScrolledDown;
            mouseWheelHook.WheelScrolledUp += MouseWheelHookOnScrolledUp;
            mouseWheelHook.WheelTilted += MouseWheelHook_WheelTilted;
        }

        void MouseWheelHook_WheelTilted(object sender, EventArgs e)
        {
            viewModel.SwapBetweenPanes();
        }

        void MouseWheelHookOnScrolledUp(object sender, EventArgs eventArgs)
        {
            viewModel.ShowPreviousItem();
        }

        void MouseWheelHookOnScrolledDown(object sender, EventArgs eventArgs)
        {
            viewModel.ShowNextItem();
        }

        void ClipboardListWindow_SourceInitialized(object sender, EventArgs e)
        {
            handleContainer.Handle = HandleSource.Handle;

            OnWindowHandleReady();
        }

        void SetupKeyInterception()
        {
            keyInterceptor.AddInterceptingKey(
                handleContainer.Handle,
                Key.Up);
            keyInterceptor.AddInterceptingKey(
                handleContainer.Handle,
                Key.Down);
            keyInterceptor.AddInterceptingKey(
                handleContainer.Handle,
                Key.Left);
            keyInterceptor.AddInterceptingKey(
                handleContainer.Handle,
                Key.Right);
            keyInterceptor.AddInterceptingKey(
                handleContainer.Handle,
                Key.Home);
            keyInterceptor.AddInterceptingKey(
                handleContainer.Handle,
                Key.Delete);
        }

        public HwndSource HandleSource => PresentationSource.FromVisual(this) as HwndSource;

        void ClipboardListWindow_Activated(object sender, EventArgs e)
        {
            Activated -= ClipboardListWindow_Activated;
            Hide();
        }

        void OnWindowHandleReady()
        {
            SetupKeyInterception();
            SetupWindowMessageHook();
        }

        void SetupWindowMessageHook()
        {
            windowMessageHook.Connect(this);
        }

        void SetupViewModel()
        {
            viewModel.UserInterfaceShown += ViewModel_UserInterfaceShown;
            viewModel.UserInterfaceHidden += ViewModel_UserInterfaceHidden;

            DataContext = viewModel;
        }

        void ViewModel_UserInterfaceHidden(
            object sender,
            UserInterfaceHiddenEventArgument e)
        {
            Hide();
            mouseWheelHook.ResetAccumulatedWheelDelta();
        }

        void ViewModel_UserInterfaceShown(
            object sender,
            UserInterfaceShownEventArgument e)
        {
            Show();
        }

        public void AddHwndSourceHook(HwndSourceHook hook)
        {
            HandleSource.AddHook(hook);
        }

        public void RemoveHwndSourceHook(HwndSourceHook hook)
        {
            HandleSource.RemoveHook(hook);
        }

        public void Dispose()
        {
            viewModel.UserInterfaceShown -= ViewModel_UserInterfaceShown;
            viewModel.UserInterfaceHidden -= ViewModel_UserInterfaceHidden;

            mouseWheelHook.WheelScrolledDown -= MouseWheelHookOnScrolledDown;
            mouseWheelHook.WheelScrolledUp -= MouseWheelHookOnScrolledUp;
            mouseWheelHook.WheelTilted -= MouseWheelHook_WheelTilted;
        }
    }
}