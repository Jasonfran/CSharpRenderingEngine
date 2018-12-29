using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SharpFont;
using NotImplementedException = System.NotImplementedException;

namespace Engine.Renderer
{
    public class FontRenderer
    {
        private Library library;
        private Face face;

        private Dictionary<char, Character> characters;

        private int VAO;
        private int VBO;

        private Matrix4 projection;
        private readonly float screenWidth;
        private readonly float screenHeight;

        public FontRenderer(float screenWidth, float screenHeight)
        {
            characters = new Dictionary<char, Character>();
            projection = Matrix4.CreateOrthographicOffCenter(0.0f, 1920, 0, 1080, 0.0f, 100.0f);

            library = new Library();
            face = new Face(library, "assets/Roboto-Regular.ttf");
            face.SetPixelSizes(0, 48);
            //face.SetCharmap(face.CharMaps[1]);
            //Console.WriteLine(face.CharMaps[2].Encoding);
            face.SelectCharmap(Encoding.Unicode);
            LoadCharacterSet();
            LoadVertexBuffers();
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void LoadCharacterSet()
        {
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (int i = 0; i < 128; i++)
            {
                char c = (char)i;
                var glyphIndex = face.GetCharIndex(c);
                face.LoadGlyph(glyphIndex, LoadFlags.Render, LoadTarget.Normal);

                int textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R16, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows, 0, PixelFormat.Red, PixelType.UnsignedByte, face.Glyph.Bitmap.Buffer);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                var character = new Character(textureId, 
                    new Vector2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows), 
                    new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop), 
                    face.Glyph.Advance.X.Value);

                characters.Add(c, character);
            }
        }

        private void LoadVertexBuffers()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void RenderText(Shader shader, string text, float x, float y, float scale, Vector3 color)
        {
            shader.Use();
            shader.SetVec3("textColor", color);
            shader.SetMat4("projection", projection);
            GL.BindVertexArray(VAO);
            GL.ActiveTexture(TextureUnit.Texture0);

            foreach (char c in text)
            {
                var character = characters[c];
                var xPos = x + character.Bearing.X * scale;
                var yPos = y - (character.Size.Y - character.Bearing.Y) * scale;

                var w = character.Size.X * scale;
                var h = character.Size.Y * scale;


                var vertices = new [,]
                {
                    {xPos, yPos + h, 0.0f, 0.0f},
                    {xPos, yPos, 0.0f, 1.0f},
                    {xPos + w, yPos, 1.0f, 1.0f},
                    {xPos, yPos + h, 0.0f, 0.0f},
                    {xPos + w, yPos, 1.0f, 1.0f},
                    {xPos + w, yPos + h, 1.0f, 0.0f}
                };

                GL.BindTexture(TextureTarget.Texture2D, character.TextureId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, 6 * 4 * sizeof(float), vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                x += (character.Advance >> 6) * scale;
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}