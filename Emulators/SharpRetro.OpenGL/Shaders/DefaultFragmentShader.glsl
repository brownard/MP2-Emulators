#version 120

uniform sampler2D fragTex;
varying vec2 vTexCoord;

void main(){
	gl_FragColor = texture2D(fragTex, vTexCoord);
}