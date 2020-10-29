using System;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.WebClient.Services.Interfaces;

namespace LatronArs.WebClient.Helpers
{
    public static class WebGLHelper
    {
        public static async Task<WebGLProgram> CompileProgram(WebGLContext gl, string vertex, string fragment)
        {
            var program = await gl.CreateProgramAsync();

            var vertexShader = await gl.CreateShaderAsync(ShaderType.VERTEX_SHADER);
            await gl.ShaderSourceAsync(vertexShader, vertex);
            await gl.CompileShaderAsync(vertexShader);

            var fragmentShader = await gl.CreateShaderAsync(ShaderType.FRAGMENT_SHADER);
            await gl.ShaderSourceAsync(fragmentShader, fragment);
            await gl.CompileShaderAsync(fragmentShader);

            await gl.AttachShaderAsync(program, vertexShader);
            await gl.AttachShaderAsync(program, fragmentShader);
            await gl.LinkProgramAsync(program);
            if (await gl.GetProgramParameterAsync<bool>(program, ProgramParameter.LINK_STATUS))
            {
                return program;
            }

            throw new Exception("Shader program is not compiled");
        }

        public static void FillBackground(byte[] backgrounds, byte r, byte g, byte b, int texturePosition)
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
            double cameraLeft,
            double cameraTop,
            int tileWidth,
            int tileHeight,
            int tileHeightOffset,
            int texturePosition)
        {
            var canvasX = (int)((x - cameraLeft) * tileWidth);
            var canvasY = (int)((y - cameraTop) * tileHeight);
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

        public static async Task DrawArrays(
            WebGLContext gl,
            WebGLProgram program,
            float[] vertexPositions,
            float[] textureMapping,
            float[] backgroundTextureMapping,
            byte[] colors,
            byte[] backgrounds,
            byte[] masks,
            WebGLTexture texture,
            WebGLTexture mask,
            int width,
            int height,
            int cameraX,
            int cameraY,
            int colorsWidth,
            int colorsHeight,
            int textureWidth,
            int textureHeight)
        {
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
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 0x2600);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 0x2600);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, colorsWidth, colorsHeight, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, colors);

            var backgroundTexture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, backgroundTexture);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 0x2600);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 0x2600);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, colorsWidth, colorsHeight, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, backgrounds);

            var maskTexture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, maskTexture);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 0x2600);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 0x2600);
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

            await gl.UniformAsync(positionResolutionLocation, width, height);
            await gl.UniformAsync(textureResolutionLocation, textureWidth, textureHeight);

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

            await gl.DrawArraysAsync(Primitive.TRIANGLES, 0, vertexPositions.Length / 2);
        }
    }
}