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
    }
}