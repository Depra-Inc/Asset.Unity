﻿using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Interfaces.Files;
using UnityEngine;

namespace Depra.Assets.Runtime.EditorScope
{
    public sealed class PreloadedAsset<TAsset> : ILoadableAsset<TAsset> where TAsset : Object
    {
        private readonly ILoadableAsset<TAsset> _asset;

        public PreloadedAsset(ILoadableAsset<TAsset> asset) => _asset = asset;

        public string Name => _asset.Name;

        public string Path => _asset.Path;

        public bool IsLoaded => _asset.IsLoaded;

        public TAsset Load()
        {
#if UNITY_EDITOR
            if (PreloadedAssetLoader.TryLoadAsset(out TAsset asset) ||
                AssetDatabaseLoader.TryLoadAsset(out asset))
            {
                return asset;
            }
#endif

            return _asset.Load();
        }

        public void Unload() => _asset.Unload();

        public void LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
#if UNITY_EDITOR
            if (PreloadedAssetLoader.TryLoadAsset(out TAsset asset) ||
                AssetDatabaseLoader.TryLoadAsset(out asset))
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(asset);
                return;
            }
#endif

            _asset.LoadAsync(callbacks);
        }
    }
}