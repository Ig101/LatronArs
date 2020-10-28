precision mediump float;

uniform sampler2D u_color;
uniform sampler2D u_backgroundColor;
uniform sampler2D u_maskMap;

uniform sampler2D u_texture;
uniform sampler2D u_mask;

varying vec2 v_texCoord;
varying vec2 v_backgroundTexCoord;
varying vec2 v_position;

void main() {
    vec4 color = texture2D(u_color, v_position);
    vec3 backgroundColor = texture2D(u_backgroundColor, v_position).rgb;
    float activity = texture2D(u_activityMap, v_position).r;
    vec4 textureColor = texture2D(u_texture, v_texCoord);
    vec3 backgroundTextureColor = texture2D(u_texture, v_backgroundTexCoord).rgb;
    float alpha = color.a * textureColor.a * 255.0;
    if (activity != 0.0) {
        gl_FragColor = vec4(
            (color.r * (1.0 - textureColor.g) + textureColor.g) * textureColor.r * alpha + backgroundColor.r * backgroundTextureColor.r * (1.0 - alpha),
            (color.g * (1.0 - textureColor.g) + textureColor.g) * textureColor.r * alpha + backgroundColor.g * backgroundTextureColor.r * (1.0 - alpha),
            (color.b * (1.0 - textureColor.g) + textureColor.g) * textureColor.r * alpha + backgroundColor.b * backgroundTextureColor.r * (1.0 - alpha),
            1.0);
    } else {
        gl_FragColor = vec4(
            color.r * textureColor.b * alpha + backgroundColor.r * backgroundTextureColor.r * (1.0 - alpha),
            color.g * textureColor.b * alpha + backgroundColor.g * backgroundTextureColor.r * (1.0 - alpha),
            color.b * textureColor.b * alpha + backgroundColor.b * backgroundTextureColor.r * (1.0 - alpha),
            1.0);
    }
}