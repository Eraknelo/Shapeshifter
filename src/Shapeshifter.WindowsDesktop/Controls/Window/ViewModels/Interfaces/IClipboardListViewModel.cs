﻿namespace Shapeshifter.WindowsDesktop.Controls.Window.ViewModels.Interfaces
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Data.Actions.Interfaces;
    using Data.Interfaces;

    using Infrastructure.Events;

    public interface IClipboardListViewModel: INotifyPropertyChanged
    {
        event EventHandler<UserInterfaceShownEventArgument> UserInterfaceShown;

        event EventHandler<UserInterfaceHiddenEventArgument> UserInterfaceHidden;

        IClipboardDataControlPackage SelectedElement { get; set; }

        IAction SelectedAction { get; set; }

        ObservableCollection<IClipboardDataControlPackage> Elements { get; }

        ObservableCollection<IAction> Actions { get; }

        void ShowNextItem();

        void ShowPreviousItem();

        void SwapBetweenPanes();
    }
}