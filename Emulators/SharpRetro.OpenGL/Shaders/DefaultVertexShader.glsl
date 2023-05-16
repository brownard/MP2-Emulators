#version 120

uniform mat4 modelViewProjection;

attribute vec3 position;
attribute vec2 texCoords;

varying vec2 vTexCoord;

void main(){
	vTexCoord = texCoords;
	gl_Position = modelViewProjection * vec4(position.xy, 0.0, 1.0);
}
