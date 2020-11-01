using System;
using System.Linq;
using System.Threading.Tasks;
using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.WebClient.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Helpers
{
    public static class WebGLHelper
    {
        public static void FillLight(byte[] backgrounds, byte r, byte g, byte b, int texturePosition)
        {
            backgrounds[texturePosition * 4] = r;
            backgrounds[(texturePosition * 4) + 1] = g;
            backgrounds[(texturePosition * 4) + 2] = b;
            backgrounds[(texturePosition * 4) + 3] = 255;
        }

        public static void FillColor(
            byte[] colors,
            byte[] masks,
            byte r,
            byte g,
            byte b,
            byte a,
            bool shines,
            bool silhouette,
            bool active,
            int texturePosition)
        {
            colors[texturePosition * 4] = r;
            colors[(texturePosition * 4) + 1] = g;
            colors[(texturePosition * 4) + 2] = b;
            colors[(texturePosition * 4) + 3] = a;

            masks[texturePosition * 4] = shines ? 255 : 0;
            masks[(texturePosition * 4) + 1] = silhouette ? 255 : 0;
            masks[(texturePosition * 4) + 2] = 255;
            masks[(texturePosition * 4) + 3] = active ? 255 : 0;
        }

        public static void FillSprite(
            ISpritesService spritesService,
            float[] textureMapping,
            SpriteDefinition spriteDefinition,
            int texturePosition)
        {
            var (spritePosition, mirrored) = spritesService.GetSpritePositionByDefinition(spriteDefinition);
            if (mirrored)
            {
                textureMapping[texturePosition * 12] = spritePosition.X + spritesService.SpriteWidth;
                textureMapping[(texturePosition * 12) + 1] = spritePosition.Y;
                textureMapping[(texturePosition * 12) + 2] = spritePosition.X;
                textureMapping[(texturePosition * 12) + 3] = spritePosition.Y;
                textureMapping[(texturePosition * 12) + 4] = spritePosition.X + spritesService.SpriteWidth;
                textureMapping[(texturePosition * 12) + 5] = spritePosition.Y + spritesService.SpriteHeight;
                textureMapping[(texturePosition * 12) + 6] = spritePosition.X + spritesService.SpriteWidth;
                textureMapping[(texturePosition * 12) + 7] = spritePosition.Y + spritesService.SpriteHeight;
                textureMapping[(texturePosition * 12) + 8] = spritePosition.X;
                textureMapping[(texturePosition * 12) + 9] = spritePosition.Y;
                textureMapping[(texturePosition * 12) + 10] = spritePosition.X;
                textureMapping[(texturePosition * 12) + 11] = spritePosition.Y + spritesService.SpriteHeight;
            }
            else
            {
                textureMapping[texturePosition * 12] = spritePosition.X;
                textureMapping[(texturePosition * 12) + 1] = spritePosition.Y;
                textureMapping[(texturePosition * 12) + 2] = spritePosition.X + spritesService.SpriteWidth;
                textureMapping[(texturePosition * 12) + 3] = spritePosition.Y;
                textureMapping[(texturePosition * 12) + 4] = spritePosition.X;
                textureMapping[(texturePosition * 12) + 5] = spritePosition.Y + spritesService.SpriteHeight;
                textureMapping[(texturePosition * 12) + 6] = spritePosition.X;
                textureMapping[(texturePosition * 12) + 7] = spritePosition.Y + spritesService.SpriteHeight;
                textureMapping[(texturePosition * 12) + 8] = spritePosition.X + spritesService.SpriteWidth;
                textureMapping[(texturePosition * 12) + 9] = spritePosition.Y;
                textureMapping[(texturePosition * 12) + 10] = spritePosition.X + spritesService.SpriteWidth;
                textureMapping[(texturePosition * 12) + 11] = spritePosition.Y + spritesService.SpriteHeight;
            }
        }

        public static void FillVertexPosition(
            float[] vertexPositions,
            int x,
            int y,
            int cameraLeft,
            int cameraTop,
            int tileWidth,
            int tileHeight,
            int tileHeightOffset,
            int texturePosition)
        {
            var canvasX = (x - cameraLeft) * tileWidth;
            var canvasY = (y - cameraTop) * tileHeight;
            vertexPositions[texturePosition * 12] = canvasX;
            vertexPositions[(texturePosition * 12) + 1] = canvasY - tileHeightOffset;
            vertexPositions[(texturePosition * 12) + 2] = canvasX + tileWidth;
            vertexPositions[(texturePosition * 12) + 3] = canvasY - tileHeightOffset;
            vertexPositions[(texturePosition * 12) + 4] = canvasX;
            vertexPositions[(texturePosition * 12) + 5] = canvasY + tileHeight;
            vertexPositions[(texturePosition * 12) + 6] = canvasX;
            vertexPositions[(texturePosition * 12) + 7] = canvasY + tileHeight;
            vertexPositions[(texturePosition * 12) + 8] = canvasX + tileWidth;
            vertexPositions[(texturePosition * 12) + 9] = canvasY - tileHeightOffset;
            vertexPositions[(texturePosition * 12) + 10] = canvasX + tileWidth;
            vertexPositions[(texturePosition * 12) + 11] = canvasY + tileHeight;
        }

        public static void DrawArrays(
            IJSRuntime jsRuntime,
            ElementReference canvas,
            float[] vertexPositions,
            float[] textureMapping,
            float[] backgroundTextureMapping,
            byte[] colors,
            byte[] backgrounds,
            byte[] masks,
            int width,
            int height,
            int cameraX,
            int cameraY,
            int colorsWidth,
            int colorsHeight,
            int textureWidth,
            int textureHeight)
        {
            if (jsRuntime is IJSUnmarshalledRuntime webAssemblyJSRuntime)
            {
                var canvasId = int.Parse(canvas.Id);
                webAssemblyJSRuntime.InvokeUnmarshalled<int, object>("sceneExtensions.setupContext", canvasId);
                webAssemblyJSRuntime.InvokeUnmarshalled<float[], float[], float[], object>("sceneExtensions.setupVertices", vertexPositions, textureMapping, backgroundTextureMapping);
                webAssemblyJSRuntime.InvokeUnmarshalled<byte[], byte[], byte[], object>("sceneExtensions.setupColors", colors, backgrounds, masks);
                webAssemblyJSRuntime.InvokeUnmarshalled<int, int, int, object>("sceneExtensions.setupXAxle", cameraX, colorsWidth, textureWidth);
                webAssemblyJSRuntime.InvokeUnmarshalled<int, int, int, object>("sceneExtensions.setupYAxle", cameraY, colorsHeight, textureHeight);
                webAssemblyJSRuntime.InvokeUnmarshalled<int, int, object>("sceneExtensions.draw", width, height);
             /*   webAssemblyJSRuntime.InvokeUnmarshalled<byte[], byte[], byte[], object>("downloadByteArray", bytes, bytes2, bytes3);
             //   webAssemblyJSRuntime.InvokeUnmarshalled<float[], float[], float[], object>("downloadFloatArray", vertexes, textureMapping, backgroundTextureMapping);
                webAssemblyJSRuntime.InvokeUnmarshalled<int, int, int, object>("downloadFloatValues", (right - left + 1) * TileSize, (bottom - top + 1) * TileSize, (int)((left - cameraLeft) * TileSize));*/
            }
        }

      /*      WebGLContext gl,
            WebGLProgram program,
            float[] vertexPositions,
            float[] textureMapping,
            float[] backgroundTextureMapping,
            int[] colors,
            int[] backgrounds,
            int[] masks,
            WebGLTexture texture,
            WebGLTexture mask,
            int width,
            int height,
            int cameraX,
            int cameraY,
            int colorsWidth,
            int colorsHeight,
            int textureWidth,
            int textureHeight)*/
            /*await gl.ClearColorAsync(0, 0, 0, 1);

            uint positionLocation = (uint)await gl.GetAttribLocationAsync(program, "a_position");
            uint texcoordLocation = (uint)await gl.GetAttribLocationAsync(program, "a_texCoord");
            uint backgroundTexcoordLocation = (uint)await gl.GetAttribLocationAsync(program, "a_backgroundTexCoord");

            var positionBuffer = await gl.CreateBufferAsync();
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, positionBuffer);
            await gl.BufferDataAsync(BufferType.ARRAY_BUFFER, vertexPositions, BufferUsageHint.STATIC_DRAW);

            var texcoordBuffer = await gl.CreateBufferAsync();
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, texcoordBuffer);
            await gl.BufferDataAsync(BufferType.ARRAY_BUFFER, textureMapping, BufferUsageHint.STATIC_DRAW);

            var backgroundTexcoordBuffer = await gl.CreateBufferAsync();
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, backgroundTexcoordBuffer);
            await gl.BufferDataAsync(BufferType.ARRAY_BUFFER, backgroundTextureMapping, BufferUsageHint.STATIC_DRAW);

            var colorTexture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, colorTexture);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 33071);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 33071);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 9728);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 9728);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, colorsWidth, colorsHeight, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, colors);

            var backgroundTexture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, backgroundTexture);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 33071);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 33071);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 9728);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 9728);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, colorsWidth, colorsHeight, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, backgrounds);

            var maskTexture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, maskTexture);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 33071);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 33071);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 9728);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 9728);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, colorsWidth, colorsHeight, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, masks);

            var positionResolutionLocation = await gl.GetUniformLocationAsync(program, "u_positionResolution");
            var textureResolutionLocation = await gl.GetUniformLocationAsync(program, "u_textureResolution");

            await gl.ViewportAsync(cameraX, cameraY, width, height);
            await gl.UseProgramAsync(program);

            await gl.EnableVertexAttribArrayAsync(positionLocation);
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, positionBuffer);
            await gl.VertexAttribPointerAsync(positionLocation, 2, DataType.FLOAT, false, 0, 0);

            await gl.EnableVertexAttribArrayAsync(texcoordLocation);
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, texcoordBuffer);
            await gl.VertexAttribPointerAsync(texcoordLocation, 2, DataType.FLOAT, false, 0, 0);

            await gl.EnableVertexAttribArrayAsync(backgroundTexcoordLocation);
            await gl.BindBufferAsync(BufferType.ARRAY_BUFFER, backgroundTexcoordBuffer);
            await gl.VertexAttribPointerAsync(backgroundTexcoordLocation, 2, DataType.FLOAT, false, 0, 0);

            await gl.UniformAsync(positionResolutionLocation, (float)width, (float)height);
            await gl.UniformAsync(textureResolutionLocation, (float)textureWidth, (float)textureHeight);

            var colorLocation = await gl.GetUniformLocationAsync(program, "u_color");
            var backgroundLocation = await gl.GetUniformLocationAsync(program, "u_backgroundColor");
            var maskMapLocation = await gl.GetUniformLocationAsync(program, "u_maskMap");

            var textureLocation = await gl.GetUniformLocationAsync(program, "u_texture");
            var maskLocation = await gl.GetUniformLocationAsync(program, "u_mask");

            await gl.UniformAsync(colorLocation, 0);
            await gl.UniformAsync(backgroundLocation, 1);
            await gl.UniformAsync(maskMapLocation, 2);
            await gl.UniformAsync(textureLocation, 3);
            await gl.UniformAsync(maskLocation, 4);

            await gl.ActiveTextureAsync(Texture.TEXTURE0);
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, colorTexture);
            await gl.ActiveTextureAsync(Texture.TEXTURE1);
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, backgroundTexture);
            await gl.ActiveTextureAsync(Texture.TEXTURE2);
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, maskTexture);
            await gl.ActiveTextureAsync(Texture.TEXTURE3);
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, texture);
            await gl.ActiveTextureAsync(Texture.TEXTURE4);
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, mask);
            await gl.EnableAsync(EnableCap.BLEND);
            await gl.BlendFuncAsync(BlendingMode.SRC_ALPHA, BlendingMode.ONE_MINUS_SRC_ALPHA);

            var lengths = vertexPositions.Length / 2;
            await gl.DrawArraysAsync(Primitive.TRIANGLES, 0, lengths);*/
    }
}