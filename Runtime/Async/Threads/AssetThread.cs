﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Depra.Assets.Runtime.Async.Threads
{
    internal sealed class AssetThread<TAsset> : IAssetThread<TAsset>
    {
        private readonly Task<TAsset> _task;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public AssetThread(Task<TAsset> task)
        {
            _task = task;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start(Action<TAsset> onLoaded, Action<float> onProgress, Action<Exception> onFailed)
        {
            Task.Run(() => _task);
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}