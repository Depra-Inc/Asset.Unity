﻿using System;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Internal.Patterns;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
    public sealed class AssetBundleAssetFile<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetIdent _ident;
        private readonly AssetBundleFile _assetBundle;

        private TAsset _loadedAsset;

        public AssetBundleAssetFile(AssetIdent ident, AssetBundleFile assetBundle)
        {
            _ident = ident;
            _assetBundle = assetBundle ?? throw new ArgumentNullException(nameof(assetBundle));
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAsset != null;

        public FileSize Size => IsLoaded
            ? new FileSize(Profiler.GetRuntimeMemorySizeLong(_loadedAsset))
            : throw new AssetBundleFileNotLoadedException(Name, _assetBundle.Name);

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var loadedAsset = _assetBundle.Load<TAsset>(Name);
            EnsureAsset(loadedAsset, exception => throw exception);
            _loadedAsset = loadedAsset;

            return loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            if (IsLoaded == false)
            {
                return _assetBundle.LoadAsync(Name, callbacks
                    .AddGuard(asset => EnsureAsset(asset, callbacks.InvokeFailedEvent))
                    .ReturnTo(asset => _loadedAsset = asset));
            }

            callbacks.InvokeProgressEvent(1f);
            callbacks.InvokeLoadedEvent(_loadedAsset);
            return new EmptyDisposable();
        }

        public void Dispose() => Unload();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(TAsset asset, Action<Exception> onFailed)
        {
            if (asset == null)
            {
                onFailed?.Invoke(new AssetBundleFileLoadingException(Name, _assetBundle.Name));
            }
        }

        public static implicit operator TAsset(AssetBundleAssetFile<TAsset> assetFile) => assetFile.Load();
    }
}