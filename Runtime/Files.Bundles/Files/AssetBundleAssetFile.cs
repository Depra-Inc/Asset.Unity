﻿using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
    public sealed class AssetBundleAssetFile<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetIdent _ident;
        private readonly AssetBundle _assetBundle;
        private readonly ICoroutineHost _coroutineHost;

        private TAsset _loadedAsset;

        public AssetBundleAssetFile(AssetIdent ident, AssetBundle assetBundle, ICoroutineHost coroutineHost = null)
        {
            _ident = ident;
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;
            _assetBundle = assetBundle ? assetBundle : throw new ArgumentNullException(nameof(assetBundle));
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var loadedAsset = _assetBundle.LoadAsset<TAsset>(Name);
            return OnLoaded(loadedAsset, onFailed: exception => throw exception);
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public IAsyncToken LoadAsync(Action<TAsset> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            if (IsLoaded)
            {
                return AlreadyLoadedAsset<TAsset>.Create(_loadedAsset, onLoaded, onProgress);
            }

            var loadingThread = new MainAssetThread<TAsset>(_coroutineHost, LoadingProcess);
            loadingThread.Start(OnLoadedInternal, onProgress, onFailed);
            void OnLoadedInternal(TAsset loadedAsset) => OnLoaded(loadedAsset, onFailed, onLoaded);
            
            return new AsyncActionToken(loadingThread.Cancel);
        }

        private TAsset OnLoaded(TAsset asset, Action<Exception> onFailed, Action<TAsset> onLoaded = null)
        {
            EnsureAsset(asset, onFailed);
            _loadedAsset = asset;
            onLoaded?.Invoke(_loadedAsset);
            RefreshSize(_loadedAsset);

            return _loadedAsset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerator LoadingProcess(Action<TAsset> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            var request = _assetBundle.LoadAssetAsync<TAsset>(Name);
            while (request.isDone == false)
            {
                onProgress?.Invoke(request.progress);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onLoaded.Invoke((TAsset)request.asset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RefreshSize(TAsset asset) =>
            Size = new FileSize(Profiler.GetRuntimeMemorySizeLong(asset));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(TAsset asset, Action<Exception> onFailed)
        {
            if (asset == null)
            {
                onFailed?.Invoke(new AssetBundleFileLoadingException(Name, _assetBundle.name));
            }
        }

        void IDisposable.Dispose() => Unload();

        public static implicit operator TAsset(AssetBundleAssetFile<TAsset> assetFile) => assetFile.Load();
    }
}