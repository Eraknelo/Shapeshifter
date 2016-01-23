﻿namespace Shapeshifter.WindowsDesktop.Services.Messages.Interceptors.Hotkeys
{
    using System;
    using System.Threading;
    using System.Windows.Input;

    using Interfaces;

    using Native;
    using Native.Interfaces;

    class HotkeyInterception: IHotkeyInterception
    {
        readonly IKeyboardNativeApi keyboardNativeApi;

        static int interceptionId;

        public int InterceptionId { get; }

        public bool ControlNeeded { get; set; }

        public bool NoRepeat { get; set; }

        public Key Key { get; set; }

        public HotkeyInterception(
            IKeyboardNativeApi keyboardNativeApi)
        {
            this.keyboardNativeApi = keyboardNativeApi;
            InterceptionId = Interlocked.Increment(ref interceptionId);
        }

        public void Start(IntPtr windowHandle)
        {
            UnregisterHotkey(windowHandle);

            var modifier = 0;
            if (ControlNeeded)
            {
                modifier |= KeyboardNativeApi.MOD_CONTROL;
            }
            if (NoRepeat)
            {
                modifier |= KeyboardNativeApi.MOD_NOREPEAT;
            }

            InstallHotkey(windowHandle, modifier, Key);
        }

        void InstallHotkey(IntPtr windowHandle, int modifier, Key key)
        {
            var keyCode = KeyInterop.VirtualKeyFromKey(key);
            var registrationResult = keyboardNativeApi.RegisterHotKey(
                windowHandle,
                InterceptionId,
                modifier,
                keyCode);
            if (!registrationResult)
            {
                throw new InvalidOperationException(
                    $"Couldn't install the hotkey interceptor for key {Key}.");
            }
        }

        public void Stop(IntPtr windowHandle)
        {
            var registrationResult = UnregisterHotkey(windowHandle);
            if (!registrationResult)
            {
                throw new InvalidOperationException(
                    $"Couldn't uninstall the hotkey interceptor for key {Key}.");
            }
        }

        bool UnregisterHotkey(IntPtr windowHandle)
        {
            return keyboardNativeApi.UnregisterHotKey(
                windowHandle,
                InterceptionId);
        }
    }
}