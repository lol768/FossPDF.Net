﻿#if NET6_0_OR_GREATER

using System;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(FossPDF.Previewer.HotReloadManager))]

namespace FossPDF.Previewer
{
    /// <summary>
    /// Helper for subscribing to hot reload notifications.
    /// </summary>
    internal static class HotReloadManager
    {
        public static event EventHandler? UpdateApplicationRequested;

        public static void UpdateApplication(Type[]? _)
        {
            UpdateApplicationRequested?.Invoke(null, EventArgs.Empty);
        }
    }
}

#endif
