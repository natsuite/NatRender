/* 
*   NatRender
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.Rendering {

    using System;
    using UnityEngine;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Readback provider using blocking readbacks.
    /// This provider is supported on all platforms.
    /// </summary>
    public sealed class SyncReadback : IReadback {

        #region --Client API--
        /// <summary>
        /// Create a blockng readback provider.
        /// </summary>
        /// <param name="width">Output pixel buffer width.</param>
        /// <param name="height">Output pixel buffer height.</param>
        public SyncReadback (int width, int height) => this.frameBuffer = new Texture2D(width, height, TextureFormat.RGBA32, false, false);

        /// <summary>
        /// Request a readback.
        /// </summary>
        /// <param name="texture">Input texture.</param>
        /// <param name="handler">Readback handler.</param>
        public void Request (Texture texture, ReadbackDelegate handler) {
            var renderTexture = RenderTexture.GetTemporary(frameBuffer.width, frameBuffer.height, 0, RenderTextureFormat.Default);
            Graphics.Blit(texture, renderTexture);
            RenderTexture.active = renderTexture;
            frameBuffer.ReadPixels(new Rect(0, 0, frameBuffer.width, frameBuffer.height), 0, 0);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            handler(frameBuffer.GetRawTextureData<byte>());
        }

        /// <summary>
        /// Request a readback.
        /// </summary>
        /// <param name="texture">Input texture.</param>
        /// <param name="handler">Readback handler.</param>
        public unsafe void Request (Texture texture, NativeReadbackDelegate handler) => Request(texture, buffer => handler(NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(buffer)));

        /// <summary>
        /// Dispose the readback provider.
        /// </summary>
        public void Dispose () => Texture2D.Destroy(frameBuffer);
        #endregion


        #region --Operations--

        private readonly Texture2D frameBuffer;
        #endregion
    }
}