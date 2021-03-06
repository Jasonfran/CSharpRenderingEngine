﻿using OpenTK;

namespace Engine.Renderer
{
    public class RenderTextCommand : RenderCommand
    {
        public string Text { get; private set; }
        public Vector3 Color { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public RenderTextCommand()
        {
            Type = RenderCommandType.RenderText;
        }

        public void Package(string text, float x, float y, Vector3 color)
        {
            Text = text;
            Color = color;
            X = x;
            Y = y;
        }
    }
}