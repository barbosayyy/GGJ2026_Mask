shader_type canvas_item;
render_mode blend_mix;

uniform vec4 fog_color : source_color = vec4(0.04, 0.06, 0.08, 1.0);
uniform vec4 moon_color = vec4(0.55, 0.65, 0.75, 1.0);
uniform vec4 tint_color = vec4(0.05, 0.09, 0.06, 1.0);

uniform float fog_density : hint_range(0.0, 3.0) = 1.2;
uniform float fog_speed : hint_range(0.0, 2.0) = 0.25;
uniform float vignette : hint_range(0.0, 1.0) = 0.85;
uniform float noise_scale : hint_range(0.1, 5.0) = 1.6;
uniform float moon_strength : hint_range(0.0, 2.0) = 0.35;
uniform float darkness : hint_range(0.0, 1.0) = 0.55; // LOWERED

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x) +
           (c - a) * u.y * (1.0 - u.x) +
           (d - b) * u.x * u.y;
}

void fragment() {
    vec2 uv = UV;
    vec3 col = tint_color.rgb;

    float n = noise(uv * noise_scale + vec2(TIME * fog_speed, TIME * fog_speed * 0.5));
    float fog = smoothstep(0.25, 0.75, n) * fog_density;

    col *= (1.0 - fog * 0.35);

    vec2 moon_pos = vec2(0.75, 0.25);
    float moon_dist = distance(uv, moon_pos);
    float moon = smoothstep(0.35, 0.0, moon_dist) * moon_strength;
    col += moon_color.rgb * moon;

    vec2 center = vec2(0.5, 0.5);
    float v = distance(uv, center);
    float vign = smoothstep(0.5, vignette, v);
    col *= (1.0 - vign);

    col = mix(col, fog_color.rgb, fog * 0.45);
    col *= (1.0 - darkness);

    // output alpha for blending
    COLOR = vec4(col, 0.7);
}
