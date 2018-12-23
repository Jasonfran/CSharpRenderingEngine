using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Renderer
{
    public class Shader
    {
        public int ProgramId { get; }

        public Shader(int programId)
        {
            ProgramId = programId;
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        public void SetUniformBlockBinding(string name, int index)
        {
            var location = GL.GetUniformBlockIndex(ProgramId, name);
            GL.UniformBlockBinding(ProgramId, location, index);
        }

        public void SetMat4(string name, Matrix4 mat4)
        {
            var location = GL.GetUniformLocation(ProgramId, name);
            GL.UniformMatrix4(location, false, ref mat4);
        }

        public void SetVec3(string name, Vector3 vec3)
        {
            var location = GL.GetUniformLocation(ProgramId, name);
            GL.Uniform3(location, vec3);
        }

        public void SetInt(string name, int value)
        {
            var location = GL.GetUniformLocation(ProgramId, name);
            GL.Uniform1(location, value);
        }

        public void SetFloat(string name, float value)
        {
            var location = GL.GetUniformLocation(ProgramId, name);
            GL.Uniform1(location, value);
        }

        public void SetMaterial(string name, Material material)
        {
            var textureUnitCounter = 0;
            if (material.AmbientTexture != null)
            {
                var textureUnit = textureUnitCounter++;
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                GL.BindTexture(TextureTarget.Texture2D, material.AmbientTexture.TextureId);
                SetInt($"{name}.ambientTexture", textureUnit);
                SetInt($"{name}.hasAmbientTexture", 1);
            }
            else
            {
                SetVec3($"{name}.ambientColor", new Vector3(material.AmbientColor));
                SetInt($"{name}.hasAmbientTexture", 0);
            }

            if (material.DiffuseTexture != null)
            {
                var textureUnit = textureUnitCounter++;
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                GL.BindTexture(TextureTarget.Texture2D, material.DiffuseTexture.TextureId);
                SetInt($"{name}.diffuseTexture", textureUnit);
                SetInt($"{name}.hasDiffuseTexture", 1);
            }
            else
            {
                SetVec3($"{name}.diffuseColor", new Vector3(material.DiffuseColor));
                SetInt($"{name}.hasDiffuseTexture", 0);
            }

            if (material.SpecularTexture != null)
            {
                var textureUnit = textureUnitCounter++;
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                GL.BindTexture(TextureTarget.Texture2D, material.SpecularTexture.TextureId);
                SetInt($"{name}.specularTexture", textureUnit);
                SetInt($"{name}.hasSpecularTexture", 1);
            }
        }
    }
}