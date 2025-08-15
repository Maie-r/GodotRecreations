shader_type canvas_item;

uniform float Alpha = 1;

void vertex()
{
	// Called for every vertex the material is visible on.
}

void fragment()
{
	vec4 input_color = texture(TEXTURE, UV) * COLOR;
	COLOR = vec4(input_color.r, input_color.g, input_color.b, input_color.a*Alpha) ;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
