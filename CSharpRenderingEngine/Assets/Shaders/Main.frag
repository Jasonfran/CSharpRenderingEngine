#version 430 core

struct Material {
	sampler2D ambientTexture;
	sampler2D diffuseTexture;
	sampler2D specularTexture;
    
	vec3 ambientColor;
    vec3 diffuseColor;
    vec3 specularColor;

	bool hasAmbientTexture;
	bool hasDiffuseTexture;
	bool hasSpecularTexture;

    float shininess;
}; 

struct Light{
	vec3 position;
	vec3 ambientColor;
	vec3 diffuseColor;
	vec3 specularColor;
};
  
layout (std140) uniform Lights
{
	int maxNumofLights;
	int numOfLights;
    Light light[16];
};

in VS_OUT {
	vec2 texCoords;
	vec3 normal;
	vec3 fragPos;
} fs_in;

uniform Material material;

out vec4 FragColor;

float ambientLightStrength = 0.1;

void main(){
	if(material.hasDiffuseTexture){
		
		vec3 norm = normalize(fs_in.normal);
		vec3 lightDir = normalize(light[0].position - fs_in.fragPos);
		float diff = max(dot(norm, lightDir), 0.0);

		vec3 ambient = vec3(texture(material.ambientTexture, fs_in.texCoords)) * ambientLightStrength;
		vec3 diffuse = diff * light[0].diffuseColor;
		vec3 result = (ambient + diffuse) * vec3(texture(material.diffuseTexture, fs_in.texCoords)); 

		FragColor = vec4(result, 1.0f);
	}else{
		FragColor = vec4(material.diffuseColor, 1.0f);
	}
}