shader_type spatial;
render_mode unshaded;

uniform sampler2D screen_texture : source_color;

void fragment() {
    // Calculate screen space coordinates
    vec2 screen_coords = FRAGCOORD.xy / VIEWPORT_SIZE;
    // The y coordinate is flipped in Godot, so we invert it
    screen_coords.y = 1.0 - screen_coords.y;
    // Sample the texture using the screen space coordinates
    ALBEDO = texture(screen_texture, screen_coords).rgb;
}