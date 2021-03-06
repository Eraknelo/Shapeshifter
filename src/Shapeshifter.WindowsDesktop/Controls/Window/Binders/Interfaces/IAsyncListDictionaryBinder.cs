﻿namespace Shapeshifter.WindowsDesktop.Controls.Window.Binders.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    public interface IAsyncListDictionaryBinder<TKey, TData>
        where TKey : class
    {
        TData Default { get; set; }

        void Bind(
            ObservableCollection<TKey> sourceCollection,
            ObservableCollection<TData> destinationCollection,
            Func<TKey, Task<IEnumerable<TData>>> mappingFunction);

        void LoadFromKey(TKey key);
    }
}