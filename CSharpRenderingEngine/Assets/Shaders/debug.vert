#version 430 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec3 color;
layout (location = 3) in vec2 texCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 mvp;

uniform float linewidth;

out VS_OUT {

	vec3 normal;
	vec3 fragPos;
	vec3 color;
} vs_out;

void main(){
	vs_out.fragPos = vec3(model * vec4(position, 1.0));
	vs_out.normal = mat3(transpose(inverse(model))) * normal;
	vs_out.color = color;

	vec4 delta = vec4(normal * linewidth, 0);
	vec4 pos = view * model * vec4(position, 1.0f);

	gl_Position = projection * (pos + delta);
}