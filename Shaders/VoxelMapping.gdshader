shader_type spatial;

uniform sampler2D mapping_texture : filter_nearest;
uniform sampler2D tiles_texture : filter_nearest;

//we have a texture that maps to a voxel, we should be getting a fragcoord
//that maps to what voxel we're seeing
//
//that pixel, from the mapping_texture, then tells us 
//what tile to use from the tiles_texture
//
//red = tile index
//green = coloration
//blue = degradation
varying vec2 uv;
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec3 data = texture(mapping_texture, UV).rgb;
	float tileUVX = (UV.x*256.0);
	tileUVX = tileUVX-floor(tileUVX);
	float tileUVY = (UV.y*256.0);
	tileUVY = tileUVY-floor(tileUVY);	
	highp int red = int(data.r*255.0);
	highp int green = int(data.g*255.0);
	highp int blue = int(data.b*255.0);
	highp int tx = red % 16;
	highp int ty = red / 16;
	vec4 color = texture(tiles_texture, vec2((float(tx)+tileUVX)/16.0, (float(ty)+tileUVY)/16.0));
	ALBEDO = color.rgb;
	if(color.a < 0.5)
		discard;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
