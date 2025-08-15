shader_type canvas_item;

uniform vec4 Colorize : source_color;

void vertex()
{
	// Called for every vertex the material is visible on.
}

void fragment()
{
	vec4 input_color = texture(TEXTURE, UV);
	COLOR = vec4(1, input_color.g, input_color.b, input_color.a) * Colorize;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
