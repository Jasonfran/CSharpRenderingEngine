#version 430 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texCoords;


uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 mvp;

out VS_OUT {

	vec2 texCoords;
	vec3 normal;
	vec3 fragPos;

} vs_out;

void main(){
	gl_Position = mvp * vec4(position, 1.0);
	vs_out.fragPos = vec3(model * vec4(position, 1.0));
	vs_out.normal = mat3(transpose(inverse(model))) * normal;
	vs_out.texCoords = texCoords;
}