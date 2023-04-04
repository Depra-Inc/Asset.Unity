﻿using System;
using System.Collections;
using Depra.Assets.Runtime.Async.Threads;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Runtime.Utils;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromFile : AssetBundleFile
    {
        private readonly ICoroutineHost _coroutineHost;
        private AssetBundleCreateRequest _createRequest;

        public AssetBundleFromFile(AssetIdent ident, ICoroutineHost coroutineHost = null) : base(ident) =>
            _coroutineHost = coroutineHost ?? AssetCoroutineHook.Instance;

        protected override AssetBundle LoadOverride() =>
            AssetBundle.LoadFromFile(Path);

        protected override IAssetThread<AssetBundle> RequestAsync() =>
            new MainAssetThread<AssetBundle>(_coroutineHost, LoadingProcess, CancelRequest);

        private IEnumerator LoadingProcess(
            Action<AssetBundle> onLoaded,
            Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            _createRequest = AssetBundle.LoadFromFileAsync(Path);
            while (_createRequest.isDone == false)
            {
                onProgress?.Invoke(_createRequest.progress);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onLoaded.Invoke(_createRequest.assetBundle);
        }

        private void CancelRequest()
        {
            if (_createRequest == null || _createRequest.assetBundle == null)
            {
                return;
            }
            
            _createRequest.assetBundle.Unload(true);
            _createRequest = null;
        }
    }
}