shader_type spatial;

uniform vec3 pulse_color = vec3(1.0, 0.84, 0.0);
uniform float alpha = 1.0;

void fragment() {
    ALBEDO = pulse_color;
    ALPHA = alpha;
    EMISSION = pulse_color * 2.0;

    if (ALPHA <= 0.01) {
        discard;
    }
}
