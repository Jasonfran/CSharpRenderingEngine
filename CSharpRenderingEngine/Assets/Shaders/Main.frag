#version 430 core

in VS_OUT {
	vec2 texCoords;
	vec3 normal;
	vec3 fragPos;
} fs_in;

out vec4 FragColor;

float ambientLightStrength = 0.1;

void main(){

		FragColor = vec4(fs_in.fragPos, 1.0f);
}