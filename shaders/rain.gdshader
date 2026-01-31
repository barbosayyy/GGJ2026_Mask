shader_type canvas_item;

uniform int count: hint_range(0, 4000) = 1600;
uniform float angle: hint_range(-90.0, 90.0) = -45.0;
uniform float speed: hint_range(25.0, 500.0) = 300.0;
uniform float blur: hint_range(0.0005, 0.1) = 0.001;
uniform vec4 colour: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec2 size = vec2(0.005, 0.3);

float line_sdf(vec2 p, vec2 s) {
    vec2 d = abs(p) - s;
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float Hash(float x) {
    return fract(sin(x * 18.34) * 51.78);
}

float Hash2(float x) {
    return fract(sin(x * 25.42) * 21.24);
}

vec2 rot(vec2 v, float a) {
    float c = cos(a);
    float s = sin(a);
    return vec2(v.x * c - v.y * s, v.x * s + v.y * c);
}

void fragment() {
    vec2 s = size * 0.1;
    float time = TIME + 1000.0;
    float rad = radians(angle);

    // rotate UV
    vec2 uv = rot(UV, -rad);

    float output = 0.0;

    // **increase spawn area**
    // this ensures rain can come from off-screen and still hit the view
    float spawn_margin = 1.5;

    for (int i = 1; i <= count; i++) {
        float h1 = Hash(float(i));
        float h2 = Hash2(float(i));

        // spawn in a larger area (bigger than 0..1)
        vec2 pos = vec2(h1 * spawn_margin - (spawn_margin - 1.0) * 0.5,
                        h2 * spawn_margin - (spawn_margin - 1.0) * 0.5);

        // move along y in rotated space
        pos.y -= time * speed * 0.1;

        // wrap inside the larger area
        pos = mod(pos + spawn_margin, spawn_margin) - (spawn_margin - 1.0) * 0.5;

        float sdf = line_sdf(uv - pos, s);
        output += clamp(-sdf / blur, 0.0, 1.0);
    }

    COLOR = vec4(colour.rgb, output);
}
